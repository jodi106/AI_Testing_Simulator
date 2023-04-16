using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.ViewControllers.PopupControllers
{

    /// <summary>
    /// Controller for a yes/no popup dialog box that can be displayed to the user.
    /// </summary>
    public class YesNoPopupController : MonoBehaviour
    {

        private UIDocument document;

        private Label Title;
        private Label Description;
        private Button yesButton;
        private Button noButton;

        private bool result;

        /// <summary>
        /// Initializes the controller by getting the UIDocument component and setting the rootVisualElement to not be displayed.
        /// </summary>
        public void Awake()
        {
            this.document = gameObject.GetComponent<UIDocument>();
            this.document.rootVisualElement.style.display = DisplayStyle.None;

            Title = this.document.rootVisualElement.Q<Label>("Title");
            Description = this.document.rootVisualElement.Q<Label>("Description");
            noButton = this.document.rootVisualElement.Q<Button>("CancelButton");
            yesButton = this.document.rootVisualElement.Q<Button>("ConfirmButton");
        }

        /// <summary>
        /// Shows the yes/no popup dialog box to the user and waits for a response.
        /// </summary>
        /// <param name="title">The title of the dialog box.</param>
        /// <param name="message">The message displayed in the dialog box.</param>
        /// <returns>A Task<bool> representing the user's response. True if they clicked "Yes", false if they clicked "No".</returns>
        public async Task<bool> Show(string title, string message)
        {
            Title.text = title;
            Description.text = message;
            this.document.rootVisualElement.style.display = DisplayStyle.Flex;

            var tcs = new TaskCompletionSource<bool>();

            yesButton.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                if (!tcs.Task.IsCompleted)
                {
                    this.document.rootVisualElement.style.display = DisplayStyle.None;
                    tcs.SetResult(true);
                }
            });

            noButton.RegisterCallback<ClickEvent>((ClickEvent) =>
            {
                if (!tcs.Task.IsCompleted)
                {
                    this.document.rootVisualElement.style.display = DisplayStyle.None;
                    tcs.SetResult(false);
                }
            });

            return await tcs.Task;
        }
    }
}

