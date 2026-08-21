using System.Collections.Generic;
using System.Linq;
using RSBot.Core.Network;
using RSBot.Core.Objects.Skill;

namespace RSBot.Core.Objects;

public class State
{
    /// <summary>
    ///     Gets or sets the state of the life.
    /// </summary>
    /// <value>
    ///     The state of the life.
    /// </value>
    public LifeState LifeState { get; set; }

    /// <summary>
    ///     Gets or sets the motion.
    /// </summary>
    /// <value>
    ///     The motion.
    /// </value>
    public MotionState MotionState { get; set; }

    /// <summary>
    ///     Gets or sets the state of the body.
    /// </summary>
    /// <value>
    ///     The state of the body.
    /// </value>
    public BodyState BodyState { get; set; }

    /// <summary>
    ///     Gets or sets the state of the hit.
    /// </summary>
    /// <value>
    ///     The state of the hit.
    /// </value>
    public ActionHitStateFlag HitState { get; set; }

    /// <summary>
    ///     Gets or sets the walk speed.
    /// </summary>
    /// <value>
    ///     The walk speed.
    /// </value>
    public float WalkSpeed { get; set; }

    /// <summary>
    ///     Gets or sets the run speed.
    /// </summary>
    /// <value>
    ///     The run speed.
    /// </value>
    public float RunSpeed { get; set; }

    /// <summary>
    ///     Gets or sets the berzerk speed.
    /// </summary>
    /// <value>
    ///     The berzerk speed.
    /// </value>
    public float BerzerkSpeed { get; set; }

    /// <summary>
    ///     Gets or sets the active buffs.
    /// </summary>
    /// <value>
    ///     The active buffs.
    /// </value>
    public List<SkillInfo> ActiveBuffs { get; } = new();

    /// <summary>
    ///     Gets the active item perks.
    /// </summary>
    /// <value>
    ///     The active item perks.
    /// </value>
    public Dictionary<uint, ItemPerk> ActiveItemPerks { get; } = new();

    /// <summary>
    ///     Gets or sets the state of the PVP.
    /// </summary>
    /// <value>
    ///     The state of the PVP.
    /// </value>
    public PvpState PvpState { get; set; }

    /// <summary>
    ///     Gets or sets the state of the battle.
    /// </summary>
    /// <value>
    ///     The state of the battle.
    /// </value>
    public BattleState BattleState { get; set; }

    /// <summary>
    ///     Gets or sets the state of the scroll.
    /// </summary>
    /// <value>
    ///     The state of the scroll.
    /// </value>
    public ScrollState ScrollState { get; set; }

    /// <summary>
    ///     Gets or sets the dialog state.
    /// </summary>
    public DialogState DialogState { get; set; }

