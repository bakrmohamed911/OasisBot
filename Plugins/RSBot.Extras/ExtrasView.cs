using System.Windows.Forms;
using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;

namespace RSBot.Extras
{
    public class ExtrasView : IPluginView
    {
        public string InternalName => "RSBot.Extras";
        public string DisplayName => "Extras";
        public bool DisplayAsTab => true;
        public int Index => 7;
        public bool RequireIngame => false;
        public Control View => Views.View.Instance;

        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
            LanguageManager.Translate(Views.View.SoundNotificationWindow, Kernel.Language);
        }
    }
}
