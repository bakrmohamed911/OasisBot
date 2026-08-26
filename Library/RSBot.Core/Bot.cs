using System;
using System.Threading;
using System.Threading.Tasks;
using RSBot.Core.Components;
using RSBot.Core.Event;
using RSBot.Core.Plugins;

namespace RSBot.Core;

public class Bot
{
    private readonly object _lock = new();
    private Task _workerTask;

    /// <summary>
    ///     Gets or sets a value indicating whether this <see cref="Bot" /> is running.
    /// </summary>
    /// <value>
    ///     <c>true</c> if running; otherwise, <c>false</c>.
    /// </value>
    public volatile bool Running;

    /// <summary>
    ///     Gets or sets to the <see cref="CancellationToken" />
    /// </summary>
    public CancellationTokenSource TokenSource;

    /// <summary>
    ///     Gets the base.
    /// </summary>
    /// <value>
    ///     The base.
    /// </value>
    public IBotbase Botbase { get; private set; }
    public IBotbaseView BotbaseView { get; private set; }

    /// <summary>
    ///     Sets the botbase.
    /// </summary>
    /// <param name="botBase">The bot base.</param>
    public void SetBotbase(IBotbase botBase)
    {
        Botbase = botBase;

        EventManager.FireEvent("OnSetBotbase", botBase);
    }

    public void SetBotbaseView(IBotbaseView botBaseView)
    {
        BotbaseView = botBaseView;
        EventManager.FireEvent("OnSetBotbaseView", botBaseView);
    }

    /// <summary>
    ///     Starts this instance.
    /// </summary>
    public void Start()
    {
        CancellationTokenSource tokenSource;

        lock (_lock)
        {
            if (Running || Botbase == null || (_workerTask != null && !_workerTask.IsCompleted))
                return;

            tokenSource = new CancellationTokenSource();
            TokenSource = tokenSource;
            Running = true;

            // LongRunning, like Kernel.ComponentUpdaterAsync's own tick loop - this task
            // lives for the entire bot session (every Botbase.Tick() call: target/attack/
            // movement/loot/buff/berzerk/protection, each of which can synchronously block
            // on AwaitCallback.AwaitResponse() waiting on the server) rather than for a
            // single short unit of work, so it shouldn't be a plain Task.Run competing with
            // the general ThreadPool for a worker on every one of those blocking waits -
            // that competition, under sustained combat load, was part of what could push
            // the pool into starvation long enough for the whole app to go silent (see the
            // comment in AwaitCallback.AwaitResponse for the other half of this).
            _workerTask = Task.Factory.StartNew(
                () => RunAsync(tokenSource),
                tokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            ).Unwrap();
        }
    }

    private async Task RunAsync(CancellationTokenSource tokenSource)
    {
        var token = tokenSource.Token;

        try
        {
            EventManager.FireEvent("OnStartBot");
            Botbase.Start();

            while (!token.IsCancellationRequested)
            {
                if (Game.Ready)
                    Botbase.Tick();

                await Task.Delay(100, token);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (Exception ex)
        {
            Log.Fatal(ex);
        }
        finally
        {
            if (ReferenceEquals(TokenSource, tokenSource))
                Running = false;
        }
    }

    /// <summary>
    ///     Stops this instance.
    /// </summary>
    public void Stop()
    {
        CancellationTokenSource tokenSource;

        lock (_lock)
        {
            if (Botbase == null || !Running)
                return;

            Running = false;
            tokenSource = TokenSource;
        }

        if (tokenSource != null && !tokenSource.IsCancellationRequested)
            tokenSource.Cancel();

        EventManager.FireEvent("OnStopBot");
        Log.Notify($"Stopping bot {Botbase.Name}");

        CancelActionOnStop();

        Game.SelectedEntity = null;

        ScriptManager.Stop();
        ShoppingManager.Stop();
        PickupManager.Stop();
        Botbase.Stop();

        Log.Notify($"Stopped bot {Botbase.Name}");
        Log.Status("Bot stopped");
    }

    private void CancelActionOnStop()
    {
        var player = Game.Player;
        if (player == null || !player.InAction)
            return;

        _ = CancelActionOnStopAsync();

        async Task CancelActionOnStopAsync()
        {
            const int attempts = 5;
            const int retryDelay = 1000;

            for (var i = 0; i < attempts; i++)
            {
                if (Running || !Game.Ready || !ReferenceEquals(Game.Player, player) || !player.InAction)
                    return;

                SkillManager.CancelAction(0);

                if (i == attempts - 1)
                    return;

                await Task.Delay(retryDelay).ConfigureAwait(false);
            }
        }
    }
}
