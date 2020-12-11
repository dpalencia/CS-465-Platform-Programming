using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SelectionStatus {
        DEFAULT,
        HIGHLIGHTED,
        SELECTED
}


// This class keeps track of positions for a node.
public class NodeManager : MonoBehaviour {

    // Currently selected nodes
    public static NodeManager selectedNodeManager;
    
    // Bool keeps track of whether to pause/play
    // the simulation.
    public static bool run = false;

    public SelectionStatus selectionStatus;

    // Interpolation variable, ranging from 0 to 1
    // Check if the current node is selected
    public float t; // position interpolation variable

    // Current and next colors.
    // These will always have
    // full RGB values
    public Color currentColor;
    public Color nextColor; 

    public static float interpSpeed = .5f;

    // The current node's renderer object,
    // used to update its color and texture
    private Renderer Renderer;

    // Nodedata holds the nodedata
    // of the node at its particular index
    public NodeData nodeData;

    // during the simulation, 
    // these are for keeping track of node mobvement.
    public Vector3 startPosition;
    public Vector3 nextPosition;


    // Color materials for default/highlight/selected   
    public Material defaultMaterial;
    public Material selectedMaterial;
    public Material highlightMaterial;

    // Keeps track of whether or not a node has picked up
    // the data of the next node
    // This is so that this only happens once per movement cycle.
    public bool transferred;

    // This is the row index.
    // To keep track of where in the row the node should be
    // in the simulation.
    public int simIndex; 

    public static void setInterpSpeed(float newSpeed) {
        interpSpeed = newSpeed;
    }
    public static void toggleRun() {
        run = !run;
    }


    public void moveNodeToNextPosition() {
        // This function handles moving the node to the next position
        // typically triggered by the selection manager
        if(selectionStatus == SelectionStatus.SELECTED) {
            transform.position = nodeData.nextPosition;
        }
    }

    private float interpolateSine(float t) {
        // Takes as input an itnerpolation variable
        // ranging from 0 to 1

        // the amplitude of this function
        // is modified to go through one half cycle of the sine function
        // in the range between 0 and 1
        return (Mathf.Sin((t * 2.0f * Mathf.PI - (Mathf.PI/ 2.0f))) / 2.0f) + (0.5f);
    }

    public void resetNode() {
        transform.position = nodeData.startPosition;
    }

    public void setNodeToTarget() {
        transform.position = nodeData.nextPosition;
    }

    public void clearSelection() {
        selectionStatus = SelectionStatus.DEFAULT;
    }

    public void setNodeDataHeight(float height) {
        nodeData.setNodeHeight(height);
    }

      public void setStartColorR(float colorValue) {
        currentColor.r = colorValue;
        nodeData.setStartColorR(colorValue);
    }

    public void setStartColorG(float colorValue) {
        currentColor.g = colorValue;
        nodeData.setStartColorG(colorValue);
    }

    public void setStartColorB(float colorValue) {
        currentColor.b = colorValue;
        nodeData.setStartColorB(colorValue);
    }

    public void setEndColorR(float colorValue) {
        nextColor.r = colorValue;
        nodeData.setEndColorR(colorValue);
    }

    public void setEndColorG(float colorValue) {
        nextColor.g = colorValue;
        nodeData.setEndColorG(colorValue);
    }
    
    public void setEndColorB(float colorValue) {
        nextColor.b = colorValue;
        nodeData.setEndColorB(colorValue);
    }

    public void copyNodeData(NodeData newNodeData) {
        nextColor = newNodeData.endColor;
        currentColor = newNodeData.startColor;
        // Be careful not to copy the entire position variable. Just the y component.
        startPosition.y = newNodeData.startPosition.y;
        nextPosition.y = newNodeData.nextPosition.y;
        nodeData.copyNodeData(newNodeData);
    }

    void Start()
    {
        NodeData.nodeDataChanged += moveNodeToNextPosition;
        UIManager.clearSelection += clearSelection;
        UIManager.resetNodes += resetNode;
        UIManager.setNodesToTargets += setNodeToTarget;

        defaultMaterial = (Material)Resources.Load("defaultMaterial", typeof(Material));
        selectedMaterial = (Material)Resources.Load("selectedMaterial", typeof(Material));
        highlightMaterial = (Material)Resources.Load("highlightMaterial", typeof(Material));

        // Set the node settings
        nodeData = this.GetComponent<NodeData>();

        // -1 so that it starts at the current node when it enters the render loop
        simIndex = nodeData.platformIndexM - 1;
        startPosition = nodeData.startPosition;
        nextPosition = nodeData.nextPosition;
        transferred = false;
        t = 0;


        Renderer = GetComponent<Renderer>();

        selectionStatus = SelectionStatus.DEFAULT;

        // Initialize Start color to default color
        // Set end color to random RGB values.
        currentColor = nodeData.startColor;
        nextColor = nodeData.endColor;

    }   

    void OnDestroy() {
        NodeData.nodeDataChanged -= moveNodeToNextPosition;
        UIManager.resetNodes -= resetNode;
        UIManager.clearSelection -= clearSelection;
        UIManager.setNodesToTargets -= setNodeToTarget;   
    }


    void Update()
    {

        if(UIManager.currentScene == CurrentScene.NODE) {
            if(Renderer != null) {
                switch(selectionStatus)  {
                    case SelectionStatus.HIGHLIGHTED:
                        Renderer.material = highlightMaterial;
                        break;
                    case SelectionStatus.SELECTED:
                        Renderer.material = selectedMaterial;
                        break;
                    case SelectionStatus.DEFAULT:
                        Renderer.material = defaultMaterial;
                        break;
                }
            }
        }
        
        if(run && UIManager.currentScene == CurrentScene.SIMULATE) {
            float dy = transform.position.y - startPosition.y;
            // Input variable to sine function
            t += Time.deltaTime * interpSpeed;
            // Result of sine function
            float interpolatePosition= interpolateSine(t);
            // If we're close enough to 0,
            if (interpolatePosition < 0.001) {
                if(!transferred) {
                    simIndex = (simIndex + 1) % PlatformManager.platformConfig.M;
                    NodeData nextNodeData = PlatformManager.Instance.nodeDataList[simIndex][nodeData.platformIndexN];
                    nextPosition.y = nextNodeData.nextPosition.y;
                    startPosition.y = nextNodeData.startPosition.y;
                    currentColor = nextNodeData.startColor;
                    nextColor = nextNodeData.endColor;
                    transferred = true;
                }
            }
            else {
                transform.position = Vector3.Lerp(startPosition, nextPosition, interpolatePosition);
                Renderer.material.color = Color.Lerp(currentColor, nextColor, interpolatePosition);
                transferred = false;
            }
        }
    }
}