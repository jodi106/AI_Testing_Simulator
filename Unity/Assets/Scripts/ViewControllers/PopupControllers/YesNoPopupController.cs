using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.ViewControllers.PopupControllers
{
    public class YesNoPopupController : MonoBehaviour
    {

        private UIDocument document;

        private Label Title;
        private Label Description;
        private Button yesButton;
        private Button noButton;

        private bool result;

        public void Awake()
        {
            this.document = gameObject.GetComponent<UIDocument>();
            this.document.rootVisualElement.style.display = DisplayStyle.None;

            Title = this.document.rootVisualElement.Q<Label>("Title");
            Description = this.document.rootVisualElement.Q<Label>("Description");
            noButton = this.document.rootVisualElement.Q<Button>("CancelButton");
            yesButton = this.document.rootVisualElement.Q<Button>("ConfirmButton");
        }

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

