using System;
using System.Collections.Generic;
using System.IO;
using RSBot.Core;
using RSBot.Core.Objects;

namespace RSBot.Training.Components;

/// <summary>
///     Drives an optional "training place" patrol: a fixed waypoint loop (converted from
///     legacy training-place scripts) that the character continuously walks - fighting
///     anything it encounters along the way via <see cref="MovementBundle" /> keeping
///     <see cref="Container" />.Bot.Area.Position synced to the player's current position -
///     instead of wandering randomly within one fixed circular Training Area.
///     <para>
///         Deliberately does NOT go through <see cref="ScriptManager" />: that's a single
///         shared/global script slot also used for town and walkback scripts elsewhere in the
///         app, and loading a patrol route through it would risk clobbering (or being
///         clobbered by) whichever of those runs next. This keeps its own independent
///         waypoint list instead.
///     </para>
/// </summary>
public static class TrainingPlaceManager
{
    private static List<Position> _waypoints = new();

    /// <summary>
    ///     Parallel to <see cref="_waypoints" /> - milliseconds to hold position at waypoint[i]
    ///     (after arriving, before advancing past it) once reached, or 0 for none. Populated from
    ///     "wait Milliseconds" lines, each attached to whichever waypoint immediately precedes it
    ///     in the script - see <see cref="Load" />.
    /// </summary>
    private static List<int> _waitMsAfterWaypoint = new();

    /// <summary>
    ///     True right after <see cref="Load" /> until <see cref="SyncToNearestWaypoint" /> has run
    ///     once - see that method for why the resume-position check happens there instead of
    ///     inside <see cref="Load" /> itself.
    /// </summary>
    private static bool _needsResumeSync;

    /// <summary>
    ///     Gets a value indicating whether a patrol route is currently loaded and active.
    /// </summary>
    public static bool IsActive { get; private set; }

    /// <summary>
    ///     Gets the display name of the currently loaded mob/route, or null if none.
    /// </summary>
    public static string SelectedMobName { get; private set; }

    /// <summary>
    ///     Gets the index of the waypoint currently being walked towards.
    /// </summary>
    public static int CurrentIndex { get; private set; }

    /// <summary>
    ///     Gets the waypoint currently being walked towards.
    /// </summary>
    public static Position CurrentWaypoint => _waypoints.Count == 0 ? default : _waypoints[CurrentIndex];

    /// <summary>
    ///     Gets the milliseconds the character should hold position at the current waypoint
    ///     (once reached) before advancing, or 0 if none was recorded. A non-zero value most
    ///     often marks a floor teleporter baked into the recorded route: the original script
    ///     waited here for the teleport animation/transition to actually complete before the
    ///     next leg, and skipping that wait sends the character towards the next waypoint using
    ///     its still-pre-teleport position, which is exactly what leaves it stuck - see
    ///     <see cref="RSBot.Training.Bundle.Movement.MovementBundle" />.
    /// </summary>
    public static int CurrentWaitMs => _waitMsAfterWaypoint.Count == 0 ? 0 : _waitMsAfterWaypoint[CurrentIndex];

    /// <summary>
    ///     Gets a value indicating whether the character has physically reached the route's last
    ///     waypoint at least once since it was loaded. Sticky - once true, stays true even after
    ///     looping back to the start, distinguishing the one-time initial travel leg (town to
    ///     the actual grinding spot, where <see cref="Bot.Botbase.Tick" /> suppresses Target/
    ///     Attack so the trip there isn't interrupted by every monster along the way) from the
    ///     ongoing patrol once arrived (where fighting is exactly the point).
    /// </summary>
    public static bool HasArrived { get; private set; }

