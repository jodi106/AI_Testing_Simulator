using Assets.Enums;
using Assets.Repos;
using Entity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ExportScenario.XMLBuilder;
using System.Collections.ObjectModel;

public class MainController : MonoBehaviour
{
    public VisualTreeAsset eventEntryTemplate;
    //Spawned Cars
    public GameObject carPrefab;

    //Event Bar (Center Bottom)
    private ListView eventList;
    private Button addEntityButton;
    private Button removeEntityButton;
    private Button editEntityButton;
    private Button worldSettingsButton;

    //Action Buttons (Center Left)
    private VisualElement actionButtons;
    private Button setPathButton;
    private Button deletePathButton;
    private Button cancelPathButton;
    private Button submitPathButton;

    private ScenarioInfo info;

    private IBaseEntityController selectedEntity;
    private bool preventDeselection;

    //Car Settings Popup
    [SerializeField]
    GameObject VehicleSettingsPopup;

    //ExportButton
    [SerializeField]
    public UnityEngine.UI.Button exportButton;

    public GameObject WorldSettingsPopup;

    void Start()
    {
        this.info = new ScenarioInfo();
        this.selectedEntity = null;
        this.preventDeselection = false;
        var editorGUI = GameObject.Find("EditorGUI").GetComponent<UIDocument>().rootVisualElement;

        initializeEventList(editorGUI);
        initializeButtonBar(editorGUI);
        initializeEventButtons(editorGUI);
        initializeWorldSettingsPopUp();

        EventManager.StartListening(typeof(ChangeSelectedEntityAction), x =>
        {
            var action = new ChangeSelectedEntityAction(x);
            this.setSelectedEntity(action.entity);
        });

        EventManager.StartListening(typeof(MouseClickAction), x =>
        {
            if (!preventDeselection)
            {
                EventManager.TriggerEvent(new ChangeSelectedEntityAction(ChangeSelectedEntityAction.NONE));
            }
        });

        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {
            this.preventDeselection = false;
        });

        EventManager.StartListening(typeof(SubmitPathSelectionAction), x =>
        {
            this.preventDeselection = false;
            deletePathButton.style.display = DisplayStyle.Flex;
            submitPathButton.style.display = DisplayStyle.None;
            cancelPathButton.style.display = DisplayStyle.None;
        });

