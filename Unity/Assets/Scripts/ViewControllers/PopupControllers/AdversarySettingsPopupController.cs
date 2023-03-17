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
    private Adversary vehicle;
    private Ego egoVehicle;
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
    private DropdownField startRouteDropdown;
    private TextField startRouteTimeField;
    private TextField startRouteDistanceField;
    private Label startRouteWaypointTimeLabel;
    private Button deleteStartRouteWaypointButton;
    private Label startRouteInfoLabel;

    private string startRouteType = null;

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
            saveStartRouteInfo(startRouteType);
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

        // ---------------------- Start Route Properties ----------------------

        startRouteDropdown = this.document.rootVisualElement.Q<DropdownField>("StartRouteDropdown");
        startRouteTimeField = this.document.rootVisualElement.Q<TextField>("StartRouteTime");
        startRouteDistanceField = this.document.rootVisualElement.Q<TextField>("StartRouteDistance");
        startRouteWaypointTimeLabel = this.document.rootVisualElement.Q<Label>("StartRouteWaypointLabel");
        deleteStartRouteWaypointButton = this.document.rootVisualElement.Q<Button>("DeleteStartRouteWaypoint");
        startRouteInfoLabel = this.document.rootVisualElement.Q<Label>("StartRouteInfoLabel");

        startRouteDropdown.RegisterValueChangedCallback((evt) =>
        {
            switch (evt.newValue)
            {
                case "Time":
                    startRouteTimeField.style.display = DisplayStyle.Flex;
                    startRouteDistanceField.style.display = DisplayStyle.None;
                    this.startRouteType = "Time";
                    break;
                case "Ego Vehicle":
                    startRouteTimeField.style.display = DisplayStyle.None;
                    startRouteDistanceField.style.display = DisplayStyle.Flex;
                    this.startRouteType = "Ego";
                    break;
            }
        });
        startRouteTimeField.RegisterCallback<InputEvent>((InputEvent) =>
        {
            if (Regex.Match(InputEvent.newData, @"^(\d)*$").Success) // only digits
            {
                vehicle.StartRouteInfo.Time = InputEvent.newData.Length == 0 ? 0 : Int32.Parse(InputEvent.newData);
            }
            else
            {
                startRouteTimeField.value = InputEvent.previousData;
            }
        });
        startRouteDistanceField.RegisterCallback<InputEvent>((InputEvent) =>
        {
            if (Regex.Match(InputEvent.newData, @"^(\d)*$").Success) // only digits
            {
                vehicle.StartRouteInfo.Distance = InputEvent.newData.Length == 0 ? 0 : Int32.Parse(InputEvent.newData);
            }
            else
            {
                startRouteDistanceField.value = InputEvent.previousData;
            }
        });

        deleteStartRouteWaypointButton.RegisterCallback<ClickEvent>((clickEvent) =>
        {
            foreach (Waypoint waypoint in vehicle.StartRouteInfo.Vehicle.Path.WaypointList)
            {
                if (waypoint.StartRouteOfOtherVehicle == vehicle)
                {
                    waypoint.StartRouteOfOtherVehicle = null;
                }
            }
            resetStartRouteFields();
            vehicle.StartRouteInfo = new StartRouteInfo(vehicle, 0); ;
        });
    }

    public void open(AdversaryViewController controller, Color color, Ego egoVehicle)
    {
        this.controller = controller;
        this.vehicle = (Adversary) controller.getEntity();
        this.egoVehicle = egoVehicle;
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

        if (vehicle.StartRouteInfo != null)
        {
            switch (vehicle.StartRouteInfo.Type)
            {
                case "Waypoint":
                    loadStartRouteInfoWaypoint();
                    break;
                case "Time":
                    loadStartRouteInfoTime();
                    break;
                case "Ego":
                    loadStartRouteInfoEgo();
                    break;
            }
        }
        else
        {
            resetStartRouteFields();
            vehicle.StartRouteInfo = new StartRouteInfo(vehicle, 0);
        }

        MainController.freeze = true;
    }


    private void loadStartRouteInfoWaypoint()
    {
        this.startRouteType = "Waypoint";
        startRouteDropdown.style.display = DisplayStyle.None;
        startRouteTimeField.style.display = DisplayStyle.None;
        startRouteDistanceField.style.display = DisplayStyle.None;
        startRouteWaypointTimeLabel.style.display = DisplayStyle.Flex;
        deleteStartRouteWaypointButton.style.display = DisplayStyle.Flex;
        startRouteInfoLabel.style.display = DisplayStyle.None;
        startRouteWaypointTimeLabel.text = vehicle.StartRouteInfo.Vehicle.Id + " reaches a specific Waypoint";
    }

    private void loadStartRouteInfoTime()
    {
        resetStartRouteFields();
        this.startRouteType = "Time";
        startRouteDropdown.index = 0;
        startRouteTimeField.value = vehicle.StartRouteInfo.Time.ToString();
    }

    private void loadStartRouteInfoEgo()
    {
        resetStartRouteFields();
        this.startRouteType = "Ego";
        startRouteDropdown.index = 1;
        startRouteDistanceField.value = vehicle.StartRouteInfo.Distance.ToString();
    }

    private void resetStartRouteFields()
    {
        startRouteDistanceField.value = "5";
        startRouteTimeField.value = "0";
        startRouteDropdown.index = 0;
        startRouteDropdown.style.display = DisplayStyle.Flex;
        startRouteTimeField.style.display= DisplayStyle.Flex;
        startRouteDistanceField.style.display= DisplayStyle.None;
        startRouteWaypointTimeLabel.style.display = DisplayStyle.None;
        deleteStartRouteWaypointButton.style.display = DisplayStyle.None;
        startRouteInfoLabel.style.display = DisplayStyle.Flex;
    }

    private void saveStartRouteInfo(String type)
    {
        switch (type)
        {
            case "Time":
                vehicle.StartRouteInfo = new StartRouteInfo(vehicle, Int32.Parse(startRouteTimeField.value));
                break;
            case "Ego":
                vehicle.StartRouteInfo = new StartRouteInfo(vehicle, 
                    vehicle.SpawnPoint, Int32.Parse(startRouteDistanceField.value), egoVehicle); 
                break;
        }
    }
}
