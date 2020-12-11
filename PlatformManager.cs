using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Convert this into a singleton.
// Node selection will be handled on here as well

public class PlatformManager : PlatformGenericSingleton<PlatformManager>
{

  // Todo: make this a singleton

  // Attach platform specific events to the platform manager:
  

  // Event that gets triggered any time PlatformManager related UI
  // is updated
  public delegate void OnPlatformManagerChanged();
  public static event OnPlatformManagerChanged platformManagerChanged;

  // Private so that we can
  // abstract accessing config data
  // with setters in this class.
  public static PlatformConfigurationData platformConfig = new PlatformConfigurationData(0, 0, 0, 0);
  
  public List<List<GameObject>> nodes;
  //public List<NodeData> nodeDataList;
  public List<List<NodeData>> nodeDataList;

  public List<string> nodeJsonList = new List<string>();
  
  public void setOffset(float offset) {
    if (platformConfig != null) {
      platformConfig.deltaSpace = offset;
    }
    platformManagerChanged();
  }

  public void setMaxHeight(float maxHeight) {
    if (platformConfig != null) {
      platformConfig.MaximumHeight = maxHeight;
    }
    platformManagerChanged();
  }

  public void setM(int m) {
    platformConfig.M = m;
    platformManagerChanged();
  }

  public void setN(int n) {
    platformConfig.N = n;
    platformManagerChanged();
  }
  
  public float getMaxHeight() {
    return platformConfig.MaximumHeight;
  }

  public int getN() {
    return platformConfig.N;
  }

  public int getM() {
    return platformConfig.M;
  }

  public float getDeltaSpace() {
    return platformConfig.deltaSpace;
  }

  public void destroyPlatform() {
    // Use foreach so we don't depend on m and n values to iterate
    // through the platform matrix.
    foreach (List<GameObject> nodeList in this.nodes) {
      foreach(GameObject node in nodeList) {
        Destroy(node);
      }
    NodeManager.selectedNodeManager = null;
    }

    // Reset the nodeData list
    nodeDataList = new List<List<NodeData>>();
  }

  public void setPlatform(bool fromJSON = false) {

    // Initializing both GameObject and NodeData lists
    nodes = new List<List<GameObject>>();

    // Single dimension NodeData list so that we can write it to JSON
    // If no nodeDataList exists, create it
    nodeDataList = new List<List<NodeData>>();

    for (int x = 0; x < platformConfig.M; x++) {

      nodes.Add(new List<GameObject>());
      nodeDataList.Add(new List<NodeData>());

      for(int z = 0; z < platformConfig.N; z++) {
        // Create the object, set its position, scale it
        Vector3 position;
        position = new Vector3(x + platformConfig.deltaSpace * x, 0, z + platformConfig.deltaSpace * z);


        nodes[x].Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
        nodes[x][z].name = string.Format("[{0}, 0, {1}]", x, z);

        // Pancake the cube
        nodes[x][z].transform.localScale = new Vector3(1, 0.1f, 1);


        // Add the NodeManager script to each cube.
        NodeManager manager = nodes[x][z].AddComponent<NodeManager>();

        // Add NodeData object to NodeData list
        NodeData currentNodeData = nodes[x][z].AddComponent<NodeData>();
        
        // If not from json, set attributes directly
        if(!fromJSON) {
          currentNodeData.setStartPosition(position);
          currentNodeData.setNextPosition(position);
          currentNodeData.platformIndexM = x;
          currentNodeData.platformIndexN = z;
          currentNodeData.setNodeRandomColors();

        } else {
          // Otheriwes read them directly from the json
          currentNodeData.buildFromJSON(nodeJsonList[platformConfig.N * x + z]);
        }

        nodeDataList[x].Add(currentNodeData);
        nodes[x][z].transform.position = currentNodeData.nextPosition;

        // Set items not to destroy on load
        // to preserve them between scenes.0
        DontDestroyOnLoad(nodes[x][z]);

      }
  
    }

  }

  public void rebuildPlatform() {
    destroyPlatform();
    setPlatform();
  }

  void OnEnable() {
    
  }

  void OnDisable() {
    UIManager.buildPlatform -= rebuildPlatform;
  }

    // Start is called before the first frame update
    void Start(){
      UIManager.buildPlatform += rebuildPlatform;
      UIManager.writeData += writeToJSON;
      setPlatform();
    }


  public void readFromJSON() {
    // Hardcode the input location for now
    string inputPath = "PlatformDataInput.json";
    System.IO.StreamReader file = new System.IO.StreamReader(inputPath);

    // Set platform config from json
    string platformConfigJSON = file.ReadLine();
    platformConfig.buildFromJSON(platformConfigJSON);

    // Begin to read the rest of the file line by line
    string line;
    nodeJsonList.Clear();
    // Read the input file line by line,
    // add the line to a list
    while((line = file.ReadLine()) != null) {
      nodeJsonList.Add(line);
    }

    // Build up the platform by processing all lines
    destroyPlatform();
    setPlatform(true);
          platformManagerChanged();
  }

    void writeToJSON() {
      string platformJson = JsonUtility.ToJson(platformConfig);
      System.IO.File.WriteAllText(Application.dataPath + "/PlatformData.json", platformJson + "\n");

      foreach (List<NodeData> list in nodeDataList) {
        foreach(NodeData nodeData in list) {
          string nodeJson = JsonUtility.ToJson(nodeData);
          System.IO.File.AppendAllText(Application.dataPath + "/PlatformData.json", nodeJson + "\n");
        }
      }
    }
}
