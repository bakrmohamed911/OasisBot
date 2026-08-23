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
    }
}
