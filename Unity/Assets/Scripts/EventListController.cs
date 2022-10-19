using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EventListController
{
    private VisualTreeAsset eventEntryTemplate;
    private ListView eventList;
    private List<EventData> events;

    public void InitializeEventsList(VisualElement root, VisualTreeAsset listElementTemplate)
    {
        events = new List<EventData>();
        events.AddRange(Resources.LoadAll<EventData>("Events"));

        // Store a reference to the template for the list entries
        eventEntryTemplate = listElementTemplate;

        // Store a reference to the character list element
        eventList = root.Q<ListView>("character-list");

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
    }

    void OnCharacterSelected(IEnumerable<object> selectedItems)
    {}
}