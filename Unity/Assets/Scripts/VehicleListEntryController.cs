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

    public void setEventData(BaseEntity entity)
    {
        label.text = entity.Id.ToString();
        container.style.backgroundColor = entity.Color.ToUnityColor();
        //TODO: maybe use common subclass for ego and vehicle
        if (entity is Adversary v)
        {
            category.text = v.Category.ToString();
            model.text = v.Model.DisplayName;
        }
        if (entity is Ego e)
        {
            category.text = e.Category.ToString();
            model.text = e.Model.DisplayName;
        }
    }
}