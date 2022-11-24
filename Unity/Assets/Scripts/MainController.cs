using Assets.Enums;
using Assets.Helpers;
using Assets.Repos;
using Entity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using ExportScenario.XMLBuilder;

public class MainController : MonoBehaviour
{
    public VisualTreeAsset eventEntryTemplate;
    //Spawned Cars
    public GameObject carPrefab;

    //Event Bar (Center Bottom)
    private ListView eventList;
    private UnityEngine.UIElements.Button addEntityButton;
    private UnityEngine.UIElements.Button removeEntityButton;
    private UnityEngine.UIElements.Button editEntityButton;
    private UnityEngine.UIElements.Button worldSettingsButton;

    //Action Buttons (Center Left)
    private VisualElement actionButtons;
    private UnityEngine.UIElements.Button setPathButton;
    private UnityEngine.UIElements.Button deletePathButton;
    private UnityEngine.UIElements.Button cancelPathButton;
    private UnityEngine.UIElements.Button submitPathButton;

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

    private void initializeButtonBar(VisualElement editorGUI)
    {
        addEntityButton = editorGUI.Q<UnityEngine.UIElements.Button>("addEntityButton");
        removeEntityButton = editorGUI.Q<UnityEngine.UIElements.Button>("removeEntityButton");
        editEntityButton = editorGUI.Q<UnityEngine.UIElements.Button>("editEntityButton");
        worldSettingsButton = editorGUI.Q<UnityEngine.UIElements.Button>("worldSettingsButton");

        addEntityButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            var pos = Input.mousePosition;
            pos.z = -0.1f;
            var vehicleGameObject = Instantiate(carPrefab, pos, Quaternion.identity);

            var vehiclePosition = new Location(pos.x, pos.y, 0, 0);
            var path = new Path();
            Vehicle v = new(vehiclePosition, path, category: VehicleCategory.Car);

            var viewController = vehicleGameObject.GetComponent<AdversaryViewController>();
            v.View = viewController;
            viewController.setVehicle(v);

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

            UnityEngine.UIElements.Button ExitButton = ThePopup.Q<UnityEngine.UIElements.Button>("ExitButton");
            ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                VehicleSettingsPopup.SetActive(false);
            });

            UnityEngine.UIElements.Button SaveButton = ThePopup.Q<UnityEngine.UIElements.Button>("SaveButton");
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
            foreach (var model in allmodels)
            {
                var newRadio = new RadioButton(model.DisplayName);
                group.Add(newRadio);
            }

            var categoriesGroup = ThePopup.Q<RadioButtonGroup>("AllPossibleCategories");
            List<VehicleCategory> allcateogires = new List<VehicleCategory>();
            allcateogires.Add(VehicleCategory.Car);
            allcateogires.Add(VehicleCategory.Bike);
            allcateogires.Add(VehicleCategory.Motorcycle);
            foreach (var category in allcateogires)
            {
                var newRadio = new RadioButton(category.GetDescription());
                categoriesGroup.Add(newRadio);
            }

            categoriesGroup.HandleEvent(ClickEvent);

            categoriesGroup.RegisterCallback<ChangeEvent<MouseCaptureEvent>>((ChangeEvent) =>
            {
                //METHOD WITH RADIO GROUP
                var group = ThePopup.Q<RadioButtonGroup>("AllPossibleModels");
                var allmodels = vehicleModelRepo.GetModelsBasedOnCategory(VehicleCategory.Bike);
                foreach (var model in allmodels)
                {
                    var newRadio = new RadioButton(model.DisplayName);
                    group.Add(newRadio);
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
        setPathButton = editorGUI.Q<UnityEngine.UIElements.Button>("setPathButton");
        deletePathButton = editorGUI.Q<UnityEngine.UIElements.Button>("deletePathButton");
        cancelPathButton = editorGUI.Q<UnityEngine.UIElements.Button>("cancelPathButton");
        submitPathButton = editorGUI.Q<UnityEngine.UIElements.Button>("submitPathButton");

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
        var popup = WorldSettingsPopup.GetComponent<UIDocument>().rootVisualElement;
        var exitButton = popup.Q<UnityEngine.UIElements.Button>("Exit");

        exitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            WorldSettingsPopup.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
        });

        WorldSettingsPopup.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
    }

    void ExportOnClick()
    {
        //This Function is bind with the "Export button"
        //The actual binding is made in the Start function of the script
        //Make sure to change this function to export the desired version or desired files.
        //Anything written here will be run at the time of pressing "Export" Button
        BuildXML exporter = new BuildXML(this.info);
        exporter.CombineXML();
    }
}