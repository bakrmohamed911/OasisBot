using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Event;
using RSBot.Core.Objects;

namespace RSBot.Training.Bundle.Attack;

internal class AttackBundle : IBundle
{
    /// <summary>
    ///     Distance within which TargetBundle is allowed to select a *new* target.
    ///     Shared with TargetBundle.GetNearestEnemy() so a mob is never selected in the
    ///     first place if it's already further away than we're willing to disengage at
    ///     below - that asymmetry (select-near / disengage-far) is what actually prevents
    ///     the flicker; see DisengageDistance for why the two aren't the same value.
    /// </summary>
    public const int EngageDistance = 25;

    /// <summary>
    ///     Distance at which an *already selected* target gets dropped. Deliberately
    ///     wider than EngageDistance: without this gap, a mob drifting back and forth
    ///     across a single cutoff (kiting, or just the player's own movement jitter)
    ///     causes an endless deselect-then-immediately-reselect loop, each reselect being
    ///     a real blocking network round-trip (SpawnedBionic.TrySelect only skips that
    ///     round-trip when the entity is *already* the current selection - which it never
    ///     is right after a deselect). The gap means a target has to genuinely leave
    ///     engagement range, not just graze a line, before we let go of it.
    /// </summary>
    private const int DisengageDistance = 35;

    /// <summary>
    ///     The last tick count for checking func call
    /// </summary>
    private int _lastTick = Kernel.TickCount;

    /// <summary>
    ///     Invokes this instance.
    /// </summary>
    public void Invoke()
    {
        if (Game.SelectedEntity == null || !Game.Player.CanAttack)
            return;

        if (Game.SelectedEntity.IsBehindObstacle)
        {
            Log.Debug("Deselecting entity because it moved behind an obstacle!");

            if (Game.Player.InAction)
                SkillManager.CancelAction();

            // Fire the same event the server-rejection path uses (ActionSkillCastResponse)
            // instead of nulling Game.SelectedEntity ourselves - that event's handler in
            // TargetBundle also blacklists the mob for BLACKLIST_TIMEOUT, which is the
            // part that actually matters here: without it, GetNearestEnemy() has no idea
            // this mob was just rejected and picks the exact same one again next tick.
            EventManager.FireEvent("OnTargetBehindObstacle");

            return;
        }

        // TargetBundle's own selection filter only checks distance from the training area's
        // *center* (Area.IsInSight), not from the player - so once the player has drifted
        // away from center (e.g. while chasing an earlier, now-dead target), it can still
        // select something that's legitimately "in area" yet dozens of meters from where the
        // player actually stands. Cast() below then walks the character the entire way there
        // with InAction held for the whole trip - multiple seconds during which LootBundle's
        // own InAction check keeps it from ever getting a turn, which is exactly what was
        // delaying pickup of loot from the kill(s) that happened before this one. Reject
        // targets past a sane engagement distance up front, independent of dontFollowMobs
        // (which only reacts *after* a target's already been committed to), so the character
        // only ever walks a short distance to reach what it's about to fight.
        // A *new* target is never selected past EngageDistance (25m) - TargetBundle's
        // GetNearestEnemy() filters on the same constant. What's checked here is only
        // whether an *already engaged* target has drifted out to DisengageDistance (35m);
        // see that constant's doc comment for why it's deliberately wider than the
        // selection distance instead of reusing it.
        // A mob that's actively attacking the player is never disengaged for being far away -
        // it's still hitting the player regardless of what the distance figure says (a ranged
        // attacker is expected to be well past melee range; a knockback or lag spike can also
        // put a genuinely-engaged melee mob briefly outside DisengageDistance). Without this,
        // exactly that kind of attacker got dropped and blacklisted here on the very next tick
        // after TargetBundle selected it, over and over - from the player's side that reads as
        // "keeps saying no target" while still visibly being hit by something.
        if (!Game.SelectedEntity.AttackingPlayer && Game.SelectedEntity.DistanceToPlayer > DisengageDistance)
        {
            Log.Debug($"Deselecting entity because it's {Game.SelectedEntity.DistanceToPlayer:0.0}m away - too far to engage!");

            if (Game.Player.InAction)
                SkillManager.CancelAction();

            // Same reasoning as the obstacle case above: fire an event TargetBundle
            // blacklists on, rather than nulling Game.SelectedEntity directly, so the
            // same too-far mob can't be immediately reselected next tick.
            EventManager.FireEvent("OnTargetOutOfRange");

            return;
        }

        bool dontFollowMobs = PlayerConfig.Get<bool>("RSBot.Training.checkBoxDontFollowMobs");
        if (dontFollowMobs && !Container.Bot.Area.IsInSight(Game.SelectedEntity))
        {
            Log.Debug("Deselecting entity because it moved far away from training area!");

            if (Game.Player.InAction)
                SkillManager.CancelAction();

            Game.SelectedEntity?.TryDeselect();
            Game.SelectedEntity = null;

            double distance = Game.Player.Position.DistanceTo(Container.Bot.Area.Position);
            bool hasCollision = Game.Player.Position.HasCollisionBetween(Container.Bot.Area.Position);

            if (distance > Container.Bot.Area.Radius && !hasCollision)
                Game.Player.MoveTo(Container.Bot.Area.Position, false);

            return;
        }

        if (
            SkillManager.ImbueSkill != null
            && !Game.Player.State.HasActiveBuff(SkillManager.ImbueSkill, out _)
            && SkillManager.ImbueSkill.CanBeCasted
        )
            SkillManager.ImbueSkill.Cast(buff: true);

        if (Kernel.TickCount - _lastTick < 500)
            return;

        _lastTick = Kernel.TickCount;

        //if (Game.Player.InAction && !SkillManager.IsLastCastedBasic)
        //  return;

        var useTeleportSkill = PlayerConfig.Get("RSBot.Skills.checkUseTeleportSkill", false);
        if (useTeleportSkill && CastTeleportation())
            return;

        //var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var skill = SkillManager.GetNextSkill();

        //Log.Debug($"Getnextskill: {stopwatch.ElapsedMilliseconds} Action:{Game.Player.InAction} Entity:{Game.SelectedEntity != null} LA:{SkillManager.IsLastCastedBasic} Skill:{skill}");

        if (!Game.Player.InAction)
            Log.Status("Attacking");

        if (skill == null)
        {
            if (Game.Player.InAction)
                return;

            if (PlayerConfig.Get("RSBot.Skills.checkUseDefaultAttack", true))
                SkillManager.CastAutoAttack();

            return;
        }

        if (Game.Player.InAction && SkillManager.IsLastCastedBasic)
            SkillManager.CancelAction();

        var uniqueId = Game.SelectedEntity?.UniqueId;
        if (uniqueId == null)
            return;

        skill?.Cast(uniqueId.Value);
    }

