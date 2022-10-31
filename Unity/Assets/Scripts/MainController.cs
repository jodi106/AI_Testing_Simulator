using Assets.Enums;
using Dtos;
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
    private Button button;

    private ScenarioInfoModel info;

    private IBaseEntityController selectedEntity;

    [SerializeField]
    GameObject SettingsPopup;

    private Button SaveButton;

    private Button ExitButton;

    void Start()
    {
        this.info = new ScenarioInfoModel();
        this.selectedEntity = null;
        var editorGUI = GameObject.Find("EditorGUI").GetComponent<UIDocument>().rootVisualElement;
        

        initializeEventList(editorGUI);
        initializeButtonBar(editorGUI);

        EventManager.StartListening(typeof(ChangeSelectedEntityAction), x =>
        {
            var action = new ChangeSelectedEntityAction(x);
            this.setSelectedEntity(action.entity);
        });
    }

    private void setSelectedEntity(IBaseEntityController entity)
    {
        this.selectedEntity?.deselect();
        this.selectedEntity = entity;

        //Debug.Log(this.selectedEntity);

        if(selectedEntity != null)
        {
            //Enabling PopUp
            SettingsPopup.SetActive(true);
            //Finding Popup
            var ThePopup = SettingsPopup.GetComponent<UIDocument>().rootVisualElement;
            //Finding EXIT Button From popup
            ExitButton = ThePopup.Q<Button>("Exit");
            //adding callback function to exit
            ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                SettingsPopup.SetActive(false);
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
    }

    private void initializeButtonBar(VisualElement editorGUI)
    {
        button = editorGUI.Q<Button>("button");

        button.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            var pos = Input.mousePosition;
            pos.z = -0.1f;
            var vehicleGameObject = Instantiate(carPrefab, pos, Quaternion.identity);

            var vehiclePosition = new Coord3D(pos.x, pos.y, 0, 0);
            var path = new Path(new List<Dtos.Event>());
            VehicleModel v = new(vehiclePosition, VehicleCategory.Car, path);

            var viewController = vehicleGameObject.GetComponent<VehicleViewController>();
            v.View = viewController;
            viewController.vehicle = v;

            setSelectedEntity(viewController);

            info.Vehicles.Add(v);
        });
    }

    private void initializeEventList(VisualElement editorGUI)
    {
        // Store a reference to the character list element
        eventList = editorGUI.Q<ListView>("character-list");

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
}