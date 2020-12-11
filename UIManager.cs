using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public enum CurrentScene {
        MAIN,
        PLATFORM,
        NODE,
        SIMULATE
}


// Use events and delegates  in here.
// Raise events, consume events
public class UIManager : PlatformGenericSingleton<UIManager> {

    // UI Events:
    // Trigger the PlatformUpdateUI event
    // Any time an appropriate UI method is changed.
    public delegate void OnUpdateCameraPosition(PlatformConfigurationData pcd);
    public static event OnUpdateCameraPosition updateCameraPosition;

    // Mainly used to update camera modes on scene switching
    public delegate void OnSceneSwitch();
    public static event OnSceneSwitch sceneSwitched;

    public delegate void OnBuildPlatform();
    public static event OnBuildPlatform buildPlatform;

    public delegate void OnWriteData();
    public static event OnWriteData  writeData;


    public delegate void ClearSelection();
    public static event ClearSelection clearSelection;

    public delegate void ResetNodes();
    public static event ResetNodes resetNodes;

    public delegate void SetNodesToTargets();
    public static event SetNodesToTargets setNodesToTargets;

    public Slider ySlider;
    public Slider deltaSlider;
    public Slider maximumHeightSlider;
    public Slider selectedNodeHeightSlider;

    // The single instance of PlatformManager
    // since this is a singleton
    public PlatformManager platformManager;
    // Text components for stats panel
    public Text statsDeltaSpacing;
    public Text statsPlatformSize;
    public Text statsMaximumHeight;
    public Text simSpeedNumText;
    public static CurrentScene currentScene = CurrentScene.MAIN;

    public GameObject canvasPlatform;
    public GameObject canvasMain;
    public GameObject canvasNode;
    public GameObject canvasSimulate;

    public Text selectedNodeText;
    public  Text selectedNodeHeight;

    public Image startColorImage;
    public Image endColorImage;

    public InputField startR;
    public InputField startG;
    public InputField startB;
    public InputField endR;
    public InputField endG;
    public InputField endB;
    public NodeData uiNodeData;
    


    void OnEnable() {
        setComponents();
    }

    void onDisable() {
        PlatformManager.platformManagerChanged -= setPlatformUI;
    }
    
    void Start() {
        platformManager = PlatformManager.Instance;
        PlatformManager.platformManagerChanged += setPlatformUI;

        SelectionManager.nodeSelected += setNodeUI;
        NodeData.nodeDataChanged += setNodeUI;

        setPlatformUI();   
             
        uiNodeData = gameObject.GetComponent<NodeData>();
        
    }   

    void setNodeUI() {
        if(NodeManager.selectedNodeManager != null) {
        NodeData currentNodeData = NodeManager.selectedNodeManager.nodeData;
            if(ySlider != null) {
                ySlider.value = NodeManager.selectedNodeManager.nodeData.nextPosition.y;
            }
            
            if(selectedNodeHeight != null) {
                selectedNodeHeight.text = currentNodeData.nextPosition.y.ToString();
            }

            if(selectedNodeText != null) {
                selectedNodeText.text = string.Format("[{0}, {1}]", currentNodeData.platformIndexM, currentNodeData.platformIndexN);
            }
            
            if(startColorImage != null) {
                startColorImage.color = currentNodeData.startColor;
            }

            if(endColorImage != null) {
                endColorImage.color = currentNodeData.endColor;
            }

            if (startR != null) {
                startR.text = currentNodeData.startColor.r.ToString();
            }
            if (startG != null) {
                startG.text = currentNodeData.startColor.g.ToString();
            }
            if (startB != null) {
                startB.text = currentNodeData.startColor.b.ToString();
            }
            if (endR != null) {
                endR.text = currentNodeData.endColor.r.ToString();
            }
            if (endG != null) {
                endG.text = currentNodeData.endColor.g.ToString();
            }
            if (endB != null) {
                endB.text = currentNodeData.endColor.b.ToString();
            }
        }
    }

