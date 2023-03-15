using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.Entities;

public class MenuController : MonoBehaviour
{
    private MenuOptions options;
    private UIDocument document;
    public void init(MenuOptions options)
    {
        this.options = options;
        this.gameObject.SetActive(true);

        this.document = gameObject.GetComponent<UIDocument>();
        Debug.Log("document" + this.document);
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

        List<string> timeDayOptions = new List<string> { };
        foreach (var option in Enum.GetValues(typeof(TimeDay)))
        {
            timeDayOptions.Add(option.ToString());
        }

        var timeDay = this.document.rootVisualElement.Q<DropdownField>("DayTime");
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
            this.options.Visibility = evt.newValue;
        });

    }
    public void open()
    {
        Debug.Log(document);
        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}

