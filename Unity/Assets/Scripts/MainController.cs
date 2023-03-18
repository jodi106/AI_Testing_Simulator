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
using System.Runtime.Serialization.Formatters.Binary;
using SimpleFileBrowser;
using System.Collections;

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
    private VisualElement buttonBar;

    //Action Buttons (Center Left)
    private uGUI.Button removeEntityButton;
    private uGUI.Button editEntityButton;
    private uGUI.Toggle snapToggle;
    private GameObject actionButtonCanvas;

    public ScenarioInfo info { get; private set; }

    private IBaseController selectedEntity;

    private WorldSettingsPopupController worldSettingsController;
    private SnapController snapController;
    public  WarningPopupController warningPopupController;

    public static bool freeze = false; // if true, a popup GUI is open and the user shouln't change paths or vehicles!

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
            setSelectedEntity(null);
        });

        GameObject popups = GameObject.Find("PopUps");
        this.worldSettingsController = popups.transform.Find("WorldSettingsPopUpAdvanced").gameObject.GetComponent<WorldSettingsPopupController>();
        this.worldSettingsController.init(this.info.WorldOptions);

        this.warningPopupController = GameObject.Find("PopUps").transform.Find("WarningPopUp").gameObject.GetComponent<WarningPopupController>();
        this.warningPopupController.gameObject.SetActive(true);
    }

    public void loadScenarioInfo(ScenarioInfo info)
    {
        this.setSelectedEntity(null);
        info = (ScenarioInfo)info.Clone(); //Do we need this? Exported .bin Info already the one before export changes? - Stefan
        EventManager.TriggerEvent(new MapChangeAction(""));
        EventManager.TriggerEvent(new MapChangeAction("Town10HD"));//info.MapURL));
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

    public void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            this.setSelectedEntity(null);
        }
    }

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

    public void moveActionButtons(Vector2 pos)
    {
        this.actionButtonCanvas.transform.position = new Vector3(pos.x, (float)(pos.y - 0.5), -1f);
    }

    public void addSimulationEntity(Adversary entity)
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

    public void removeSimulationEntity(Adversary entity)
    {
        if(entity is Adversary v)
        {
            foreach (Waypoint w in v.Path.WaypointList)
            {
                if (w.StartRouteOfOtherVehicle is not null)
                {
                    Adversary otherVehicle = w.StartRouteOfOtherVehicle;
                    otherVehicle.StartRouteInfo = null;
                }
            }
            this.info.Vehicles.Remove(v);
        } else if (entity is Adversary p)
        {
            this.info.Pedestrians.Remove(p);
        }
    }

    public void setEgo(Ego ego)
    {
        this.info.setEgo(ego);
    }

    public void createEntity(VehicleCategory category)
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
    }

    private void initializeButtonBar(VisualElement editorGUI)
    {
        addPedestrianButton = editorGUI.Q<Button>("addPedestrianButton");
        addCarButton = editorGUI.Q<Button>("addCarButton");
        addMotorcycleButton = editorGUI.Q<Button>("addMotorcycleButton");
        addBikeButton = editorGUI.Q<Button>("addBikeButton");
        worldSettingsButton = editorGUI.Q<Button>("worldSettingsButton");
        exportButton = editorGUI.Q<Button>("exportButton");
        loadButton = editorGUI.Q<Button>("loadButton");
        saveButton = editorGUI.Q<Button>("saveButton");
        homeButton = editorGUI.Q<Button>("homeButton");
        buttonBar = editorGUI.Q<VisualElement>("buttons");
        

        addCarButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            createEntity(VehicleCategory.Car);
            setSelectedEntity(null);
        });

        addBikeButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            createEntity(VehicleCategory.Bike);
            setSelectedEntity(null);
        });

        addMotorcycleButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            createEntity(VehicleCategory.Motorcycle);
            setSelectedEntity(null);
        });

        addPedestrianButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            createEntity(VehicleCategory.Pedestrian);
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

        homeButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (freeze) return;
            var m = Camera.main.GetComponent<CameraMovement>();
            m.Home();
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

        buttonBar.visible = false;
    }

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
            this.selectedEntity?.setIgnoreWaypoints(!x);
        });

        actionButtonCanvas.SetActive(false);
    }

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

    public void refreshEntityList()
    {
        this.eventList.Rebuild();
    }

    IEnumerator openSaveDialogWrapper(ScenarioInfo exportInfo)
    {
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files);
        if (FileBrowser.Success)
        {
            exportInfo.Path = FileBrowser.Result[0];
            BuildXML doc = new BuildXML(exportInfo);
            doc.CombineXML();
        }
    }

    //This Function is bind with the "Export button". The actual binding is made in the Start function of the script
    //Anything written here will be run at the time of pressing "Export" Button
    void ExportOnClick()
    {
        //Uncomment to test Loading and Saving
        //SaveBinaryScenarioInfo(info);
        //LoadBinaryScenarioInfo();

        // Catch errors and display it to the user
        if (info.EgoVehicle == null)
        {
            //#if UNITY_EDITOR
            //EditorUtility.DisplayDialog(
            //    "No AI vehicle placed",
            //    "You must place a vehicle first!",
            //    "Ok");
            //#else

            this.setSelectedEntity(null);
            string title = "No AI vehicle placed";
            string description = "You must place a vehicle first!";
            this.warningPopupController.open(title, description);
            freeze = false;

            //#endif

            return;
        }   

        //Creates a deepcopy of the ScenarioInfo object. This is done to prevent the fixes here to change the original object and lead to problems. 
        ScenarioInfo exportInfo = (ScenarioInfo)info.Clone();

        // use the following line to use the original object to export, for troubleshooting if the fault is maybe with the cloning
        //exportInfo = this.info;
        
        // To have right number format e.g. 80.5 instead of 80,5
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        // ------------------------------------------------------------------------
        // TODO remove these lines later once these values are set in Unity
        exportInfo.MapURL = "Town10HD";

        // ------------------------------------------------------------------------

        exportInfo.EgoVehicle.getCarlaLocation();
        // ------------------------------------------------------------------------

        // Create .xosc file


        StartCoroutine(openSaveDialogWrapper(exportInfo));
    }

    private void LoadBinaryScenarioInfo()
    {

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream("data.bin", FileMode.Open))
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

    private void SaveBinaryScenarioInfo(ScenarioInfo info)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream("data.bin", FileMode.Create))
        {
            formatter.Serialize(stream, info);
        }
    }
    //private bool isWaypointListEmptyOrNull(Vehicle vehicle)
    //{
    //    //return (vehicle.Path.WaypointList is not null && vehicle.Path.WaypointList.Count >= 1);
    //    return vehicle.Path.WaypointList?.Any() != true;
    //}
}

