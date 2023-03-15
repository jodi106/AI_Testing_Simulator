using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UIElements;
using ExportScenario.XMLBuilder;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using uGUI = UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public VisualTreeAsset eventEntryTemplate;
    //Spawned Cars
    public GameObject vehiclePrefab;
    public GameObject egoPrefab;
    public GameObject pedPrefab;

    //Event Bar (Center Bottom)
    private ListView eventList;
    private Button addCarButton;
    private Button addMotorcycleButton;
    private Button addBikeButton;
    private Button addPedestrianButton;
    private Button worldSettingsButton;
    private Button exportButton;
    private Button homeButton;

    private Button menuButton;

    //Action Buttons (Center Left)
    private uGUI.Button removeEntityButton;
    private uGUI.Button editEntityButton;
    private uGUI.Toggle snapToggle;
    private GameObject actionButtonCanvas;

    private ScenarioInfo info;

    private IBaseController selectedEntity;

    private WorldSettingsPopupController worldSettingsController;
    private SnapController snapController;

    void Start()
    {
        this.info = new ScenarioInfo();
        this.info.onEgoChanged = () =>
        {
            refreshEntityList();
        };
        this.selectedEntity = null;
        var editorGUI = GameObject.Find("EditorGUI").GetComponent<UIDocument>().rootVisualElement;

        initializeEventList(editorGUI);
        initializeButtonBar(editorGUI);
        initializeActionButtons();

        this.snapController = Camera.main.GetComponent<SnapController>();
        EventManager.StartListening(typeof(MouseClickAction), x =>
        {
            if (!snapController.IgnoreClicks)
            {
                this.setSelectedEntity(null);
            }
        });

        GameObject popups = GameObject.Find("PopUps");
        this.worldSettingsController = popups.transform.Find("WorldSettingsPopUpAdvanced").gameObject.GetComponent<WorldSettingsPopupController>();
        this.worldSettingsController.init(this.info.WorldOptions);
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
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

    public void addVehicle(Vehicle vehicle)
    {
        this.info.Vehicles.Add(vehicle);
        
    }

    public void addPedestrian(Pedestrian pedestrian)
    {
        this.info.Pedestrians.Add(pedestrian);
    }

    public void removeVehicle(Vehicle vehicle)
    {
        this.info.Vehicles.Remove(vehicle);
    }

    public void removePedestrian(Pedestrian pedestrian)
    {
        this.info.Pedestrians.Remove(pedestrian);
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
        VehicleViewController viewController = null;
        if (this.info.EgoVehicle is null)
        {
            viewController = vehicleGameObject.GetComponent<EgoViewController>();
        }
        else
        {
            AdversaryViewController adversaryController = vehicleGameObject.GetComponent<AdversaryViewController>();
            //TODO fix model; make ego a vehicle and make controllers return vehicles
            adversaryController.setCategory(category);
            viewController = adversaryController;
        }
        Color color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        color = new Color(color.r, color.g, color.b, 1);
        viewController.getEntity().setColor(color);
    }

    private void initializeButtonBar(VisualElement editorGUI)
    {
        addPedestrianButton = editorGUI.Q<Button>("addPedestrianButton");
        addCarButton = editorGUI.Q<Button>("addCarButton");
        addMotorcycleButton = editorGUI.Q<Button>("addMotorcycleButton");
        addBikeButton = editorGUI.Q<Button>("addBikeButton");
        worldSettingsButton = editorGUI.Q<Button>("worldSettingsButton");
        exportButton = editorGUI.Q<Button>("exportButton");
        homeButton = editorGUI.Q<Button>("homeButton");

        addCarButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            createEntity(VehicleCategory.Car);
            setSelectedEntity(null);
        });

        addBikeButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            createEntity(VehicleCategory.Bike);
            setSelectedEntity(null);
        });

        addMotorcycleButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            createEntity(VehicleCategory.Motorcycle);
            setSelectedEntity(null);
        });

        addPedestrianButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            createEntity(VehicleCategory.Pedestrian);
            setSelectedEntity(null);
        });

        worldSettingsButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            worldSettingsController.open();
        });

        exportButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            ExportOnClick();
        });

        homeButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            var m = Camera.main.GetComponent<CameraMovement>();
            m.Home();
        });
    }

    private void initializeActionButtons()
    {
        actionButtonCanvas = GameObject.Find("ActionButtonCanvas");
        removeEntityButton = GameObject.Find("ActionButtonCanvas/DeleteButton").GetComponent<uGUI.Button>();
        editEntityButton = GameObject.Find("ActionButtonCanvas/EditButton").GetComponent<uGUI.Button>();
        snapToggle = GameObject.Find("ActionButtonCanvas/SnapToggle").GetComponent<uGUI.Toggle>();

        removeEntityButton.onClick.AddListener(() =>
        {
            selectedEntity?.destroy();
            setSelectedEntity(null);
        });

        editEntityButton.onClick.AddListener(() =>
        {
            this.selectedEntity?.openEditDialog();
        });

        snapToggle.onValueChanged.AddListener(x =>
        {
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
            (item.userData as VehicleListEntryController).setEventData(info.allEntities.ElementAt(index));
        };

        info.Vehicles.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs args) =>
        {
            refreshEntityList();
        };

        info.Pedestrians.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs args) =>
        {
            refreshEntityList();
        };

        eventList.itemsSource = info.allEntities;
    }

    public void refreshEntityList()
    {
        this.eventList.Rebuild();
    }

    //This Function is bind with the "Export button". The actual binding is made in the Start function of the script
    //Anything written here will be run at the time of pressing "Export" Button
    void ExportOnClick()
    {
        // Catch errors and display it to the user
        if (info.EgoVehicle == null)
        {
            EditorUtility.DisplayDialog(
                "No AI vehicle placed",
                "You must place a vehicle first!",
                "Ok");
            return;
        }

        //Creates a Copy of the exportInfo, so that
        //ScenarioInfo exportInfo = (ScenarioInfo)info.Clone();

        // To have right number format e.g. 80.5 instead of 80,5
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        // ------------------------------------------------------------------------
        // TODO remove these lines later once these values are set in Unity
        info.MapURL = "Town10HD";
        // ------------------------------------------------------------------------
        // Required to create AssignRouteAction and coordinate conversion (do not delete this!) 
        foreach (Vehicle vehicle in info.Vehicles)
        {
            vehicle.getCarlaLocation();
            if (vehicle.Path is not null && !isWaypointListEmptyOrNull(vehicle))
            {
                vehicle.Path.InitAssignRouteWaypoint(vehicle.SpawnPoint.Rot);
            }
        }
        info.EgoVehicle.getCarlaLocation();
        // ------------------------------------------------------------------------

        // Create .xosc file
        info.Path = EditorUtility.SaveFilePanel("Save created scenario as .xosc file", "", "scenario", "xosc");
        //info.Path = "OurScenario33.xosc"; // only for faster testing: disable explorer
        if (info.Path.Length > 0) // "save" is pressed in explorer
        {
            BuildXML doc = new BuildXML(info);
            doc.CombineXML();
        }
    }

    private bool isWaypointListEmptyOrNull(Vehicle vehicle)
    {
        //return (vehicle.Path.WaypointList is not null && vehicle.Path.WaypointList.Count >= 1);
        return vehicle.Path.WaypointList?.Any() != true;
    }
}
