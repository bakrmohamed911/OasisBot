using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RSBot.Core;

namespace RSBot.Training.Components;

/// <summary>
///     One catalogued training place: a patrol-route script file plus the Name/Level metadata
///     entered when it was recorded or imported, so it can be found again by name or level.
/// </summary>
public class TrainingPlaceEntry
{
    public string Name { get; set; }
    public int Level { get; set; }
    public string FilePath { get; set; }

    /// <summary>
    ///     The training area captured from this script's own "area ..." command line, if it has
    ///     one (see <see cref="TrainingPlaceCatalog.TryParseAreaLine" />) - null for scripts that
    ///     don't (imported/converted ones, or older catalog entries saved before this was
    ///     tracked), in which case activating falls back to the route's first waypoint.
    /// </summary>
    public TrainingPlaceArea Area { get; set; }

    public override string ToString() => $"{Name} (Lv {Level})";
}

/// <summary>
///     The same Region/XOffset/YOffset/ZOffset/Radius values a
///     "area Region XOffset YOffset ZOffset Radius" script line applies (see
///     RSBot.Training.Components.TrainingAreaScriptCommand) - captured here so activating a
///     catalogued place can restore them directly instead of guessing from the route itself.
/// </summary>
public class TrainingPlaceArea
{
    public ushort Region { get; set; }
    public float XOffset { get; set; }
    public float YOffset { get; set; }
    public float ZOffset { get; set; }
    public int Radius { get; set; }
}

/// <summary>
///     Persists the list of catalogued training places (see <see cref="TrainingPlaceEntry" />) as
///     one JSON file under Data/Scripts/TrainingPlaces, independent of the character profile -
///     recorded/imported routes are reusable across characters.
/// </summary>
public static class TrainingPlaceCatalog
{
    private static List<TrainingPlaceEntry> _entries;

    public static IReadOnlyList<TrainingPlaceEntry> Entries => _entries ??= Load();

    private static string CatalogPath =>
        Path.Combine(Kernel.BasePath, "Data", "Scripts", "TrainingPlaces", "catalog.json");

    /// <summary>
    ///     Returns the catalogued entries matching <paramref name="query" /> by name (substring,
    ///     case-insensitive) or by an exact level match when the query parses as a number.
    ///     An empty/whitespace query returns every entry.
    /// </summary>
    public static IEnumerable<TrainingPlaceEntry> Search(string query)
    {
        var all = Entries;

        if (string.IsNullOrWhiteSpace(query))
            return all;

        query = query.Trim();
        var isLevelQuery = int.TryParse(query, out var level);

        return all.Where(entry =>
            entry.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || (isLevelQuery && entry.Level == level)
        );
    }

    /// <summary>
    ///     Adds (or replaces, matched by name) a catalog entry and persists immediately.
    /// </summary>
    public static void Add(TrainingPlaceEntry entry)
    {
        _entries ??= Load();
        _entries.RemoveAll(e => string.Equals(e.Name, entry.Name, StringComparison.OrdinalIgnoreCase));
        _entries.Add(entry);
        Save();
    }

    /// <summary>
    ///     Removes a catalog entry and persists immediately.
    /// </summary>
    public static void Remove(TrainingPlaceEntry entry)
    {
        _entries ??= Load();
        _entries.Remove(entry);
        Save();
    }

    /// <summary>
    ///     Scans script lines for a trailing "area Region XOffset YOffset ZOffset Radius"
    ///     command - the exact same format/argument order
    ///     RSBot.Training.Components.TrainingAreaScriptCommand parses when that line runs as
    ///     part of the script - and returns it, or null if the script has none. A recorded
    ///     script picks one up automatically whenever the Area section is set while recording;
    ///     if there's more than one (set more than once during the same recording), the LAST one
    ///     wins, matching what actually took effect by the time recording stopped.
    /// </summary>
    public static TrainingPlaceArea TryParseAreaLine(IEnumerable<string> lines)
    {
        TrainingPlaceArea result = null;

        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (!line.StartsWith("area ", StringComparison.OrdinalIgnoreCase))
                continue;

            var parts = line.Split(' ');
            if (
                parts.Length < 6
                || !ushort.TryParse(parts[1], out var region)
                || !float.TryParse(parts[2], out var xOffset)
                || !float.TryParse(parts[3], out var yOffset)
                || !float.TryParse(parts[4], out var zOffset)
                || !int.TryParse(parts[5], out var radius)
            )
                continue;

            result = new TrainingPlaceArea
            {
                Region = region,
                XOffset = xOffset,
                YOffset = yOffset,
                ZOffset = zOffset,
                Radius = radius,
            };
        }

        return result;
    }

    private static List<TrainingPlaceEntry> Load()
    {
        try
        {
            if (!File.Exists(CatalogPath))
                return new List<TrainingPlaceEntry>();

            var json = File.ReadAllText(CatalogPath);
            return JsonSerializer.Deserialize<List<TrainingPlaceEntry>>(json) ?? new List<TrainingPlaceEntry>();
        }
        catch (Exception ex)
        {
            Log.Error($"[TrainingPlaceCatalog] Failed to load catalog: {ex.Message}");
            return new List<TrainingPlaceEntry>();
        }
    }

    private static void Save()
    {
        try
        {
            var directory = Path.GetDirectoryName(CatalogPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(CatalogPath, json);
        }
        catch (Exception ex)
        {
            Log.Error($"[TrainingPlaceCatalog] Failed to save catalog: {ex.Message}");
        }
    }
}
