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
    public GameObject carPrefab;

    private ListView eventList;
    private Button addEntityButton;
    private Button removeEntityButton;
    private Button editEntityButton;
    private Button worldSettingsButton;

    private ScenarioInfo info;

    private IBaseEntityController selectedEntity;

    [SerializeField]
    GameObject VehicleSettingsPopup;
    public GameObject WorldSettingsPopup;

    private Button SaveButton;

    private Button ExitButton;

    void Start()
    {
        this.info = new ScenarioInfo();
        this.selectedEntity = null;
        var editorGUI = GameObject.Find("EditorGUI").GetComponent<UIDocument>().rootVisualElement;


        initializeEventList(editorGUI);
        initializeButtonBar(editorGUI);
        initializeWorldSettingsPopUp();

        EventManager.StartListening(typeof(ChangeSelectedEntityAction), x =>
        {
            var action = new ChangeSelectedEntityAction(x);
            this.setSelectedEntity(action.entity);
            if (action.entity is null)
            {
                removeEntityButton.style.display = DisplayStyle.None;
                editEntityButton.style.display = DisplayStyle.None;
            }
        });
    }

    private void setSelectedEntity(IBaseEntityController entity)
    {
        this.selectedEntity?.deselect();
        this.selectedEntity = entity;

        //Debug.Log(this.selectedEntity);

        if (selectedEntity != null)
        {
            //Enabling PopUp
            VehicleSettingsPopup.SetActive(true);
            //Finding Popup
            var ThePopup = VehicleSettingsPopup.GetComponent<UIDocument>().rootVisualElement;
            //Finding EXIT Button From popup
            ExitButton = ThePopup.Q<Button>("Exit");
            //adding callback function to exit
            ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                VehicleSettingsPopup.SetActive(false);
            });
            //Finding SAVE Button From popup
            SaveButton = ThePopup.Q<Button>("Save");
            //adding callback function to save
            ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                ///!!!!!!!
                ///!!!!!!!
                ///!!!!!!!
                ///!!!!!!!
                ///Code for saving the data to scenarioInfoFile later on...
                ///!!!!!!!
                ///!!!!!!!
                ///!!!!!!!
                ///!!!!!!!
                Debug.Log("Saving Data of the Vehicle with id:" + selectedEntity);
            });

            //These lines should be later switched to entity properties.
            //These are only for visual purposes

            //Finding the Id Field
            IntegerField iDField = ThePopup.Q<IntegerField>("ID");
            //Setting value of the field
            iDField.SetValueWithoutNotify(1);

            //Finding the SpawnPointField
            Vector2Field SpawnPointField = ThePopup.Q<Vector2Field>("SpawnPoint");
            //Setting value of the field
            Vector2 TestValue = new Vector2(125, 350);
            SpawnPointField.SetValueWithoutNotify(TestValue);
        }

        this.selectedEntity?.select();
        this.removeEntityButton.style.display = DisplayStyle.Flex;
        this.editEntityButton.style.display = DisplayStyle.Flex;
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

        worldSettingsButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            WorldSettingsPopup.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
        });
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