
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlatformGenericSingleton<T>: MonoBehaviour where T: Component {
    private static T instance;

    public static T Instance {
        // This is a nice way to use getters and setters
        get {
            if(instance != null) {
                instance = FindObjectOfType<T>();
            }
            else {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                instance = obj.AddComponent<T>();
            }
            return instance;
        }
    }

    public virtual void Awake() {
        // This is used to initialize 
        // the object on scene load
        // Ensures that the singleton instance
        // will not be destroyed
        if (instance == null) {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

}