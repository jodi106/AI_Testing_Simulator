using Assets.Enums;
using Assets.Repos;
using Entity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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
    private Button cancelPathButton;

    private ScenarioInfo info;

    private IBaseEntityController selectedEntity;
    private bool preventDeselection;

    //Car Settings Popup
    [SerializeField]
    GameObject VehicleSettingsPopup;

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
            if(!preventDeselection)
            {
                EventManager.TriggerEvent(new ChangeSelectedEntityAction(ChangeSelectedEntityAction.NONE));
            }
        });

        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {
            this.preventDeselection = false;
        });
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

            var vehiclePosition = new Location(pos.x, pos.y, 0, 0);
            var path = new Path(new List<Waypoint>());
            Vehicle v = new(vehiclePosition, path, category:VehicleCategory.Car);

            var viewController = vehicleGameObject.GetComponent<VehicleViewController>();
            v.View = viewController;
            viewController.vehicle = v;

            setSelectedEntity(viewController);

            info.Vehicles.Add(v);
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
                ///Code for saving the data to scenarioInfoFile later on...
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
            //METHOD WITH RADIO GROUP
            var group = ThePopup.Q<RadioButtonGroup>("AllPossibleModels");
            var allmodels = vehicleModelRepo.GetModelsBasedOnCategory(VehicleCategory.Car);
            foreach(var model in allmodels)
            {
                var newRadio = new RadioButton(model.DisplayName);
                group.Add(newRadio);
            }

            /*
            //METHOD WITH LIST VIEW
            // Store a reference to the character list element
            var listView = ThePopup.Q<ListView>("AllPossibleModels");
            var allModels = vehicleModelRepo.GetModelsBasedOnCategory(VehicleCategory.Car);
            
            // Set up a make item function for a list entry
            listView.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                var newListEntry = eventEntryTemplate.Instantiate();

                // Instantiate a controller for the data
                var entryController = new Label();

                // Assign the controller script to the visual element
                newListEntry.userData = entryController;

                // Initialize the controller script
                entryController.text = "B";

                // Return the root of the instantiated visual tree
                return newListEntry;
            };

            // Set up bind function for a specific list entry
            listView.bindItem = (item, index) =>
            {
                (item.userData as Label).text = "A"+index;
            };

            info.Vehicles.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs args) =>
            {
                listView.Rebuild();
            };

            listView.itemsSource = allModels;*/


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
        cancelPathButton = editorGUI.Q<Button>("cancelPathButton");
        actionButtons = editorGUI.Q<VisualElement>("actionButtons");

        setPathButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            this.selectedEntity.triggerPathRequest();
            this.preventDeselection = true;
            setPathButton.style.display = DisplayStyle.None;
            cancelPathButton.style.display = DisplayStyle.Flex;
        });

        cancelPathButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            EventManager.TriggerEvent(new CancelPathSelectionAction());
        });

        EventManager.StartListening(typeof(CancelPathSelectionAction), x =>
        {
            setPathButton.style.display = DisplayStyle.Flex;
            cancelPathButton.style.display = DisplayStyle.None;
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
        var popup = WorldSettingsPopup.GetComponent<UIDocument>().rootVisualElement;
        var exitButton = popup.Q<Button>("Exit");

        exitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            WorldSettingsPopup.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
        });

        WorldSettingsPopup.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
    }
}