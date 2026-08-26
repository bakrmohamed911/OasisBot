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

        public override string ToString() => Name;
    }
}
