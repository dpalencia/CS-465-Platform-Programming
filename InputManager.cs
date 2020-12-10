using UnityEngine;

public class InputManager : MonoBehaviour {
  private void Update() {
        // Quit application
        if(Input.GetKeyDown("q")) {
            Application.Quit();
        }

        // Start/stop the simulation
        if (Input.GetKeyDown("t")) {
            NodeManager.toggleRun();
        }

        // u and j increase/decrease
        // the up and down movement speed
        /*
        if (Input.GetKey("u")) {
            NodeManager.movementSpeed += 0.7f * Time.deltaTime;
        }

        if (Input.GetKey("j")) {
            NodeManager.movementSpeed -= 0.7f * Time.deltaTime;
        }*/

  }
}