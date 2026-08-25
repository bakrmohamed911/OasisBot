using System;
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
    ///     Kernel.TickCount when the character first arrived at <see cref="_patrolWaitIndex" />
    ///     and started holding position there for TrainingPlaceManager.CurrentWaitMs, or -1 if
    ///     not currently holding. See <see cref="AdvancePatrol" /> for why this hold exists.
    /// </summary>
    private int _patrolWaitStartTick = -1;

    /// <summary>
    ///     Which waypoint index <see cref="_patrolWaitStartTick" />'s hold applies to. Needed
    ///     because TrainingPlaceManager.CurrentIndex can change out from under this bundle
    ///     between ticks (e.g. TrySkipToReachableWaypoint jumping ahead) - without this check, a
    ///     stale start tick from a previous waypoint would let a new waypoint's wait requirement
    ///     appear already satisfied.
    /// </summary>
    private int _patrolWaitIndex = -1;

    /// <summary>
    ///     Which waypoint index the "Lost the route" warning in <see cref="AdvancePatrol" /> was
    ///     last logged for, or -1 if it hasn't fired since arriving there. Without this, the
    ///     warning re-logs on every single tick for as long as the character stays stuck (many
    ///     times a second) - it only needs to say so once per stuck spot.
    /// </summary>
    private int _lastLostRouteWarnIndex = -1;

    /// <summary>
    ///     Invokes this instance.
    /// </summary>
    public void Invoke()
    {
        // Runs once per TrainingPlaceManager.Load() (a no-op every other tick) - re-syncs
        // CurrentIndex to whichever waypoint is actually closest to the player right now,
        // instead of leaving a freshly (re)loaded route sitting at waypoint 0 regardless of
        // where the character really is (see SyncToNearestWaypoint's own comment for why this
        // matters, and why it runs here on the tick thread rather than inside Load() itself).
        if (TrainingPlaceManager.IsActive)
            TrainingPlaceManager.SyncToNearestWaypoint(Game.Player.Position);

        // Keep the Training Area's own center following the player instead of a fixed
        // point, while a training-place patrol is still underway. Every other bundle (Target,
        // Attack, Loot) already filters by distance from Container.Bot.Area.Position/
        // Radius, so this alone makes all of that existing logic work along the whole
        // patrol route rather than just within one fixed circle - no changes needed there.
        // Done unconditionally, before any of the early returns below, so it stays current
        // even while mid-fight (the player barely moves then anyway, so this is cheap).
        //
        // Once the route is fully walked (HasArrived), stop re-centering on the player and
        // leave Area wherever it last was (right at the arrival spot) - Views.Main.
        // ActivateTrainingPlace() already configured Area.Radius from the script's own "area"
        // line (if it had one) back when the route was first selected, and that's never been
        // touched by this recentering (Radius isn't part of it), so it's still correct here.
        // Continuing to recenter after arrival would just make Position track the player's
        // combat wandering with no fixed circle left to wander within.
        if (TrainingPlaceManager.IsActive && !TrainingPlaceManager.HasArrived)
            Container.Bot.SetAreaPosition(Game.Player.Position);

        // While a training-place patrol's one-time initial trip to the actual grinding spot is
        // still underway (see TrainingPlaceManager.HasArrived, and Bot.Botbase.Tick which
        // suppresses Target/Attack for the same reason), don't let being attacked or having a
        // selected entity pause movement either - nothing is going to fight back to resolve
        // either condition during this phase, so pausing here left the character stuck in
        // place for as long as whatever attacked it kept attacking, with no way out.
        var travelingToDestination = TrainingPlaceManager.IsActive && !TrainingPlaceManager.HasArrived;

        if (Game.SelectedEntity != null && !LastEntityWasBehindObstacle && !travelingToDestination)
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
        if (playerUnderAttack && !LastEntityWasBehindObstacle && !travelingToDestination)
            return;

        if (Game.Player.Movement.Moving)
            return;

        // Once HasArrived, the patrol is done - fall through to the normal circular-area
        // movement below instead (now centered on FinalArea, or the player's arrival spot if
        // the route had no "area" line - see the top of this method). Continuing to call
        // AdvancePatrol() here would walk the whole recorded route in reverse trying to reach
        // waypoint 0 again, which - being a one-way trip through however many floor teleporters -
        // has no way to actually succeed.
        if (TrainingPlaceManager.IsActive && !TrainingPlaceManager.HasArrived)
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
    ///     Walks towards the current training-place waypoint, advancing whenever it's reached.
    ///     Only called while the route is still being walked (see the HasArrived check in
    ///     <see cref="Invoke" />) - once the last waypoint is reached, this stops running.
    ///     Uses MoveTo's non-blocking form (sleep: false) - it only issues the move and returns
    ///     immediately, so Target/Attack/Loot still get a turn every tick instead of this bundle
    ///     monopolizing the whole tick for however long the walk takes, the way ScriptManager's
    ///     own blocking "move" command execution would have.
    /// </summary>
    private void AdvancePatrol()
    {
        var waypoint = TrainingPlaceManager.CurrentWaypoint;
        var distance = Game.Player.Position.DistanceTo(waypoint);

        const int arrivalRadius = 5;
        if (distance <= arrivalRadius)
        {
            // A waypoint recorded with a "wait" (TrainingPlaceManager.CurrentWaitMs > 0) most
            // often marks a floor teleporter - the original script paused here for the teleport
            // animation/transition to actually complete before continuing. Advancing straight
            // through instead sent the very next MoveTo out using the character's still-pre-
            // teleport position, which either walked it nowhere (still mid-transition) or made
            // the next leg look like a huge, "unreachable" jump once the teleport did land -
            // exactly the case TrySkipToReachableWaypoint below exists to recover from. Holding
            // here first means that recovery is usually never needed: by the time the wait
            // elapses, the position update from the completed teleport means the next waypoint
            // is close by again, same as any other leg.
            var waitMs = TrainingPlaceManager.CurrentWaitMs;
            if (waitMs > 0)
            {
                if (_patrolWaitIndex != TrainingPlaceManager.CurrentIndex)
                {
                    _patrolWaitIndex = TrainingPlaceManager.CurrentIndex;
                    _patrolWaitStartTick = Kernel.TickCount;
                }

                if (Kernel.TickCount - _patrolWaitStartTick < waitMs)
                {
                    Log.Status($"Waiting at waypoint ({waitMs}ms - likely a teleporter)...");
                    return;
                }
            }

            _patrolWaitIndex = -1;
            TrainingPlaceManager.Advance();
            return;
        }

        // Player.MoveTo refuses anything past 150 units in one call (see Player.cs) - a floor
        // teleporter baked into the recorded route (e.g. a cave entrance) jumps the character's
        // real position far past whatever the very next waypoint expected, since teleporter
        // destinations aren't perfectly reproducible run to run. Rather than get stuck retrying
        // the same now-unreachable waypoint forever (MoveTo silently no-ops past its own range,
        // and this never checked its result to notice), look ahead for the first waypoint the
        // recording itself continued near right after the same jump. Stays as a fallback for
        // whenever the teleport lands further out than the wait above accounts for.
        const int moveToRange = 150;
        if (distance > moveToRange)
        {
            if (!TrainingPlaceManager.TrySkipToReachableWaypoint(Game.Player.Position, moveToRange))
            {
                // Only log once per stuck waypoint, not on every tick the character stays stuck
                // there (this would otherwise fire many times a second, flooding the log).
                if (_lastLostRouteWarnIndex != TrainingPlaceManager.CurrentIndex)
                {
                    _lastLostRouteWarnIndex = TrainingPlaceManager.CurrentIndex;

                    // Diagnostic dump - DistanceTo() computes X/Y from Region+Offset, and that
                    // formula itself branches on Region.IsDungeon (see Position.cs), taking a
                    // completely different basis (world-aligned sector math vs raw local offset)
                    // on either side of that flag. If current and target disagree on IsDungeon,
                    // the subtraction is comparing two incompatible coordinate systems and the
                    // resulting "distance" is meaningless regardless of true physical proximity -
                    // logging both positions' raw Region/Offset makes that distinguishable from a
                    // genuine teleport-never-fired case (where current would still equal the
                    // pre-wait position) without needing to reproduce this again to add logging.
                    var current = Game.Player.Position;
                    Log.Warn(
                        $"[Training] Lost the route after a large position jump (teleporter?) - no "
                            + $"nearby waypoint found within {moveToRange} units (computed distance: "
                            + $"{Math.Round(distance, 1)}). Current: region {current.Region.Id} "
                            + $"(dungeon={current.Region.IsDungeon}) offset {current.XOffset:0}/{current.YOffset:0} "
                            + $"-> Target: region {waypoint.Region.Id} (dungeon={waypoint.Region.IsDungeon}) "
                            + $"offset {waypoint.XOffset:0}/{waypoint.YOffset:0}. Move/teleport back onto the "
                            + "route manually to resume."
                    );
                }

                return;
            }

            _lastLostRouteWarnIndex = -1;
            waypoint = TrainingPlaceManager.CurrentWaypoint;
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
        _patrolWaitStartTick = -1;
        _patrolWaitIndex = -1;
        _lastLostRouteWarnIndex = -1;
    }
}
