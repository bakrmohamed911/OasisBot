using System.Collections.Generic;
using System.Linq;
using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Event;
using RSBot.Core.Extensions;
using RSBot.Core.Objects;
using RSBot.Core.Objects.Spawn;

namespace RSBot.Training.Bundle.Target;

internal class TargetBundle : IBundle
{
    private const int BLACKLIST_TIMEOUT = 5_000;

    #region Fields

    private Dictionary<uint, int> _blacklist;
    private int _lastDiagnosticTick;

    #endregion Fields

    #region Constructor

    public TargetBundle()
    {
        SubscribeEvents();
    }

    #endregion Constructor

    #region Events

    private void OnTargetBehindObstacle() => DeselectAndBlacklist("behind an obstacle");

    private void OnTargetOutOfRange() => DeselectAndBlacklist("out of engage range");

    /// <summary>
    ///     Drops the current selection and blacklists it for BLACKLIST_TIMEOUT so
    ///     GetNearestEnemy() won't immediately hand the exact same mob straight back on
    ///     the very next tick. Both AttackBundle's own client-side checks (obstacle,
    ///     out-of-range) and the server-rejected-cast path (ActionSkillCastResponse) route
    ///     through here via events, rather than any of them nulling Game.SelectedEntity
    ///     directly - a direct null-out has no way to reach this blacklist, which is
    ///     exactly what used to cause an immediate deselect-then-reselect flicker (each
    ///     reselect being a real blocking network round-trip, since TrySelect can only
    ///     skip that round-trip when the entity is *already* the current selection).
    /// </summary>
    private void DeselectAndBlacklist(string reason)
    {
        if (Game.SelectedEntity == null)
            return;

        var selectedEntityUniqueId = Game.SelectedEntity.UniqueId;
        Game.SelectedEntity?.TryDeselect();
        Game.SelectedEntity = null;

        Bundles.Movement.LastEntityWasBehindObstacle = true;

        if (_blacklist?.TryAdd(selectedEntityUniqueId, Kernel.TickCount) == true)
            Log.Debug($"Add mob [{selectedEntityUniqueId}] to blacklist for {BLACKLIST_TIMEOUT}ms ({reason})");
    }

    #endregion Events

    #region Methods

    private void SubscribeEvents()
    {
        EventManager.SubscribeEvent("OnTargetBehindObstacle", OnTargetBehindObstacle);
        EventManager.SubscribeEvent("OnTargetOutOfRange", OnTargetOutOfRange);
    }

    /// <summary>
    ///     Invokes this instance.
    /// </summary>
    public void Invoke()
    {
        _blacklist?.RemoveAll(
            (uniqueId, tick) =>
            {
                var flag = Kernel.TickCount - tick > BLACKLIST_TIMEOUT;
                if (flag)
                    Log.Debug($"Removed mob [{uniqueId} from blacklist!");

                return flag;
            }
        );

        var attacker = GetFromCurrentAttackers();
        if (attacker != null && Container.Bot.Area.IsInSight(attacker) && Game.SelectedEntity == null)
        {
            Log.Debug("[TargetBundle] Emergency situation: Attacking the weaker mob first!");

            if (attacker.TrySelect())
                Bundles.Movement.LastEntityWasBehindObstacle = false;

            return;
        }

        if (
            attacker != null
            && Container.Bot.Area.IsInSight(attacker)
            && SpawnManager.TryGetEntity<SpawnedMonster>(Game.SelectedEntity.UniqueId, out var selectedMonster)
            && (byte)attacker.Rarity < (byte)selectedMonster.Rarity
        )
        {
            Log.Debug("[TargetBundle] Emergency situation: Found a weaker mob to attack first, switching target!");

            if (attacker.TrySelect())
                Bundles.Movement.LastEntityWasBehindObstacle = false;

            return;
        }

        var warlockModeEnabled = PlayerConfig.Get("RSBot.Skills.checkWarlockMode", false);
        if (warlockModeEnabled && Game.SelectedEntity?.State.HasTwoDots() == true)
            return;

        if (Game.SelectedEntity != null && Game.SelectedEntity is not SpawnedMonster)
            Game.SelectedEntity = null;

        if (Game.SelectedEntity?.State.LifeState == LifeState.Alive)
            return;

        var monster = GetNearestEnemy();
        if (monster == null)
        {
            DiagnoseNoTarget();
            return;
        }

        // Same bypass as the filter inside GetNearestEnemy() itself (see its comment) - an
        // attacking monster it already chose to hand back here can't then be thrown away by
        // this redundant outer check just because the fight has drifted outside the area.
        if (!monster.AttackingPlayer && !Container.Bot.Area.IsInSight(monster))
            return;

        if (monster.TrySelect())
            Bundles.Movement.LastEntityWasBehindObstacle = false;
    }

