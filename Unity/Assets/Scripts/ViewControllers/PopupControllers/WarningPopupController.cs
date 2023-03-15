using Assets.Enums;
using Entity;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.Repos;
using System.Text.RegularExpressions;

public class WarningPopupController : MonoBehaviour
{
    private UIDocument document;
    private Label Title;
    private Label Description;
    private Button ExitButton;

    public void Awake()
    {
        this.document = gameObject.GetComponent<UIDocument>();
        this.document.rootVisualElement.style.display = DisplayStyle.None;

        Title = this.document.rootVisualElement.Q<Label>("Title");
        Description = this.document.rootVisualElement.Q<Label>("Description");
        ExitButton = this.document.rootVisualElement.Q<Button>("Exit");

        ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            this.document.rootVisualElement.style.display = DisplayStyle.None;
            //Time.timeScale = 1;
        });
    }

    public void open()
    {
        // TODO set Title and Description text
        //Title.text = "hei";

        // TODO user can't click anything else except "ok" button. Freeze background
        //Time.timeScale = 0;
        //bool t = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
    }

}
