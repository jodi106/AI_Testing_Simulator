using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.Repos;
using System.Text.RegularExpressions;

public class EgoSettingsPopupController : MonoBehaviour
{
    private EgoViewController controller;
    private Ego ego;
    private UIDocument document;
    private TextField iDField;
    private TextField agentField;
    private TextField locationField;
    private TextField initialSpeedField;
    private DropdownField possibleModelsField;
    private DropdownField possibleCategoriesField;
    private TextField colorField;
    private Slider rSlider;
    private Slider gSlider;
    private Slider bSlider;


    public void Awake()
    {
        this.document = gameObject.GetComponent<UIDocument>();
        this.document.rootVisualElement.style.display = DisplayStyle.None;

        Label label = this.document.rootVisualElement.Q<Label>("Label");
        label.text = "Options";

        Button ExitButton = this.document.rootVisualElement.Q<Button>("Exit");

        ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            MainController.freeze = false;
            this.ego = null;
            this.document.rootVisualElement.style.display = DisplayStyle.None;
        });

        //Vehicle ego = selectedEntity.getEntity();
        var spawnPoint = new Location(new Vector3(1, 1, 1), 1);

        var egoModels = VehicleModelRepository.GetModelsBasedOnCategory(VehicleCategory.Car);

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

        locationField = this.document.rootVisualElement.Q<TextField>("Location");
        locationField.SetEnabled(false);
        
        List<string> allPossibleModels = new List<string> { };
        possibleModelsField = this.document.rootVisualElement.Q<DropdownField>("AllPossibleModels");
        possibleModelsField.choices = allPossibleModels;
        possibleModelsField.RegisterValueChangedCallback((evt) =>
        {
            EntityModel model = VehicleModelRepository.findModel(evt.newValue);
            ego.setModel(model);
        });


        List<string> allPossibleCateogories = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(VehicleCategory)))
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
                foreach (var option in VehicleModelRepository.GetModelsBasedOnCategory(VehicleCategory.Car))
                {
                    allPossibleModels.Add(option.DisplayName);
                }
                possibleModelsField.choices = allPossibleModels;
                ego.setCategory(VehicleCategory.Car);
                EntityModel model = VehicleModelRepository.getDefaultCarModel();
                ego.setModel(model);
                possibleModelsField.value = ego.Model.DisplayName.ToString();
            }
            if (evt.newValue == "Bike")
            {
                List<string> allPossibleModels = new List<string> { };
                foreach (var option in VehicleModelRepository.GetModelsBasedOnCategory(VehicleCategory.Bike))
                {
                    allPossibleModels.Add(option.DisplayName);
                }
                possibleModelsField.choices = allPossibleModels;
                ego.setCategory(VehicleCategory.Bike);
                EntityModel model = VehicleModelRepository.getDefaultBikeModel();
                ego.setModel(model);
                possibleModelsField.value = ego.Model.DisplayName.ToString();
            }
            if (evt.newValue == "Motorcycle")
            {
                List<string> allPossibleModels = new List<string> { };
                foreach (var option in VehicleModelRepository.GetModelsBasedOnCategory(VehicleCategory.Motorcycle))
                {
                    allPossibleModels.Add(option.DisplayName);
                }
                possibleModelsField.choices = allPossibleModels;
                ego.setCategory(VehicleCategory.Motorcycle);
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
    public void open(EgoViewController controller, Color color)
    {
        this.controller = controller;
        this.ego = (Ego)controller.getEntity();
        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
        iDField.value = ego.Id.ToString();
        initialSpeedField.value = ego.InitialSpeedKMH.ToString();
        agentField.value = ego.Agent.ToString();
        locationField.value = String.Format("{0}, {1}", ego.SpawnPoint.X, ego.SpawnPoint.Y);
        possibleCategoriesField.value = ego.Category.ToString();
        possibleModelsField.value = ego.Model.DisplayName.ToString();
        colorField.ElementAt(1).style.backgroundColor = color;
        rSlider.value = color.r;
        gSlider.value = color.g;
        bSlider.value = color.b;

        MainController.freeze = true;
    }
}
