using Assets.Enums;
using Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;

public class WaypointSettingsPopupController : MonoBehaviour
{
    private WaypointViewController controller;
    private WarningPopupController warningPopupController;
    private Waypoint waypoint;
    private Adversary vehicle;
    private ObservableCollection<Adversary> allVehicles;
    private int waypointId;

    private UIDocument document;
    private DropdownField[] possibleActionsField; // SpeedAction, StopAction, LaneChangeOption
    private TextField[] actionTextField; // speed value, stop duration
    private DropdownField[] actionDropdownField; // direction

    // Text of actionTextField depending if it belongs to SpeedAction or StopAction
    private static readonly string ACTION_TEXT_SPEED = "Speed:";
    private static readonly string ACTION_TEXT_STOP_DURATION = "Stop Duration (sec):";
    private static readonly int ACTIONS_NR = 3;

    // Logic: after pressing EXIT button, these local actions will override the previous waypoint.actions
    private ActionType[] actions = new ActionType[ACTIONS_NR];

    Button ExitButton;
    Button AddActionButton;
    Button DeleteActionsButton;

    private Toggle startRouteToggle;
    private DropdownField startRouteVehicleField;

    public void Awake()
    {
        this.document = gameObject.GetComponent<UIDocument>();
        this.document.rootVisualElement.style.display = DisplayStyle.None;

        possibleActionsField = new DropdownField[ACTIONS_NR];
        possibleActionsField[0] = this.document.rootVisualElement.Q<DropdownField>("Actions1");
        possibleActionsField[1] = this.document.rootVisualElement.Q<DropdownField>("Actions2");
        possibleActionsField[2] = this.document.rootVisualElement.Q<DropdownField>("Actions3");

        actionTextField = new TextField[ACTIONS_NR];
        actionTextField[0] = this.document.rootVisualElement.Q<TextField>("TextField1");
        actionTextField[1] = this.document.rootVisualElement.Q<TextField>("TextField2");
        actionTextField[2] = this.document.rootVisualElement.Q<TextField>("TextField3");

        actionDropdownField = new DropdownField[ACTIONS_NR];
        actionDropdownField[0] = this.document.rootVisualElement.Q<DropdownField>("Dropdown1");
        actionDropdownField[1] = this.document.rootVisualElement.Q<DropdownField>("Dropdown2");
        actionDropdownField[2] = this.document.rootVisualElement.Q<DropdownField>("Dropdown3");

        for (int i = 0; i < ACTIONS_NR; i++)
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
                string title = "No Vehicle selected";
                string description = "You must select another vehicle to start that vehicle's route\nor disable the toggle!";
                warningPopupController.open(title, description);
                return;
            }
            overwriteActionsCarla(this.actions); // TODO move method to waypoint class ?
            MainController.freeze = false;
            this.document.rootVisualElement.style.display = DisplayStyle.None;
        });

        AddActionButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            int numberOfActions = actions.Count(s => s != null);
            
            if ((possibleActionsField[numberOfActions].style.display == DisplayStyle.Flex) 
            && possibleActionsField[numberOfActions].value == null)
            {
                //#if UNITY_EDITOR
                //EditorUtility.DisplayDialog(
                //"No Action selected",
                //"You must select an action first before adding new actions!",
                //"Ok");
                //#endif

                string title = "No Action selected";
                string description = "You must select an action first before adding new actions!";
                this.warningPopupController.open(title, description);

                return;
            }

            possibleActionsField[numberOfActions].style.display = DisplayStyle.Flex;
            configureActionChoices(numberOfActions);

            if (numberOfActions >= ACTIONS_NR-1) // max number of actions reached
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
                resetStartRouteVehicleToggle();
                if (this.waypoint.StartRouteOfOtherVehicle != null)
                {
                    this.waypoint.StartRouteOfOtherVehicle.StartRouteInfo = new StartRouteInfo(vehicle, 0); ;
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
            foreach (Adversary veh in allVehicles)
            {
                if (veh.Id == evt.newValue) 
                {
                    if (veh.StartRouteInfo != null && veh.StartRouteInfo.Type == "Waypoint") // check if another vehicle already starts that vehicle's route
                    {
                        //#if UNITY_EDITOR
                        //EditorUtility.DisplayDialog(
                        //    "Vehicle already chosen by another Waypoint",
                        //    "This vehicle is already started by another Waypoint! " +
                        //    "You can remove that option in the corresponding waypoint or in the " + 
                        //    veh.Id + " vehicle settings.",
                        //    "Ok");
                        //#endif

                        string title = "Vehicle already chosen by another Waypoint";
                        string description = "This vehicle is already started by another Waypoint! " +
                        "\nYou can remove that option in the corresponding waypoint or in the\n" +
                            veh.Id + " vehicle settings.";
                        this.warningPopupController.open(title, description);

                        startRouteVehicleField.value = null;
                        return;
                    }

                    veh.StartRouteInfo = new StartRouteInfo(this.vehicle, this.waypoint);
                    this.waypoint.StartRouteOfOtherVehicle = veh;
                    Debug.Log("Start route of that vehicle: " + veh.Id);
                    break;
                }
            }
        });
    }


    public void open(WaypointViewController controller, BaseEntity vehicle, ObservableCollection<Adversary> allSimVehicles, WarningPopupController warning)
    {
        this.controller = controller;
        this.controller.deselect();
        this.warningPopupController = warning;
        this.waypoint = controller.waypoint;
        this.vehicle = (Adversary)vehicle;
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
                        updateActionTextField(waypoint.Actions[i].Name, waypoint.Actions[i].AbsoluteTargetSpeedValueKMH, i, ACTION_TEXT_SPEED);
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
            if (waypoint.Actions.Count >= ACTIONS_NR) AddActionButton.style.display = DisplayStyle.None;
        }

        // ---------------------- Start Route Properties ----------------------

        bool otherVehicleExists = false;

        // Add choices for dropdown GUI element (all other simulation vehicles)
        startRouteVehicleField.choices = new List<string> { };
        foreach (Adversary veh in allSimVehicles)
        {
            if (veh.Id != vehicle.Id) // don't add yourself
            {
                startRouteVehicleField.choices.Add(veh.Id.ToString());

                if (waypoint.StartRouteOfOtherVehicle == veh) otherVehicleExists = true;
               
            }
        }

        if (waypoint.StartRouteOfOtherVehicle == null || !otherVehicleExists) resetStartRouteVehicleToggle();
        else
        {
            startRouteToggle.SetEnabled(true);
            startRouteToggle.value = true;
            startRouteVehicleField.value = waypoint.StartRouteOfOtherVehicle.Id;
        }

        if (startRouteVehicleField.choices.Count() > 0) startRouteToggle.SetEnabled(true);

        this.document.rootVisualElement.style.display = DisplayStyle.Flex; // Show this GUI

        MainController.freeze = true;
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
                    actions[index].AbsoluteTargetSpeedValueKMH = InputEvent.newData.Length == 0 ? 0 : Double.Parse(InputEvent.newData);
                }
                else if (possibleActionsField[index].value == "StopAction")
                {
                    actions[index].StopDuration = InputEvent.newData.Length == 0 ? 0 : Double.Parse(InputEvent.newData);
                    //actions[index].AbsoluteTargetSpeedValueKMH = vehicle.InitialSpeedKMH; // TODO
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
        this.actionDropdownField[i].style.display = DisplayStyle.None;
        if (actionName == "SpeedAction")
        {
            this.actionTextField[i].value = value.ToString();
            this.actions[i] = new ActionType(actionName, value);
        }
        else if (actionName == "StopAction")
        {
            this.actionTextField[i].value = value.ToString();
            this.actions[i] = new ActionType(actionName, value, GetCurrentSpeed()); 
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

        this.actions[i] = new ActionType(actionName, vehicle.Id, targetLaneValue); // LaneChangeAction
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
        for (int i = 0; i < ACTIONS_NR; i++)
        {
            this.possibleActionsField[i].value = null;
            this.possibleActionsField[i].value = null;
            this.actionTextField[i].value = null;
            this.possibleActionsField[i].style.display = DisplayStyle.None;
            this.actionDropdownField[i].style.display = DisplayStyle.None;
            this.actionTextField[i].style.display = DisplayStyle.None;
        }
        this.actions = new ActionType[ACTIONS_NR];
        this.possibleActionsField[0].style.display = DisplayStyle.Flex;
        AddActionButton.style.display = DisplayStyle.Flex;
    }

    private void resetStartRouteVehicleToggle()
    {
        startRouteToggle.value = false;
        startRouteVehicleField.value = null;
        startRouteVehicleField.SetEnabled(false);
    }

    private void overwriteActionsCarla(ActionType[] actions)
    {
        /// Add all GUI actions to the waypoint's attribute 'Actions'
        waypoint.Actions = new List<ActionType>();

        int actionsNotNullLength = this.actions.Count(s => s != null);
        for (int i = 0; i < actionsNotNullLength; i++)
        {
            waypoint.Actions.Add(actions[i]);

            if (actions[i].Name == "SpeedAction")
            {
                SetCorrectCurrentSpeedToNextStopAction(actions[i].AbsoluteTargetSpeedValueKMH);
            }
        }
    }

    /// User changes currentSpeed after a StopAction in another waypoint was added
    private void SetCorrectCurrentSpeedToNextStopAction(double currentSpeed)
    {
        bool done = false;

        int start = vehicle.Path.WaypointList.FindIndex(waypoint => waypoint == this.waypoint);
        for (int i = start + 1; i < vehicle.Path.WaypointList.Count; i++)
        {
            Waypoint w = vehicle.Path.WaypointList[i];
            if (w.Actions?.Any() != null)
            {
                foreach (ActionType ac in w.Actions)
                {
                    // there's another speedAction before the next StopAction
                    if (ac.Name == "SpeedAction")
                    {
                        currentSpeed = ac.AbsoluteTargetSpeedValueKMH;
                        if (done) return;
                    }
                    if (ac.Name == "StopAction")
                    {
                        ac.AbsoluteTargetSpeedValueKMH = currentSpeed;
                        done = true;
                        //return;
                    }
                }
            }
        }
    }

    private double GetCurrentSpeed()
    {
        double currentSpeed = vehicle.InitialSpeedKMH;

        foreach (Waypoint w in vehicle.Path.WaypointList)
        {
            if (w == waypoint) return currentSpeed;

            if (w.Actions?.Any() != null)
            {
                foreach (ActionType ac in w.Actions)
                {
                    if (ac.Name == "SpeedAction")
                    {
                        currentSpeed = ac.AbsoluteTargetSpeedValueKMH;
                    }
                }
            }
        }
        return currentSpeed; // shouldn't be reached
    }
}
