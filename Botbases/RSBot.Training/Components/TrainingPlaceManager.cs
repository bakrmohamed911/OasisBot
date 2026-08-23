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
    ///     Loads a converted training-place script - one "move XOffset YOffset ZOffset
    ///     XSector YSector" line per waypoint. Lines it doesn't recognize (the commented-out
    ///     untranslated teleport/fly/buff/skill/kill originals, blank lines, headers) are
    ///     skipped rather than treated as errors.
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

        foreach (var rawLine in File.ReadAllLines(filePath))
        {
            var line = rawLine.Trim();
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
        }

        if (waypoints.Count == 0)
        {
            Log.Warn($"[TrainingPlace] No usable waypoints found in {filePath}");
            return false;
        }

        _waypoints = waypoints;
        CurrentIndex = 0;
        SelectedMobName = mobName;
        IsActive = true;

        Log.Notify($"[TrainingPlace] Loaded {waypoints.Count} waypoint(s) for [{mobName}] - patrolling this route.");

        return true;
    }

    /// <summary>
    ///     Advances to the next waypoint, looping back to the first once the route's end is
    ///     reached.
    /// </summary>
    public static void Advance()
    {
        if (_waypoints.Count == 0)
            return;

        CurrentIndex = (CurrentIndex + 1) % _waypoints.Count;
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
        CurrentIndex = 0;
    }
}
