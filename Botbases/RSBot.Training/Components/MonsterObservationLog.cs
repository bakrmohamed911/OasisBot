using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RSBot.Core;
using RSBot.Core.Event;
using RSBot.Core.Objects;
using RSBot.Core.Objects.Spawn;

namespace RSBot.Training.Components;

/// <summary>
///     A live-learned "what monster spawns where" database, built purely from what this
///     installation has actually seen in-game, rather than a hand-compiled/researched list.
///     <para>
///         There's no static reference file (client-side or otherwise) that maps monsters to the
///         regions they spawn in - spawn placement is server-controlled, and every private server
///         can differ - so any pre-built list would either be incomplete or simply wrong for a
///         given server. This instead subscribes to "OnSpawnMonster" (fired for every monster this
///         character has ever seen spawn) and records region/monster pairs as they're actually
///         observed, which is slower to fill in than a pre-made list but is guaranteed accurate
///         for whichever server this is actually running against.
///     </para>
///     Persisted to Data/Scripts/MonsterObservations.json - shared across every account/character/
///     profile on this install (like navigation_linkage.json), since "what spawns in this region"
///     is a fact about the server, not about any one character.
/// </summary>
public static class MonsterObservationLog
{
    private static readonly string FilePath = Path.Combine(
        Kernel.BasePath,
        "Data",
        "Scripts",
        "MonsterObservations.json"
    );

    private static readonly object _lock = new();

    // Region -> CodeName -> observation. Dictionary, not a list, so re-observing the same
    // monster in the same region (which will happen constantly during normal play) is a cheap
    // no-op lookup instead of an ever-growing duplicate list.
    private static Dictionary<ushort, Dictionary<string, ObservedMonster>> _byRegion;
    private static bool _dirty;
    private static bool _subscribed;

    /// <summary>
    ///     Starts recording observations. Safe to call more than once - only subscribes once.
    /// </summary>
    public static void Initialize()
    {
        if (_subscribed)
            return;

        _subscribed = true;
        Load();

        EventManager.SubscribeEvent("OnSpawnMonster", new Action<SpawnedMonster>(OnSpawnMonster));
    }

    private static void OnSpawnMonster(SpawnedMonster monster)
    {
        try
        {
            var record = monster.Record;
            if (record == null || string.IsNullOrEmpty(record.CodeName))
                return;

            var region = monster.Position.Region.Id;

            lock (_lock)
            {
                if (!_byRegion.TryGetValue(region, out var monsters))
                {
                    monsters = new Dictionary<string, ObservedMonster>();
                    _byRegion[region] = monsters;
                }

                // Already known for this region - nothing new to persist. This is the common
                // case on every tick once an area's monster set has actually been discovered,
                // so bailing out here is what keeps this from writing to disk constantly.
                if (monsters.ContainsKey(record.CodeName))
                    return;

                monsters[record.CodeName] = new ObservedMonster
                {
                    CodeName = record.CodeName,
                    Name = record.GetRealName(),
                    Level = record.Level,
                    FirstSeen = DateTime.Now.ToString("yyyy-MM-dd"),
                };

                _dirty = true;
            }

            // Notify (not Debug) so this is visible by default - the whole point is to let the
            // user confirm live, in the log, that discovery is actually happening while they
            // train, rather than silently trusting a file on disk.
            Log.Notify($"[TrainingZones] Discovered Lv.{record.Level} {record.GetRealName()} in region {region}");

            Save();
        }
        catch (Exception e)
        {
            // Never let a logging hiccup here interfere with the actual tick loop this fires
            // from (EventManager.FireEvent already isolates this per-subscriber, but there's no
            // reason to risk it).
            Log.Debug($"[MonsterObservationLog] Failed to record spawn: {e.Message}");
        }
    }

