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
    private Button removeEntityButton;
    private Button editEntityButton;
    private Button toggleSnapButton;
    private Button worldSettingsButton;
    private Button exportButton;
    private Button homeButton;

    //Action Buttons (Center Left)
    private VisualElement actionButtons;

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
        initializeEventButtons(editorGUI);

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
            this.selectedEntity?.deselect();
            this.selectedEntity = entity;
            this.selectedEntity?.select();
            this.removeEntityButton.style.display = DisplayStyle.Flex;
            this.editEntityButton.style.display = DisplayStyle.Flex;
            this.toggleSnapButton.style.display = DisplayStyle.Flex;
            this.actionButtons.style.display = DisplayStyle.Flex;
        }
        else
        {
            this.selectedEntity?.deselect();
            this.selectedEntity = null;
            removeEntityButton.style.display = DisplayStyle.None;
            editEntityButton.style.display = DisplayStyle.None;
            toggleSnapButton.style.display = DisplayStyle.None;
            this.actionButtons.style.display = DisplayStyle.None;
        }

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
        removeEntityButton = editorGUI.Q<Button>("removeEntityButton");
        editEntityButton = editorGUI.Q<Button>("editEntityButton");
        toggleSnapButton = editorGUI.Q<Button>("toggleSnapButton");
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

        removeEntityButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            selectedEntity?.destroy();
            setSelectedEntity(null);
        });

        editEntityButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            this.selectedEntity?.openEditDialog();
        });

        toggleSnapButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            this.selectedEntity?.setIgnoreWaypoints(!this.selectedEntity.shouldIgnoreWaypoints());
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

    private void initializeEventButtons(VisualElement editorGUI)
    {
        actionButtons = editorGUI.Q<VisualElement>("actionButtons");
        actionButtons.style.display = DisplayStyle.None;
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

    //TODO remove once routes are working
    void addWaypointsAfter4Meters(ScenarioInfo info)
    {
        foreach(var v in info.Vehicles)
        {
            var first = v.Path.WaypointList[0].Location.Vector3;
            var second = v.Path.WaypointList[1].Location.Vector3;
            var middle = (second - first).normalized + first; // 4 m * 25 pixel/m * 100 pixel/unit
            var waypoint = new Waypoint(new Location(middle), new ActionType("MoveToAction"), new List<TriggerInfo>());
            v.Path.WaypointList.Insert(1,waypoint);
        }
    }

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

        addWaypointsAfter4Meters(this.info);

        //This Function is bind with the "Export button"
        //The actual binding is made in the Start function of the script
        //Make sure to change this function to export the desired version or desired files.
        //Anything written here will be run at the time of pressing "Export" Button

        // To have right number format e.g. 80.5 instead of 80,5
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        // ------------------------------------------------------------------------
        // TODO remove these lines later once these values are set in Unity
        info.MapURL = "Town10HD";
        foreach (Vehicle veh in info.Vehicles)
        {
            veh.InitialSpeed = 10;
        }
        // ------------------------------------------------------------------------
        // Required to create AssignRouteAction and coordinate conversion (do not delete this!) 
        foreach (Vehicle vehicle in info.Vehicles)
        {
            vehicle.getCarlaLocation();
            if (vehicle.Path is not null && !isWaypointListEmptyOrNull(vehicle))
            {
                vehicle.Path.InitAssignRouteWaypoint();
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
