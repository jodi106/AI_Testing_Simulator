using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UIElements;
using ExportScenario.XMLBuilder;
using System.Collections.ObjectModel;

public class MainController : MonoBehaviour
{
    public VisualTreeAsset eventEntryTemplate;
    //Spawned Cars
    public GameObject carPrefab;
    public GameObject egoPrefab;

    //Event Bar (Center Bottom)
    private ListView eventList;
    private Button addEntityButton;
    private Button removeEntityButton;
    private Button editEntityButton;
    private Button worldSettingsButton;
    private Button exportButton;

    //Action Buttons (Center Left)
    private VisualElement actionButtons;
    private Button setPathButton;
    private Button deletePathButton;
    private Button cancelPathButton;
    private Button submitPathButton;

    private ScenarioInfo info;

    private IBaseEntityController selectedEntity;
    private bool preventDeselection;

    private WorldSettingsPopupController worldSettingsController;

    void Start()
    {
        this.info = new ScenarioInfo();
        this.selectedEntity = null;
        this.preventDeselection = false;
        var editorGUI = GameObject.Find("EditorGUI").GetComponent<UIDocument>().rootVisualElement;

        initializeEventList(editorGUI);
        initializeButtonBar(editorGUI);
        initializeEventButtons(editorGUI);

        EventManager.StartListening(typeof(MouseClickAction), x =>
        {
            if (!preventDeselection)
            {
                this.setSelectedEntity(null);
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

        GameObject popups = GameObject.Find("PopUps");
        this.worldSettingsController = popups.transform.Find("WorldSettingsPopUpAdvanced").gameObject.GetComponent<WorldSettingsPopupController>();
        this.worldSettingsController.init(this.info.WorldOptions);
    }

    public void setSelectedEntity(IBaseEntityController entity)
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
        this.info.Vehicles.Add(vehicle);
        this.preventDeselection = false;
    }

    public void removeVehicle(Vehicle vehicle)
    {
        this.info.Vehicles.Remove(vehicle);
    }

    public void setEgo(Ego ego)
    {
        this.info.EgoVehicle = ego;
        this.preventDeselection = false;
    }

    private void initializeButtonBar(VisualElement editorGUI)
    {
        addEntityButton = editorGUI.Q<Button>("addEntityButton");
        removeEntityButton = editorGUI.Q<Button>("removeEntityButton");
        editEntityButton = editorGUI.Q<Button>("editEntityButton");
        worldSettingsButton = editorGUI.Q<Button>("worldSettingsButton");
        exportButton = editorGUI.Q<Button>("exportButton");

        addEntityButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            var pos = Input.mousePosition;
            pos.z = -0.1f;
            GameObject prefab = this.info.EgoVehicle is null ? egoPrefab : carPrefab;
            var vehicleGameObject = Instantiate(prefab, pos, Quaternion.identity);
            VehicleViewController viewController = null;
            if (this.info.EgoVehicle is null)
            {
                viewController = vehicleGameObject.GetComponent<EgoViewController>();
            } else
            {
                viewController = vehicleGameObject.GetComponent<AdversaryViewController>();
            }
            Color color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            color = new Color(color.r, color.g, color.b, 1);
            viewController.setColor(color);
            setSelectedEntity(viewController);
            this.preventDeselection = true;
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

        worldSettingsButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            worldSettingsController.open();
        });

        exportButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            ExportOnClick();
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

    void ExportOnClick()
    {
        //This Function is bind with the "Export button"
        //The actual binding is made in the Start function of the script
        //Make sure to change this function to export the desired version or desired files.
        //Anything written here will be run at the time of pressing "Export" Button

        // To have right number format e.g. 80.5 instead of 80,5
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");


        


        // ------------------------------------------------------------------------
        // TODO remove these lines later once these values are set in Unity
        info.Name = "OurScenario3";
        info.MapURL = "Town10HD";
        info.EgoVehicle.Model = new EntityModel("vehicle.nissan.micra");
        // ------------------------------------------------------------------------
        // Required to create AssignRouteAction (do not delete this!)
        foreach (Vehicle vehicle in info.Vehicles)
        {
            vehicle.Path.InitAssignRouteWaypoint();
        }

        // Create .xosc file
        BuildXML doc = new BuildXML(info);
        doc.CombineXML();
    }


    //Only for Town06 later do as extension method for Vector3 or Location

}
