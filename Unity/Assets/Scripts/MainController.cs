using Assets.Enums;
using Models;
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

    private ScenarioInfo info;

    private IBaseEntityController selectedEntity;

    //Car Settings Popup
    [SerializeField]
    GameObject VehicleSettingsPopup;

    public GameObject WorldSettingsPopup;

    void Start()
    {
        this.info = new ScenarioInfo();
        this.selectedEntity = null;
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
            Vehicle v = new(vehiclePosition, VehicleCategory.Car, path);

            var viewController = vehicleGameObject.GetComponent<VehicleViewController>();
            v.View = viewController;
            viewController.vehicle = v;

            setSelectedEntity(viewController);

            info.Vehicles.Add(v);
        });

        removeEntityButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            BaseModel entity = selectedEntity.getEntity();
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
            //Enabling PopUp
            VehicleSettingsPopup.SetActive(true);
            //Finding Popup
            var ThePopup = VehicleSettingsPopup.GetComponent<UIDocument>().rootVisualElement;
            //Finding EXIT Button From popup
            Button ExitButton = ThePopup.Q<Button>("Exit");
            //adding callback function to exit
            ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                VehicleSettingsPopup.SetActive(false);
            });
            //Finding SAVE Button From popup
            Button SaveButton = ThePopup.Q<Button>("Save");
            //adding callback function to save
            SaveButton.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                ///!!!!!!!
                ///Code for saving the data to scenarioInfoFile later on...
                ///!!!!!!!
                Debug.Log("Saving Data of the Vehicle with id:" + selectedEntity);
            });
            //Getting Entity
            BaseModel entity = selectedEntity.getEntity();

            //Finding the Id Field
            IntegerField iDField = ThePopup.Q<IntegerField>("ID");
            //Setting value of the field
            iDField.SetValueWithoutNotify(entity.Id);


            Debug.Log(entity.Id);
            Debug.Log(entity.SpawnPoint);
            Debug.Log(entity.ToString());

            //Finding the SpawnPointField
            Vector3Field SpawnPointField = ThePopup.Q<Vector3Field>("SpawnPoint");
            //Setting value of the field
            SpawnPointField.SetValueWithoutNotify(entity.SpawnPoint.Vector3);


        });

        worldSettingsButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            WorldSettingsPopup.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
        });
    }

    private void initializeEventButtons(VisualElement editorGUI)
    {
        setPathButton = editorGUI.Q<Button>("setPathButton");
        actionButtons = editorGUI.Q<VisualElement>("actionButtons");

        setPathButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            this.selectedEntity.triggerPathRequest();
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

        // Register to get a callback when an item is selected
        eventList.onSelectionChange += OnCharacterSelected;
    }

    void OnCharacterSelected(IEnumerable<object> selectedItems)
    { }

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