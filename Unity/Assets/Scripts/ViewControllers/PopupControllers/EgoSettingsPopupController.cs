using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.Repos;
using System.Text.RegularExpressions;


///<summary>
/// Represents a controller for the ego settings popup.
///</summary>
public class EgoSettingsPopupController : SettingsPopupController
{
    private EgoViewController controller;
    private Ego ego;
    private TextField iDField;
    private TextField agentField;
    //private TextField locationField;
    private TextField initialSpeedField;
    private DropdownField possibleModelsField;
    private DropdownField possibleCategoriesField;
    private TextField colorField;
    private Slider rSlider;
    private Slider gSlider;
    private Slider bSlider;

    /// <summary>
    /// This method is called when the script instance is being loaded. It initializes the UIDocument and sets its rootVisualElement to display none. It also sets the text of a Label to "Options", and registers a callback for a Button to set MainController.freeze to false, set this.ego to null, and set the rootVisualElement display to none. Additionally, it creates a new Location and obtains a list of ego models based on AdversaryCategory.Car. The method registers callbacks for TextFields iDField, initialSpeedField, and agentField, and sets the choices of two DropdownFields based on the AdversaryCategory selected. Finally, the method registers callbacks for three Sliders to update the color of ego's vehicle.
    /// </summary>
    public override void Awake()
    {
        base.Awake();

        Label label = this.document.rootVisualElement.Q<Label>("Label");
        label.text = "Options";

        Button ExitButton = this.document.rootVisualElement.Q<Button>("Exit");

        ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            OnExit();
        });

        //Vehicle ego = selectedEntity.getEntity();
        var spawnPoint = new Location(new Vector3(1, 1, 1), 1);

        var egoModels = VehicleModelRepository.GetModelsBasedOnCategory(AdversaryCategory.Car);

        iDField = this.document.rootVisualElement.Q<TextField>("ID");
        iDField.RegisterCallback<InputEvent>((InputEvent) =>
        {
            ego.Id = InputEvent.newData;
        });

        initialSpeedField = this.document.rootVisualElement.Q<TextField>("CarSpeed");
        initialSpeedField.maxLength = 3;
        initialSpeedField.RegisterCallback<InputEvent>((InputEvent) =>
        {
            if (Regex.Match(InputEvent.newData, @"^(\d)*$").Success) // only digits
            {
                this.ego.InitialSpeedKMH = InputEvent.newData.Length == 0 ? 0 : Double.Parse(InputEvent.newData);
            }
            else
            {
                initialSpeedField.value = InputEvent.previousData;
            }
        });

        agentField = this.document.rootVisualElement.Q<TextField>("Agent");
        agentField.RegisterCallback<InputEvent>((InputEvent) =>
        {
            ego.Agent = InputEvent.newData;
            // TODO PopUp Window with information about agents
        });

        //locationField = this.document.rootVisualElement.Q<TextField>("Location");
        //locationField.SetEnabled(false);
        
        List<string> allPossibleModels = new List<string> { };
        possibleModelsField = this.document.rootVisualElement.Q<DropdownField>("AllPossibleModels");
        possibleModelsField.choices = allPossibleModels;
        possibleModelsField.RegisterValueChangedCallback((evt) =>
        {
            EntityModel model = VehicleModelRepository.findModel(evt.newValue);
            ego.setModel(model);
        });


        List<string> allPossibleCateogories = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(AdversaryCategory)))
        {
            if (option.ToString() == "Null")
            {
                continue;
            }
            allPossibleCateogories.Add(option.ToString());
        }
        possibleCategoriesField = this.document.rootVisualElement.Q<DropdownField>("AllPossibleCategories");
        possibleCategoriesField.choices = allPossibleCateogories;
        possibleCategoriesField.RegisterValueChangedCallback((evt) =>
        {
            if (evt.newValue == "Car")
            {
                List<string> allPossibleModels = new List<string> { };
                foreach (var option in VehicleModelRepository.GetModelsBasedOnCategory(AdversaryCategory.Car))
                {
                    allPossibleModels.Add(option.DisplayName);
                }
                possibleModelsField.choices = allPossibleModels;
                ego.setCategory(AdversaryCategory.Car);
                EntityModel model = VehicleModelRepository.getDefaultCarModel();
                ego.setModel(model);
                possibleModelsField.value = ego.Model.DisplayName.ToString();
            }
            if (evt.newValue == "Bike")
            {
                List<string> allPossibleModels = new List<string> { };
                foreach (var option in VehicleModelRepository.GetModelsBasedOnCategory(AdversaryCategory.Bike))
                {
                    allPossibleModels.Add(option.DisplayName);
                }
                possibleModelsField.choices = allPossibleModels;
                ego.setCategory(AdversaryCategory.Bike);
                EntityModel model = VehicleModelRepository.getDefaultBikeModel();
                ego.setModel(model);
                possibleModelsField.value = ego.Model.DisplayName.ToString();
            }
            if (evt.newValue == "Motorcycle")
            {
                List<string> allPossibleModels = new List<string> { };
                foreach (var option in VehicleModelRepository.GetModelsBasedOnCategory(AdversaryCategory.Motorcycle))
                {
                    allPossibleModels.Add(option.DisplayName);
                }
                possibleModelsField.choices = allPossibleModels;
                ego.setCategory(AdversaryCategory.Motorcycle);
                EntityModel model = VehicleModelRepository.getDefaultMotorcycleModel();
                ego.setModel(model);
                possibleModelsField.value = ego.Model.DisplayName.ToString();
            }
        });

        colorField = this.document.rootVisualElement.Q<TextField>("Color");

        rSlider = this.document.rootVisualElement.Q<Slider>("R");
        rSlider.RegisterValueChangedCallback((changeEvent) =>
        {
            Color color = new Color(changeEvent.newValue, gSlider.value, bSlider.value);
            ego.setColor(color);
            colorField.ElementAt(1).style.backgroundColor = color;
        });

        gSlider = this.document.rootVisualElement.Q<Slider>("G");
        gSlider.RegisterValueChangedCallback((changeEvent) =>
        {
            Color color = new Color(rSlider.value, changeEvent.newValue, bSlider.value);
            ego.setColor(color);
            colorField.ElementAt(1).style.backgroundColor = color;
        });

        bSlider = this.document.rootVisualElement.Q<Slider>("B");
        bSlider.RegisterValueChangedCallback((changeEvent) =>
        {
            Color color = new Color(rSlider.value, gSlider.value, changeEvent.newValue);
            ego.setColor(color);
            colorField.ElementAt(1).style.backgroundColor = color;
        });
    }

    protected override void OnExit()
    {
        MainController.freeze = false;
        this.ego = null;
        this.document.rootVisualElement.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Opens the EgoViewController and initializes the fields with the given values.
    /// </summary>
    /// <param name="controller">The EgoViewController instance to open.</param>
    /// <param name="color">The Color object to set the background color of the field.</param>
    public void Open(EgoViewController controller, Color color)
    {
        this.controller = controller;
        this.ego = (Ego)controller.GetEntity();
        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
        iDField.value = ego.Id.ToString();
        initialSpeedField.value = ego.InitialSpeedKMH.ToString();
        agentField.value = ego.Agent.ToString();
        //locationField.value = String.Format("{0}, {1}", ego.SpawnPoint.X, ego.SpawnPoint.Y);
        possibleCategoriesField.value = ego.Category.ToString();
        possibleModelsField.value = ego.Model.DisplayName.ToString();
        colorField.ElementAt(1).style.backgroundColor = color;
        rSlider.value = color.r;
        gSlider.value = color.g;
        bSlider.value = color.b;

        MainController.freeze = true;
    }
}
