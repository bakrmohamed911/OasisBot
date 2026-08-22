using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Objects;

namespace RSBot.Training.Bundle.Loot;

internal class LootBundle : IBundle
{
    /// <summary>
    ///     Gets the configuration.
    /// </summary>
    /// <value>
    ///     The configuration.
    /// </value>
    public LootConfig Config { get; private set; }

    /// <summary>
    ///     Invokes this instance.
    /// </summary>
    public void Invoke()
    {
        if (Bundles.Loot.Config.DontPickupWhileBotting)
            return;

        //If we use the ability pet, we can attack during the work of the Pickup manager
        if (Config.UseAbilityPet && Game.Player.HasActiveAbilityPet && !PickupManager.RunningAbilityPetPickup)
            PickupManager.RunAbilityPet(Container.Bot.Area.Position, Container.Bot.Area.Radius);

        if ((Bundles.Loot.Config.DontPickupInBerzerk && Game.Player.Berzerking) || ScriptManager.Running)
            return;

        // Used to unconditionally skip the whole loot pass whenever ANY monster happened to
        // be selected and alive - not just the one actually being fought. Botbase.Tick() calls
        // Loot before Target, so on every tick after the very first one, Game.SelectedEntity is
        // whatever Target.Invoke() picked as the *next* target the tick before; while training
        // in an area with more than one mob around (i.e. almost always), there's essentially
        // always another live mob selected, so this gate was blocking pickup permanently rather
        // than just during an actual attack - "pick up gold/loot right away after a kill" never
        // got a chance to run. PickupManager.RunPlayer (and SpawnedItem.Pickup itself) already
        // check Game.Player.InAction before moving/picking up per item, which is the actual
        // thing worth guarding against (walking off mid-swing) - so that's the only check
        // needed here now.
        if (Game.Player.InAction)
            return;

        PickupManager.RunPlayer(Game.Player.Position, Container.Bot.Area.Position, Container.Bot.Area.Radius);
    }

    /// <summary>
    ///     Refreshes this instance.
    /// </summary>
    public void Refresh()
    {
        Config = new LootConfig
        {
            UseAbilityPet = PlayerConfig.Get("RSBot.Items.Pickup.EnableAbilityPet", true),
            DontPickupWhileBotting = PlayerConfig.Get("RSBot.Items.Pickup.DontPickupWhileBotting", false),
            DontPickupInBerzerk = PlayerConfig.Get("RSBot.Items.Pickup.DontPickupInBerzerk", true),
        };
    }

    public void Stop()
    {
        PickupManager.Stop();
    }
}