    /// <summary>
    ///     Logs why GetNearestEnemy() came back empty (no alive monsters spawned at all vs.
    ///     some spawned but all filtered out - most commonly because the training Area
    ///     doesn't actually cover where the player/mobs are). Surfaces a config mistake
    ///     that otherwise looks identical to a crash: bot runs, buffs itself, but never
    ///     attacks anything, with no error anywhere. Throttled to avoid spamming every tick.
    /// </summary>
    private void DiagnoseNoTarget()
    {
        if (Kernel.TickCount - _lastDiagnosticTick < 3000)
            return;

        _lastDiagnosticTick = Kernel.TickCount;

        var hasAny = SpawnManager.TryGetEntities<SpawnedMonster>(
            m => m.State.LifeState == LifeState.Alive,
            out var aliveMonsters
        );

        if (!hasAny || !aliveMonsters.Any())
        {
            Log.Debug("[TargetBundle] No alive monsters spawned nearby at all.");
            return;
        }

        var aliveMonstersList = aliveMonsters.ToList();
        var inSightCount = aliveMonstersList.Count(m => Container.Bot.Area.IsInSight(m));

        Log.Debug(
            $"[TargetBundle] {aliveMonstersList.Count} alive monster(s) spawned, {inSightCount} inside training area "
                + $"(Area pos={Container.Bot.Area.Position}, radius={Container.Bot.Area.Radius}; Player pos={Game.Player.Position}). "
                + $"Nearest spawned: {aliveMonstersList.OrderBy(m => m.Movement.Source.DistanceTo(Container.Bot.Area.Position)).First().Record.GetRealName()} "
                + $"at {aliveMonstersList.Min(m => m.Movement.Source.DistanceTo(Container.Bot.Area.Position)):0.0}m from area center."
        );
    }

    private SpawnedMonster GetFromCurrentAttackers()
    {
        var attackWeakerFirst = PlayerConfig.Get<bool>("RSBot.Training.checkAttackWeakerFirst");
        if (!attackWeakerFirst || !IsEmergencySituation())
            return null;

        if (
            !SpawnManager.TryGetEntities<SpawnedMonster>(
                e => e.AttackingPlayer && e.State.LifeState == LifeState.Alive,
                out var entities
            )
        )
            return null;

        // OrderBy().OrderBy().OrderByDescending() does NOT chain like ThenBy - each call
        // re-sorts from scratch, so only the LAST one (distance) actually decided the result;
        // rarity/level only broke ties at identical distances, which next-to-never happens.
        // That's the opposite of "weaker first": it was picking whichever attacker happened
        // to be farthest away, rarity and level be damned - easily the giant itself if it was
        // standing back throwing ranged attacks while the general/champion closed in.
        return entities
            .OrderBy(e => (byte)e.Rarity)
            .ThenBy(e => e.Record.Level)
            .ThenByDescending(e => e.Position.DistanceToPlayer())
            .FirstOrDefault();
    }

