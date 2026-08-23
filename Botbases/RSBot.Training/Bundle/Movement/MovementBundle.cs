using System.Threading;
using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Objects.Spawn;
using RSBot.Training.Components;

namespace RSBot.Training.Bundle.Movement;

internal class MovementBundle : IBundle
{
    /// <summary>
    ///     Gets or sets the configuration.
    /// </summary>
    /// <value>
    ///     The configuration.
    /// </value>
    public MovementConfig Config { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether [last entity was behind obstacle].
    ///     Used to move around even though the player is being attacked.
    /// </summary>
    /// <value>
    ///     <c>true</c> if [last entity was behind obstacle]; otherwise, <c>false</c>.
    /// </value>
    public bool LastEntityWasBehindObstacle { get; set; }

    /// <summary>
    ///     Kernel.TickCount when the current hold-for-loot streak below started, or -1 if not
    ///     currently holding. Mirrors the same-purpose field in Botbase.cs - see its comment
    ///     for why this can't be an unconditional wait on PickupManager.HasPendingLoot: this
    ///     bundle used to just return here for as long as HasPendingLoot said there was
    ///     anything pending at all, with no time bound of its own, so a single item it
    ///     couldn't actually get around to (or a genuinely-reachable one Loot just hadn't had a
    ///     free tick for yet) left the character standing still forever, never wandering off to
    ///     find the next mob once nothing nearby was actively attacking it.
    /// </summary>
    private int _holdForLootStartTick = -1;

    /// <summary>
    ///     Invokes this instance.
    /// </summary>
    public void Invoke()
    {
        // Keep the Training Area's own center following the player instead of a fixed
        // point, whenever a training-place patrol is active. Every other bundle (Target,
        // Attack, Loot) already filters by distance from Container.Bot.Area.Position/
        // Radius, so this alone makes all of that existing logic work along the whole
        // patrol route rather than just within one fixed circle - no changes needed there.
        // Done unconditionally, before any of the early returns below, so it stays current
        // even while mid-fight (the player barely moves then anyway, so this is cheap).
        if (TrainingPlaceManager.IsActive)
            Container.Bot.SetAreaPosition(Game.Player.Position);

        if (Game.SelectedEntity != null && !LastEntityWasBehindObstacle)
            return;

        // Don't wander off looking for the next fight while there's still loot from the last
        // kill sitting nearby - Botbase.Tick() holds Target/Attack back for the same reason
        // (see its own comment for why LootBundle can't always be trusted to get a clean turn
        // on its own), but that's pointless if this bundle just walks the character away from
        // the drop in the meantime anyway. Time-bounded the same way Botbase.Tick() bounds its
        // own hold, and for the same reason: this must never be able to leave the character
        // standing still indefinitely just because HasPendingLoot keeps reporting something.
        const int maxHoldForLootMs = 5000;

        var wantsToHoldForLoot = PickupManager.HasPendingLoot(
            Game.Player.Position,
            Container.Bot.Area.Position,
            Container.Bot.Area.Radius
        );

        if (!wantsToHoldForLoot)
            _holdForLootStartTick = -1;
        else if (_holdForLootStartTick < 0)
            _holdForLootStartTick = Kernel.TickCount;

        if (wantsToHoldForLoot && Kernel.TickCount - _holdForLootStartTick < maxHoldForLootMs)
            return;

        var playerUnderAttack = SpawnManager.Any<SpawnedMonster>(m =>
            m.AttackingPlayer && Container.Bot.Area.IsInSight(m)
        );
        if (playerUnderAttack && !LastEntityWasBehindObstacle)
            return;

        if (Game.Player.Movement.Moving)
            return;

        if (TrainingPlaceManager.IsActive)
        {
            AdvancePatrol();
            return;
        }

        if (
            PlayerConfig.Get("RSBot.Party.AlwaysFollowPartyMaster", false)
            && Game.Party.IsInParty
            && !Game.Party.IsLeader
        )
        {
            if (Game.Player.InAction)
                return;

            var player = Game.Party.Leader?.Player;
            if (player != null && player.Position.DistanceToPlayer() >= 10)
                Game.Player.MoveTo(player.Position);

            return;
        }

        var distance = Game.Player.Position.DistanceTo(Container.Bot.Area.Position);
        var hasCollision = Game.Player.Position.HasCollisionBetween(Container.Bot.Area.Position);

        //Go back if the player is out of the radius
        if ((distance > Container.Bot.Area.Radius || (Config.WalkToCenter && distance > 3)) && !hasCollision)
        {
            Log.Status("Walking to center");
            Game.Player.MoveTo(Container.Bot.Area.Position);

            return;
        }

        if (Config.WalkToCenter)
            return;

        Log.Status("Walking around");

        //Find a not colliding position. Do it in a while loop to prevent the bot from processing it in the next cycle (tick).
        //This is how we can find our next position very fast instead of waiting for the next circle to come.
        var destination = Container.Bot.Area.GetRandomPosition();

        var attempt = 0;
        while (Game.Player.Position.HasCollisionBetween(destination) && distance < Container.Bot.Area.Radius)
        {
            destination = Container.Bot.Area.GetRandomPosition();
            if (attempt++ > 3)
                break;

            Thread.Sleep(100);
        }

        Game.Player.MoveTo(destination, false);
    }

    /// <summary>
    ///     Walks towards the current training-place waypoint, advancing (and looping back to
    ///     the start once the route ends) whenever it's reached. Uses MoveTo's non-blocking
    ///     form (sleep: false) - it only issues the move and returns immediately, so Target/
    ///     Attack/Loot still get a turn every tick instead of this bundle monopolizing the
    ///     whole tick for however long the walk takes, the way ScriptManager's own blocking
    ///     "move" command execution would have.
    /// </summary>
    private void AdvancePatrol()
    {
        var waypoint = TrainingPlaceManager.CurrentWaypoint;
        var distance = Game.Player.Position.DistanceTo(waypoint);

        const int arrivalRadius = 5;
        if (distance <= arrivalRadius)
        {
            TrainingPlaceManager.Advance();
            return;
        }

        Log.Status($"Walking training route [{TrainingPlaceManager.SelectedMobName}]");
        Game.Player.MoveTo(waypoint, false);
    }

    /// <summary>
    ///     Refreshes this instance.
    /// </summary>
    public void Refresh()
    {
        Config = new MovementConfig
        {
            WalkAround = PlayerConfig.Get("RSBot.Training.radioWalkAround", true),
            WalkToCenter = PlayerConfig.Get<bool>("RSBot.Training.radioCenter"),
        };
    }

    public void Stop()
    {
        LastEntityWasBehindObstacle = false;
        _holdForLootStartTick = -1;
    }
}
