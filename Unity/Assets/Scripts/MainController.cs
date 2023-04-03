using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UIElements;
using ExportScenario.XMLBuilder;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using uGUI = UnityEngine.UI;
using System.IO;
using Assets.Scripts.ViewControllers.PopupControllers;
using System.Runtime.Serialization.Formatters.Binary;
using SimpleFileBrowser;
using System.Collections;
using System;


/// <summary>
/// MainController manages the user interface and the interactions with the scenario editor.
/// </summary>
public class MainController : MonoBehaviour
{
    public VisualTreeAsset eventEntryTemplate;
    //Spawned Cars
    public GameObject vehiclePrefab;
    public GameObject egoPrefab;
    public GameObject pedPrefab;

    public UISkin skin;

    //Event Bar (Center Bottom)
    private ListView eventList;
    private Button addCarButton;
    private Button addMotorcycleButton;
    private Button addBikeButton;
    private Button addPedestrianButton;
    private Button worldSettingsButton;
    private Button exportButton;
    private Button loadButton;
    private Button saveButton;
    private Button homeButton;
    private Button exitButton;
    
    private VisualElement buttonBar;

    private uGUI.Button removeEntityButton;
    private uGUI.Button editEntityButton;
    private uGUI.Toggle snapToggle;
    private GameObject actionButtonCanvas;


    /// <summary>
    /// The ScenarioInfo for the current scenario being edited.
    /// </summary>
    public ScenarioInfo info { get; private set; }

    private IBaseController selectedEntity;

    private WorldSettingsPopupController worldSettingsController;
    private SnapController snapController;
    public WarningPopupController warningPopupController;
    public YesNoPopupController yesNoPopupController;
    public HelpPopupController helpPopupController;

    // If true don't show this explanation anymore because the user has clicked 'DoNotShowAgain'
    public static bool[] helpComplete = new bool[] { false, false }; 
    // [0] --> false: user clicks 'snapToggle' in actionButtonCanvas, show Free/Tied path explanation
    // [1] --> false: WaypointSettings, show 'LaneChangeAction' explanation
    // ... in case more help popups are needed

    public static bool freeze = false; // if true, a popup GUI is open and the user shouln't change paths or vehicles!

    /// <summary>
    /// Initializes the MainController and sets up event listeners.
    /// </summary
    void Start()
    {
        this.info = new ScenarioInfo();
        this.info.onEgoChanged = () =>
        {
            eventList.itemsSource = info.allEntities;
            refreshEntityList();
        };
        this.selectedEntity = null;
        var editorGUI = GameObject.Find("EditorGUI").GetComponent<UIDocument>().rootVisualElement;

        initializeEventList(editorGUI);
        initializeButtonBar(editorGUI);
        initializeActionButtons();

        FileBrowser.Skin = skin;

        this.snapController = Camera.main.GetComponent<SnapController>();

        EventManager.StartListening(typeof(MouseClickAction), x =>
        {
            if (!snapController.IgnoreClicks)
            {
                this.setSelectedEntity(null);
            }
        });

        EventManager.StartListening(typeof(MapChangeAction), x =>
        {
            var action = new MapChangeAction(x);
            buttonBar.visible = action.name != "" ? true : false;
            info.MapURL = action.name;
            setSelectedEntity(null);
            Adversary.resetAutoIncrementID();
        });

        EventManager.StartListening(typeof(CompletePlacementAction), x =>
        {
            enableButtonBar();
        });

        EventManager.StartListening(typeof(CancelPlacementAction), x =>
        {
            enableButtonBar();
        });

        GameObject popups = GameObject.Find("PopUps");
        this.worldSettingsController = popups.transform.Find("WorldSettingsPopUpAdvanced").gameObject.GetComponent<WorldSettingsPopupController>();
        this.worldSettingsController.init(this.info.WorldOptions);

        this.warningPopupController = GameObject.Find("PopUps").transform.Find("WarningPopUp").gameObject.GetComponent<WarningPopupController>();
        this.warningPopupController.gameObject.SetActive(true);

        this.yesNoPopupController = GameObject.Find("PopUps").transform.Find("YesNoPopup").gameObject.GetComponent<YesNoPopupController>();
        this.yesNoPopupController.gameObject.SetActive(true);

        //this.helpPopupController = GameObject.FindWithTag("HelpPopup").gameObject.GetComponent<HelpPopupController>();
        this.helpPopupController = GameObject.Find("PopUps").transform.Find("HelpPopUp").gameObject.GetComponent<HelpPopupController>();
        this.helpPopupController.gameObject.SetActive(true);
    }

