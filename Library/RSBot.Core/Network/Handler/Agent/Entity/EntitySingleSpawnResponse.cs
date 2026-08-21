using System;
using RSBot.Core.Components;
using RSBot.Core.Extensions;

namespace RSBot.Core.Network.Handler.Agent.Entity;

internal class EntitySingleSpawnResponse : IPacketHandler
{
    /// <summary>
    ///     Gets or sets the opcode.
    /// </summary>
    /// <value>
    ///     The opcode.
    /// </value>
    public ushort Opcode => 0x3015;

    /// <summary>
    ///     Gets or sets the destination.
    /// </summary>
    /// <value>
    ///     The destination.
    /// </value>
    public PacketDestination Destination => PacketDestination.Client;

    /// <summary>
    ///     Handles the packet.
    /// </summary>
    /// <param name="packet">The packet.</param>
    public void Invoke(Packet packet)
    {
        // Unlike EntityGroupSpawnEndResponse (which wraps each entity's Parse call), this
        // had no exception handling at all - a single misparsed entity here (e.g. the
        // Vietnam274 layout-misalignment bug currently being chased) would throw all the
        // way up out of the packet dispatcher instead of just dropping this one entity.
        try
        {
            SpawnManager.Parse(packet);
        }
        catch (Exception ex)
        {
            Log.Warn($"Single spawn parse failed: {ex}");

            // Rare (only on an actual parse failure, not per-entity/per-tick), so this
            // doesn't carry the log-volume risk the removed per-entity Movement/Position
            // debug logging did. GetBytes() returns the whole underlying buffer regardless
            // of where the reader cursor ended up, so this is ground truth for computing
            // the correct field layout by hand instead of guessing from downstream symptoms.
            Log.Debug($"[Spawn] Raw packet bytes for the failed parse:\n{packet.GetBytes().HexDump(0, packet.Length)}");
        }
    }
}
