using Entity;
using UnityEngine;
using UnityEngine.UIElements;

public class VehicleListEntryController
{
    Label label;
    Label category;
    Label model;
    VisualElement container;

    public void SetVisualElement(VisualElement visualElement)
    {
        label = visualElement.Q<Label>("label");
        category = visualElement.Q<Label>("category");
        model = visualElement.Q<Label>("model");
        container = visualElement.Q<VisualElement>("box");
    }

    public void setEventData(Vehicle entity)
    {
        label.text = entity.Id.ToString();
        category.text = entity.Category.ToString();
        model.text = entity.Model.DisplayName;
        container.style.backgroundColor = entity.color;
    }
}