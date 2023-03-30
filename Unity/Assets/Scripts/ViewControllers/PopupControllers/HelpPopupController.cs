using UnityEngine;
using UnityEngine.UIElements;

public class HelpPopupController : MonoBehaviour
{
    private UIDocument document;
    private Label Title;
    private Label Description;
    private Toggle DoNotShowAgain;
    private Button ExitButton;

    private int helpIndex = -1;

    public void Awake()
    {
        this.document = gameObject.GetComponent<UIDocument>();
        this.document.rootVisualElement.style.display = DisplayStyle.None;

        Title = this.document.rootVisualElement.Q<Label>("Title");
        Description = this.document.rootVisualElement.Q<Label>("Description");
        DoNotShowAgain = this.document.rootVisualElement.Q<Toggle>("DoNotShowAgain");
        ExitButton = this.document.rootVisualElement.Q<Button>("OkayButton");

        ExitButton.RegisterCallback<ClickEvent>((ClickEvent) =>
        {
            if (DoNotShowAgain.value == true && helpIndex >= 0) MainController.helpComplete[helpIndex] = true;
            this.document.rootVisualElement.style.display = DisplayStyle.None;
            MainController.freeze = false;
        });
    }

    public void open(string title, string description, int helpIndex)
    {
        MainController.freeze = true;

        Title.text = title;
        Description.text = description;

        this.helpIndex = helpIndex;

        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}