        UnityEngine.UI.Button btn = exportButton.GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(ExportOnClick);
    }

    private void setSelectedEntity(IBaseEntityController entity)
    {
        if (entity != null)
        {
            this.selectedEntity?.deselect();
            this.selectedEntity = entity;
            this.selectedEntity?.select();
            this.removeEntityButton.style.display = DisplayStyle.Flex;
            this.editEntityButton.style.display = DisplayStyle.Flex;
            this.actionButtons.style.display = DisplayStyle.Flex;
            if (entity.hasAction())
            {
                this.setPathButton.style.display = DisplayStyle.None;
                this.deletePathButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                this.setPathButton.style.display = DisplayStyle.Flex;
                this.deletePathButton.style.display = DisplayStyle.None;
            }
            this.submitPathButton.style.display = DisplayStyle.None;
            this.cancelPathButton.style.display = DisplayStyle.None;
        }
        else
        {
            this.selectedEntity?.deselect();
            this.selectedEntity = null;
            removeEntityButton.style.display = DisplayStyle.None;
            editEntityButton.style.display = DisplayStyle.None;
            this.actionButtons.style.display = DisplayStyle.None;
        }

    }

    public void addVehicle(Vehicle vehicle)
    {
        Debug.Log("Vehicle added");
        this.info.Vehicles.Add(vehicle);
        this.preventDeselection = false;
    }

    private void initializeButtonBar(VisualElement editorGUI)
    {
        addEntityButton = editorGUI.Q<Button>("addEntityButton");
        removeEntityButton = editorGUI.Q<Button>("removeEntityButton");
        editEntityButton = editorGUI.Q<Button>("editEntityButton");
        worldSettingsButton = editorGUI.Q<Button>("worldSettingsButton");

        addEntityButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            var pos = Input.mousePosition;
            pos.z = -0.1f;
            var vehicleGameObject = Instantiate(carPrefab, pos, Quaternion.identity);
            var viewController = vehicleGameObject.GetComponent<AdversaryViewController>();
            UnityEngine.Color color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            color = new UnityEngine.Color(color.r, color.g, color.b, 1);
            viewController.setColor(color);
            setSelectedEntity(viewController);
            this.preventDeselection = true;
        });

        removeEntityButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            var entity = selectedEntity.getEntity();
            //TODO: check how to prevent type checking here
            if (entity is Vehicle vehicle)
            {
                info.Vehicles.Remove(vehicle);
            }
            selectedEntity.destroy();
            setSelectedEntity(null);
        });

        editEntityButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            VehicleSettingsPopup.SetActive(true);
            var ThePopup = VehicleSettingsPopup.GetComponent<UIDocument>().rootVisualElement;

            Button ExitButton = ThePopup.Q<Button>("ExitButton");
            ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                VehicleSettingsPopup.SetActive(false);
            });

            Button SaveButton = ThePopup.Q<Button>("SaveButton");
            SaveButton.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                ///!!!!!!!
                ///Write here the code for saving the data to scenarioInfoFile later on...
                ///!!!!!!!
                Debug.Log("Saving Data of the Vehicle with id:" + selectedEntity);
                VehicleSettingsPopup.SetActive(false);
            });

            //Vehicle vehicle = selectedEntity.getEntity();
            var spawnPoint = new Location(new Vector3(1, 1, 1), 1);

            var vehicleModelRepo = new VehicleModelRepository();

            var vehicleModels = vehicleModelRepo.GetModelsBasedOnCategory(VehicleCategory.Car);

            Vehicle vehicle = new Vehicle(spawnPoint, vehicleModels[1], VehicleCategory.Car);

            var iDField = ThePopup.Q<IntegerField>("ID");
            iDField.SetValueWithoutNotify(vehicle.Id);

            iDField.RegisterCallback<InputEvent>((InputEvent) =>
            {
                vehicle.Id = Int32.Parse(InputEvent.newData);
            });

            var CarSpeed = ThePopup.Q<IntegerField>("CarSpeed");

            CarSpeed.RegisterCallback<InputEvent>((InputEvent) =>
            {
                if (CarSpeed.value >= 500)
                {
                    Debug.Log("HEHE! TO BIG VALUE FOR CAR SPEED , WE DON'T HAVE BUGATTI'S HERE! ONLY AUDI'S...");
                }
                else if (CarSpeed.value < 0)
                {
                    Debug.Log("HEHE! TO SMALL FOR CAR SPEED , WE DON'T HAVE TURTLES HERE! ONLY AUDI'S...");
                }
                else
                {
                    Debug.Log("Set Car Speed at: " + CarSpeed.value);
                }
            });

            List<string> allPossibleModels = new List<string> { };
            var possibleModelsField = ThePopup.Q<DropdownField>("AllPossibleModels");
            possibleModelsField.choices = allPossibleModels;
            possibleModelsField.RegisterValueChangedCallback((evt) =>
            {
                Debug.Log("New Model: " + evt.newValue);
            });

            
            List<string> allPossibleCateogories = new List<string> {};
            foreach (var option in Enum.GetValues(typeof(VehicleCategory)))
            {
                allPossibleCateogories.Add(option.ToString());
            }
            var possibleCategoriesField = ThePopup.Q<DropdownField>("AllPossibleCategories");
            possibleCategoriesField.choices = allPossibleCateogories;
            possibleCategoriesField.RegisterValueChangedCallback((evt) =>
            {
                if(evt.newValue=="Car")
                {
                    List<string> allPossibleModels = new List<string> { };
                    foreach (var option in vehicleModelRepo.GetModelsBasedOnCategory(VehicleCategory.Car))
                    {
                        allPossibleModels.Add(option.DisplayName);
                    }
                    var possibleModelsField = ThePopup.Q<DropdownField>("AllPossibleModels");
                    possibleModelsField.choices = allPossibleModels;
                }
                if (evt.newValue == "Bike")
                {
                    List<string> allPossibleModels = new List<string> { };
                    foreach (var option in vehicleModelRepo.GetModelsBasedOnCategory(VehicleCategory.Bike))
                    {
                        allPossibleModels.Add(option.DisplayName);
                    }
                    var possibleModelsField = ThePopup.Q<DropdownField>("AllPossibleModels");
                    possibleModelsField.choices = allPossibleModels;
                }
                if (evt.newValue == "Motorcycle")
                {
                    List<string> allPossibleModels = new List<string> { };
                    foreach (var option in vehicleModelRepo.GetModelsBasedOnCategory(VehicleCategory.Motorcycle))
                    {
                        allPossibleModels.Add(option.DisplayName);
                    }
                    var possibleModelsField = ThePopup.Q<DropdownField>("AllPossibleModels");
                    possibleModelsField.choices = allPossibleModels;
                }
                if (evt.newValue == "Null")
                {
                    List<string> allPossibleModels = new List<string> { };
                    var possibleModelsField = ThePopup.Q<DropdownField>("AllPossibleModels");
                    possibleModelsField.choices = allPossibleModels;
                }
            });

            Vector3Field SpawnPointField = ThePopup.Q<Vector3Field>("SpawnPoint");

            SpawnPointField.SetValueWithoutNotify(vehicle.SpawnPoint.Vector3);

            SpawnPointField.RegisterCallback<InputEvent>((InputEvent) =>
            {
                Debug.Log("spawnpoint " + InputEvent.newData);

            });

            var xcoord = SpawnPointField.Q<FloatField>("unity-x-input");

            xcoord.RegisterCallback<InputEvent>((InputEvent) =>
            {
                Debug.Log("xfield " + InputEvent.newData);

            });
            var vehicleModelList = ThePopup.Q<ListView>("VehicleModelList");
        });

        worldSettingsButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            WorldSettingsPopup.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
        });
    }

    private void initializeEventButtons(VisualElement editorGUI)
    {
        setPathButton = editorGUI.Q<Button>("setPathButton");
        deletePathButton = editorGUI.Q<Button>("deletePathButton");
        cancelPathButton = editorGUI.Q<Button>("cancelPathButton");
        submitPathButton = editorGUI.Q<Button>("submitPathButton");

        actionButtons = editorGUI.Q<VisualElement>("actionButtons");

        setPathButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            this.selectedEntity.triggerActionSelection();
            this.preventDeselection = true;
            setPathButton.style.display = DisplayStyle.None;
            cancelPathButton.style.display = DisplayStyle.Flex;
            submitPathButton.style.display = DisplayStyle.Flex;
        });

        deletePathButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            this.selectedEntity.deleteAction();
            setPathButton.style.display = DisplayStyle.Flex;
            deletePathButton.style.display = DisplayStyle.None;
        });

        cancelPathButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            EventManager.TriggerEvent(new CancelPathSelectionAction());
        });

        submitPathButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            EventManager.TriggerEvent(new SubmitPathSelectionAction());
        });

        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {

            setPathButton.style.display = DisplayStyle.Flex;
            cancelPathButton.style.display = DisplayStyle.None;
            submitPathButton.style.display = DisplayStyle.None;
        });

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
            (item.userData as VehicleListEntryController).setEventData(info.Vehicles[index]);
        };

        info.Vehicles.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs args) =>
        {
            this.eventList.Rebuild();
        };

        eventList.itemsSource = info.Vehicles;
    }

    void initializeWorldSettingsPopUp()
    {
        WorldSettingsPopup.SetActive(true);
        WorldSettingsPopup.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
        var popUp = WorldSettingsPopup.GetComponent<UIDocument>().rootVisualElement;

        var exitButton = popUp.Q<Button>("Exit");
        exitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            WorldSettingsPopup.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
        });

        var dayTime = popUp.Q<TextField>("Daytime");
        dayTime.RegisterCallback<KeyDownEvent>((KeyDownEvent) =>
        {
            if (KeyDownEvent.keyCode == KeyCode.Return)
            {
                //Debug.Log("Daytime: "+dayTime.text);
                this.info.WorldOptions.Date_Time = (string)dayTime.text;
            }
        });

        var sunIntesity = popUp.Q<Slider>("SunIntensity");
        sunIntesity.RegisterValueChangedCallback((evt) =>
        {
            //Debug.Log("Sun Intensity: "+evt.newValue);
            this.info.WorldOptions.SunIntensity = (float)evt.newValue;
        });

        List<string> cloudStateOptions = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(CloudState)))
        {
            cloudStateOptions.Add(option.ToString());
        }
        var cloudState = popUp.Q<DropdownField>("CloudState");
        cloudState.choices = cloudStateOptions;
        cloudState.RegisterValueChangedCallback((evt) =>
        {
            int index = cloudStateOptions.IndexOf(evt.newValue);
            CloudState userOption = (CloudState)index;
            //Debug.Log("Cloud State: " + userOption);
            this.info.WorldOptions.CloudState = userOption;
        });

        List<string> precipitationTypeOptions = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(PrecipitationType)))
        {
            precipitationTypeOptions.Add(option.ToString());
        }
        var precipitationType = popUp.Q<DropdownField>("PrecipitationType");
        precipitationType.choices = precipitationTypeOptions;
        precipitationType.RegisterValueChangedCallback((evt) =>
        {
            int index = precipitationTypeOptions.IndexOf(evt.newValue);
            PrecipitationType userOption = (PrecipitationType)index;
            //Debug.Log("Precipitation Type: " + userOption);
            this.info.WorldOptions.PrecipitationType = userOption;
        });

        var precipitationIntesity = popUp.Q<Slider>("PrecipitationIntensity");
        precipitationIntesity.RegisterValueChangedCallback((evt) =>
        {
            //Debug.Log("Precipitation Intensity: " + precipitationIntesity.showInputField + " " + evt.newValue);
            this.info.WorldOptions.PrecipitationIntensity = (float)evt.newValue;
        });

        var sunAzimuth = popUp.Q<Slider>("SunAzimuth");
        sunAzimuth.RegisterValueChangedCallback((evt) =>
        {
            //Debug.Log("Sun Azimuth: " + sunAzimuth.showInputField + " " + evt.newValue);
            this.info.WorldOptions.SunAzimuth = (double)evt.newValue;
        });

        var sunElevation = popUp.Q<Slider>("SunElevation");
        sunElevation.RegisterValueChangedCallback((evt) =>
        {
            //Debug.Log("Sun Elevation: " + sunElevation.showInputField + " " + evt.newValue);
            this.info.WorldOptions.SunElevation = (double)evt.newValue;
        });

        var fogVisualRange = popUp.Q<LongField>("FogVisualRange");
        fogVisualRange.RegisterCallback<KeyDownEvent>((KeyDownEvent) =>
        {
            if (KeyDownEvent.keyCode == KeyCode.Return)
            {
                //Debug.Log("Fog Visual Range: " + fogVisualRange.text);
                this.info.WorldOptions.FogVisualRange = (double)fogVisualRange.value;
            }
        });

        var frictionScaleFactor = popUp.Q<FloatField>("FrictionScaleFactor");
        frictionScaleFactor.RegisterCallback<KeyDownEvent>((KeyDownEvent) =>
        {
            if (KeyDownEvent.keyCode == KeyCode.Return)
            {
                //Debug.Log("Friction Scale Factor: " + frictionScaleFactor.value);
                this.info.WorldOptions.FrictionScaleFactor = (double)frictionScaleFactor.value;
            }
        });
        var simpleSettingsButton = popUp.Q<Button>("SimpleSettings");
        var advancedSettingsButton = popUp.Q<Button>("AdvancedSettings");
        advancedSettingsButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            frictionScaleFactor.visible = true;
            fogVisualRange.visible = true;
            sunAzimuth.visible = true;
            sunIntesity.visible = true;
            sunElevation.visible = true;
            precipitationIntesity.visible = true;
            simpleSettingsButton.visible = true;
            advancedSettingsButton.visible = false;
        });

        
        simpleSettingsButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            frictionScaleFactor.visible = false;
            fogVisualRange.visible = false;
            sunAzimuth.visible = false;
            sunIntesity.visible = false;
            sunElevation.visible = false;
            precipitationIntesity.visible = false;
            simpleSettingsButton.visible = false;
            advancedSettingsButton.visible = true;
        });
    }

    void ExportOnClick()
    {
        //This Function is bind with the "Export button"
        //The actual binding is made in the Start function of the script
        //Make sure to change this function to export the desired version or desired files.
        //Anything written here will be run at the time of pressing "Export" Button

        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        // Global Weather
        WorldOptions worldOptions = new WorldOptions("2022-09-24T12:00:00", 100000, 0.85F, 0, 1.31, CloudState.Free, PrecipitationType.Dry, 0, 1.0);

        // Spawn AI Ego Vehicle
        Ego egoVehicle = new Ego(new Location(258.0f, -145.7f, 0.3f, 30), new EntityModel("vehicle.volkswagen.t2"), 0);


        // SIMULATION VEHICLES:

        // Define when to trigger a Waypoint Action. Each Waypoint has one TriggerList.
        List<TriggerInfo> triggerW1 = new List<TriggerInfo>();
        triggerW1.Add(new TriggerInfo("SimulationTimeCondition", 0, "greaterThan"));
        List<TriggerInfo> triggerW2 = new List<TriggerInfo>();
        triggerW2.Add(new TriggerInfo("DistanceCondition", "adversary2", "lessThan", 15, new Location(250f, 10f, 0.3f, 270)));
        List<TriggerInfo> triggerW3 = new List<TriggerInfo>();
        triggerW3.Add(new TriggerInfo("DistanceCondition", "adversary2", "lessThan", 15, new Location(100f, 10f, 0.3f, 270)));

        // Define what can happen on each Waypoint
        List<Waypoint> eventListAdversary2 = new List<Waypoint>();
        //eventListAdversary2.Add(new Waypoint( new Location(250, 10, 0.3f, 270), new ActionType("LaneChangeAction", "adversary2", 1, "linear", 25, "distance"), triggerW2));
        eventListAdversary2.Add(new Waypoint( new Location(100, 10, 0.3f, 270), new ActionType("SpeedAction", 20, "step", 10.0, "time"), triggerW1)); // 10s bc. otherwise scenario stops before vehicle stopped
        eventListAdversary2.Add(new Waypoint(new Location(100, 10, 0.3f, 270), new ActionType("SpeedAction", 20, "step", 10.0, "time"), triggerW1)); // 10s bc. otherwise scenario stops before vehicle stopped

        // Add the Waypoint List to the Path of a Vehicle
        Path path_veh_1 = new Path();
        // Update info:
        // when creating new Path object, list of positions from all Waypoints within the Path is created as RoutePositions attribute
        Path path_veh_2 = new Path(eventListAdversary2);
        // Set the Positions for AssignRouteAction. When first creating the Waypoint containing the AssignRouteAction, this is not possible as the other waypoints are not yet created.
        // This is why Waypoint1's ActionType "AssignRouteAction" is initialized with an empty list of Coordinates
        // With the current structure, AssignRouteAction always has to be the first Waypoint
        //path_veh_2.EventList[0].ActionTypeInfo.Positions = path_veh_2.getRoutePositions();
        // ToDo: Discuss how to effiently execute the process in line 56 when connecting gui and export. 

        // Spawn Simulation Vehicles with settings from above


        /*
        var vehicles = new ObservableCollection<Vehicle>();
        vehicles.Add(new Vehicle(new Location(300, -172, 0.3f, 160), new EntityModel("vehicle.audi.tt"), path_veh_1, initialSpeed: 20));
        vehicles.Add(new Vehicle(new Location(239, -169, 0.3f, 0), new EntityModel("vehicle.lincoln.mkz_2017"), path_veh_2, initialSpeed: 15.0));
        */


        // SIMULATION PEDESTRIANS:

        Path path_ped_1 = new Path();
        var ped = new ObservableCollection<Pedestrian>();
        ped.Add(new Pedestrian(new Location(255, -190, 0.8f, 90), new EntityModel("walker.pedestrian.0001"), path_ped_1, initialSpeed: 1));


        foreach (var veh in this.info.Vehicles)
        {

            veh.Model = new EntityModel("vehicle.audi.tt");
            var CarlaCoords = SnapController.UnityToCarla(veh.SpawnPoint.Vector3.x, veh.SpawnPoint.Vector3.y);
            var CarlaRot = SnapController.UnityRotToRadians(veh.SpawnPoint.Rot);

            veh.SpawnPoint.Vector3 = new Vector3(CarlaCoords.x, CarlaCoords.y, 0.3f);
            veh.SpawnPoint.Rot = CarlaRot;

            
            
            //veh.Path = new Path(eventListAdversary2);

            veh.InitialSpeed = 10.0;


        }
        

        // Combine every information into one ScenarioInfo Instance
        ScenarioInfo dummy = new ScenarioInfo("OurScenario3", ped, "Town10HD", worldOptions, egoVehicle, this.info.Vehicles);

        // Create .xosc file


        BuildXML doc = new BuildXML(dummy);
        doc.CombineXML();
    }


    //Only for Town06 later do as extension method for Vector3 or Location

}
