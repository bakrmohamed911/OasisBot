using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using RSBot.Core.Objects;

namespace RSBot.Training.Components;

/// <summary>
///     Converts legacy training-place waypoint scripts - go(x,y) / go,x,y / delay(N) / wait /
///     teleport / fly / buff / skill / kill / scriptversion= - into OasisBot's native
///     "move XOffset YOffset ZOffset XSector YSector" / "wait Milliseconds" script syntax, so a
///     file imported via "Import file..." doesn't need to already be in that format.
///     <para>
///         C# port of the one-off scratchpad/ConvertTrainingScripts.ps1 batch-conversion script
///         used to migrate an existing folder of legacy scripts; kept in sync with it.
///     </para>
/// </summary>
public static class LegacyScriptConverter
{
    private static readonly Regex GoParen = new(
        @"^go\((-?\d+(?:\.\d+)?),\s*(-?\d+(?:\.\d+)?)\)",
        RegexOptions.Compiled
    );

    private static readonly Regex GoComma = new(@"^go,(-?\d+(?:\.\d+)?),(-?\d+(?:\.\d+)?)", RegexOptions.Compiled);

    private static readonly Regex Delay = new(@"^delay\((\d+)\)", RegexOptions.Compiled);

    private static readonly Regex UntranslatableCommand = new(
        @"^(teleport|fly|buff|skill|kill|scriptversion)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    /// <summary>
    ///     The packed-hex "move(HEXBLOB)" format has no known byte layout - guessing wrong here
    ///     would silently send the character to the wrong spot with no error, unlike a plain
    ///     parse failure, so files using it are rejected rather than converted.
    /// </summary>
    public static bool IsUnsupportedHexFormat(IEnumerable<string> lines) =>
        lines.Any(line => line.TrimStart().StartsWith("move(", StringComparison.OrdinalIgnoreCase));

    /// <summary>
    ///     True if <paramref name="lines" /> contains legacy syntax (go/delay/wait/teleport/fly/
    ///     buff/skill/kill/scriptversion=) or has no usable native "move " line at all - either
    ///     way, converting it is worthwhile before handing it to <see cref="TrainingPlaceManager" />.
    /// </summary>
    public static bool NeedsConversion(IEnumerable<string> lines)
    {
        var hasNativeMove = false;

        foreach (var raw in lines)
        {
            var line = raw.Trim();

            if (line.StartsWith("move ", StringComparison.OrdinalIgnoreCase))
            {
                hasNativeMove = true;
                continue;
            }

            if (
                GoParen.IsMatch(line)
                || GoComma.IsMatch(line)
                || Delay.IsMatch(line)
                || line.Equals("wait", StringComparison.OrdinalIgnoreCase)
                || UntranslatableCommand.IsMatch(line)
            )
                return true; // legacy syntax present - needs conversion regardless of anything else
        }

        return !hasNativeMove; // no legacy syntax recognized, but also nothing natively usable
    }

    /// <summary>
    ///     Converts legacy script lines into native "move"/"wait" syntax. Commands with no safe
    ///     automatic translation (teleport/fly/buff/skill/kill/scriptversion=) are kept as
    ///     commented-out originals rather than guessed at - matching <see cref="TrainingPlaceManager" />'s
    ///     "skip what it doesn't recognize" tolerance when the route is later loaded.
    /// </summary>
    /// <param name="lines">The source file's lines.</param>
    /// <param name="sourceFileName">Original file name, recorded in a header comment.</param>
    /// <param name="unrecognizedCount">
    ///     Set to how many lines couldn't be translated (untranslatable commands plus fully
    ///     unrecognized lines) - callers can surface this as a "review recommended" notice.
    /// </param>
    public static string[] Convert(IEnumerable<string> lines, string sourceFileName, out int unrecognizedCount)
    {
        var outLines = new List<string> { $"# Converted from legacy training-place script: {sourceFileName}" };
        var unrecognized = 0;

        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0)
                continue;

            var m = GoParen.Match(line);
            if (m.Success)
            {
                outLines.Add(FormatMove(m));
                continue;
            }

            m = GoComma.Match(line);
            if (m.Success)
            {
                outLines.Add(FormatMove(m));
                continue;
            }

            m = Delay.Match(line);
            if (m.Success)
            {
                outLines.Add($"wait {m.Groups[1].Value}");
                continue;
            }

            if (line.Equals("wait", StringComparison.OrdinalIgnoreCase))
            {
                outLines.Add("wait 1000  # no explicit duration in original - defaulted to 1s");
                continue;
            }

            if (UntranslatableCommand.IsMatch(line))
            {
                outLines.Add($"# UNTRANSLATED (needs manual mapping): {line}");
                unrecognized++;
                continue;
            }

            if (line.StartsWith("//") || line.StartsWith("#"))
            {
                outLines.Add($"# {line}");
                continue;
            }

            outLines.Add($"# UNRECOGNIZED LINE: {line}");
            unrecognized++;
        }

        unrecognizedCount = unrecognized;
        return outLines.ToArray();
    }

    /// <summary>
    ///     Formats a "move" line from a matched go(x,y)/go,x,y pair, reusing <see cref="Position" />'s
    ///     own global-to-region conversion (its (float x, float y) constructor) rather than a
    ///     second, independent copy of that formula.
    /// </summary>
    private static string FormatMove(Match m)
    {
        var x = float.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
        var y = float.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
        var pos = new Position(x, y);

        return string.Format(
            CultureInfo.InvariantCulture,
            "move {0:0.##} {1:0.##} 0 {2} {3}",
            pos.XOffset,
            pos.YOffset,
            pos.Region.X,
            pos.Region.Y
        );
    }
}
