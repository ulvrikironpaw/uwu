using System;

namespace UWU.Common
{
    internal class PromptUtils
    {
        internal static bool IsDialogOpen()
        {
            return TextInput.IsVisible();
        }

        internal static void LaunchDialog(string title, string initialValue, Action<string> onComplete)
        {
            var textReceiver = new TextDialogMediator();
            textReceiver.SetText(initialValue);
            textReceiver.OnComplete = onComplete;

            // Use Valheim's built-in TextInput UI (the one used for signs)
            TextInput.instance.RequestText(
                textReceiver,
                title,
                24
            );
        }
    }

    class TextDialogMediator : TextReceiver
    {
        public Action<string> OnComplete { get; set; }

        private string text = "";

        public string GetText()
        {
            return text;
        }

        public void SetText(string text)
        {
            this.text = text;
            OnComplete?.Invoke(text);
        }
    }
}