    void setPlatformUI() {
        if (statsDeltaSpacing != null) {
            statsDeltaSpacing.text = string.Format("Delta Spacing: {0}", platformManager.getDeltaSpace());
        }
        if (statsPlatformSize != null)  {
            statsPlatformSize.text = string.Format("Platform Size (M x N): {0} x {1}", platformManager.getM(), platformManager.getN());
        }

        if (statsMaximumHeight != null) {
            statsMaximumHeight.text = string.Format("Maximum Height: {0}", platformManager.getMaxHeight());
        }
        if (selectedNodeHeightSlider != null) {
            selectedNodeHeightSlider.maxValue = platformManager.getMaxHeight();
        }
        if (simSpeedNumText != null) {
            simSpeedNumText.text = NodeManager.interpSpeed.ToString();
        }
    }

    void setComponents() {
        // Set all members to their respective UI components. 
        // Gets instance of the platform manager class so we can call its methods.
        // This class manages the building and positioning of nodes.

        statsDeltaSpacing = GameObject.Find("DeltaSpacing").GetComponent<Text>();
        statsPlatformSize = GameObject.Find("PlatformSize").GetComponent<Text>();
        statsMaximumHeight = GameObject.Find("MaximumHeight").GetComponent<Text>();

        canvasPlatform = GameObject.Find("PlatformCanvasObject");
        canvasMain = GameObject.Find("MainMenuCanvasObject");
        canvasNode = GameObject.Find("NodeProgrammingCanvasObject");
        canvasSimulate = GameObject.Find("SimulateCanvasObject");

        
        selectedNodeText = GameObject.Find("SelectedNodeText").GetComponent<Text>();
        selectedNodeHeight = GameObject.Find("NodeHeightText").GetComponent<Text>();
        simSpeedNumText = GameObject.Find("SimSpeedNum").GetComponent<Text>();

        ySlider = GameObject.Find("NodeHeightSlider").GetComponent<Slider>();
        maximumHeightSlider = GameObject.Find("MaximumHeight").GetComponent<Slider>();
        selectedNodeHeightSlider = GameObject.Find("NodeHeightSlider").GetComponent<Slider>();

        startColorImage = GameObject.Find("StartColorImage").GetComponent<Image>();
        endColorImage = GameObject.Find("EndColorImage").GetComponent<Image>();

        startR = GameObject.Find("NodeStartColorR").GetComponent<InputField>();
        startG = GameObject.Find("NodeStartColorG").GetComponent<InputField>();
        startB = GameObject.Find("NodeStartColorB").GetComponent<InputField>();
        endR = GameObject.Find("NodeEndColorR").GetComponent<InputField>();
        endG = GameObject.Find("NodeEndColorG").GetComponent<InputField>();
        endB = GameObject.Find("NodeEndColorB").GetComponent<InputField>();

    }