    /// <summary>
    /// Loads a ScenarioInfo object into the editor.
    /// </summary>
    /// <param name="info">The ScenarioInfo object to load.</param>
    public void loadScenarioInfo(ScenarioInfo info)
    {
        //info = (ScenarioInfo)info.Clone(); //Do we need this? Exported .bin Info already the one before export changes? - Stefan
        EventManager.TriggerEvent(new MapChangeAction(info.MapURL));
        info.onEgoChanged = this.info.onEgoChanged;
        this.info = info;
        foreach(Adversary v in info.Vehicles)
        {
            var viewController = Instantiate(vehiclePrefab, v.SpawnPoint.Vector3Ser.ToVector3(), Quaternion.identity).GetComponent<AdversaryViewController>();
            viewController.init(v);
        }
        foreach (Adversary p in info.Pedestrians)
        {
            var viewController = Instantiate(vehiclePrefab, p.SpawnPoint.Vector3Ser.ToVector3(), Quaternion.identity).GetComponent<AdversaryViewController>();
            viewController.init(p);
        }
        if (info.EgoVehicle is not null)
        {
            var egoController = Instantiate(egoPrefab, info.EgoVehicle.SpawnPoint.Vector3Ser.ToVector3(), Quaternion.identity).GetComponent<EgoViewController>();
            egoController.init(info.EgoVehicle);
        }
        var editorGUI = GameObject.Find("EditorGUI").GetComponent<UIDocument>().rootVisualElement;
        initializeEventList(editorGUI);
        eventList.itemsSource = info.allEntities;
        refreshEntityList();
    }

