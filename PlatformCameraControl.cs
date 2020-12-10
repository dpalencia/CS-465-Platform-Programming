using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCameraControl : PlatformGenericSingleton<PlatformCameraControl>
{
    public float centerX = 0f;
    public float centerZ = 0f;

    public float speed = 30.0f;
    public float rotateSpeed = 100.0f;

    protected float fDistance = 1;
    protected float fSpeed = 1;

    public float deltaSpace;

    // Interpolation vectors and variable
    public Vector3 oldPosition;
    public Vector3 newPosition;
    public Vector3 oldTarget;
    public Vector3 newTarget;
    public Vector3 targetPosition;
    public float t;


    // Keep track of platform  center to focus the camera on when not editing nodes
    public Vector3 platformCenterPosition;
    public Vector3 platformCenterTarget;

    private void OnEnable()
    {
        UIManager.updateCameraPosition += UIManager_BuildPlatformOnClicked;
        SelectionManager.nodeSelected += camToNode;
        UIManager.sceneSwitched += camSceneSwitch;
        t = 1;
    }

    private void UIManager_BuildPlatformOnClicked(PlatformConfigurationData pcd)
    {
        // Initialize vectors in here 
        // position and interpolation
        centerX = pcd.M / 2;
        centerZ = pcd.N / 2;
        deltaSpace = pcd.deltaSpace;
        platformCenterPosition = new Vector3(pcd.M - 5, 15, pcd.N - 5);
        oldPosition = platformCenterPosition;
        newPosition = platformCenterPosition;
        platformCenterTarget = new Vector3(centerX, 0, centerZ);
        oldTarget = new Vector3(0,0,0);
        newTarget = platformCenterTarget;
        t = 0;
    }

    private void OnDisable()
    {
        UIManager.updateCameraPosition -= UIManager_BuildPlatformOnClicked;
    }

    private void camSceneSwitch() {
        if(UIManager.currentScene == CurrentScene.NODE) {
            camToNode();
        } else {
            camToCenter();
        }
    }
    private void camToCenter() {
        t = 0;
        oldPosition = transform.position;
        newPosition = platformCenterPosition;
        oldTarget = targetPosition;
        newTarget = platformCenterTarget;
    }

    private void camToNode() {
        t = 0;
        oldTarget = targetPosition;
        if(NodeManager.selectedNodeManager.nodeData != null)
            newTarget = NodeManager.selectedNodeManager.nodeData.startPosition;
        oldPosition = transform.position;
        newPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {   
        transform.LookAt(targetPosition); 

        // Look at the center of the platform when not editing nodes
    
        if (t < 1) {
            transform.position = Vector3.Lerp(oldPosition, newPosition, t);
            targetPosition = Vector3.Lerp(oldTarget, newTarget, t);
            t += Time.deltaTime * 1.5f;
        } 


        #region ROTATE VERTICAL/HORIZONTAL
        if (Input.GetKey(KeyCode.RightArrow))
            transform.RotateAround(targetPosition, -Vector3.up, Time.deltaTime * rotateSpeed);
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.RotateAround(targetPosition, Vector3.up, Time.deltaTime * rotateSpeed);
        if (Input.GetKey(KeyCode.UpArrow))
            transform.RotateAround(targetPosition, Vector3.right, Time.deltaTime * rotateSpeed);
        if (Input.GetKey(KeyCode.DownArrow))
            transform.RotateAround(targetPosition, -Vector3.right, Time.deltaTime * rotateSpeed);
        #endregion

        // Forward/backward/up/down

        if(Input.GetKey(KeyCode.W)) {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.A)) {
            transform.RotateAround(targetPosition, Vector3.up, Time.deltaTime * rotateSpeed);
        }
        if(Input.GetKey(KeyCode.S)) {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.D)) {
            transform.RotateAround(targetPosition, -Vector3.up, Time.deltaTime * rotateSpeed);
        }
        if(Input.GetKey(KeyCode.R)) {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.F)) {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }
}
