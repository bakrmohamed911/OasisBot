using System;
using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Objects;
using RSBot.Core.Plugins;
using RSBot.Training.Bot;
using RSBot.Training.Bundle;
using RSBot.Training.Components;
using RSBot.Training.Subscriber;

namespace RSBot.Training
{
    public class TrainingBase : IBotbase
    {
        public TrainingManager Manager { get; private set; }
        public string Name => "RSBot.Training";
        public Area Area => Container.Bot.Area;

        /// <summary>
        ///     Kernel.TickCount when <see cref="Tick" /> last logged its gate diagnostic - see
        ///     that log line's own comment for why this exists.
        /// </summary>
        private int _lastGateDiagTick = -100_000;

        /// <summary>
        ///     Ticks this instance. It's the botbase main-loop
        /// </summary>
        public void Tick()
        {
            // TEMPORARY diagnostic - remove once the "reconnect mid-route leaves the character
            // standing still" report is root-caused. Throttled to once per 3s (not every tick)
            // to avoid adding meaningfully to log volume. Reports every early-return gate this
            // method has, in order, so whichever one is stuck true is visible directly instead
            // of guessed at.
            if (Kernel.TickCount - _lastGateDiagTick >= 3000)
            {
                _lastGateDiagTick = Kernel.TickCount;
                Log.Debug(
                    $"[Training] Tick gate: Running={Kernel.Bot.Running} "
                        + $"Exchanging={Game.Player.Exchanging} Untouchable={Game.Player.Untouchable} "
                        + $"LifeState={Game.Player.State.LifeState} "
                        + $"AreaDistance={Container.Bot.Area.Position.DistanceToPlayer():0} "
                        + $"LoopRunning={Bundles.Loop.Running} ScrollState={Game.Player.State.ScrollState} "
                        + $"PatrolActive={TrainingPlaceManager.IsActive} "
                        + $"PatrolIndex={TrainingPlaceManager.CurrentIndex} "
                        + $"HasArrived={TrainingPlaceManager.HasArrived}"
                );
            }

            if (!Kernel.Bot.Running)
                return;

            if (Game.Player.Exchanging)
                return;

            if (Game.Player.Untouchable)
                return;

            if (Game.Player.State.LifeState == LifeState.Dead)
                return;

            // Begin the loopback if needed - this is what runs the current region's town script
            // (Data/Scripts/Towns/{RegionId}.rbs, if any - store/repair/buy at whichever NPCs
            // that town's script visits) before falling back to walking back to the Training
            // Area's center, so it still needs to fire even while a training-place patrol is
            // active. See LoopBundle.CheckForWalkbackScript for where that fallback itself is
            // suppressed for patrol mode instead - MovementBundle.AdvancePatrol() already walks
            // the recorded route independent of distance, so the fallback would otherwise send
            // the character off toward whatever CalculatePathToTrainingArea() came up with
            // instead of the route actually recorded.
            if (Container.Bot.Area.Position.DistanceToPlayer() > 80)
                Bundles.Loop.Start();

            if (Bundles.Loop.Running)
                return;

            //Nothing if in scroll state!
            if (
                Game.Player.State.ScrollState == ScrollState.NormalScroll
                || Game.Player.State.ScrollState == ScrollState.ThiefScroll
            )
                return;

            try
            {
                Container.Bot.Tick();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex);
            }
        }

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        public void Start()
        {
            if (Kernel.Bot.Botbase.Area.Position.X == 0)
            {
                Log.WarnLang("ConfigureTrainingAreaBeforeStartBot");
                Kernel.Bot.Stop();
            }

            //Already reloading when config saved via ConfigSubscriber
            //Bundles.Reload();
            //Container.Bot.Reload();
        }

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public void Stop()
        {
            lock (Container.Lock)
            {
                Bundles.Stop();
            }
        }

        /// <summary>
        ///     Always initialize the botbase so other botbases can make use of its otherwise internal features.
        /// </summary>
        public void Register()
        {
            Manager = new TrainingManager();
            Container.Lock = new object();
            Container.Bot = new Botbase();

            //Bundles.Reload();

            BundleSubscriber.SubscribeEvents();
            ConfigSubscriber.SubscribeEvents();
            TeleportSubscriber.SubscribeEvents();

            ScriptManager.CommandHandlers.Add(new TrainingAreaScriptCommand());
            Log.Debug("[Training] Botbase registered to the kernel!");
        }
    }
}
