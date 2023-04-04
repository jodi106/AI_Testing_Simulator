using Entity;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// A controller class for updating vehicle list entries in the UI.
/// </summary>
public class VehicleListEntryController
{
    Label label;
    Label category;
    Label model;
    VisualElement container;
    public BaseEntity entity { get; protected set; }

    /// <summary>
    /// Sets the VisualElement that the controller will update.
    /// </summary>
    /// <param name="visualElement">The VisualElement containing the label, category, model, and container elements.</param>
    public void SetVisualElement(VisualElement visualElement)
    {
        label = visualElement.Q<Label>("label");
        category = visualElement.Q<Label>("category");
        model = visualElement.Q<Label>("model");
        container = visualElement.Q<VisualElement>("box");
    }

    /// <summary>
    /// Updates the UI elements with data from the given BaseEntity.
    /// </summary>
    /// <param name="entity">The BaseEntity containing the data to display.</param>
    public void setEventData(BaseEntity entity)
    {
        this.entity = entity;
        label.text = entity.Id.ToString();
        container.style.backgroundColor = entity.Color.ToUnityColor();
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