using System;
using System.Threading;
using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Event;
using RSBot.Core.Objects;
using RSBot.Training.Bundle;

namespace RSBot.Training.Bot;

internal class Botbase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Botbase" /> class.
    /// </summary>
    public Botbase()
    {
        EventManager.SubscribeEvent("OnSetTrainingArea", Reload);
    }

    /// <summary>
    ///     Gets the area.
    /// </summary>
    /// <value>
    ///     The area.
    /// </value>
    public Area Area { get; private set; }

    /// <summary>
    ///     Kernel.TickCount when the current hold-for-loot streak (see Tick()) started, or -1
    ///     if not currently holding. Backs a hard ceiling on how long Target/Attack can be held
    ///     back waiting for loot - see the MAX_HOLD_FOR_LOOT_MS comment in Tick() for why this
    ///     safety net exists on top of PickupManager already filtering out unreachable items.
    /// </summary>
    private int _holdForLootStartTick = -1;

    /// <summary>
    ///     Updates just the Area's center position, leaving Radius/etc. untouched. Area is a
    ///     struct, so `Area.Position = x` from outside this class can't compile (it would
    ///     only mutate a copy) - this does the read-mutate-write on the actual backing field
    ///     for callers (e.g. a training-place patrol keeping the center following the player)
    ///     that need to move the center without going through Reload().
    /// </summary>
    /// <param name="position">The new center position.</param>
    public void SetAreaPosition(Position position)
    {
        var area = Area;
        area.Position = position;
        Area = area;
    }

    /// <summary>
    ///     Reloads this instance by re-reading the configuration.
    /// </summary>
    public void Reload()
    {
        Area = new Area
        {
            Position = new Position(
                PlayerConfig.Get<ushort>("RSBot.Area.Region"),
                PlayerConfig.Get<float>("RSBot.Area.X"),
                PlayerConfig.Get<float>("RSBot.Area.Y"),
                PlayerConfig.Get<float>("RSBot.Area.Z")
            ),
            Radius = Math.Clamp(PlayerConfig.Get("RSBot.Area.Radius", 50), 5, 100),
        };
    }

    /// <summary>
    ///     Ticks this instance.
    /// </summary>
    public void Tick()
    {
        if (!Kernel.Bot.Running)
            return;

        if (Game.Player.HasActiveVehicle)
        {
            Game.Player.Vehicle.Dismount();
            Thread.Sleep(1000);
        }

        //Wait for the pickup manager to finish
        if (PickupManager.RunningPlayerPickup)
            return;

        if (
            Bundles.Loop.Config.UseSpeedDrug
            && Game.Player.State.ActiveBuffs.FindIndex(p => p.Record.Params.Contains(1752396901)) < 0
        )
        {
            var item = Game.Player.Inventory.GetItem(
                new TypeIdFilter(3, 3, 13, 1),
                p => p.Record.Desc1.Contains("_SPEED_")
            );
            item?.Use();
        }

        var noAttack = PlayerConfig.Get("RSBot.Skills.checkBoxNoAttack", false);

        //Check for protection
        Bundles.Protection.Invoke();

        //Resurrect party members if needed
        Bundles.Resurrect.Invoke();

        //Cast buffs
        Bundles.Buff.Invoke();

        // Buff the configured party members if needed
        Bundles.PartyBuff.Invoke();

        //Loot items
        Bundles.Loot.Invoke();

        // Between fights (no target currently selected), hold off engaging a new one for as
        // long as there's still unclaimed loot from the last kill nearby. Loot.Invoke() above
        // already runs before Target/Attack every tick and would get first crack at it on its
        // own - but during a long/chained combat rotation (a tanky champion in particular,
        // whose fight runs long enough to rack up several overlapping attack/buff/potion
        // "entered action" states with no clean gap between them) Game.Player.InAction can
        // stay continuously true right up until the next fight is already underway, so
        // LootBundle's own InAction gate never gets a turn before Target grabs a fresh target.
        // The previous kill's drop - often the more valuable one, for a champion - then gets
        // left behind and risks despawning while the bot keeps grinding through other mobs
        // instead of ever coming back for it. This check is deliberately cheap (no walking/
        // pickup work, just an existence scan) so paying for it every tick is negligible.
        // Hard ceiling on top of the above: PickupManager.Condition() now filters out
        // unreachable (behind-obstacle) items, but that's the second line of defense, not the
        // only one - this used to be able to deadlock combat entirely on a single stuck item
        // (e.g. one left over in the world from before a bot restart) with no way out, since
        // HasPendingLoot would report it "pending" forever and Target/Attack would never run
        // again. Give up holding after a few seconds regardless of what HasPendingLoot says,
        // so a bug or edge case here costs a few seconds of delayed looting, not a permanently
        // defenseless character.
        const int maxHoldForLootMs = 5000;

        var wantsToHoldForLoot =
            Game.SelectedEntity == null
            && PickupManager.HasPendingLoot(Game.Player.Position, Area.Position, Area.Radius);

        if (!wantsToHoldForLoot)
            _holdForLootStartTick = -1;
        else if (_holdForLootStartTick < 0)
            _holdForLootStartTick = Kernel.TickCount;

        var holdForLoot = wantsToHoldForLoot && Kernel.TickCount - _holdForLootStartTick < maxHoldForLootMs;

        //Select next target
        if (!noAttack && !holdForLoot)
            Bundles.Target.Invoke();

        //Check for berzerk
        Bundles.Berzerk.Invoke();

        //Cast skill against enemy
        if (!noAttack && !holdForLoot)
            Bundles.Attack.Invoke();

        //Move around (maybe)
        Bundles.Movement.Invoke();
    }
}
