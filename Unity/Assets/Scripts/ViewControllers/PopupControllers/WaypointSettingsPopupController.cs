using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.Repos;
using System.Text.RegularExpressions;

public class WaypointSettingsPopupController : MonoBehaviour
{
    private WaypointViewController controller;
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

    public void Awake()
    {
        Button ExitButton = this.document.rootVisualElement.Q<Button>("Exit");

        ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            this.vehicle = null;
            this.document.rootVisualElement.style.display = DisplayStyle.None;
        });
    }
    public void open(WaypointViewController controller)
    {
        this.controller = controller;
        //this.vehicle = (Vehicle)controller.getEntity();
        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
        //iDField.value = vehicle.Id.ToString();
        
    }
}
