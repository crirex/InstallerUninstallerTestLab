using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace Common.Utils
{
    public enum DialogType : byte
    {
        Info = 0,
        Message,
        Result,
        Alert,
    }

    internal static class UserDialog
    {
        public static bool GetResponseDialog(in string message, DialogType type = default)
        {
            return MessageBox.Show(text: message, caption: type.ToString(), buttons: MessageBoxButtons.YesNo) == DialogResult.Yes;
        }
        public static string GetInputDialog(in string message, in string defaultResponse = default, DialogType type = default)
        {
            return Interaction.InputBox(Prompt: message, Title: type.ToString(), DefaultResponse: defaultResponse);
        }
        public static void MessageDialog(in string message, DialogType type = DialogType.Message)
        {
            MessageBox.Show(text: message, caption: type.ToString());
        }
    }
}