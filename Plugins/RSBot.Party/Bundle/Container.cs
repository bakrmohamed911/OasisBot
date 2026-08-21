using RSBot.Party.Bundle.AutoParty;
using RSBot.Party.Bundle.Commands;
using RSBot.Party.Bundle.PartyMatching;

namespace RSBot.Party.Bundle;

internal static class Container
{
    private static AutoPartyBundle _autoParty;

    private static PartyMatchingBundle _partyMatching;

    private static CommandsBundle _commands;

    /// <summary>
    ///     Gets or sets the automatic party.
    /// </summary>
    /// <value>
    ///     The automatic party.
    /// </value>
    // Self-initializing: game-event handlers (PartyManager.OnEnterGame, OnPartyMemberLeave,
    // OnPartyDismiss, ...) can fire before the Party settings UI has ever been opened -
    // which used to be the only place Refresh() got called - and previously NullReferenceException'd
    // on this being null since it's an ordinary process-crashing exception on those event threads.
    public static AutoPartyBundle AutoParty
    {
        get => _autoParty ??= new AutoPartyBundle();
        set => _autoParty = value;
    }

    /// <summary>
    ///     Gets or sets the party matching.
    /// </summary>
    /// <value>
    ///     The party matching.
    /// </value>
    public static PartyMatchingBundle PartyMatching
    {
        get => _partyMatching ??= new PartyMatchingBundle();
        set => _partyMatching = value;
    }

    /// <summary>
    ///     Gets or sets the party matching.
    /// </summary>
    /// <value>
    ///     The party matching.
    /// </value>
    public static CommandsBundle Commands
    {
        get => _commands ??= new CommandsBundle();
        set => _commands = value;
    }

    /// <summary>
    ///     Refreshes this instance.
    /// </summary>
    public static void Refresh()
    {
        AutoParty.Refresh();
        Commands.Refresh();
    }
}
