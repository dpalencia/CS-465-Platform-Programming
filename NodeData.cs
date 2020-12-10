using UnityEngine;

[System.Serializable]
public class NodeData : MonoBehaviour {
    // The class that holds all of the starting data
    // for the platform simulation.
    // Includes platform indices,
    // start and end positions,
    // and start and end colors,

    // Also raises events any time the data is changed
    // usually from the node manager 
    public delegate void NodeDataChanged();
    public static event NodeDataChanged nodeDataChanged;

    public int platformIndexM;
    public int platformIndexN;

    public Vector3 startPosition;

    public Vector3 nextPosition;

    public Color startColor;
    public Color endColor;

    public void setNodeRandomColors() {
        startColor = new Color(0, 0, 0, 1);
        endColor = new Color(0, 0, 0, 1);
    }

    public void copyNodeData(NodeData newNodeData) {
        startColor = newNodeData.startColor;
        endColor = newNodeData.endColor;
        startPosition.y = newNodeData.startPosition.y;
        nextPosition.y = newNodeData.nextPosition.y;
        nodeDataChanged();
    }


    public void setStartColorR(float colorValue) {
        startColor.r = colorValue;
        nodeDataChanged();
    }

    public void setStartColorG(float colorValue) {
        startColor.g = colorValue;
        nodeDataChanged();
    }

    public void setStartColorB(float colorValue) {
        startColor.b = colorValue;
        nodeDataChanged();
    }

    public void setEndColorR(float colorValue) {
        endColor.r = colorValue;
        nodeDataChanged();
    }

    public void setEndColorG(float colorValue) {
        endColor.g = colorValue;
        nodeDataChanged();
    }
    
    public void setEndColorB(float colorValue) {
        endColor.b = colorValue;
        nodeDataChanged();
    }

    public void setStartPosition(Vector3 position) {
        this.startPosition = position;
        nodeDataChanged();
    }

    public void setNextPosition(Vector3 position) {
        this.nextPosition = position;
        nodeDataChanged();
    }

    public void setNodeHeight(float height) {
        this.nextPosition = new Vector3(startPosition.x, height, startPosition.z);
        nodeDataChanged();
    }

    public void buildFromJSON(string inputJSON) {
        JsonUtility.FromJsonOverwrite(inputJSON, this);
    }
}