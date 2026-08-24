using System.IO;
using System.Threading;
using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Objects;
using RSBot.Training.Bot;
using RSBot.Training.Components;

namespace RSBot.Training.Bundle.Loop;

internal class LoopBundle : IBundle
{
    /// <summary>
    ///     Gets the configuration.
    /// </summary>
    /// <value>
    ///     The configuration.
    /// </value>
    public LoopConfig Config { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="LoopBundle" /> is running.
    /// </summary>
    /// <value>
    ///     <c>true</c> if running; otherwise, <c>false</c>.
    /// </value>
    public bool Running { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether [townscript running].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [townscript running]; otherwise, <c>false</c>.
    /// </value>
    public bool TownscriptRunning { get; private set; }

    /// <summary>
    ///     Invokes this instance.
    /// </summary>
    public void Invoke()
    {
        if (!Running)
            return;

        if (Config.UseVehicle && !Game.Player.HasActiveVehicle && !Game.Player.IsInDungeon)
        {
            Game.Player.SummonVehicle();

            //Wait for the vehicle to spawn
            Thread.Sleep(1000);
        }

        //We don't need to use buffs in town...
        if (Config.CastBuffs && !TownscriptRunning)
            Bundles.Buff.Invoke();
    }

    /// <summary>
    ///     Refreshes this instance.
    /// </summary>
    public void Refresh()
    {
        Config = new LoopConfig
        {
            WalkScript = PlayerConfig.Get<string>("RSBot.Walkback.File"),
            UseSpeedDrug = PlayerConfig.Get<bool>("RSBot.Training.checkUseSpeedDrug", true),
            UseVehicle = PlayerConfig.Get<bool>("RSBot.Training.checkUseMount", true),
            CastBuffs = PlayerConfig.Get<bool>("RSBot.Training.checkCastBuffs", true),
            UseReverse = PlayerConfig.Get<bool>("RSBot.Training.checkBoxUseReverse", false),
        };
    }

    public void Stop()
    {
        if (ScriptManager.Running)
            ScriptManager.Stop();

        if (ShoppingManager.Running)
            ShoppingManager.Stop();

        Running = false;
    }

    /// <summary>
    ///     Starts this instance.
    /// </summary>
    public void Start()
    {
        Running = true;

        Refresh();
        CheckForTownScript();

        Running = false;
    }

    /// <summary>
    ///     Checks for town script.
    /// </summary>
    public void CheckForTownScript()
    {
        if (ScriptManager.Running)
            return;

        var filename = Path.Combine(
            ScriptManager.InitialDirectory,
            "Towns",
            Game.Player.Movement.Source.Region + ".rbs"
        );

        //The player is in town, therefore, we need to run the town script first.
        if (!File.Exists(filename))
        {
            CheckForWalkbackScript();
            return;
        }

        if (PlayerConfig.Get<bool>("RSBot.Protection.checkStopBotOnReturnToTown"))
        {
            Kernel.Bot.Stop();
            return;
        }

        Log.NotifyLang("LoadingTownScript", filename);

        TownscriptRunning = true;

        ScriptManager.Load(filename);
        ScriptManager.RunScript(false);

        if (Running && Config.UseReverse)
        {
            var filter = new TypeIdFilter(3, 3, 3, 3);
            var item = Game.Player.Inventory.GetItem(filter);
            if (item != null)
                /*
                    2 => go to last recall point
                    3 => go to last died position
                    7 => select position on map

                    error codes:
                        85: Cannot find the place where you selected as recall point.
                        86: Cannot find the place where you died.
                 */
                if (item.UseTo(3))
                {
                    TownscriptRunning = false;
                    return;
                }
        }

        TownscriptRunning = false;

        Invoke();

        CheckForWalkbackScript(true);
    }

    /// <summary>
    ///     Checks for walkback script.
    /// </summary>
    public void CheckForWalkbackScript(bool startFromTown = false)
    {
        if (ScriptManager.Running || !Kernel.Bot.Running)
            return;

        // This mechanism (a configured walkback script, or else NavigationManager's dynamic
        // navmesh path) exists to get back to the center of a fixed circular Training Area -
        // meaningless while a training-place patrol route is active instead, where
        // MovementBundle.AdvancePatrol() already walks the recorded route on its own,
        // independent of distance. Letting the dynamic-path fallback run here anyway sent the
        // character off toward whatever CalculatePathToTrainingArea() came up with instead of
        // the route actually recorded - CheckForTownScript() (buy/repair/store at the current
        // region's town script, if any) still runs before this either way.
        if (TrainingPlaceManager.IsActive)
            return;

        if (Config.WalkScript == null || !File.Exists(Config.WalkScript))
        {
            Log.Notify("No walkback script found. Attempting to generate a dynamic path...");
            if (NavigationManager.CalculatePathToTrainingArea())
            {
                var dynamicScript = NavigationManager.GenerateRBSFile();
                if (dynamicScript != null)
                {
                    Config.WalkScript = dynamicScript;
                }
            }
        }

        if (Config.WalkScript == null || !File.Exists(Config.WalkScript))
            return;

        Invoke();
        Log.NotifyLang("LoadingWalkScript", Config.WalkScript);

        ScriptManager.Load(Config.WalkScript);
        ScriptManager.RunScript(!startFromTown);
    }
}
