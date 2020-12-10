using UnityEngine;
using UnityEngine.UI;

// Use events and delegates  in here.
// Raise events, consume events
[System.Serializable]
public class PlatformConfigurationData {
    // M and N dimensions
    public int M;
    public int N;
    // Maximum configuration height for the nodes
    public float MaximumHeight;
    // Node delta spacing
    public float deltaSpace;
    
    public PlatformConfigurationData() {
        this.M = 0;
        this.N = 0;
        this.MaximumHeight = 0;
        this.deltaSpace = 0;
    }

    public PlatformConfigurationData(int M, int N, float MaximumHeight, float deltaSpace) {
        this.M = M;
        this.N = N;
        this.MaximumHeight = MaximumHeight;
        this.deltaSpace = deltaSpace;
    }
    
    // Using the fromJSON utility to return an instance of PlatformConfigurationDAta
    public void buildFromJSON(string jsonString) {
        JsonUtility.FromJsonOverwrite(jsonString, this);
    }

}