    /// <summary>
    ///     Loads a converted training-place script - one "move XOffset YOffset ZOffset
    ///     XSector YSector" line per waypoint, plus any "wait Milliseconds" lines (each attached
    ///     to the waypoint immediately preceding it - see <see cref="CurrentWaitMs" />). Other
    ///     lines it doesn't recognize (the commented-out untranslated teleport/fly/buff/skill/
    ///     kill originals, "cast"/"area"/"store"/"repair"/"buy" lines the recorder or
    ///     LegacyScriptConverter may have added, blank lines, headers) are skipped rather than
    ///     treated as errors.
    ///     <para>
    ///         Starts at waypoint 0 immediately, then re-syncs to whichever waypoint is actually
    ///         closest once <see cref="SyncToNearestWaypoint" /> gets its first tick - see that
    ///         method for why the character's position isn't checked here instead.
    ///     </para>
    /// </summary>
    /// <param name="filePath">Path to the converted script file.</param>
    /// <param name="mobName">Display name to report while this route is active.</param>
    /// <returns><c>true</c> if at least one waypoint was loaded and the route is now active.</returns>
    public static bool Load(string filePath, string mobName)
    {
        if (!File.Exists(filePath))
        {
            Log.Warn($"[TrainingPlace] Script not found: {filePath}");
            return false;
        }

        var waypoints = new List<Position>();
        var waitMsAfterWaypoint = new List<int>();

        foreach (var rawLine in File.ReadAllLines(filePath))
        {
            var line = rawLine.Trim();

            if (line.StartsWith("wait ", StringComparison.OrdinalIgnoreCase))
            {
                // Attaches to whichever waypoint was added most recently - a "wait" line with no
                // preceding waypoint at all (script starts with one, or malformed) has nowhere to
                // attach to, so it's dropped rather than guessed at.
                if (waypoints.Count == 0)
                    continue;

                var waitParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (waitParts.Length >= 2 && int.TryParse(waitParts[1], out var waitMs))
                    waitMsAfterWaypoint[^1] += waitMs;

                continue;
            }

            if (!line.StartsWith("move "))
                continue;

            var parts = line.Split(' ');
            if (
                parts.Length < 6
                || !float.TryParse(parts[1], out var xOffset)
                || !float.TryParse(parts[2], out var yOffset)
                || !float.TryParse(parts[3], out var zOffset)
                || !byte.TryParse(parts[4], out var xSector)
                || !byte.TryParse(parts[5], out var ySector)
            )
                continue;

            waypoints.Add(new Position(xSector, ySector, xOffset, yOffset, zOffset));
            waitMsAfterWaypoint.Add(0);
        }

        if (waypoints.Count == 0)
        {
            Log.Warn($"[TrainingPlace] No usable waypoints found in {filePath}");
            return false;
        }

        _waypoints = waypoints;
        _waitMsAfterWaypoint = waitMsAfterWaypoint;
        CurrentIndex = 0;
        SelectedMobName = mobName;
        HasArrived = false;
        IsActive = true;
        _needsResumeSync = true;

        Log.Notify($"[TrainingPlace] Loaded {waypoints.Count} waypoint(s) for [{mobName}] - patrolling this route.");

        return true;
    }

    /// <summary>
    ///     Runs once per <see cref="Load" />, on the first tick afterwards - re-syncs
    ///     <see cref="CurrentIndex" /> to whichever recorded waypoint in <paramref name="current" />
    ///     's region is closest to it, rather than leaving the route sitting at waypoint 0.
    ///     <para>
    ///         Load() is called again on every reconnect (<c>Views.Main.OnLoadCharacter</c>
    ///         re-selects the last-active place via <c>SelectSavedTrainingPlace</c>), and if the
    ///         character is already deep into the route (e.g. mid-dungeon after a floor
    ///         teleporter) when a disconnect/relogin happens, blindly restarting at 0 sent it on
    ///         an impossible cross-region trek back to the route's start - which immediately fails
    ///         the same way a floor teleporter landing off-route does, this time permanently,
    ///         since there's nothing later to recover into.
    ///     </para>
    ///     <para>
    ///         Deliberately NOT done inside Load() itself, even though it would be simpler - Load()
    ///         runs on the UI thread (selecting a route, or the above reconnect path) where the
    ///         game connection may not be fully ready yet, while this runs from
    ///         <see cref="RSBot.Training.Bundle.Movement.MovementBundle" />'s regular per-tick
    ///         Invoke() on the bot's own tick thread, which already reads player position
    ///         constantly with no such risk.
    ///     </para>
    /// </summary>
    /// <param name="current">The character's current position.</param>
    public static void SyncToNearestWaypoint(Position current)
    {
        if (!_needsResumeSync)
            return;

        _needsResumeSync = false;

        var bestIndex = -1;
        var bestDistance = double.MaxValue;

        // Restricted to same-region matches because DistanceTo can't be trusted to compare
        // positions across a dungeon/overworld region boundary (its X/Y formula bases dungeon
        // coordinates differently - see MovementBundle.AdvancePatrol's "Lost the route"
        // diagnostic for the details), so any cross-region "closest" result would be meaningless
        // rather than actually close.
        for (var index = 0; index < _waypoints.Count; index++)
        {
            if (_waypoints[index].Region.Id != current.Region.Id)
                continue;

            var distance = current.DistanceTo(_waypoints[index]);
            if (distance >= bestDistance)
                continue;

            bestDistance = distance;
            bestIndex = index;
        }

        if (bestIndex < 0)
            return; // no waypoint in the current region - a genuinely fresh start, waypoint 0 stands.

        CurrentIndex = bestIndex;
        HasArrived = bestIndex == _waypoints.Count - 1;

        if (bestIndex > 0)
            Log.Notify($"[TrainingPlace] Resuming route [{SelectedMobName}] near waypoint {bestIndex}.");
    }