    public void onSliderChanged(Slider slider) {
        switch(slider.name) {
            case "MaxHeightSlider":
                platformManager.setMaxHeight(slider.value);
                selectedNodeHeightSlider.maxValue = slider.value;
                break;
            case "DeltaSlider":
                platformManager.setOffset(slider.value);
                break;
            case "NodeHeightSlider":
                NodeManager.selectedNodeManager.setNodeDataHeight(slider.value);
                uiNodeData.setNodeHeight(slider.value);
                break;
            case "SimSpeedSlider":
                NodeManager.setInterpSpeed(slider.value);
                setPlatformUI();
                
                break;
        }
    }
    public void onTextValueChanged(InputField inputField) {
        float newColorVal;
        switch(inputField.name) {
            case "DimensionM": 
                int newM;
                if (int.TryParse(inputField.text, out newM) == true) {
                    platformManager.setM(newM);
                }
                break;
            case "DimensionN":
                int newN;
                if (int.TryParse(inputField.text, out newN) == true) {
                    platformManager.setN(newN);
                }
                break;
            case "NodeStartColorR":
                if(float.TryParse(inputField.text, out newColorVal) == true) {
                    NodeManager.selectedNodeManager.setStartColorR(newColorVal);
                    uiNodeData.setStartColorR(newColorVal);
                }
                break;
            case "NodeStartColorG":
                if(float.TryParse(inputField.text, out newColorVal) == true) {
                    NodeManager.selectedNodeManager.setStartColorG(newColorVal);
                    uiNodeData.setStartColorG(newColorVal);
                }
                break;
            case "NodeStartColorB":
                if(float.TryParse(inputField.text, out newColorVal) == true) {
                    NodeManager.selectedNodeManager.setStartColorB(newColorVal);
                    uiNodeData.setStartColorB(newColorVal);
                }
                break;
            case "NodeEndColorR":
                if(float.TryParse(inputField.text, out newColorVal) == true) {
                    NodeManager.selectedNodeManager.setEndColorR(newColorVal);
                    uiNodeData.setEndColorR(newColorVal);
                }
                break;
            case "NodeEndColorG":
                if(float.TryParse(inputField.text, out newColorVal) == true) {
                    NodeManager.selectedNodeManager.setEndColorG(newColorVal);
                    uiNodeData.setEndColorG(newColorVal);
                }
                break;
            case "NodeEndColorB":
                if(float.TryParse(inputField.text, out newColorVal) == true) {
                    NodeManager.selectedNodeManager.setEndColorB(newColorVal);
                    uiNodeData.setEndColorB(newColorVal);
                }
                break;
        }
    }


    public void canvasSwitcher() {
         if(clearSelection != null) {
            clearSelection();
        }

        if(canvasMain != null) {
            canvasMain.SetActive(false);
            if(currentScene == CurrentScene.MAIN) {
                canvasMain.SetActive(true);
            }
        }

        if(canvasPlatform != null) {
            canvasPlatform.SetActive(false);
              if(currentScene == CurrentScene.PLATFORM) {
                canvasPlatform.SetActive(true);
            }
        }

        if(canvasNode != null) {
            canvasNode.SetActive(false);
            if(currentScene == CurrentScene.NODE) {
                canvasNode.SetActive(true);
            }
        }

        if(canvasSimulate != null) {
            canvasSimulate.SetActive(false);
            if(currentScene == CurrentScene.SIMULATE) {
                canvasSimulate.SetActive(true);
            }
        }

        // Raise scene switch event
        sceneSwitched();

    }

    public void onButtonClicked(Button button) {
        if(button != null) {
            switch(button.name) {
                case "MainMenuButton":
                    SceneManager.LoadScene("MainMenuScene");
                    currentScene = CurrentScene.MAIN;
                    canvasSwitcher();
                    resetNodes();
                    break;
                case "BuildPlatformButton":
                    buildPlatform();
                    updateCameraPosition(PlatformManager.platformConfig);
                    break;
                case "SimulateButton":
                    SceneManager.LoadScene("SimulateScene");
                    currentScene = CurrentScene.SIMULATE;
                    canvasSwitcher();
                    resetNodes();
                    break;
                case "SimulateStartButton":
                    NodeManager.toggleRun();
                    button.GetComponentInChildren<Text>().text = NodeManager.run ? "Stop" : "Start";
                    break;
                case "PlatformSetupButton":
                    SceneManager.LoadScene("PlatformSetupscene", LoadSceneMode.Single);
                    currentScene = CurrentScene.PLATFORM;
                    canvasSwitcher();
                    resetNodes();
                    break;
                case "ProgramButton":
                    SceneManager.LoadScene("NodeProgrammingScene");
                    currentScene = CurrentScene.NODE;
                    canvasSwitcher();
                    setNodesToTargets();
                    break;
                case "WriteFileButton":
                    writeData();
                    break;
                case "ProgramNodeVals":
                    // Program node with currently selected values for the node
                    NodeManager.selectedNodeManager.copyNodeData(uiNodeData);
                    break;
                case "ReadFileButton":
                    PlatformManager.Instance.readFromJSON();
                    updateCameraPosition(PlatformManager.platformConfig);
                    break;
                case "ExitButton":
                    Application.Quit();
                    break;
                    
                default:
                    break;
            }
        }

    }
}
