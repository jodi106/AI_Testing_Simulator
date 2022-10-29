using Assets.Enums;
using Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainController : MonoBehaviour
{
    public VisualTreeAsset eventEntryTemplate;
    public GameObject carPrefab;

    private ListView eventList;
    private Button button;

    private ScenarioInfo info;

    private IBaseEntityController selectedEntity;

    void Start()
    {
        this.info = new ScenarioInfo();
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
            var path = new Path(new List<Entities.Event>());
            Vehicle v = new Vehicle(vehiclePosition, VehicleCategory.Car, path);

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