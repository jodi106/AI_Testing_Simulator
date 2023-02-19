using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.Repos;
using System.Text.RegularExpressions;

public class AdversarySettingsPopupController : MonoBehaviour
{
    private AdversaryViewController controller;
    private Vehicle vehicle;
    private UIDocument document;
    private TextField iDField;
    private TextField initialSpeedField;
    private TextField locationField;
    private DropdownField possibleModelsField;
    private DropdownField possibleCategoriesField;
    private TextField colorField;
    private Slider rSlider;
    private Slider gSlider;
    private Slider bSlider;
    private TextField startRouteTimeField;
    private Label startRouteWaypointLabel;
    private Button deleteStartRouteWaypoint;

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
            vehicle.Id = InputEvent.newData;
        });

        initialSpeedField = this.document.rootVisualElement.Q<TextField>("CarSpeed");
        initialSpeedField.maxLength = 3;
        initialSpeedField.RegisterCallback<InputEvent>((InputEvent) =>
        {
            if (Regex.Match(InputEvent.newData, @"^(\d)*$").Success) // only digits
            {
                this.vehicle.InitialSpeedKMH = InputEvent.newData.Length == 0 ? 0 : Double.Parse(InputEvent.newData);
            }
            else
            {
                initialSpeedField.value = InputEvent.previousData;
            }
        });

        locationField = this.document.rootVisualElement.Q<TextField>("Location");
        locationField.SetEnabled(false);

        List<string> allPossibleModels = new List<string> { };
        possibleModelsField = this.document.rootVisualElement.Q<DropdownField>("AllPossibleModels");
        possibleModelsField.choices = allPossibleModels;
        possibleModelsField.RegisterValueChangedCallback((evt) =>
        {
            EntityModel model = VehicleModelRepository.findModel(evt.newValue);
            vehicle.setModel(model);
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
            VehicleCategory cat;
            switch (evt.newValue)
            {
                case "Car":
                    cat = VehicleCategory.Car;
                    break;
                case "Bike":
                    cat = VehicleCategory.Bike;
                    break;
                case "Motorcycle":
                    cat = VehicleCategory.Motorcycle;
                    break;
                case "Pedestrian":
                    cat = VehicleCategory.Pedestrian;
                    break;
                default:
                    cat = VehicleCategory.Null;
                    break;
            }
            List<string> allPossibleModels = new List<string> { };
            foreach (var option in VehicleModelRepository.GetModelsBasedOnCategory(cat))
            {
                allPossibleModels.Add(option.DisplayName);
            }
            possibleModelsField.choices = allPossibleModels;
            vehicle.setCategory(cat);
            EntityModel model = VehicleModelRepository.getDefaultModel(cat);
            vehicle.setModel(model);
            possibleModelsField.value = vehicle.Model.DisplayName.ToString();
        });

        colorField = this.document.rootVisualElement.Q<TextField>("Color");

        rSlider = this.document.rootVisualElement.Q<Slider>("R");
        rSlider.RegisterValueChangedCallback((changeEvent) =>
        {
            Color color = new Color(changeEvent.newValue, gSlider.value, bSlider.value);
            vehicle.setColor(color);
            colorField.ElementAt(1).style.backgroundColor = color;
        });

        gSlider = this.document.rootVisualElement.Q<Slider>("G");
        gSlider.RegisterValueChangedCallback((changeEvent) =>
        {
            Color color = new Color(rSlider.value, changeEvent.newValue, bSlider.value);
            vehicle.setColor(color);
            colorField.ElementAt(1).style.backgroundColor = color;
        });

        bSlider = this.document.rootVisualElement.Q<Slider>("B");
        bSlider.RegisterValueChangedCallback((changeEvent) =>
        {
            Color color = new Color(rSlider.value, gSlider.value, changeEvent.newValue);
            vehicle.setColor(color);
            colorField.ElementAt(1).style.backgroundColor = color;
        });

        startRouteTimeField = this.document.rootVisualElement.Q<TextField>("StartRouteTime");
        startRouteWaypointLabel = this.document.rootVisualElement.Q<Label>("StartRouteWaypointLabel");
        deleteStartRouteWaypoint = this.document.rootVisualElement.Q<Button>("DeleteStartRouteWaypoint");
        // TODO
        //startRouteWaypointLabel.style.display = DisplayStyle.None;
    }
    public void open(AdversaryViewController controller, Color color)
    {
        this.controller = controller;
        this.vehicle = (Vehicle) controller.getEntity();
        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
        iDField.value = vehicle.Id.ToString();
        initialSpeedField.value = vehicle.InitialSpeedKMH.ToString();
        locationField.value = String.Format("{0}, {1}", vehicle.SpawnPoint.X, vehicle.SpawnPoint.Y);
        possibleCategoriesField.value = vehicle.Category.ToString();
        possibleModelsField.value = vehicle.Model.DisplayName.ToString();
        colorField.ElementAt(1).style.backgroundColor = color;
        rSlider.value = color.r;
        gSlider.value = color.g;
        bSlider.value = color.b;
    }
}
