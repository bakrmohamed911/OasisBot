using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RSBot.Core.Components;
using RSBot.Core.Event;

namespace RSBot.Core;

public static class PlayerConfig
{
    /// <summary>
    ///     Prefixes that stay scoped to the individual character/account rather than becoming
    ///     shared - skills/buffs (what to cast) and the shopping list (what to buy when a
    ///     training-place run starts). Every other key is shared globally across every profile,
    ///     account, and character, since in practice those settings (training area, item
    ///     filters, party/protection thresholds, avoidance lists, etc.) are meant to be
    ///     identical everywhere and re-entering them per character was pure duplication.
    /// </summary>
    private static readonly string[] PerCharacterPrefixes = { "RSBot.Skills.", "RSBot.Party.Buffing", "RSBot.Shopping." };

    /// <summary>
    ///     Carve-outs from <see cref="PerCharacterPrefixes" /> - "what to sell/store" (and the
    ///     matching "also sell/store pet items" toggles) is a filter list like every other shared
    ///     item filter, not part of "what to buy"/"what to cast", so it's shared too even though
    ///     it lives under the same "RSBot.Shopping." prefix as the (still per-character) buy list.
    /// </summary>
    private static readonly string[] SharedExceptions = { "RSBot.Shopping.Sell", "RSBot.Shopping.Store" };

    /// <summary>
    ///     The per-character config - only ever holds keys matching <see cref="PerCharacterPrefixes" />.
    /// </summary>
    private static Config _config;

    /// <summary>
    ///     The shared config - every key that isn't in <see cref="PerCharacterPrefixes" />. Loaded
    ///     once, eagerly, independent of profile/account/character selection (mirrors how
    ///     <see cref="ProfileManager" />'s own config loads immediately in its static ctor) - so
    ///     shared settings are readable even before any character has logged in.
    /// </summary>
    private static readonly Config _shared = new(Path.Combine(Kernel.BasePath, "User", "Shared.rs"));

    /// <summary>
    ///     Resolves a character name to the username of the account it belongs to, or null if
    ///     unknown. <see cref="Components.Accounts" />/<see cref="Components.Account" /> live in the
    ///     RSBot.General plugin, which depends on this library - not the other way around - so this
    ///     library can't reference them directly. RSBot.General's plugin Initialize() sets this once
    ///     instead. Left null (falling back to the old flat per-profile layout, no account subfolder)
    ///     if that plugin isn't loaded or the character isn't recognized under any saved account.
    /// </summary>
    public static Func<string, string> AccountResolver { get; set; }

    /// <summary>
    ///     The config directory
    /// </summary>
    private static string _configDirectory => Path.Combine(Kernel.BasePath, "User", ProfileManager.SelectedProfile);

    /// <summary>
    ///     True if <paramref name="key" /> should be read/written from the per-character config
    ///     instead of the shared one.
    /// </summary>
    private static bool IsPerCharacter(string key) =>
        !SharedExceptions.Any(key.StartsWith) && PerCharacterPrefixes.Any(key.StartsWith);

    /// <summary>
    ///     Load config from file
    /// </summary>
    /// <param name="charName">The character name.</param>
    public static void Load(string charName)
    {
        var account = AccountResolver?.Invoke(charName);
        var directory = account == null ? _configDirectory : Path.Combine(_configDirectory, account);

        MigrateFlatFileIfNeeded(charName, directory);

        _config = new Config(Path.Combine(directory, charName + ".rs"));

        Log.Notify("[Player] settings have been loaded!");
    }