    /// <summary>
    ///     Creates a new state object by the given packet
    /// </summary>
    /// <param name="packet">The packet.</param>
    /// <returns></returns>
    public void Deserialize(Packet packet)
    {
        Log.Debug($"[State] pos={packet.Length - packet.Remaining} before LifeState, remaining={packet.Remaining}");

        LifeState = (LifeState)packet.ReadByte();

        if (LifeState == 0)
            LifeState = LifeState.Alive;

        if (Game.ClientType > GameClientType.Thailand)
            packet.ReadByte(); //unkByte0

        MotionState = (MotionState)packet.ReadByte();
        BodyState = (BodyState)packet.ReadByte();

        Log.Debug($"[State] pos={packet.Length - packet.Remaining} LifeState={LifeState} MotionState={MotionState} BodyState={BodyState}, remaining={packet.Remaining}");

        // Re-excluded Vietnam274: the previous fix here (re-adding this read for Vietnam274)
        // was validated only against the local player's own character data and got it
        // backwards. A raw hex dump of a failing MOB_CA_PERYTON_CLON spawn packet proved it:
        // reading this byte put WalkSpeed/RunSpeed/BerzerkSpeed one byte off, decoding as
        // garbage denormalized floats (e.g. ~6E-39) and buffCount as a nonsensical 2 with
        // only 4 bytes left in the packet (guaranteed EndOfStreamException). Shifting the
        // read window back by exactly this one byte - i.e. NOT reading it - decodes the same
        // bytes as WalkSpeed=21.0, sane Run/BerzerkSpeed values, and buffCount=0 with the
        // parse finishing cleanly. vSRO 274 does not send this byte after all.
        if (Game.ClientType > GameClientType.Vietnam193 && Game.ClientType != GameClientType.Vietnam274)
            packet.ReadByte(); // hasRedArrowEffect

        WalkSpeed = packet.ReadFloat();
        RunSpeed = packet.ReadFloat();
        BerzerkSpeed = packet.ReadFloat();

        Log.Debug($"[State] pos={packet.Length - packet.Remaining} speeds walk={WalkSpeed} run={RunSpeed} bzerk={BerzerkSpeed}, remaining={packet.Remaining}");

        var buffCount = packet.ReadByte();
        Log.Debug($"[State] pos={packet.Length - packet.Remaining} buffCount={buffCount}, remaining={packet.Remaining}");

        for (var i = 0; i < buffCount; i++)
        {
            var id = packet.ReadUInt();
            var token = packet.ReadUInt();

            var buff = new SkillInfo(id, token);
            if (buff.Record == null)
            {
                Log.Debug($"[State] buff {i}: id={id} token={token} Record=null, remaining={packet.Remaining}");
                continue;
            }

            if (buff.Record.Params.Contains(1701213281))
                packet.ReadBool(); //IsCreator

            Log.Debug($"[State] buff {i}: id={id} token={token}, remaining={packet.Remaining}");
            ActiveBuffs.Add(buff);
        }

        Log.Debug($"[State] pos={packet.Length - packet.Remaining} done, remaining={packet.Remaining}");
    }

    /// <summary>
    ///     Gets the active buff by skill identifier.
    /// </summary>
    /// <returns></returns>
    public bool HasActiveBuff(SkillInfo skill, out SkillInfo buff)
    {
        buff = null;
        if (skill == null || skill.Record == null)
            return false;

        // TODO: Some buffs have Action_Overlap = 0. When filtering only by indirect criteria, such as Action_Overlap,
        // there are situations when the buff is not added, because another buff is detected as this buff (Physical Screen = Morale Screen).
        // We need to find an attribute that allows us to accurately determine the inheritance of buffs from another books (Body Deity -> Angel's Body).
        // As a temporary solution, we are filtering only the skills of one group, but in this case,
        // buffs from different books/series are not considered inheritable.
        buff = ActiveBuffs.Find(p =>
            p.Record.Action_Overlap == skill.Record.Action_Overlap
            && p.Record.Basic_Group.StartsWith(skill.Record.Basic_Group)
            && p.Record.Basic_Activity == skill.Record.Basic_Activity
            && p.Record.ReqCast_Weapon1 == skill.Record.ReqCast_Weapon1
            && p.Record.ReqCast_Weapon2 == skill.Record.ReqCast_Weapon2
            && p.Record.TargetType_Animal == skill.Record.TargetType_Animal
            && p.Record.Target_Required == skill.Record.Target_Required
            && p.Record.ReqLearn_Race == skill.Record.ReqLearn_Race
        );

        return buff != null;
    }

    /// <summary>
    ///     Gets the active buff by skill identifier.
    /// </summary>
    /// <returns></returns>
    public bool TryGetActiveBuff(uint token, out SkillInfo buff)
    {
        buff = ActiveBuffs.Find(p => p.Token == token);

        return buff != null;
    }

    /// <summary>
    ///     Gets the active buff by skill identifier.
    /// </summary>
    /// <returns></returns>
    public bool TryRemoveActiveBuff(uint token, out SkillInfo removedBuff)
    {
        removedBuff = ActiveBuffs.Find(p => p.Token == token);
        if (removedBuff == null)
            return false;

        return ActiveBuffs.Remove(removedBuff);
    }

    /// <summary>
    ///     Checks two active DoTs.
    /// </summary>
    /// <returns></returns>
    public bool HasTwoDots() => ActiveBuffs.Count(b => b.IsDot) >= 2;
}
