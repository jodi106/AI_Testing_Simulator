using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.Repos;

public class AdversarySettingsPopupController : MonoBehaviour
{
    private AdversaryViewController controller;
    private Vehicle vehicle;
    private UIDocument document;
    private TextField iDField;
    private TextField locationField;
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
            this.vehicle = null;
            this.document.rootVisualElement.style.display = DisplayStyle.None;
        });

        //Vehicle vehicle = selectedEntity.getEntity();
        var spawnPoint = new Location(new Vector3(1, 1, 1), 1);

        var vehicleModels = VehicleModelRepository.GetModelsBasedOnCategory(VehicleCategory.Car);

        iDField = this.document.rootVisualElement.Q<TextField>("ID");

        iDField.RegisterCallback<InputEvent>((InputEvent) =>
        {
            vehicle.Id = Int32.Parse(InputEvent.newData);
        });

        locationField = this.document.rootVisualElement.Q<TextField>("Location");
        locationField.SetEnabled(false);

        var carSpeedField = this.document.rootVisualElement.Q<TextField>("CarSpeed");
        carSpeedField.SetEnabled(false);

        List<string> allPossibleModels = new List<string> { };
        possibleModelsField = this.document.rootVisualElement.Q<DropdownField>("AllPossibleModels");
        possibleModelsField.choices = allPossibleModels;
        possibleModelsField.RegisterValueChangedCallback((evt) =>
        {
            Debug.Log("New Model: " + evt.newValue);
            //TODO: get Model from repository and set it
        });


        List<string> allPossibleCateogories = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(VehicleCategory)))
        {
            if(option.ToString() == "Null")
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
                vehicle.setCategory(VehicleCategory.Car);
                EntityModel model = VehicleModelRepository.getDefaultCarModel();
                vehicle.Model = model;
                possibleModelsField.value = vehicle.Model.Name.ToString();
            }
            if (evt.newValue == "Bike")
            {
                List<string> allPossibleModels = new List<string> { };
                foreach (var option in VehicleModelRepository.GetModelsBasedOnCategory(VehicleCategory.Bike))
                {
                    allPossibleModels.Add(option.DisplayName);
                }
                possibleModelsField.choices = allPossibleModels;
                vehicle.setCategory(VehicleCategory.Bike);
                EntityModel model = VehicleModelRepository.getDefaultBikeModel();
                vehicle.Model = model;
                possibleModelsField.value = vehicle.Model.Name.ToString();
            }
            if (evt.newValue == "Motorcycle")
            {
                List<string> allPossibleModels = new List<string> { };
                foreach (var option in VehicleModelRepository.GetModelsBasedOnCategory(VehicleCategory.Motorcycle))
                {
                    allPossibleModels.Add(option.DisplayName);
                }
                possibleModelsField.choices = allPossibleModels;
                vehicle.setCategory(VehicleCategory.Motorcycle);
                EntityModel model = VehicleModelRepository.getDefaultMotorcycleModel();
                vehicle.Model = model;
                possibleModelsField.value = vehicle.Model.Name.ToString();
            }
        });

        colorField = this.document.rootVisualElement.Q<TextField>("Color");

        rSlider = this.document.rootVisualElement.Q<Slider>("R");
        rSlider.RegisterValueChangedCallback((changeEvent) =>
        {
            Color color = new Color(changeEvent.newValue, gSlider.value, bSlider.value);
            controller.setColor(color);
            colorField.ElementAt(1).style.backgroundColor = color;
        });

        gSlider = this.document.rootVisualElement.Q<Slider>("G");
        gSlider.RegisterValueChangedCallback((changeEvent) =>
        {
            Color color = new Color(rSlider.value, changeEvent.newValue, bSlider.value);
            controller.setColor(color);
            colorField.ElementAt(1).style.backgroundColor = color;
        });

        bSlider = this.document.rootVisualElement.Q<Slider>("B");
        bSlider.RegisterValueChangedCallback((changeEvent) =>
        {
            Color color = new Color(rSlider.value, gSlider.value, changeEvent.newValue);
            controller.setColor(color);
            colorField.ElementAt(1).style.backgroundColor = color;
        });
    }
    public void open(AdversaryViewController controller, Color color)
    {
        this.controller = controller;
        this.vehicle = (Vehicle) controller.getEntity();
        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
        iDField.value = vehicle.Id.ToString();
        locationField.value = String.Format("{0}, {1}", vehicle.SpawnPoint.X, vehicle.SpawnPoint.Y);
        possibleCategoriesField.value = vehicle.Category.ToString();
        possibleModelsField.value = vehicle.Model.Name.ToString();
        colorField.ElementAt(1).style.backgroundColor = color;
        rSlider.value = color.r;
        gSlider.value = color.g;
        bSlider.value = color.b;
    }
}
