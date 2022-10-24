using Entities;
using UnityEngine.UIElements;

public class VehicleListEntryController
{
    Label label;

    public void SetVisualElement(VisualElement visualElement)
    {
        label = visualElement.Q<Label>("label");
    }

    public void setEventData(Vehicle vehicle)
    {
        label.text = vehicle.Category + " " + vehicle.Id;
    }
}