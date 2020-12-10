using UnityEngine;
public class SimulateCanvas : PlatformGenericSingleton<SimulateCanvas>  {
    void Start() {
        this.gameObject.SetActive(false);
    }
}