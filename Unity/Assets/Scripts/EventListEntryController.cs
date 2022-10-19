using UnityEngine.UIElements;

public class EventListEntryController
{
    Label label;

    public void SetVisualElement(VisualElement visualElement)
    {
        label = visualElement.Q<Label>("label");
    }

    public void setEventData(EventData characterData)
    {
        label.text = characterData.label;
    }
}