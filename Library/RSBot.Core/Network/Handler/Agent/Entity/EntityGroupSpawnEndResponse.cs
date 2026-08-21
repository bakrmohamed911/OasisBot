using System;
using RSBot.Core.Components;
using RSBot.Core.Event;
using RSBot.Core.Extensions;

namespace RSBot.Core.Network.Handler.Agent.Entity;

internal class EntityGroupSpawnEndResponse : IPacketHandler
{
    /// <summary>
    ///     Gets or sets the opcode.
    /// </summary>
    /// <value>
    ///     The opcode.
    /// </value>
    public ushort Opcode => 0x3018;

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
        packet = Game.SpawnInfo.Packet;
        packet.Lock();

        for (var i = 0; i < Game.SpawnInfo.Amount; i++)
            try
            {
                switch (Game.SpawnInfo.Type)
                {
                    case 0x01: //Spawn

                        SpawnManager.Parse(packet, true);

                        break;

                    case 0x02: //Despawn
                        var uniqueId = packet.ReadUInt();
                        SpawnManager.TryRemove(uniqueId, out var removedEntity);
                        EventManager.FireEvent("OnDespawnEntity", removedEntity);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Logged at Warn (was Debug, with no exception detail) while a Vietnam274
                // packet-misalignment bug is being chased - see the matching comment in
                // SpawnManager.Parse. Breaking out of the loop is intentional: the shared
                // packet buffer's read cursor is left at an unknown position once one
                // entity in the group misparses, so the remaining entities in this group
                // can't be parsed either.
                Log.Warn($"Spawn parse failed at index {i}/{Game.SpawnInfo.Amount}: {ex}");

                // Rare (only on an actual parse failure, not per-entity/per-tick) - see the
                // matching comment in EntitySingleSpawnResponse for why this is safe here
                // despite the log-volume crash risk documented on Movement.FromPacket.
                Log.Debug($"[Spawn] Raw packet bytes for the failed group spawn:\n{packet.GetBytes().HexDump(0, packet.Length)}");
                break;
            }

        Game.SpawnInfo = null; //release some resources!
    }
}
