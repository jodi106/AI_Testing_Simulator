using Models;
using System.ComponentModel;
using UnityEngine.UIElements;

public class VehicleListEntryController
{
    Label label;
    Label category;

    public void SetVisualElement(VisualElement visualElement)
    {
        label = visualElement.Q<Label>("label");
        category = visualElement.Q<Label>("category");
    }

    public void setEventData(VehicleModel vehicle)
    {
        label.text = "" + vehicle.Id;
        category.text = "" + vehicle.Category;
    }
}