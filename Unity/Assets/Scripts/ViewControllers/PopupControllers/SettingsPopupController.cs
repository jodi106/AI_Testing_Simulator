using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class SettingsPopupController : MonoBehaviour
{

    protected VisualElement tint;
    protected UIDocument document;

    protected abstract void OnExit();

    public virtual void Awake()
    {
        this.document = gameObject.GetComponent<UIDocument>();
        this.document.rootVisualElement.style.display = DisplayStyle.None;

        tint = this.document.rootVisualElement.Q<VisualElement>("Tint");
        tint.RegisterCallback<ClickEvent>((clickEvent) =>
        {
            if (clickEvent.target == tint)
            {
                OnExit();
            }
        });
    }

    public void Update()
    {
        if (this.document.rootVisualElement.style.display != DisplayStyle.None && Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit();
        }
    }
}