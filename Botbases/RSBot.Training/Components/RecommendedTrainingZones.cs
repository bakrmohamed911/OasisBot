using System.Collections.Generic;

namespace RSBot.Training.Components;

/// <summary>
///     A reference list of well-known Silkroad Online leveling zones by character level, shown
///     in the Training tab's "Recommended zone" dropdown purely as travel guidance - selecting
///     an entry does NOT set the Training Area's coordinates automatically. Compiled from public,
///     widely-documented game progression knowledge (zone names and level ranges), not from any
///     bot script or coordinate database - there's no reliable way to source exact, verified
///     region/X/Y/Z coordinates for these zones without either fabricating numbers (which would
///     silently drive bad automated movement if wrong) or extracting them from a live game
///     session. The safe, always-correct way to get real coordinates is still what the Area
///     section's own "Current" button already does: stand where you want to hunt, in-game, and
///     click it - see <see cref="RSBot.Training.Views.Main.btnGetCurrent_Click" />.
///     <para>
///         Coverage gap, called out rather than guessed at: level 100-131 isn't well documented
///         in public sources at the time this was compiled, and the level cap on this specific
///         server (141) goes one past the standard game's top documented bracket (Fire Temple,
///         136-140) - that's likely server-specific custom content this list can't cover.
///     </para>
/// </summary>
public static class RecommendedTrainingZones
{
    public static readonly IReadOnlyList<RecommendedZone> All = new List<RecommendedZone>
    {
        new(1, 20, "Jangan (China)", "Starting continent - China. Beginner mobs around Jangan's outskirts."),
        new(1, 20, "Constantinople (Europe)", "Starting continent - Europe, the alternate starting city."),
        new(
            20,
            40,
            "Donwhang (Western China)",
            "West of Jangan. Bandit Stronghold and Donwhang Cave are nearby."
        ),
        new(31, 40, "Samarkand (Central Asia)", "Europe-side continuation past Constantinople."),
        new(
            31,
            60,
            "Hotan (Oasis Kingdom)",
            "Western desert city - gateway to Taklamakan Desert, Karakoram, and Roc Mountain."
        ),
        new(
            35,
            110,
            "Forgotten World",
            "A separate instanced dungeon complex (Togui Village, Flame Mountain, Shipwreck, etc.) spanning a very wide level range - check the entry NPC for which sub-area fits your level."
        ),
        new(70, 90, "Roc Mountain", "Added with the level-90 content update."),
        new(70, 100, "Ch'in Tomb", "Requires level 70+ to enter; monsters scale up toward level 100."),
        new(
            95,
            100,
            "Alexandria",
            "New continent - requires level 95 to enter, level 100 for full access (Desert of Storms and Cloud, Valley of Kings, Tomb of the Pharaoh, Temple)."
        ),
        new(131, 135, "Ice Temple (Shambhala Shore)", "Enter via the Mortifying Monk NPC in Taklamakan."),
        new(136, 140, "Fire Temple (Shambhala Shore)", "Same entry point as Ice Temple, higher bracket."),
    };

    public record RecommendedZone(int LevelFrom, int LevelTo, string Name, string Hint)
    {
        public override string ToString() => $"{LevelFrom}-{LevelTo}  ·  {Name}";
    }
}
