using UnityEngine;
using UnityEngine.UIElements;

public class MainView : MonoBehaviour
{
    [SerializeField]
    VisualTreeAsset eventEntryTemplate;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        var eventListController = new EventListController();
        eventListController.InitializeEventsList(uiDocument.rootVisualElement, eventEntryTemplate);
    }
}