using System;
using RSBot.Core.Network;

namespace RSBot.Core.Objects;

public struct Movement
{
    /// <summary>
    ///     Gets or sets the type.
    /// </summary>
    public MovementType Type;

    /// <summary>
    ///     Gets or sets the type.
    /// </summary>
    public bool Moving;

    /// <summary>
    ///     Gets or sets the has destination.
    /// </summary>
    public bool HasDestination;

    /// <summary>
    ///     Gets or sets the destination.
    /// </summary>
    public Position Destination;

    /// <summary>
    ///     Gets or sets the has source.
    /// </summary>
    public bool HasSource;

    /// <summary>
    ///     Gets or sets the source.
    /// </summary>
    public Position Source;

    /// <summary>
    ///     Gets or sets the has angle.
    /// </summary>
    public bool HasAngle;

    /// <summary>
    ///     Gets or sets the angle.
    /// </summary>
    public float Angle;

    internal double MovingX,
        MovingY;
    internal TimeSpan RemainingTime;

    /// <summary>
    ///     Motion from packet.
    /// </summary>
    /// <param name="packet">The packet.</param>
    /// <returns></returns>
    public static Movement MotionFromPacket(Packet packet)
    {
        var result = new Movement { HasDestination = packet.ReadBool() };

        if (result.HasDestination)
        {
            result.Destination = Position.FromPacketConditional(packet, false);
        }
        else
        {
            packet.ReadByte(); //0 = Spinning, 1 = Sky-/Key-walking
            result.HasAngle = true;
            result.Angle = packet.ReadShort();
        }

        result.HasSource = packet.ReadBool();
        if (result.HasSource)
        {
            result.Source = new Position { Region = packet.ReadUShort() };

            if (result.Source.Region.IsDungeon)
            {
                result.Source.XOffset = packet.ReadInt() / 10f;
                result.Source.ZOffset = packet.ReadFloat();
                result.Source.YOffset = packet.ReadInt() / 10f;
            }
            else
            {
                result.Source.XOffset = packet.ReadShort() / 10f;
                result.Source.ZOffset = packet.ReadFloat();
                result.Source.YOffset = packet.ReadShort() / 10f;
            }
        }

        return result;
    }

    /// <summary>
    ///     Froms the packet.
    /// </summary>
    /// <param name="packet">The packet.</param>
    /// <returns></returns>
    public static Movement FromPacket(Packet packet)
    {
        // This runs once per movement packet per visible entity - dozens of times a
        // second in a populated area. The per-line debug logging here (added to chase a
        // Vietnam274 packet-alignment bug that's since been found and fixed) was routing
        // that volume through synchronous cross-thread UI Invoke calls into a RichTextBox,
        // which is suspected to have contributed to a native RichEdit stack-overflow crash.
        // Removed rather than just left disabled, since "off by default" isn't good enough
        // protection against someone re-enabling Debug logging during normal play.
        var result = new Movement
        {
            Source = Position.FromPacket(packet),
            HasDestination = packet.ReadBool(),
            Type = (MovementType)packet.ReadByte(),
        };

        if (result.HasDestination)
        {
            result.Destination = Position.FromPacketConditional(packet, false);
        }
        else
        {
            packet.ReadByte(); //0 = Spinning, 1 = Sky-/Key-walking
            result.HasAngle = true;
            result.Angle = packet.ReadShort();
        }

        return result;
    }
}
