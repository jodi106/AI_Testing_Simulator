using Assets.Enums;
using Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class WaypointSettingsPopupController : MonoBehaviour
{
    private WaypointViewController controller;
    private Waypoint waypoint;
    private Vehicle vehicle;
    private ObservableCollection<Vehicle> allVehicles;
    private int waypointId;

    private UIDocument document;
    private DropdownField[] possibleActionsField; // SpeedAction, StopAction, LaneChangeOption
    private TextField[] actionTextField; // speed value, stop duration
    private DropdownField[] actionDropdownField; // direction

    // Text of actionTextField depending if it belongs to SpeedAction or StopAction
    private static readonly string ACTION_TEXT_SPEED = "Speed:";
    private static readonly string ACTION_TEXT_STOP_DURATION = "Stop Duration (sec):";

    // Logic: after pressing EXIT button, these local actions will override the previous waypoint.actions
    private ActionType[] actions = new ActionType[3];

    Button ExitButton;
    Button AddActionButton;
    Button DeleteActionsButton;

    private Toggle startRouteToggle;
    private DropdownField startRouteVehicleField;

    public void Awake()
    {
        this.document = gameObject.GetComponent<UIDocument>();
        this.document.rootVisualElement.style.display = DisplayStyle.None;

        possibleActionsField = new DropdownField[3];
        possibleActionsField[0] = this.document.rootVisualElement.Q<DropdownField>("Actions1");
        possibleActionsField[1] = this.document.rootVisualElement.Q<DropdownField>("Actions2");
        possibleActionsField[2] = this.document.rootVisualElement.Q<DropdownField>("Actions3");

        actionTextField = new TextField[3];
        actionTextField[0] = this.document.rootVisualElement.Q<TextField>("TextField1");
        actionTextField[1] = this.document.rootVisualElement.Q<TextField>("TextField2");
        actionTextField[2] = this.document.rootVisualElement.Q<TextField>("TextField3");

        actionDropdownField = new DropdownField[3];
        actionDropdownField[0] = this.document.rootVisualElement.Q<DropdownField>("Dropdown1");
        actionDropdownField[1] = this.document.rootVisualElement.Q<DropdownField>("Dropdown2");
        actionDropdownField[2] = this.document.rootVisualElement.Q<DropdownField>("Dropdown3");

        for (int i = 0; i < 3; i++)
        {
            initActionEventHandler(i);
            initActionFieldsEventHandler(i);
        }
        
        ExitButton = this.document.rootVisualElement.Q<Button>("Exit");
        AddActionButton = this.document.rootVisualElement.Q<Button>("AddAction");
        DeleteActionsButton = this.document.rootVisualElement.Q<Button>("DeleteActions");
        
        ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (startRouteToggle.value == true && startRouteVehicleField.value == null)
            {
                EditorUtility.DisplayDialog(
                "No Vehicle selected",
                "You must select another vehicle to start that vehicle's route or disable the toggle!",
                "Ok");
                return;
            }
            overrideActionsCarla(this.actions);
            this.document.rootVisualElement.style.display = DisplayStyle.None;
        });

        AddActionButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            int numberOfActions = actions.Count(s => s != null);
            if (numberOfActions == 0 || possibleActionsField[numberOfActions-1].value == null)
            {
                EditorUtility.DisplayDialog(
                "No Action selected",
                "You must select an action first before adding new actions!",
                "Ok");
                return;
            }

            possibleActionsField[numberOfActions].style.display = DisplayStyle.Flex;
            configureActionChoices(numberOfActions);

            if (numberOfActions >= 2) // max number of actions reached
            {
                AddActionButton.style.display = DisplayStyle.None;
            }
        });

        DeleteActionsButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            resetAllActionFields();
            configureActionChoices(0);
        });

        // ---------------------- Start Route Properties ----------------------

        startRouteToggle = this.document.rootVisualElement.Q<Toggle>("StartRouteConditionEnabled");
        startRouteVehicleField = this.document.rootVisualElement.Q<DropdownField>("StartRouteConditionVehicle");
        startRouteToggle.SetEnabled(false);
        startRouteVehicleField.SetEnabled(false);

        startRouteToggle.RegisterValueChangedCallback((evt) =>
        {
            if (evt.newValue == false)
            {
                deleteStartRouteVehicle();
                if (this.waypoint.StartRouteOfOtherVehicle != null)
                {
                    this.waypoint.StartRouteOfOtherVehicle.StartRouteVehicle = null;
                    this.waypoint.StartRouteOfOtherVehicle = null;
                }
            }
            else
            {
                startRouteVehicleField.SetEnabled(true);
            }
        });

        startRouteVehicleField.RegisterValueChangedCallback((evt) =>
        {
            foreach (Vehicle veh in allVehicles)
            {
                if (veh.Id == evt.newValue) 
                {
                    if (veh.StartRouteVehicle != null) // check if another vehicle already starts that vehicle's route
                    {
                        EditorUtility.DisplayDialog(
                            "Vehicle already chosen by another Waypoint",
                            "This vehicle is already started by another Waypoint! " +
                            "You can remove that option in the corresponding waypoint or in the " + 
                            veh.Id + " vehicle settings.",
                            "Ok");
                        startRouteVehicleField.value = null;
                        return;
                    }

                    veh.StartRouteVehicle = this.vehicle;
                    this.waypoint.StartRouteOfOtherVehicle = veh;
                    Debug.Log("Start route of that vehicle: " + veh.Id);
                    break;
                }
            }
        });
    }

    public void open(WaypointViewController controller, BaseEntity vehicle, ObservableCollection<Vehicle> allSimVehicles)
    {
        this.controller = controller;
        this.waypoint = controller.waypoint;
        this.vehicle = (Vehicle)vehicle;
        this.allVehicles = allSimVehicles;

        // Set GUI elements to corresponding data
        resetAllActionFields();
        configureActionChoices(0);

        if (waypoint.Actions?.Any() == true) // actions not null or empty)
        {
            for (int i = 0; i < waypoint.Actions.Count; i++)
            {
                switch (waypoint.Actions[i].Name)
                {
                    case "SpeedAction":
                        updateActionTextField(waypoint.Actions[i].Name, waypoint.Actions[i].AbsoluteTargetSpeedValue, i, ACTION_TEXT_SPEED);
                        if (i <= 1) configureActionChoices(i+1);
                        break;

                    case "StopAction":
                        updateActionTextField(waypoint.Actions[i].Name, waypoint.Actions[i].StopDuration, i, ACTION_TEXT_STOP_DURATION);
                        if (i <= 1) configureActionChoices(i + 1);
                        break;

                    case "LaneChangeAction":
                        updateActionDropownField(waypoint.Actions[i].Name, waypoint.Actions[i].RelativeTargetLaneValue, i);
                        if (i <= 1) configureActionChoices(i + 1);
                        break;
                }
            }
            if (waypoint.Actions.Count >= 3) AddActionButton.style.display = DisplayStyle.None;
        }

        // Add choices for dropdown GUI element (all other simulation vehicles)
        startRouteVehicleField.choices = new List<string> { };
        foreach (Vehicle veh in allSimVehicles)
        {
            if (veh.Id != vehicle.Id) // don't add yourself
            {
                startRouteVehicleField.choices.Add(veh.Id.ToString());
            }
        }

        if (waypoint.StartRouteOfOtherVehicle == null) deleteStartRouteVehicle();
        else
        {
            startRouteToggle.SetEnabled(true);
            startRouteToggle.value = true;
            startRouteVehicleField.value = waypoint.StartRouteOfOtherVehicle.Id;
        }

        if (startRouteVehicleField.choices.Count() > 0) startRouteToggle.SetEnabled(true);

        this.document.rootVisualElement.style.display = DisplayStyle.Flex; // Show this GUI
    }


    private void initActionEventHandler(int index)
    {
        possibleActionsField[index].RegisterValueChangedCallback((inputEvent) =>
        {
            if (inputEvent.newValue == "SpeedAction")
            {
                updateActionTextField(inputEvent.newValue, 0, index, ACTION_TEXT_SPEED);
            }
            else if (inputEvent.newValue == "StopAction")
            {
                updateActionTextField(inputEvent.newValue, 0, index, ACTION_TEXT_STOP_DURATION);
            }
            else if (inputEvent.newValue == "LaneChangeAction")
            {
                updateActionDropownField(inputEvent.newValue, 0, index);
            }
        });
    }

    private void initActionFieldsEventHandler(int index)
    {
        actionTextField[index].RegisterCallback<InputEvent>((InputEvent) =>
        {
            if (Regex.Match(InputEvent.newData, @"^(\d)*$").Success) // only digits
            {
                if (possibleActionsField[index].value == "SpeedAction")
                {
                    actions[index].AbsoluteTargetSpeedValue = InputEvent.newData.Length == 0 ? 0 : Double.Parse(InputEvent.newData);
                }
                else if (possibleActionsField[index].value == "StopAction")
                {
                    actions[index].StopDuration = InputEvent.newData.Length == 0 ? 0 : Double.Parse(InputEvent.newData);
                }
            }
            else
            {
                actionTextField[index].value = InputEvent.previousData;
            }
        });

        actionDropdownField[index].RegisterValueChangedCallback((InputEvent) =>
        {
            if (possibleActionsField[index].value == "LaneChangeAction")
            {
                if (InputEvent.newValue == "right")
                {
                    actions[index].RelativeTargetLaneValue = -1;
                }
                else
                {
                    actions[index].RelativeTargetLaneValue = 1; // TODO -1 and 1 correct?
                }
                //if (startLane.Id > 0) waypoint.Actions[i].RelativeTargetLaneValue = startLane.Id > endLane.Id ? 1 : -1;
                //if (startLane.Id < 0) waypoint.Actions[i].RelativeTargetLaneValue = startLane.Id > endLane.Id ? -1 : 1;
            }
        });
    }

    private void updateActionTextField(string actionName, double value, int i, string text)
    {
        this.possibleActionsField[i].value = actionName;
        this.possibleActionsField[i].style.display = DisplayStyle.Flex;
        this.actionTextField[i].style.display = DisplayStyle.Flex;
        this.actionTextField[i].label = text;
        if (actionName == "SpeedAction")
        {
            this.actionTextField[i].value = value.ToString();
            this.actions[i] = new ActionType(actionName, value);
        }
        else if (actionName == "StopAction")
        {
            this.actionTextField[i].value = value.ToString();
            this.actions[i] = new ActionType(actionName, value, vehicle.InitialSpeedKMH);
        }
    }

    private void updateActionDropownField(string actionName, int targetLaneValue, int i)
    {
        this.possibleActionsField[i].value = actionName;
        this.possibleActionsField[i].style.display = DisplayStyle.Flex;
        this.actionTextField[i].style.display = DisplayStyle.None;
        this.actionDropdownField[i].style.display = DisplayStyle.Flex;
        this.actionDropdownField[i].choices = new List<string> { "left", "right" };

        if (targetLaneValue == 0) this.actionDropdownField[i].value = null;
        else if (targetLaneValue == 1) this.actionDropdownField[i].value = "left";
        else if (targetLaneValue == -1) this.actionDropdownField[i].value = "right";

        this.actions[i] = new ActionType(actionName, vehicle.Id, targetLaneValue);
    }

    private void configureActionChoices(int index)
    {
        possibleActionsField[index].choices = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(ActionTypeName)))
        {
            if (option.ToString() != "AssignRouteAction")
            {
                if (index == 0)
                {
                    possibleActionsField[index].choices.Add(option.ToString());
                }
                if (index == 1)
                {
                    if (option.ToString() != possibleActionsField[0].value)
                    {
                        possibleActionsField[index].choices.Add(option.ToString());
                        possibleActionsField[0].choices.Remove(option.ToString());
                    }
                }
                if (index == 2)
                {
                    if (option.ToString() != possibleActionsField[0].value && option.ToString() != possibleActionsField[1].value)
                    {
                        possibleActionsField[index].choices.Add(option.ToString());
                        possibleActionsField[0].choices.Remove(option.ToString());
                        possibleActionsField[1].choices.Remove(option.ToString());
                    }
                }
            }
        }
    }

    private void resetAllActionFields()
    {
        for (int i = 0; i < 3; i++)
        {
            this.possibleActionsField[i].value = null;
            this.possibleActionsField[i].value = null;
            this.actionTextField[i].value = null;
            this.possibleActionsField[i].style.display = DisplayStyle.None;
            this.actionDropdownField[i].style.display = DisplayStyle.None;
            this.actionTextField[i].style.display = DisplayStyle.None;
        }
        this.actions = new ActionType[3];
        this.possibleActionsField[0].style.display = DisplayStyle.Flex;
        AddActionButton.style.display = DisplayStyle.Flex;
    }

    private void deleteStartRouteVehicle()
    {
        startRouteToggle.value = false;
        startRouteVehicleField.value = null;
        startRouteVehicleField.SetEnabled(false);
    }

    private void overrideActionsCarla(ActionType[] action)
    {
        waypoint.Actions = new List<ActionType>();

        int actionLengthNotNull = actions.Count(s => s != null);
        for (int i = 0; i < actionLengthNotNull; i++)
        {
            waypoint.Actions.Add(action[i]);
        }
    }
}
