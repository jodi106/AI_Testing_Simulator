using Assets.Enums;
using Entities;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UIElements;

public class MainController : MonoBehaviour
{
    public VisualTreeAsset eventEntryTemplate;
    public GameObject carPrefab;

    private ListView eventList;
    private Button button;

    private Model model;

    void Start()
    {
        this.model = new Model();
        var editorGUI = GameObject.Find("EditorGUI").GetComponent<UIDocument>().rootVisualElement;

        initiateEventList(editorGUI);

        button = editorGUI.Q<Button>("button");

        button.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            var pos = Input.mousePosition;
            pos.z = -0.1f;
            var vehicleGameObject = Instantiate(carPrefab, pos, Quaternion.identity);

            var vehiclePosition = new Coord3D(pos.x, pos.y, 0, 0);
            var path = new Path(new List<Entities.Event>());
            Vehicle v = new Vehicle(vehiclePosition, VehicleCategory.Car, path);
            model.vehicles.Add(v);
        });

        EventManager.StartListening(typeof(VehicleMovedAction), x =>
        {
            var action = new VehicleMovedAction(x);
            Debug.Log(action.Car);
        });
    }

    private void initiateEventList(VisualElement editorGUI)
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
            (item.userData as VehicleListEntryController).setEventData(model.vehicles[index]);
        };

        model.vehicles.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs args) =>
        {
            this.eventList.Rebuild();
        };

        eventList.itemsSource = model.vehicles;

        // Register to get a callback when an item is selected
        eventList.onSelectionChange += OnCharacterSelected;
    }

    void OnCharacterSelected(IEnumerable<object> selectedItems)
    { }
}