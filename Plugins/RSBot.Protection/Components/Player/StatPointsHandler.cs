using System;
using System.Threading;
using System.Threading.Tasks;
using RSBot.Core;
using RSBot.Core.Event;
using RSBot.Core.Network;

namespace RSBot.Protection.Components.Player;

public class StatPointsHandler
{
    public static bool CancellationRequested;

    /// <summary>
    ///     Last tick the periodic catch-up check ran at.
    /// </summary>
    private static int _lastCatchUpTick;

    /// <summary>
    ///     Guards against the periodic catch-up overlapping with itself or a level-up triggered run.
    /// </summary>
    private static bool _isDistributing;

    /// <summary>
    ///     Initializes this instance.
    /// </summary>
    public static void Initialize()
    {
        SubscribeEvents();
    }

    /// <summary>
    ///     Subscribes the events.
    /// </summary>
    private static void SubscribeEvents()
    {
        EventManager.SubscribeEvent("OnLevelUp", new Action<byte>(OnPlayerLevelUp));
        EventManager.SubscribeEvent("OnTick", OnTick);
    }

    /// <summary>
    ///     Periodic safety net: catches any stat points that piled up without a matching "OnLevelUp"
    ///     (e.g. points banked while the bot/app was closed, or granted by a stat-reset/event scroll).
    /// </summary>
    private static void OnTick()
    {
        var elapsed = Kernel.TickCount - _lastCatchUpTick;
        if (elapsed < 5000)
            return;

        _lastCatchUpTick = Kernel.TickCount;

        if (_isDistributing || Game.Player == null || Game.Player.StatPoints == 0)
            return;

        var enabledIfBotIsStopped = PlayerConfig.Get<bool>("RSBot.Protection.checkIncBotStopped", true);
        if (!Kernel.Bot.Running && !enabledIfBotIsStopped)
            return;

        var incStr = PlayerConfig.Get<bool>("RSBot.Protection.checkIncStr");
        var incInt = PlayerConfig.Get<bool>("RSBot.Protection.checkIncInt");
        if (!incStr && !incInt)
            return;

        var numStr = PlayerConfig.Get("RSBot.Protection.numIncStr", 0);
        var numInt = PlayerConfig.Get("RSBot.Protection.numIncInt", 0);

        var cycle = (incStr ? numStr : 0) + (incInt ? numInt : 0);
        if (cycle <= 0)
            return;

        // Enough "steps" to drain every currently banked stat point at the configured STR/INT ratio.
        var stepCount = (Game.Player.StatPoints + cycle - 1) / cycle;

        Task.Run(() =>
        {
            _isDistributing = true;
            try
            {
                IncreaseStatPoints(stepCount);
            }
            finally
            {
                _isDistributing = false;
            }
        });
    }

    /// <summary>
    ///     Cores the on player level up.
    /// </summary>
    private static void OnPlayerLevelUp(byte oldLevel)
    {
        var enabledIfBotIsStopped = PlayerConfig.Get<bool>("RSBot.Protection.checkIncBotStopped", true);
        if (!Kernel.Bot.Running && !enabledIfBotIsStopped)
            return;

        var levelUps = Game.Player.Level - oldLevel;

        Task.Run(() =>
        {
            _isDistributing = true;
            try
            {
                IncreaseStatPoints(levelUps);
            }
            finally
            {
                _isDistributing = false;
            }
        });
    }

    public static void IncreaseStatPoints(int stepCount)
    {
        var incStr = PlayerConfig.Get<bool>("RSBot.Protection.checkIncStr");
        var incInt = PlayerConfig.Get<bool>("RSBot.Protection.checkIncInt");

        var numStr = PlayerConfig.Get("RSBot.Protection.numIncStr", 0);
        var numInt = PlayerConfig.Get("RSBot.Protection.numIncInt", 0);

        for (var iLevelUp = 0; iLevelUp < stepCount; iLevelUp++)
        {
            if (CancellationRequested)
                return;
            if (incStr && numStr > 0)
                for (var i = 0; i < numStr; i++)
                {
                    if (CancellationRequested)
                        return;

                    Log.Notify($"Auto. increasing stat STR to {Game.Player.Strength + 1}");

                    IncreaseStr();

                    //Make sure the user has time to cancel, otherwise it's just too fast (but would still work due to the callback await)
                    Thread.Sleep(500);
                }

            if (incInt && numInt > 0)
                for (var i = 0; i < numInt; i++)
                {
                    if (CancellationRequested)
                        return;

                    Log.Notify($"Auto. increasing stat INT to {Game.Player.Intelligence + 1}");

                    IncreaseInt();

                    Thread.Sleep(500);
                }
        }
    }

    /// <summary>
    ///     Sends the STR increase packet to the server
    /// </summary>
    private static void IncreaseStr()
    {
        if (Game.Player.StatPoints == 0)
        {
            Log.Debug("Could not invest stat point: The player does not have enough points to invest.");

            return;
        }

        var callback = new AwaitCallback(null, 0xB050);

        var packet = new Packet(0x7050);

        PacketManager.SendPacket(packet, PacketDestination.Server, callback);
        callback.AwaitResponse(1000);
    }

    /// <summary>
    ///     Sends the INT increase packet to the server
    /// </summary>
    private static void IncreaseInt()
    {
        if (Game.Player.StatPoints == 0)
        {
            Log.Debug("Could not invest stat point: The player does not have enough points to invest.");

            return;
        }

        var callback = new AwaitCallback(null, 0xB051);

        var packet = new Packet(0x7051);

        PacketManager.SendPacket(packet, PacketDestination.Server, callback);
        callback.AwaitResponse(1000);
    }
}
