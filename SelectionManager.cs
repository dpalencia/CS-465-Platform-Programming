using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour {

    private Transform _previousHighlight;
    private NodeManager _previousSelection;

    public delegate void OnNodeSelected();
    public static event OnNodeSelected nodeSelected;

    private void Update() {
        
        if(_previousHighlight != null) {
            // De-highlight the node that was previously hovered over
            var selectionNodeManager = _previousHighlight.GetComponent<NodeManager>();
            if(selectionNodeManager.selectionStatus != SelectionStatus.SELECTED) {
                selectionNodeManager.selectionStatus = SelectionStatus.DEFAULT;
            }
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && !EventSystem.current.IsPointerOverGameObject()) {
            var selection = hit.transform;
            var selectionNodeManager = selection.GetComponent<NodeManager>();
            // Pick up the nodemanager instance for the selected node
            if (selectionNodeManager != null) {
                if (selectionNodeManager.selectionStatus != SelectionStatus.SELECTED) {
                    selectionNodeManager.selectionStatus = SelectionStatus.HIGHLIGHTED;
                }
                if (Input.GetMouseButtonDown(0)) {
                        if(selectionNodeManager.selectionStatus != SelectionStatus.SELECTED) {
                            // Set the currently selected node here
                            // Assigned to a static variable; one selection at a time
                            NodeManager.selectedNodeManager = selectionNodeManager;

                            // Fire the node selection event
                            nodeSelected();
                            
                            selectionNodeManager.selectionStatus = SelectionStatus.SELECTED;
     
                        
                            if(_previousSelection != null) {
                                _previousSelection.selectionStatus = SelectionStatus.DEFAULT;
                                
                            }
                            _previousSelection = selectionNodeManager;
                        } else {
                            selectionNodeManager.selectionStatus = SelectionStatus.DEFAULT;
                            _previousHighlight = null;
                        }
                    }
                 } 
                 _previousHighlight = selection;
            }
        }
}