    /// <summary>
    ///     Refreshes this instance.
    /// </summary>
    public void Refresh()
    {
        //Nothing to do here
    }

    public void Stop()
    {
        //Nothing to do
    }

    /// <summary>
    ///     Casts the teleportation skill if it's set up.
    /// </summary>
    /// <returns></returns>
    private bool CastTeleportation()
    {
        if (SkillManager.TeleportSkill?.CanBeCasted != true || Game.SelectedEntity?.State.LifeState != LifeState.Alive)
            return false;

        var distanceToMonster = Game.SelectedEntity?.DistanceToPlayer;
        var availableDistance = SkillManager.TeleportSkill.Record.Params[3] / 10;

        if (availableDistance <= 0)
        {
            Log.Warn("The selected teleportation skill does not have a distance. Is this really a teleport skill?");
        }
        else
        {
            var distanceAfterCasting = distanceToMonster - availableDistance;
            if (distanceAfterCasting < 0)
                distanceAfterCasting *= -1;

            if (distanceAfterCasting < distanceToMonster)
            {
                SkillManager.TeleportSkill.CastAt(Game.SelectedEntity.Position);

                Log.Debug(
                    $"Used teleportation skill [{SkillManager.TeleportSkill.Record.GetRealName()}] (before: {distanceToMonster}m, after: {distanceAfterCasting}m, traveled: {availableDistance}m)"
                );

                return true;
            }
        }

        return false;
    }
}
