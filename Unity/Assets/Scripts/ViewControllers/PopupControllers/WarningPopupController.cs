using UnityEngine;
using UnityEngine.UIElements;


///<summary>
///Represents a Warning Popup
///</summary>
public class WarningPopupController : MonoBehaviour
{
    //private GameObject FreezeCanvas;

    private UIDocument document;
    private Label Title;
    private Label Description;
    private Button ExitButton;

    /// <summary>
    /// Awake is called when the script instance is being loaded. 
    /// This method initializes the UI elements and sets their default values.
    /// </summary>
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
        });
    }

    /// <summary>
    /// Opens the UI element and sets the title and description text.
    /// </summary>
    /// <param name="title">The title text to set.</param>
    /// <param name="description">The description text to set.</param>
    public void Open(string title, string description)
    {
        MainController.freeze = true;

        Title.text = title;
        Description.text = description;

        this.document.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}
