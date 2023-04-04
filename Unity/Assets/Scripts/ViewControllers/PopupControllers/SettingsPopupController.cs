using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class SettingsPopupController : MonoBehaviour
{

    protected VisualElement tint;
    protected UIDocument document;

    protected abstract void onExit();

    public virtual void Awake()
    {
        this.document = gameObject.GetComponent<UIDocument>();
        this.document.rootVisualElement.style.display = DisplayStyle.None;

        tint = this.document.rootVisualElement.Q<VisualElement>("Tint");
        tint.RegisterCallback<ClickEvent>((clickEvent) =>
        {
            onExit();
        });
    }

    public void Update()
    {
        if (this.document.rootVisualElement.style.display != DisplayStyle.None && Input.GetKeyDown(KeyCode.Escape))
        {
            onExit();
        }
    }
}