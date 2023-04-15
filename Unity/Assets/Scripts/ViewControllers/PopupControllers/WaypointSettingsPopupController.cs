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


///<summary>
///This class is responsible for the waypoint settings popup.
///</summary>
public class WaypointSettingsPopupController : SettingsPopupController
{
    private WaypointViewController controller;
    private WarningPopupController warningPopupController;
    private Waypoint waypoint;
    private Adversary vehicle;
    private ObservableCollection<Adversary> allVehicles;

    private HelpPopupController helpPopupController;

    private DropdownField[] possibleActionsField; // SpeedAction, StopAction, LaneChangeOption
    private TextField[] actionTextField; // speed value, stop duration
    private DropdownField[] actionDropdownField; // direction

    // Text of actionTextField depending if it belongs to SpeedAction or StopAction
    private static readonly string ACTION_TEXT_SPEED = "Speed (km/h):";
    private static readonly string ACTION_TEXT_STOP_DURATION = "Stop Duration (sec):";
    private static int ACTIONS_NR = 3;

    // Logic: after pressing EXIT button, these local actions will override the previous waypoint.actions (Carla)
    private ActionType[] actions = new ActionType[ACTIONS_NR];

    Button ExitButton;
    Button AddActionButton;
    Button DeleteActionsButton;

    private Toggle startRouteToggle;
    private DropdownField startRouteVehicleField;

    private bool deactivateLaneChangeOption = false;