    /// <summary>
    ///     One-time, idempotent split of a pre-existing flat <c>User/{Profile}/{charName}.rs</c>
    ///     file (from before the shared/per-character split existed) into the new layout: shared
    ///     keys merge into <see cref="_shared" /> (a key already present there is left alone -
    ///     these values are supposed to be identical across characters, so whichever character
    ///     migrates first "wins" any conflict), and per-character keys move to
    ///     <paramref name="newDirectory" />. The old file is renamed to "*.rs.bak" rather than
    ///     deleted - this is real user data, and renaming both prevents re-migrating on every
    ///     future load and leaves a recoverable backup if anything looks wrong after migration.
    /// </summary>
    /// <param name="charName">The character name.</param>
    /// <param name="newDirectory">Where the character's own (now narrowed) config file belongs.</param>
    private static void MigrateFlatFileIfNeeded(string charName, string newDirectory)
    {
        var newPath = Path.Combine(newDirectory, charName + ".rs");
        if (File.Exists(newPath))
            return; // already migrated (or never needed to be - same path as the old flat layout)

        var oldPath = Path.Combine(_configDirectory, charName + ".rs");
        if (!File.Exists(oldPath))
            return; // nothing to migrate

        try
        {
            var perCharacterLines = new List<string>();
            var sharedAdded = 0;

            foreach (var line in File.ReadAllLines(oldPath))
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains('{'))
                    continue;

                var key = line.Split('{')[0];

                if (IsPerCharacter(key))
                {
                    perCharacterLines.Add(line);
                    continue;
                }

                if (_shared.Exists(key))
                    continue;

                var value = line.Split('{')[1].Split('}')[0];
                _shared.Set(key, value);
                sharedAdded++;
            }

            if (sharedAdded > 0)
                _shared.Save();

            if (perCharacterLines.Count > 0)
            {
                Directory.CreateDirectory(newDirectory);
                File.WriteAllLines(newPath, perCharacterLines);
            }

            File.Move(oldPath, oldPath + ".bak");

            Log.Notify(
                $"[Player] Migrated \"{charName}\"'s settings to the new shared/per-account layout "
                    + $"({sharedAdded} shared, {perCharacterLines.Count} per-character) - old file kept as backup."
            );
        }
        catch (Exception ex)
        {
            Log.Warn($"[Player] Could not migrate \"{charName}\"'s settings to the new layout: {ex.Message}");
        }
    }

    /// <summary>
    ///     Existses the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public static bool Exists(string key)
    {
        if (IsPerCharacter(key))
            return _config != null && _config.Exists(key);

        return _shared.Exists(key);
    }

    /// <summary>
    ///     Gets the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    public static T Get<T>(string key, T defaultValue = default)
    {
        if (IsPerCharacter(key))
            return _config == null ? defaultValue : _config.Get(key, defaultValue);

        return _shared.Get(key, defaultValue);
    }

    /// <summary>
    ///     Gets the enum value with specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    public static TEnum GetEnum<TEnum>(string key, TEnum defaultValue = default)
        where TEnum : struct
    {
        if (IsPerCharacter(key))
            return _config == null ? defaultValue : _config.GetEnum(key, defaultValue);

        return _shared.GetEnum(key, defaultValue);
    }

    /// <summary>
    ///     Sets the specified key inside the config.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void Set<T>(string key, T value)
    {
        if (IsPerCharacter(key))
            _config?.Set(key, value);
        else
            _shared.Set(key, value);
    }

    /// <summary>
    ///     Gets the array.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="delimiter">The delimiter.</param>
    /// <returns></returns>
    public static T[] GetArray<T>(string key, char delimiter = ',')
    {
        if (IsPerCharacter(key))
            return _config == null ? Array.Empty<T>() : _config.GetArray<T>(key, delimiter);

        return _shared.GetArray<T>(key, delimiter);
    }

    /// <summary>
    ///     Gets the enum value with specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="delimiter">The delimiter.</param>
    public static TEnum[] GetEnums<TEnum>(string key, char delimiter = ',')
        where TEnum : struct
    {
        if (IsPerCharacter(key))
            return _config == null ? Array.Empty<TEnum>() : _config.GetEnums<TEnum>(key, delimiter);

        return _shared.GetEnums<TEnum>(key, delimiter);
    }

    /// <summary>
    ///     Sets the array.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="values">The values.</param>
    /// <param name="delimiter">The delimiter.</param>
    public static void SetArray<T>(string key, IEnumerable<T> values, string delimiter = ",")
    {
        if (IsPerCharacter(key))
            _config?.SetArray(key, values, delimiter);
        else
            _shared.SetArray(key, values, delimiter);
    }

    /// <summary>
    ///     Saves the specified file.
    /// </summary>
    public static void Save()
    {
        _config?.Save();
        _shared.Save();

        Log.Notify("[Player] have been saved!");
        EventManager.FireEvent("OnSavePlayerConfig");
    }
}
