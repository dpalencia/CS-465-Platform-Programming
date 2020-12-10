using UnityEngine;


public class PlatformCanvas : PlatformGenericSingleton<PlatformCanvas>  {
    void Start() {
        this.gameObject.SetActive(false);
    }
}