    /// <summary>
    /// Called when the object is created. It retrieves references to UI elements in the scene and initializes them.
    /// It also initializes the event handlers for the action fields, exit button, add action button and delete actions button.
    /// </summary>
    public override void Awake()
    {
        base.Awake();

        helpPopupController = GameObject.Find("PopUps").transform.Find("HelpPopUp").gameObject.GetComponent<HelpPopupController>();
        helpPopupController.gameObject.SetActive(true);

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

        if (deactivateLaneChangeOption)
        {
            ACTIONS_NR = 2;
        }

        for (int i = 0; i < ACTIONS_NR; i++)
        {
            InitActionEventHandler(i);
            InitActionFieldsEventHandler(i);
        }

        ExitButton = this.document.rootVisualElement.Q<Button>("Exit");
        AddActionButton = this.document.rootVisualElement.Q<Button>("AddAction");
        DeleteActionsButton = this.document.rootVisualElement.Q<Button>("DeleteActions");

        ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            OnExit();
        });

        AddActionButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            int numberOfActions = actions.Count(s => s != null);

            if ((possibleActionsField[numberOfActions].style.display == DisplayStyle.Flex)
            && possibleActionsField[numberOfActions].value == null)
            {
                string title = "No Action selected";
                string description = "You must select an action first before adding new actions!";
                this.warningPopupController.Open(title, description);

                return;
            }

            possibleActionsField[numberOfActions].style.display = DisplayStyle.Flex;
            ConfigureActionChoices(numberOfActions);

            if (numberOfActions >= ACTIONS_NR - 1) // max number of actions reached
            {
                AddActionButton.style.display = DisplayStyle.None;
            }
        });

        DeleteActionsButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            ResetAllActionFields();
            ConfigureActionChoices(0);
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
                ResetStartRouteVehicleToggle();
                if (this.waypoint.StartRouteOfOtherVehicle != null)
                {
                    this.waypoint.StartRouteOfOtherVehicle.StartPathInfo = new StartPathInfo(vehicle, 0); ;
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
                if (veh.Id != evt.newValue) continue; // Get the corresponding vehicle instance

                // Check if another vehicle already starts that vehicle's route --> Popup Warning
                if (veh.StartPathInfo != null && veh.StartPathInfo.Type == "Waypoint")
                {
                    if (waypoint.StartRouteOfOtherVehicle != null && waypoint.StartRouteOfOtherVehicle.Id == veh.Id) return; // veh just has another id but nothing changend
                    string title = "Vehicle already chosen by another Waypoint";
                    string description = "This vehicle is already started by another Waypoint! " +
                    "\nYou can remove that option in the corresponding waypoint or in the\n" +
                        veh.Id + " vehicle settings.";
                    this.warningPopupController.Open(title, description);
                    startRouteVehicleField.value = null;
                    return;
                }

                // Delete old startRouteInfo
                if (evt.previousValue != null)
                {
                    foreach (Adversary vehPrevious in allVehicles)
                    {
                        if (vehPrevious.Id != evt.previousValue) continue;
                        this.waypoint.StartRouteOfOtherVehicle.StartPathInfo = new StartPathInfo(vehicle, 0);
                        this.waypoint.StartRouteOfOtherVehicle = null;
                        break;
                    }
                }

                // Set new startRouteInfo 
                veh.StartPathInfo = new StartPathInfo(this.vehicle, this.waypoint);
                this.waypoint.StartRouteOfOtherVehicle = veh;
                Debug.Log("Start route of that vehicle: " + veh.Id);
                break;
            }
        });
    }

    protected override void OnExit()
    {
        if (startRouteToggle.value == true && startRouteVehicleField.value == null)
        {
            string title = "No Vehicle selected";
            string description = "You must select another vehicle to start that vehicle's route\nor disable the toggle!";
            warningPopupController.Open(title, description);
            return;
        }
        OverwriteActionsCarla(this.actions); // TODO move method to waypoint class ?
        MainController.freeze = false;
        this.document.rootVisualElement.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Opens the GUI for the given waypoint and vehicle, setting GUI elements to corresponding data.
    /// </summary>
    /// <param name="controller">The WaypointViewController controlling the waypoint to open.</param>
    /// <param name="vehicle">The BaseEntity vehicle to open the waypoint for.</param>
    /// <param name="allSimVehicles">An ObservableCollection of Adversary vehicles in the simulation.</param>
    /// <param name="warning">The WarningPopupController for the GUI.</param>
    public void Open(WaypointViewController controller, BaseEntity vehicle, ObservableCollection<Adversary> allSimVehicles, WarningPopupController warning)
    {
        this.controller = controller;
        this.controller.Deselect();
        this.warningPopupController = warning;
        this.waypoint = controller.Waypoint;
        this.vehicle = (Adversary)vehicle;
        this.allVehicles = allSimVehicles;

        // Set GUI elements to corresponding data
        ResetAllActionFields();
        ConfigureActionChoices(0);

        if (waypoint.Actions?.Any() == true) // actions not null or empty)
        {
            for (int i = 0; i < waypoint.Actions.Count; i++)
            {
                switch (waypoint.Actions[i].Name)
                {
                    case "SpeedAction":
                        UpdateActionTextField(waypoint.Actions[i].Name, waypoint.Actions[i].AbsoluteTargetSpeedValueKMH, i, ACTION_TEXT_SPEED);
                        if (i <= 1) ConfigureActionChoices(i + 1);
                        break;

                    case "StopAction":
                        UpdateActionTextField(waypoint.Actions[i].Name, waypoint.Actions[i].StopDuration, i, ACTION_TEXT_STOP_DURATION);
                        if (i <= 1) ConfigureActionChoices(i + 1);
                        break;

                    case "LaneChangeAction":
                        UpdateActionDropownField(waypoint.Actions[i].Name, waypoint.Actions[i].RelativeTargetLaneValue, i);
                        if (i <= 1) ConfigureActionChoices(i + 1);
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

        if (waypoint.StartRouteOfOtherVehicle == null || !otherVehicleExists) ResetStartRouteVehicleToggle();
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

    /// <summary>
    /// Initializes an event handler for a given action field at the specified index in the list of possible actions.
    /// The event handler updates the corresponding GUI element based on the selected action.
    /// </summary>
    /// <param name="index">The index of the action field in the list of possible actions.</param>
    private void InitActionEventHandler(int index)
    {
        possibleActionsField[index].RegisterValueChangedCallback((inputEvent) =>
        {
            if (inputEvent.newValue == "SpeedAction")
            {
                UpdateActionTextField(inputEvent.newValue, 0, index, ACTION_TEXT_SPEED);
            }
            else if (inputEvent.newValue == "StopAction")
            {
                UpdateActionTextField(inputEvent.newValue, 0, index, ACTION_TEXT_STOP_DURATION);
            }
            else if (inputEvent.newValue == "LaneChangeAction")
            {
                UpdateActionDropownField(inputEvent.newValue, 0, index);
                if (!MainController.helpComplete[1])
                {
                    string title = "LaneChangeAction only for experienced users!";
                    string description = "To do a lane change, you should just place more waypoints on an entity's path."
                    + "\nThis option will BREAK your scenario if it's near an intersection! Do NOT do that!";
                    this.helpPopupController.Open(title, description, 1);
                }
            }
        });
    }

    /// <summary>
    /// Initializes the event handler for the action fields at a specific index.
    /// </summary>
    /// <param name="index">The index of the action field.</param>
    /// <remarks>
    /// This method registers the callback events for the action text and action dropdown fields at the specified index.
    /// If the input data in the action text field contains only digits, the method sets the corresponding action values
    /// in the actions array. If the possible action field value is "SpeedAction", the method sets the absolute target speed
    /// value, otherwise, it sets the stop duration. If the input data in the action text field contains non-digit characters,
    /// the method restores the previous data. If the possible action field value is "LaneChangeAction", the method sets the
    /// relative target lane value based on the selected value in the action dropdown field. If the selected value is "right",
    /// the method sets the value to -1, otherwise, it sets it to 1.
    /// </remarks>
    private void InitActionFieldsEventHandler(int index)
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

    /// <summary>
    /// Updates the action text field based on the given action name, value, index and text.
    /// </summary>
    /// <param name="actionName">The name of the action to be updated.</param>
    /// <param name="value">The value of the action to be updated.</param>
    /// <param name="i">The index of the action to be updated.</param>
    /// <param name="text">The text to be displayed in the action text field.</param>
    private void UpdateActionTextField(string actionName, double value, int i, string text)
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

    /// <summary>
    /// Updates the action dropdown field with the specified action name and target lane value, and sets the action type for the specified index.
    /// </summary>
    /// <param name="actionName">The name of the action to be set in the dropdown field.</param>
    /// <param name="targetLaneValue">The target lane value to be set in the dropdown field.</param>
    /// <param name="i">The index of the action to be updated.</param>
    private void UpdateActionDropownField(string actionName, int targetLaneValue, int i)
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

    /// <summary>
    /// Configures the available action choices for a specific index, based on the previously selected actions for the other indices.
    /// </summary>
    /// <param name="index">The index of the action field to be configured.</param>
    /// <remarks>
    /// The method loops through all the possible action types and adds them as choices to the action field for the specified index,
    /// with the exception of the "AssignRouteAction" type. Then, for indices 1 and 2, it removes the previously selected action types
    /// from the choices of the other action fields, to prevent duplicate selections.
    /// </remarks>
    private void ConfigureActionChoices(int index)
    {
        possibleActionsField[index].choices = new List<string> { };

        foreach (var option in Enum.GetValues(typeof(ActionTypeName)))
        {
            // Special cases: Do not add options to the dropdown
            if (deactivateLaneChangeOption && option.ToString() == "LaneChangeAction")
            {
                continue;
            }

            if (option.ToString() == "AssignRouteAction")
            {
                continue;
            }

            // Add options to the dropdown
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

    /// <summary>
    /// Resets all action fields to their default state and initializes the action type array.
    /// </summary>
    /// <remarks>
    /// This method is used to reset all the action fields to their default state when the user clears the action configuration.
    /// It initializes the action type array and hides all the action fields except the first one.
    /// </remarks>
    private void ResetAllActionFields()
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

    /// <summary>
    /// Resets the start route vehicle toggle to its default state.
    /// </summary>
    /// <remarks>
    /// This method is used to reset the start route vehicle toggle to its default state when the user clears the action configuration.
    /// It sets the toggle value to false, clears the selected vehicle from the vehicle dropdown, and disables the dropdown.
    /// </remarks>
    private void ResetStartRouteVehicleToggle()
    {
        startRouteToggle.value = false;
        startRouteVehicleField.value = null;
        startRouteVehicleField.SetEnabled(false);
    }

    /// <summary>
    /// Overwrites the actions of a waypoint with the given list of ActionType objects.
    /// </summary>
    /// <param name="actions">The list of ActionType objects to be added to the waypoint's attribute 'Actions'.</param>
    private void OverwriteActionsCarla(ActionType[] actions)
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

    // <summary>
    /// Sets the correct current speed to the next StopAction in the vehicle's path.
    /// User changes currentSpeed after a StopAction in another waypoint was added
    /// </summary>
    /// <param name="currentSpeed">The current speed value.</param>
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

    /// <summary>
    /// Gets the current speed of the vehicle up to the current waypoint.
    /// </summary>
    /// <returns>The current speed in km/h.</returns
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
