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

    public override string ToString() => $"{Name} (Lv {Level})";
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
