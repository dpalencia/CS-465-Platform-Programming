using UnityEngine;


public class NodeCanvas : PlatformGenericSingleton<NodeCanvas>  {
    void Start() {
        this.gameObject.SetActive(false);
    }
}