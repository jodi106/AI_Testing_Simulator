using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class MainController : MonoBehaviour
{
    public VisualTreeAsset eventEntryTemplate;
    public GameObject carPrefab;

    private VisualElement editorGUI;
    private ListView eventList;
    private List<EventData> events;
    private Button button;

    void Start()
    {
        events = new List<EventData>();
        events.AddRange(Resources.LoadAll<EventData>("Events"));

        editorGUI = GameObject.Find("EditorGUI").GetComponent<UIDocument>().rootVisualElement;

        // Store a reference to the character list element
        eventList = editorGUI.Q<ListView>("character-list");

        // Set up a make item function for a list entry
        eventList.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = eventEntryTemplate.Instantiate();

            // Instantiate a controller for the data
            var entryController = new EventListEntryController();

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
            (item.userData as EventListEntryController).setEventData(events[index]);
        };

        eventList.itemsSource = events;

        // Register to get a callback when an item is selected
        eventList.onSelectionChange += OnCharacterSelected;

        button = editorGUI.Q<Button>("button");

        button.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            Debug.Log("Button Clicked");
            var pos = Input.mousePosition;
            pos.z = -0.1f;
            Instantiate(carPrefab, pos, Quaternion.identity);
        });

        EventManager.StartListening(typeof(VehicleMovedAction), x =>
        {
            var action = new VehicleMovedAction(x);
            Debug.Log(action.Car);
        });
    }

    void OnCharacterSelected(IEnumerable<object> selectedItems)
    {}
}