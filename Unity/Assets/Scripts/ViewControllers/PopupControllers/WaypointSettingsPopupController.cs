using Assets.Enums;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class WaypointSettingsPopupController : MonoBehaviour
{
    private WaypointViewController controller;
    private Waypoint waypoint;
    private Vehicle vehicleRef;
    private int waypointId;

    private UIDocument document;
    private DropdownField[] possibleActionsField;
    private TextField[] actionTextField;
    private DropdownField[] actionDropdownField;

    private Toggle startRouteToggle;
    private DropdownField startRouteVehicleField;

    private int addedActions = 0;

    public void Awake()
    {
        this.document = gameObject.GetComponent<UIDocument>();
        this.document.rootVisualElement.style.display = DisplayStyle.None;

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
            actionTextField[i].style.display = DisplayStyle.None;
            actionDropdownField[i].style.display = DisplayStyle.None;
        }

        possibleActionsField = new DropdownField[3];
        possibleActionsField[0] = this.document.rootVisualElement.Q<DropdownField>("Actions1");
        possibleActionsField[1] = this.document.rootVisualElement.Q<DropdownField>("Actions2");
        possibleActionsField[2] = this.document.rootVisualElement.Q<DropdownField>("Actions3");
        possibleActionsField[1].style.display = DisplayStyle.None;
        possibleActionsField[2].style.display = DisplayStyle.None;

        initAction(0);

        Button ExitButton = this.document.rootVisualElement.Q<Button>("Exit");
        Button AddActionButton = this.document.rootVisualElement.Q<Button>("AddAction");
        Button DeleteActionsButton = this.document.rootVisualElement.Q<Button>("DeleteActions");

        ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            this.vehicleRef = null;
            this.waypoint = null;
            this.document.rootVisualElement.style.display = DisplayStyle.None;
        });

        AddActionButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (possibleActionsField[addedActions].value == null)
            {
                EditorUtility.DisplayDialog(
                "No Action selected",
                "You must select an action first before adding new actions!",
                "Ok");
                return;
            }
            this.addedActions++;
            possibleActionsField[addedActions].style.display = DisplayStyle.Flex;
            initAction(addedActions);
            if (addedActions >= 2)
            {
                AddActionButton.style.display = DisplayStyle.None;
            }
        });

        DeleteActionsButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            for (int i = 0; i <= addedActions; i++)
            {
                actionTextField[i].value = null;
                actionDropdownField[i].value = null;
                possibleActionsField[i].value = null;
                actionTextField[i].style.display = DisplayStyle.None;
                actionDropdownField[i].style.display = DisplayStyle.None;
                possibleActionsField[i].style.display = DisplayStyle.None;
            }
            addedActions = 0;
            initAction(0);
            possibleActionsField[0].style.display = DisplayStyle.Flex;
            AddActionButton.style.display = DisplayStyle.Flex;
        });

        startRouteToggle = this.document.rootVisualElement.Q<Toggle>("StartRouteConditionEnabled");
        startRouteVehicleField = this.document.rootVisualElement.Q<DropdownField>("StartRouteConditionVehicle");
        
    }

    private void initAction(int index)
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

        possibleActionsField[index].RegisterValueChangedCallback((inputEvent) =>
        {
            if (waypoint.Actions != null)
            {
                waypoint.Actions.Add(new ActionType(inputEvent.newValue, vehicleRef.Id, null)); // TODO
            }

            string value = inputEvent.newValue;
            if (value == "SpeedAction")
            {
                actionTextField[index].style.display = DisplayStyle.Flex;
                actionDropdownField[index].style.display = DisplayStyle.None;
                actionTextField[index].label = "Speed:";
            }
            else if (value == "StopAction")
            {
                actionTextField[index].style.display = DisplayStyle.Flex;
                actionDropdownField[index].style.display = DisplayStyle.None;
                actionTextField[index].label = "Stop Duration (sec):";
            }
            else if (value == "LaneChangeAction")
            {
                actionTextField[index].style.display = DisplayStyle.None;
                actionDropdownField[index].style.display = DisplayStyle.Flex;
                actionDropdownField[index].choices = new List<string> { "left", "right" };
            }
        });
    }

    public void open(WaypointViewController controller, BaseEntity vehicle)
    {
        //Debug.Log("Opening WaypointSettingsPopOp ...");
        //Debug.Log("Waypoint id:  " + controller.waypoint.Id);
        this.controller = controller;
        this.waypoint = controller.waypoint;
        this.vehicleRef = (Vehicle) vehicle;
        bool actionsEmptyOrNull = waypoint.Actions?.Any() != true;
        if (!actionsEmptyOrNull)
        {
            foreach (ActionType action in waypoint.Actions)
            {
                this.possibleActionsField[0].value = action.Name;
            }
        }

        startRouteVehicleField.choices = new List<string> { };
        //foreach (Vehicle vehicle in TODO)
        //{
        //    startRouteVehicleField.choices.Add(vehicle.Id.ToString());
        //}
        startRouteVehicleField.choices.Add(vehicleRef.Id.ToString()); // TODO REPLACE

        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}
