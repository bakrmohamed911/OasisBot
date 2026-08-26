using System.Collections.Generic;
using System.Linq;
using RSBot.Core;
using RSBot.Core.Objects;

namespace RSBot.Training.Components;

/// <summary>
///     Real, server-accurate named zones for the Training tab's Area dropdown - built directly
///     from <see cref="Game.ReferenceManager" />'s <c>TeleportData</c> (parsed straight out of
///     this exact server's own client files) rather than a hand-researched list. Every entry here
///     has a genuine region/X/Y/Z and radius, so there's no risk of sending the pathfinder toward
///     a fabricated or server-mismatched coordinate the way a web-researched guide would.
/// </summary>
public static class TrainingAreaCatalog
{
    /// <summary>
    ///     Every named zone with a usable position, deduplicated by name+region (several
    ///     teleport gates can lead into the same zone) and sorted by name.
    /// </summary>
    public static List<NamedZone> GetZones()
    {
        var seen = new HashSet<(string Name, ushort Region)>();
        var zones = new List<NamedZone>();

        foreach (var teleport in Game.ReferenceManager.TeleportData)
        {
            var name = teleport.ZoneName;
            if (string.IsNullOrWhiteSpace(name))
                continue;

            var key = (name, teleport.GenRegionID);
            if (!seen.Add(key))
                continue;

            zones.Add(
                new NamedZone
                {
                    Name = name,
                    Position = teleport.GetPosition(),
                    Radius = teleport.GenAreaRadius,
                }
            );
        }

        return zones.OrderBy(z => z.Name).ToList();
    }

    public class NamedZone
    {
        public string Name { get; set; }
        public Position Position { get; set; }
        public short Radius { get; set; }

        /// <summary>
        ///     Includes an observed level range when one's available (see
        ///     <see cref="MonsterObservationLog" /> - there's no static level data for a zone as a
        ///     whole, only for the individual monsters actually seen spawning in its region, so
        ///     this is derived the same live-learned way rather than a second, separate lookup).
        ///     Falls back to just the name for a region nothing's been observed in yet.
        /// </summary>
        public override string ToString()
        {
            var monsters = MonsterObservationLog.GetMonstersInRegion(Position.Region);
            if (monsters.Count == 0)
                return Name;

            var minLevel = monsters.Min(m => m.Level);
            var maxLevel = monsters.Max(m => m.Level);

            return minLevel == maxLevel ? $"{Name}  (Lv {minLevel})" : $"{Name}  (Lv {minLevel}-{maxLevel})";
        }
    }
}
