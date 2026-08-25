using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Objects.Spawn;

namespace RSBot.Training.Bundle.Berzerk;

internal class BerzerkBundle : IBundle
{
    /// <summary>
    ///     Gets or sets the configuration.
    /// </summary>
    /// <value>
    ///     The configuration.
    /// </value>
    public BerzerkConfig Config { get; set; }

    /// <summary>
    ///     Kernel.TickCount the diagnostic below last logged - see its own comment.
    /// </summary>
    private int _lastDiagnosticTick = -100_000;

    /// <summary>
    ///     Invokes this instance.
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public void Invoke()
    {
        // TEMPORARY diagnostic - remove once "berserk doesn't trigger for any marked rarity" is
        // root-caused. CanEnterBerzerk requires BerzerkPoints == 5 (a full gauge) - if that gate
        // is what's actually blocking things (rather than the rarity-list check further below),
        // every one of the config's own trigger conditions would look equally "not working"
        // regardless of which rarities are marked, matching the reported symptom. Throttled to
        // once/3s to avoid adding meaningfully to log volume.
        if (
            Config.WhenTargetSpecificRartiyMonster
            && Game.SelectedEntity is SpawnedMonster selectedForDiag
            && Kernel.TickCount - _lastDiagnosticTick >= 3000
        )
        {
            _lastDiagnosticTick = Kernel.TickCount;
            Log.Debug(
                $"[Berzerk] CanEnterBerzerk={Game.Player.CanEnterBerzerk} "
                    + $"BerzerkPoints={Game.Player.BerzerkPoints}/5 BodyState={Game.Player.State.BodyState} "
                    + $"HasActiveVehicle={Game.Player.HasActiveVehicle} SelectedRarity={selectedForDiag.Rarity} "
                    + $"InBerserkList={Bundles.Avoidance.UseBerserkOnMonster(selectedForDiag.Rarity)}"
            );
        }

        if (!Game.Player.CanEnterBerzerk || Game.Player.HasActiveVehicle)
            return;

        if (Config.WhenFull)
        {
            Game.Player.EnterBerzerkMode();

            return;
        }

        if (Config.SurroundedByMonsters)
        {
            var mobAmount = SpawnManager.Count<SpawnedMonster>(m => m.AttackingPlayer && m.DistanceToPlayer < 20);
            if (mobAmount >= Config.SurroundingMonsterAmount)
            {
                Game.Player.EnterBerzerkMode();
                return;
            }
        }

        if (Config.WhenTargetSpecificRartiyMonster)
        {
            if (Game.SelectedEntity is SpawnedMonster e && Bundles.Avoidance.UseBerserkOnMonster(e.Rarity))
            {
                Game.Player.EnterBerzerkMode();
                return;
            }
        }

        if (!Config.BeeingAttackedByAwareMonster)
            return;

        if (Game.SelectedEntity is not SpawnedMonster entity)
            return;

        if (Bundles.Avoidance.AvoidMonster(entity.Rarity))
            Game.Player.EnterBerzerkMode();
    }

    /// <summary>
    ///     Refreshes this instance.
    /// </summary>
    public void Refresh()
    {
        Config = new BerzerkConfig
        {
            WhenFull = PlayerConfig.Get<bool>("RSBot.Training.checkBerzerkWhenFull"),
            BeeingAttackedByAwareMonster = PlayerConfig.Get<bool>("RSBot.Training.checkBerzerkAvoidance"),
            SurroundedByMonsters = PlayerConfig.Get<bool>("RSBot.Training.checkBerzerkMonsterAmount"),
            SurroundingMonsterAmount = PlayerConfig.Get<byte>("RSBot.Training.numBerzerkMonsterAmount", 5),
            WhenTargetSpecificRartiyMonster = PlayerConfig.Get<bool>("RSBot.Training.checkBerserkOnMonsterRarity"),
        };
    }

    public void Stop()
    {
        //Nothing to do
    }
}
