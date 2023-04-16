using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;

/// <summary>
/// A controller for the World Settings Popup UI that handles user input and updates WorldOptions accordingly.
/// </summary>
public class WorldSettingsPopupController : SettingsPopupController
{
    private WarningPopupController warningPopupController;
    private WorldOptions options;

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Initializes the controller with the provided WorldOptions and sets the UI to be active but hidden.
    /// </summary>
    /// <param name="options">The WorldOptions to use for initializing the UI.</param>
    public void Init(WorldOptions options, WarningPopupController warningPopupController)
    {
        this.options = options;
        this.warningPopupController = warningPopupController;
        this.gameObject.SetActive(true);
        this.document = gameObject.GetComponent<UIDocument>();
        this.document.rootVisualElement.style.display = DisplayStyle.None;

        var exitButton = this.document.rootVisualElement.Q<Button>("Exit");
        exitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {        
            OnExit();
        });

        var dayTime = this.document.rootVisualElement.Q<TextField>("Daytime");
        dayTime.RegisterValueChangedCallback((evt) =>
        {
            options.Date_Time = (string)evt.newValue;
        });

        var sunIntesity = this.document.rootVisualElement.Q<Slider>("SunIntensity");
        sunIntesity.RegisterValueChangedCallback((evt) =>
        {
            this.options.SunIntensity = (float)evt.newValue;
        });

        List<string> cloudStateOptions = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(CloudState)))
        {
            cloudStateOptions.Add(option.ToString());
        }
        var cloudState = this.document.rootVisualElement.Q<DropdownField>("CloudState");
        cloudState.choices = cloudStateOptions;
        cloudState.RegisterValueChangedCallback((evt) =>
        {
            int index = cloudStateOptions.IndexOf(evt.newValue);
            CloudState userOption = (CloudState)index;
            this.options.CloudState = userOption;
        });

        var precipitationIntesity = this.document.rootVisualElement.Q<Slider>("PrecipitationIntensity");
        precipitationIntesity.RegisterValueChangedCallback((evt) =>
        {
            this.options.PrecipitationIntensity = (float)evt.newValue;
        });

        List<string> precipitationTypeOptions = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(PrecipitationType)))
        {
            precipitationTypeOptions.Add(option.ToString());
        }
        var precipitationType = this.document.rootVisualElement.Q<DropdownField>("PrecipitationType");
        precipitationType.choices = precipitationTypeOptions;
        precipitationType.RegisterValueChangedCallback((evt) =>
        {
            int index = precipitationTypeOptions.IndexOf(evt.newValue);
            PrecipitationType userOption = (PrecipitationType)index;
            this.options.PrecipitationType = userOption;

            // PrecipitationType is changed --> Set PrecipitationIntensity to a fitting default value
            if (this.options.PrecipitationType == PrecipitationType.Rain)
            {
                this.options.PrecipitationIntensity = (float)0.8;
                precipitationIntesity.value = (float)0.8;
            } 
            else if (this.options.PrecipitationType == PrecipitationType.Dry)
            {
                this.options.PrecipitationIntensity = (float)0.0;
                precipitationIntesity.value = (float)0.0;
            }
        });

        var sunAzimuth = this.document.rootVisualElement.Q<Slider>("SunAzimuth");
        sunAzimuth.RegisterValueChangedCallback((evt) =>
        {
            this.options.SunAzimuth = (double)evt.newValue;
        });

        var sunElevation = this.document.rootVisualElement.Q<Slider>("SunElevation");
        sunElevation.RegisterValueChangedCallback((evt) =>
        {
            this.options.SunElevation = (double)evt.newValue;
        });

        var fogVisualRange = this.document.rootVisualElement.Q<TextField>("FogVisualRange");
        fogVisualRange.RegisterCallback<KeyDownEvent>((KeyDownEvent) =>
        {
            if (KeyDownEvent.keyCode == KeyCode.Return)
            {
                Debug.Log("Fog Visual Range: " + fogVisualRange.text);
                //this.options.FogVisualRange = (double)fogVisualRange.value;
            }
        });

        var frictionScaleFactor = this.document.rootVisualElement.Q<TextField>("FrictionScaleFactor");
        frictionScaleFactor.RegisterCallback<KeyDownEvent>((KeyDownEvent) =>
        {
            if (KeyDownEvent.keyCode == KeyCode.Return)
            {
                Debug.Log("Friction Scale Factor: " + frictionScaleFactor.value);
                //this.options.FrictionScaleFactor = (double)frictionScaleFactor.value;
            }
        });
        var simpleSettingsButton = this.document.rootVisualElement.Q<Button>("SimpleSettings");
        var advancedSettingsButton = this.document.rootVisualElement.Q<Button>("AdvancedSettings");
        advancedSettingsButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            frictionScaleFactor.visible = true;
            fogVisualRange.visible = true;
            sunAzimuth.visible = true;
            sunIntesity.visible = true;
            sunElevation.visible = true;
            precipitationIntesity.visible = true;
            simpleSettingsButton.visible = true;
            advancedSettingsButton.visible = false;
        });


        simpleSettingsButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            frictionScaleFactor.visible = false;
            fogVisualRange.visible = false;
            sunAzimuth.visible = false;
            sunIntesity.visible = false;
            sunElevation.visible = false;
            precipitationIntesity.visible = false;
            simpleSettingsButton.visible = false;
            advancedSettingsButton.visible = true;
        });

    }


    protected override void OnExit()
    {
        if (!Regex.IsMatch(options.Date_Time, @"^(([01][0-9])|(2[0-4])):[0-5][0-9]:[0-9][0-9]$"))  // Input format should be hh:mm:ss
        {
            string title = "DayTime is in the wrong format!";
            string description = "Daytime must be in the format hh:mm:ss\nFor example: 12:00:00.\nAdjust it to close this GUI!";
            warningPopupController.Open(title, description);
            return;
        }

        this.document.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void Open()
    {
        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}
