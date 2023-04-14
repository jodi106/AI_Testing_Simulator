using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// A controller for the World Settings Popup UI that handles user input and updates WorldOptions accordingly.
/// </summary>
public class WorldSettingsPopupController : SettingsPopupController
{
    private WorldOptions options;

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Initializes the controller with the provided WorldOptions and sets the UI to be active but hidden.
    /// </summary>
    /// <param name="options">The WorldOptions to use for initializing the UI.</param>
    public void Init(WorldOptions options)
    {
        this.options = options;
        this.gameObject.SetActive(true);
        this.document = gameObject.GetComponent<UIDocument>();
        this.document.rootVisualElement.style.display = DisplayStyle.None;

        var exitButton = this.document.rootVisualElement.Q<Button>("Exit");
        exitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            OnExit();
        });

        var dayTime = this.document.rootVisualElement.Q<TextField>("Daytime");
        dayTime.RegisterCallback<KeyDownEvent>((KeyDownEvent) =>
        {
            if (KeyDownEvent.keyCode == KeyCode.Return)
            {
                //Debug.Log("Daytime: "+dayTime.text);
                options.Date_Time = (string)dayTime.text;
            }
        });

        var sunIntesity =  this.document.rootVisualElement.Q<Slider>("SunIntensity");
        sunIntesity.RegisterValueChangedCallback((evt) =>
        {
            //Debug.Log("Sun Intensity: "+evt.newValue);
            this.options.SunIntensity = (float)evt.newValue;
        });

        List<string> cloudStateOptions = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(CloudState)))
        {
            cloudStateOptions.Add(option.ToString());
        }
        var cloudState =  this.document.rootVisualElement.Q<DropdownField>("CloudState");
        cloudState.choices = cloudStateOptions;
        cloudState.RegisterValueChangedCallback((evt) =>
        {
            int index = cloudStateOptions.IndexOf(evt.newValue);
            CloudState userOption = (CloudState)index;
            //Debug.Log("Cloud State: " + userOption);
            this.options.CloudState = userOption;
        });

        List<string> precipitationTypeOptions = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(PrecipitationType)))
        {
            precipitationTypeOptions.Add(option.ToString());
        }
        var precipitationType =  this.document.rootVisualElement.Q<DropdownField>("PrecipitationType");
        precipitationType.choices = precipitationTypeOptions;
        precipitationType.RegisterValueChangedCallback((evt) =>
        {
            int index = precipitationTypeOptions.IndexOf(evt.newValue);
            PrecipitationType userOption = (PrecipitationType)index;
            //Debug.Log("Precipitation Type: " + userOption);
            this.options.PrecipitationType = userOption;
        });

        var precipitationIntesity =  this.document.rootVisualElement.Q<Slider>("PrecipitationIntensity");
        precipitationIntesity.RegisterValueChangedCallback((evt) =>
        {
            //Debug.Log("Precipitation Intensity: " + precipitationIntesity.showInputField + " " + evt.newValue);
            this.options.PrecipitationIntensity = (float)evt.newValue;
        });

        var sunAzimuth =  this.document.rootVisualElement.Q<Slider>("SunAzimuth");
        sunAzimuth.RegisterValueChangedCallback((evt) =>
        {
            //Debug.Log("Sun Azimuth: " + sunAzimuth.showInputField + " " + evt.newValue);
            this.options.SunAzimuth = (double)evt.newValue;
        });

        var sunElevation =  this.document.rootVisualElement.Q<Slider>("SunElevation");
        sunElevation.RegisterValueChangedCallback((evt) =>
        {
            //Debug.Log("Sun Elevation: " + sunElevation.showInputField + " " + evt.newValue);
            this.options.SunElevation = (double)evt.newValue;
        });

        var fogVisualRange =  this.document.rootVisualElement.Q<TextField>("FogVisualRange");
        fogVisualRange.RegisterCallback<KeyDownEvent>((KeyDownEvent) =>
        {
            if (KeyDownEvent.keyCode == KeyCode.Return)
            {
                Debug.Log("Fog Visual Range: " + fogVisualRange.text);
                //this.options.FogVisualRange = (double)fogVisualRange.value;
            }
        });

        var frictionScaleFactor =  this.document.rootVisualElement.Q<TextField>("FrictionScaleFactor");
        frictionScaleFactor.RegisterCallback<KeyDownEvent>((KeyDownEvent) =>
        {
            if (KeyDownEvent.keyCode == KeyCode.Return)
            {
                Debug.Log("Friction Scale Factor: " + frictionScaleFactor.value);
                //this.options.FrictionScaleFactor = (double)frictionScaleFactor.value;
            }
        });
        var simpleSettingsButton =  this.document.rootVisualElement.Q<Button>("SimpleSettings");
        var advancedSettingsButton =  this.document.rootVisualElement.Q<Button>("AdvancedSettings");
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
        MainController.freeze = false;
        this.document.rootVisualElement.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Sets the display style of the root visual element of the document to 'flex', and freezes the MainController.
    /// </summary>
    public void Open()
    {
        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
        MainController.freeze = true;
    }
}