    /// <summary>
    ///     Every monster observed so far "in" the given region - not an exact-region-only match,
    ///     since a single named zone (what a Training Area's teleport-gate entry actually points
    ///     at) almost never corresponds to just one region tile. Confirmed live: a dungeon's
    ///     observed monsters landed across two different, numerically adjacent region IDs (two
    ///     floors of the same instance), while its teleport gate's own region had zero exact
    ///     matches - an exact match here would have looked "not fetched at all" despite the
    ///     underlying data genuinely being there. Overworld regions are geographic (region ID
    ///     encodes an X/Y map tile - see <see cref="Region" />'s own layout), so nearby tiles are
    ///     included by real distance; dungeon regions aren't laid out geographically at all (IDs
    ///     are closer to arbitrary/sequential per floor), so nearby *numeric* IDs are used there
    ///     instead as the closest available signal - weaker, but the only one there is.
    /// </summary>
    public static IReadOnlyList<ObservedMonster> GetMonstersInRegion(Region region, int tileRadius = 3)
    {
        lock (_lock)
        {
            if (_byRegion == null)
                return Array.Empty<ObservedMonster>();

            var result = new Dictionary<string, ObservedMonster>();

            foreach (var kvp in _byRegion)
            {
                if (!IsNearby(region, new Region(kvp.Key), tileRadius))
                    continue;

                foreach (var monster in kvp.Value.Values)
                    result.TryAdd(monster.CodeName, monster);
            }

            return result.Values.OrderBy(m => m.Level).ToList();
        }
    }

    private static bool IsNearby(Region a, Region b, int tileRadius)
    {
        if (a.Id == b.Id)
            return true;

        if (a.IsDungeon != b.IsDungeon)
            return false;

        if (a.IsDungeon)
            return Math.Abs(a.Id - b.Id) <= tileRadius;

        return Math.Abs(a.X - b.X) <= tileRadius && Math.Abs(a.Y - b.Y) <= tileRadius;
    }

    private static void Load()
    {
        lock (_lock)
        {
            _byRegion = new Dictionary<ushort, Dictionary<string, ObservedMonster>>();

            if (!File.Exists(FilePath))
                return;

            try
            {
                var json = File.ReadAllText(FilePath);
                var flat = JsonSerializer.Deserialize<List<StoredObservation>>(json);
                if (flat == null)
                    return;

                foreach (var entry in flat)
                {
                    if (!_byRegion.TryGetValue(entry.Region, out var monsters))
                    {
                        monsters = new Dictionary<string, ObservedMonster>();
                        _byRegion[entry.Region] = monsters;
                    }

                    monsters[entry.CodeName] = new ObservedMonster
                    {
                        CodeName = entry.CodeName,
                        Name = entry.Name,
                        Level = entry.Level,
                        FirstSeen = entry.FirstSeen,
                    };
                }
            }
            catch (Exception e)
            {
                Log.Warn($"[MonsterObservationLog] Could not load {FilePath}: {e.Message}");
            }
        }
    }

    private static void Save()
    {
        List<StoredObservation> flat;

        lock (_lock)
        {
            if (!_dirty)
                return;

            flat = _byRegion
                .SelectMany(kvp =>
                    kvp.Value.Values.Select(m => new StoredObservation
                    {
                        Region = kvp.Key,
                        CodeName = m.CodeName,
                        Name = m.Name,
                        Level = m.Level,
                        FirstSeen = m.FirstSeen,
                    })
                )
                .ToList();

            _dirty = false;
        }

        try
        {
            var directory = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(flat, new JsonSerializerOptions { WriteIndented = true });

            // Same safer-replace pattern NavigationManager uses for its own shared Data/ file.
            var tempPath = FilePath + ".tmp";
            File.WriteAllText(tempPath, json);
            File.Move(tempPath, FilePath, true);
        }
        catch (Exception e)
        {
            Log.Warn($"[MonsterObservationLog] Could not save {FilePath}: {e.Message}");
        }
    }

    public class ObservedMonster
    {
        public string CodeName { get; set; }
        public string Name { get; set; }
        public byte Level { get; set; }
        public string FirstSeen { get; set; }

        public override string ToString() => $"Lv.{Level}  {Name}";
    }

    private class StoredObservation
    {
        public ushort Region { get; set; }
        public string CodeName { get; set; }
        public string Name { get; set; }
        public byte Level { get; set; }
        public string FirstSeen { get; set; }
    }
}
