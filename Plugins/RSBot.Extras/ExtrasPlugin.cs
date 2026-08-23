using RSBot.Core.Plugins;

namespace RSBot.Extras
{
    public class ExtrasPlugin : IPlugin
    {
        public string InternalName => "RSBot.Extras";
        public static ExtrasPlugin Instance { get; private set; }
        public ExtrasManager Manager { get; private set; }

        public void Initialize()
        {
            Instance = this;
            Manager = new ExtrasManager();
        }

        public void OnLoadCharacter() { }
    }
}