    /// <summary>
    ///     Advances to the next waypoint. The patrol is a one-way trip, not a loop - once the
    ///     last waypoint is reached, this holds <see cref="CurrentIndex" /> there instead of
    ///     wrapping back to the first waypoint. Wrapping used to send the character on a return
    ///     trip through the entire recorded route (regions, floor teleporters and all) in
    ///     reverse, which it has no way to actually do (there's no "undo" for a floor teleporter)
    ///     - it would just immediately fail the same large-jump/no-nearby-waypoint check that
    ///     floor teleporters trip mid-route, permanently this time, since there's no further
    ///     wait/skip-ahead recovery once it's the very last waypoint. See
    ///     <see cref="RSBot.Training.Bundle.Movement.MovementBundle" /> for what actually happens
    ///     once <see cref="HasArrived" /> is set.
    /// </summary>
    public static void Advance()
    {
        if (_waypoints.Count == 0)
            return;

        if (CurrentIndex == _waypoints.Count - 1)
        {
            HasArrived = true;
            return;
        }

        CurrentIndex++;
    }

    /// <summary>
    ///     If the current waypoint is out of Player.MoveTo's own single-call range (it refuses
    ///     anything past 150 units - see Player.cs), scans forward through the next few
    ///     waypoints for the first one within reach and jumps directly to it. Handles a floor
    ///     teleporter baked into the recorded route (e.g. a cave entrance) landing the character
    ///     far past whatever waypoint immediately followed the jump during recording - the
    ///     waypoints shortly after it should still be close to wherever the same teleporter
    ///     actually lands, since that's literally where recording continued from.
    /// </summary>
    /// <param name="from">The character's current position.</param>
    /// <param name="maxDistance">The maximum single-hop distance (matches Player.MoveTo's own cap).</param>
    /// <param name="maxScanAhead">How many waypoints ahead to look before giving up.</param>
    /// <returns><c>true</c> if a reachable waypoint was found and <see cref="CurrentIndex" /> moved to it.</returns>
    public static bool TrySkipToReachableWaypoint(Position from, float maxDistance, int maxScanAhead = 20)
    {
        if (_waypoints.Count == 0)
            return false;

        var lastScannable = Math.Min(CurrentIndex + maxScanAhead, _waypoints.Count - 1);
        for (var index = CurrentIndex + 1; index <= lastScannable; index++)
        {
            if (from.DistanceTo(_waypoints[index]) > maxDistance)
                continue;

            if (index == _waypoints.Count - 1)
                HasArrived = true;

            CurrentIndex = index;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Deactivates patrol mode, reverting movement to the normal fixed-radius Training
    ///     Area behavior.
    /// </summary>
    public static void Deactivate()
    {
        IsActive = false;
        SelectedMobName = null;
        _waypoints = new List<Position>();
        _waitMsAfterWaypoint = new List<int>();
        CurrentIndex = 0;
        HasArrived = false;
        _needsResumeSync = false;
    }
}
