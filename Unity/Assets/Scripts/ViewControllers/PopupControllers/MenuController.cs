using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    private WorldOptions options;
    private UIDocument document;
    public void init(WorldOptions options)
    {
        this.options = options;
        this.gameObject.SetActive(true);
        this.document = gameObject.GetComponent<UIDocument>();
        this.document.rootVisualElement.style.display = DisplayStyle.None;

        var exitButton = this.document.rootVisualElement.Q<Button>("Exit");
        exitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            this.document.rootVisualElement.style.display = DisplayStyle.None;
        });

        var date = this.document.rootVisualElement.Q<TextField>("Date");
        date.RegisterCallback<KeyDownEvent>((KeyDownEvent) =>
        {
            if (KeyDownEvent.keyCode == KeyCode.Return)
            {
                //Debug.Log("Daytime: "+dayTime.text);
                options.Date = (string)date.text;
            }
        });

        List<string> dayTimeOptions = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(CloudState)))
        {
            dayTimeOptions.Add(option.ToString());
        }

        List<string> timeDayOptions = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(CloudState)))
        {
            timeDayOptions.Add(option.ToString());
        }

        var timeDay = this.document.rootVisualElement.Q<DropdownField>("TimeDay");
        timeDay.choices = timeDayOptions;
        timeDay.RegisterValueChangedCallback((evt) =>
        {
            int index = timeDayOptions.IndexOf(evt.newValue);
            TimeDay userOption = (TimeDay)index;
            //Debug.Log("Cloud State: " + userOption);
            this.options.TimeDay = userOption;
        });


        var visibility = this.document.rootVisualElement.Q<Slider>("Visibility");
        visibility.RegisterValueChangedCallback((evt) =>
        {
            //Debug.Log("Sun Intensity: "+evt.newValue);
            this.options.visibility = (float)evt.newValue;
        });

    }
    public void open()
    {
        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}

