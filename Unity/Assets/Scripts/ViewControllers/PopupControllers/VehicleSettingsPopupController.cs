using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.Repos;

public class VehicleSettingsPopupController : MonoBehaviour
{
    private Vehicle vehicle;
    private UIDocument document;
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

        var vehicleModelRepo = new VehicleModelRepository();

        var vehicleModels = vehicleModelRepo.GetModelsBasedOnCategory(VehicleCategory.Car);

        Vehicle vehicle = new Vehicle(spawnPoint, vehicleModels[1], VehicleCategory.Car);

        var iDField = this.document.rootVisualElement.Q<TextField>("ID");
        iDField.SetValueWithoutNotify("" + vehicle.Id);

        iDField.RegisterCallback<InputEvent>((InputEvent) =>
        {
            vehicle.Id = Int32.Parse(InputEvent.newData);
        });

        var CarSpeed = this.document.rootVisualElement.Q<TextField>("CarSpeed");

        CarSpeed.RegisterCallback<InputEvent>((InputEvent) =>
        {
        });

        List<string> allPossibleModels = new List<string> { };
        var possibleModelsField = this.document.rootVisualElement.Q<DropdownField>("AllPossibleModels");
        possibleModelsField.choices = allPossibleModels;
        possibleModelsField.RegisterValueChangedCallback((evt) =>
        {
            Debug.Log("New Model: " + evt.newValue);
        });


        List<string> allPossibleCateogories = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(VehicleCategory)))
        {
            allPossibleCateogories.Add(option.ToString());
        }
        var possibleCategoriesField = this.document.rootVisualElement.Q<DropdownField>("AllPossibleCategories");
        possibleCategoriesField.choices = allPossibleCateogories;
        possibleCategoriesField.RegisterValueChangedCallback((evt) =>
        {
            if (evt.newValue == "Car")
            {
                List<string> allPossibleModels = new List<string> { };
                foreach (var option in vehicleModelRepo.GetModelsBasedOnCategory(VehicleCategory.Car))
                {
                    allPossibleModels.Add(option.DisplayName);
                }
                var possibleModelsField = this.document.rootVisualElement.Q<DropdownField>("AllPossibleModels");
                possibleModelsField.choices = allPossibleModels;
            }
            if (evt.newValue == "Bike")
            {
                List<string> allPossibleModels = new List<string> { };
                foreach (var option in vehicleModelRepo.GetModelsBasedOnCategory(VehicleCategory.Bike))
                {
                    allPossibleModels.Add(option.DisplayName);
                }
                var possibleModelsField = this.document.rootVisualElement.Q<DropdownField>("AllPossibleModels");
                possibleModelsField.choices = allPossibleModels;
            }
            if (evt.newValue == "Motorcycle")
            {
                List<string> allPossibleModels = new List<string> { };
                foreach (var option in vehicleModelRepo.GetModelsBasedOnCategory(VehicleCategory.Motorcycle))
                {
                    allPossibleModels.Add(option.DisplayName);
                }
                var possibleModelsField = this.document.rootVisualElement.Q<DropdownField>("AllPossibleModels");
                possibleModelsField.choices = allPossibleModels;
            }
            if (evt.newValue == "Null")
            {
                List<string> allPossibleModels = new List<string> { };
                var possibleModelsField = this.document.rootVisualElement.Q<DropdownField>("AllPossibleModels");
                possibleModelsField.choices = allPossibleModels;
            }
        });
    }
    public void open(Vehicle vehicle)
    {
        this.vehicle = vehicle;
        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}