    private bool IsEmergencySituation()
    {
        // A dangerous (avoid-listed) mob attacking is always worth reacting to.
        if (
            SpawnManager.Any<SpawnedMonster>(e =>
                e.AttackingPlayer && e.State.LifeState == LifeState.Alive && Bundles.Avoidance.AvoidMonster(e.Rarity)
            )
        )
            return true;

        // Being hit by more than one mob at once, of different rarities, is worth reacting to
        // on its own - without this, "attack weaker first" silently did nothing unless the
        // Avoidance list *also* happened to name one of the exact rarities currently attacking
        // (e.g. Giant), which is an easy-to-miss second setting for what the checkbox alone
        // sounds like it should already do: kill the general/champion adds before they pile up
        // while the giant is being tanked, instead of only ever focusing the giant.
        if (
            !SpawnManager.TryGetEntities<SpawnedMonster>(
                e => e.AttackingPlayer && e.State.LifeState == LifeState.Alive,
                out var attackers
            )
        )
            return false;

        var attackersList = attackers.ToList();
        return attackersList.Count > 1 && attackersList.Select(e => e.Rarity).Distinct().Count() > 1;
    }

    /// <summary>
    ///     Gets the nearest enemy.
    /// </summary>
    /// <returns></returns>
    private SpawnedMonster GetNearestEnemy()
    {
        var warlockModeEnabled = PlayerConfig.Get<bool>("RSBot.Skills.checkWarlockMode");
        var ignorePillar = PlayerConfig.Get<bool>("RSBot.Training.checkBoxDimensionPillar");

        if (
            !SpawnManager.TryGetEntities<SpawnedMonster>(
                m =>
                    m.State.LifeState == LifeState.Alive
                    && //Only alive
                    !(warlockModeEnabled && m.State.HasTwoDots())
                    && //Has two Dots?
                    m.IsBehindObstacle == false
                    && //Is not behind obstacle
                    (_blacklist == null || !_blacklist.ContainsKey(m.UniqueId))
                    && //Is not blacklisted
                    (m.AttackingPlayer || !Bundles.Avoidance.AvoidMonster(m.Rarity))
                    && //Is attacking player or shouldn't be avoided
                    // Being "in area" (distance from the area's *center*) doesn't mean it's
                    // reachable from where the player actually stands right now - without
                    // this, AttackBundle immediately rejects and blacklists anything past
                    // EngageDistance, so selecting it here was always wasted work (and a
                    // wasted TrySelect network round-trip) rather than just skipping to the
                    // next, actually-engageable candidate.
                    (m.AttackingPlayer || m.DistanceToPlayer <= Attack.AttackBundle.EngageDistance)
                    && //Is attacking player or within engage distance of the player
                    // A mob already attacking the player has to stay selectable regardless of
                    // the training area's bounds - the area filter exists to stop the bot
                    // wandering off to fights it doesn't need, not to make it defenseless
                    // against something already hitting it. Without this bypass (matching the
                    // two checks above it), a pull that drags the fight outside the configured
                    // Area radius left every attacker filtered out here, so GetNearestEnemy()
                    // came back null and the bot sat there getting hit by 3 mobs while
                    // reporting "no target" the whole time.
                    (m.AttackingPlayer || Container.Bot.Area.IsInSight(m))
                    && //Is in training area, or already attacking us regardless of area bounds
                    !m.Record.IsPandora
                    && //Isn't pandora box
                    !(m.Record.IsDimensionPillar && ignorePillar)
                    && //Isn't dimension pillar
                    !m.Record.IsSummonFlower,
                out var entities
            )
        )
            return default;

        return entities
            .OrderBy(m => m.Movement.Source.DistanceTo(Container.Bot.Area.Position))
            .OrderBy(m => Bundles.Avoidance.PreferMonster(m.Rarity))
            .OrderByDescending(m => m.AttackingPlayer)
            .FirstOrDefault();
    }

    /// <summary>
    ///     Refreshes this instance.
    /// </summary>
    public void Refresh()
    {
        _blacklist = new Dictionary<uint, int>(8);
    }

    public void Stop()
    {
        _blacklist = null;
    }

    #endregion Methods
}