    /// <summary>
    /// Updates the MainController and handles user input.
    /// </summary>
    public void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            this.setSelectedEntity(null);
        }
    }

    /// <summary>
    /// Sets the selected entity in the scenario editor.
    /// </summary>
    /// <param name="entity">The entity to set as the selected entity. Pass null to deselect the current entity.</param
    public void setSelectedEntity(IBaseController entity)
    {
        if (entity != null)
        {
            var position = entity.getLocation();
            this.actionButtonCanvas.transform.position = new Vector3(position.X, (float)(position.Y - 0.5), -1f);
            snapToggle.SetIsOnWithoutNotify(!entity.shouldIgnoreWaypoints());
            if (this.actionButtonCanvas.activeSelf && entity != selectedEntity)
            {
                var anim = actionButtonCanvas.GetComponent<ActionButtonsAnimation>();
                anim.onMove();
            }
            else
            {
                this.actionButtonCanvas.SetActive(true);
            }
            this.selectedEntity?.deselect();
            this.selectedEntity = entity;
            this.selectedEntity?.select();
        }
        else
        {
            this.selectedEntity?.deselect();
            this.selectedEntity = null;
            this.actionButtonCanvas.SetActive(false);
        }
    }

    /// <summary>
    /// Moves the action buttons to the specified position.
    /// </summary>
    /// <param name="pos">A Vector2 representing the new position for the action buttons.</param>
    public void moveActionButtons(Vector2 pos)
    {
        this.actionButtonCanvas.transform.position = new Vector3(pos.x, (float)(pos.y - 0.5), -1f);
    }

    /// <summary>
    /// Adds an adversary to the scenario.
    /// </summary>
    /// <param name="entity">The Adversary object to be added.</param>
    public void addAdversary(Adversary entity)
    {
        if (entity is Adversary v)
        {
            this.info.Vehicles.Add(v);
        }
        else if (entity is Adversary p)
        {
            this.info.Pedestrians.Add(p);
        }
    }

    /// <summary>
    /// Removes an adversary from the scenario.
    /// </summary>
    /// <param name="adversary">The Adversary object to be removed.</param>
    public void removeAdversary(Adversary adversary)
    {
        if(adversary is Adversary v)
        {
            foreach (Waypoint w in v.Path.WaypointList)
            {
                if (w.StartRouteOfOtherVehicle is not null)
                {
                    Adversary otherVehicle = w.StartRouteOfOtherVehicle;
                    otherVehicle.StartPathInfo = null;
                }
            }
            this.info.Vehicles.Remove(v);
        } else if (adversary is Adversary p)
        {
            this.info.Pedestrians.Remove(p);
        }
    }

    /// <summary>
    /// Sets the ego vehicle in the scenario.
    /// </summary>
    /// <param name="ego">The Ego object representing the ego vehicle.</param>
    public void setEgo(Ego ego)
    {
        this.info.setEgo(ego);
    }

    /// <summary>
    /// Creates a new adversary object without adding it to the scenario model.
    /// It will be added to the model once it is placed by the user.
    /// </summary>
    /// <param name="category">The AdversaryCategory for the new adversary.</param>
    public void createAdversary(AdversaryCategory category)
    {
        var pos = Input.mousePosition;
        pos.z = -0.1f;
        GameObject prefab = this.info.EgoVehicle is null ? egoPrefab : vehiclePrefab;
        var vehicleGameObject = Instantiate(prefab, pos, Quaternion.identity);
        VehicleViewController viewController;
        Color color;
        if (this.info.EgoVehicle is null)
        {
            viewController = vehicleGameObject.GetComponent<EgoViewController>();
            color = new Color(1f, 1f, 1f, 1f); // make Ego vehicle white
        }
        else
        {
            viewController = vehicleGameObject.GetComponent<AdversaryViewController>();
            color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            color = new Color(color.r, color.g, color.b, 1);
        }
        viewController.init(category, color);
        disableButtonBar();
    }

    /// <summary>
    /// Initializes the button bar with the appropriate event listeners.
    /// </summary>
    /// <param name="editorGUI">A reference to the editor GUI's VisualElement.</param>
    private void initializeButtonBar(VisualElement editorGUI)
    {
        addPedestrianButton = editorGUI.Q<Button>("addPedestrianButton");
        addPedestrianButton.AddManipulator(new ToolTipManipulator("Add Pedestrian"));
        addCarButton = editorGUI.Q<Button>("addCarButton");
        addCarButton.AddManipulator(new ToolTipManipulator("Add Vehicle"));
        addMotorcycleButton = editorGUI.Q<Button>("addMotorcycleButton");
        addMotorcycleButton.AddManipulator(new ToolTipManipulator("Add Motorcycle"));
        addBikeButton = editorGUI.Q<Button>("addBikeButton");
        addBikeButton.AddManipulator(new ToolTipManipulator("Add Bike"));
        worldSettingsButton = editorGUI.Q<Button>("worldSettingsButton");
        worldSettingsButton.AddManipulator(new ToolTipManipulator("Open world settings"));
        exportButton = editorGUI.Q<Button>("exportButton");
        exportButton.AddManipulator(new ToolTipManipulator("Export Scenario"));
        loadButton = editorGUI.Q<Button>("loadButton");
        loadButton.AddManipulator(new ToolTipManipulator("Load Scenario"));
        saveButton = editorGUI.Q<Button>("saveButton");
        saveButton.AddManipulator(new ToolTipManipulator("Save Scenario"));
        homeButton = editorGUI.Q<Button>("homeButton");
        homeButton.AddManipulator(new ToolTipManipulator("Open Menu"));
        exitButton = editorGUI.Q<Button>("exitButton");
        exitButton.AddManipulator(new ToolTipManipulator("Exit"));

        buttonBar = editorGUI.Q<VisualElement>("buttons");
        

        addCarButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            createAdversary(AdversaryCategory.Car);
            setSelectedEntity(null);
        });

        addBikeButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            createAdversary(AdversaryCategory.Bike);
            setSelectedEntity(null);
        });

        addMotorcycleButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            createAdversary(AdversaryCategory.Motorcycle);
            setSelectedEntity(null);
        });

        addPedestrianButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            createAdversary(AdversaryCategory.Pedestrian);
            setSelectedEntity(null);
        });

        worldSettingsButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            this.setSelectedEntity(null);
            worldSettingsController.open();
        });

        exportButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            ExportOnClick();
           //loadScenarioInfo(this.info);
        });

        homeButton.RegisterCallback<ClickEvent>(async (ClickEvent) =>
        {
            if (freeze) return;
            ReturnToHome();
        });

        loadButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            this.setSelectedEntity(null);
            LoadBinaryScenarioInfo();
        });

        saveButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            this.setSelectedEntity(null);
            SaveBinaryScenarioInfo(this.info);
        });

        exitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            this.setSelectedEntity(null);
            QuitApplication();
        });

        buttonBar.visible = false;
    }

    /// <summary>
    /// Disables the button bar, making all buttons unresponsive.
    /// </summary>
    public void disableButtonBar()
    {
        addPedestrianButton.SetEnabled(false);
        addBikeButton.SetEnabled(false);
        addCarButton.SetEnabled(false);
        addMotorcycleButton.SetEnabled(false);
        worldSettingsButton.SetEnabled(false);
        exportButton.SetEnabled(false);
        loadButton.SetEnabled(false);
        saveButton.SetEnabled(false);
        homeButton.SetEnabled(false);
        exitButton.SetEnabled(false);
    }

    /// <summary>
    /// Enables all buttons in the button bar.
    /// </summary>
    public void enableButtonBar()
    {
        addPedestrianButton.SetEnabled(true);
        addBikeButton.SetEnabled(true);
        addCarButton.SetEnabled(true);
        addMotorcycleButton.SetEnabled(true);
        worldSettingsButton.SetEnabled(true);
        exportButton.SetEnabled(true);
        loadButton.SetEnabled(true);
        saveButton.SetEnabled(true);
        homeButton.SetEnabled(true);
        exitButton.SetEnabled(true);
    }

    /// <summary>
    /// Initializes action buttons and their corresponding event listeners.
    /// </summary>
    private void initializeActionButtons()
    {
        actionButtonCanvas = GameObject.Find("ActionButtonCanvas");
        removeEntityButton = GameObject.Find("ActionButtonCanvas/DeleteButton").GetComponent<uGUI.Button>();
        editEntityButton = GameObject.Find("ActionButtonCanvas/EditButton").GetComponent<uGUI.Button>();
        snapToggle = GameObject.Find("ActionButtonCanvas/SnapToggle").GetComponent<uGUI.Toggle>();

        removeEntityButton.onClick.AddListener(() =>
        {
            if (freeze) return;
            selectedEntity?.destroy();
            setSelectedEntity(null);
        });

        editEntityButton.onClick.AddListener(() =>
        {
            if (freeze) return;
            this.selectedEntity?.openEditDialog();
        });

        snapToggle.onValueChanged.AddListener(x =>
        {
            if (freeze) return;
            if (!helpComplete[0])
            {
                string text = "You've changed the Tied-Path-Option.\n"
                + "If activated: The movement of this entity/waypoint is no longer tied to the road.\n"
                + "If deactivated: You can move this entity/waypoint freely now.\n"
                + "You can change this anytime by clicking the white icon again.";
                helpPopupController.open("Tied path Explanation", text, 0);
            }
            this.selectedEntity?.setIgnoreWaypoints(!x);
        });

        actionButtonCanvas.SetActive(false);
    }

    /// <summary>
    /// Initializes the event list and sets up the related event handlers.
    /// </summary>
    /// <param name="editorGUI">The parent VisualElement for the event list.</param>
    private void initializeEventList(VisualElement editorGUI)
    {
        // Store a reference to the character list element
        eventList = editorGUI.Q<ListView>("vehicle-list");

        // Set up a make item function for a list entry
        eventList.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = eventEntryTemplate.Instantiate();

            // Instantiate a controller for the data
            var entryController = new VehicleListEntryController();

            // Assign the controller script to the visual element
            newListEntry.userData = entryController;

            // Initialize the controller script
            entryController.SetVisualElement(newListEntry);

            // Return the root of the instantiated visual tree
            return newListEntry;
        };

        // Set up bind function for a specific list entry
        eventList.bindItem = (item, index) =>
        {
            List<BaseEntity> allEntities = info.allEntities;
            (item.userData as VehicleListEntryController).setEventData(allEntities.ElementAt(index));
        };

        info.Vehicles.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs args) =>
        {
            eventList.itemsSource = info.allEntities;
            refreshEntityList();
        };

        info.Pedestrians.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs args) =>
        {
            eventList.itemsSource = info.allEntities;
            refreshEntityList();
        };
    }

    /// <summary>
    /// Refreshes the Entity list.
    /// </summary>
    public void refreshEntityList()
    {
        this.eventList.Rebuild();
    }

    /// <summary>
    /// Coroutine to save the ScenarioInfo in the specified format (binary or XML).
    /// </summary>
    /// <param name="exportInfo">The ScenarioInfo object to be saved.</param>
    /// <param name="binary">A boolean value indicating whether to save in binary format (true) or XML format (false).</param>
    /// <returns>Returns an IEnumerator for the coroutine.</returns>
    IEnumerator saveScenarioInfoWrapper(ScenarioInfo exportInfo, bool binary)
    {
        // Set the default extension
        string defaultExtension = binary ? ".sced" : ".xosc";
        string defaultFilterString = binary ? "SCED Files (.sced)" : "XOSC Files (.xosc)";

        // Set the default filter
        FileBrowser.SetFilters(true, new FileBrowser.Filter(defaultFilterString, defaultExtension));
        FileBrowser.SetDefaultFilter(defaultExtension);

        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files);
        
        if (FileBrowser.Success)
        {
            string filePath = FileBrowser.Result[0];

            // Check if the file has the correct extension, otherwise append it
            if (System.IO.Path.GetExtension(filePath) != defaultExtension)
            {
                filePath += defaultExtension;
            }

            if (!binary)
            {
                exportInfo.Path = filePath;
                BuildXML doc = new BuildXML(exportInfo);
                doc.CombineXML();
            }
            else
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using FileStream stream = new FileStream(filePath, FileMode.Create);
                formatter.Serialize(stream, info);
            }
        }
    }

    /// <summary>
    /// Coroutine to load the binary ScenarioInfo file.
    /// </summary>
    /// <returns>Returns an IEnumerator for the coroutine.</returns>
    IEnumerator loadBinaryScenarioInfoWrapper()
    {

        // Set the filter to only allow .sced files
        var scedFilter = new FileBrowser.Filter("SCED Files (.sced)", "sced");
        FileBrowser.SetFilters(true, scedFilter);
        FileBrowser.SetDefaultFilter("sced");

        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files);
        if(FileBrowser.Success)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(FileBrowser.Result[0], FileMode.Open))
                {
                    ScenarioInfo obj = (ScenarioInfo)formatter.Deserialize(stream);
                    loadScenarioInfo(obj);
                }
            }
            catch (System.Exception)
            {
                Debug.Log("Error while loading binary file, are you sure the file exists?");
            }
        }
    }


    /// <summary>
    /// Exports the current scenario upon clicking the "Export" button.
    /// </summary>
    void ExportOnClick()
    {
        //Uncomment to test Loading and Saving
        //SaveBinaryScenarioInfo(info);
        //LoadBinaryScenarioInfo();

        // Catch errors and display it to the user
        if (info.EgoVehicle == null)
        {
            this.setSelectedEntity(null);
            string title = "No AI vehicle placed";
            string description = "You must place a vehicle first!";
            this.warningPopupController.open(title, description);
            freeze = false;
            return;
        }   

        //Creates a deepcopy of the ScenarioInfo object. This is done to prevent the fixes here to change the original object and lead to problems. 
        ScenarioInfo exportInfo = (ScenarioInfo)info.Clone();

        // use the following line to use the original object to export, for troubleshooting if the fault is maybe with the cloning
        //exportInfo = this.info;
        
        // To have right number format e.g. 80.5 instead of 80,5
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        // ------------------------------------------------------------------------

        exportInfo.EgoVehicle.getCarlaLocation();

        // ------------------------------------------------------------------------
        
        // Create .xosc file
        StartCoroutine(saveScenarioInfoWrapper(exportInfo, false));
    }

    /// <summary>
    /// Loads the binary ScenarioInfo file.
    /// </summary>
    private void LoadBinaryScenarioInfo()
    {
        StartCoroutine(loadBinaryScenarioInfoWrapper());
    }

    /// <summary>
    /// Saves the ScenarioInfo in binary format.
    /// </summary>
    /// <param name="info">The ScenarioInfo object to be saved.</param>
    private void SaveBinaryScenarioInfo(ScenarioInfo info)
    {
        StartCoroutine(saveScenarioInfoWrapper(info, true));
    }

    /// <summary>
    /// Returns the camera to the home position asynchronously.
    /// </summary>
    private async void ReturnToHome()
    {
        var result = await this.yesNoPopupController.Show("Change the map?", "Do you really want to change the map?\nMake sure to save everything! Unsaved changes WILL be lost!");
        if (result == true)
        {
            var m = Camera.main.GetComponent<CameraMovement>();
            m.Home();
        }
    }

    /// <summary>
    /// Quits the application asynchronously after prompting the user for confirmation.
    /// </summary>
    private async void QuitApplication()
    {
        var result = await this.yesNoPopupController.Show("Quit?", "Do you really want to Quit the Application?\nMake sure to save everything! Unsaved Changes WILL be lost!");

        if (result == true)
        {
            Application.Quit();
            Debug.Log("Quitting");
        }
    }

   

}

