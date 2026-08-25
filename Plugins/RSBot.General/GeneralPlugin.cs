using System.Linq;
using RSBot.Core;
using RSBot.Core.Components.Command;
using RSBot.Core.Plugins;
using RSBot.General.Components;

namespace RSBot.General
{
    public class GeneralPlugin : IPlugin
    {
        public string InternalName => "RSBot.General";
        public static GeneralPlugin Instance { get; private set; }
        public GeneralManager Manager { get; private set; }

        public void Initialize()
        {
            Instance = this;
            Manager = new GeneralManager();
            Accounts.Load();

            // Account/Accounts live here, in RSBot.General - not in RSBot.Core, which this plugin
            // depends on (not the other way around) - so PlayerConfig can't look this up itself.
            // It calls this resolver instead to find which real login account a character belongs
            // to, for its per-account config subfolder. Left unset (falls back to no account
            // subfolder) if this plugin somehow isn't loaded.
            PlayerConfig.AccountResolver = charName =>
                Accounts.SavedAccounts?.FirstOrDefault(a => a.Characters != null && a.Characters.Contains(charName))
                    ?.Username;

            CLIManager.Register(new StartClientCommand());
            CLIManager.Register(new ShowClientCommand());
            CLIManager.Register(new HideClientCommand());
        }

        public void OnLoadCharacter() { }
    }
}
