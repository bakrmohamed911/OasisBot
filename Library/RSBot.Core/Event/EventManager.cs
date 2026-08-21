using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RSBot.Core.Event;

public class EventManager
{
    private static readonly List<(string name, Delegate handler)> _listeners = new();

    /// <summary>
    ///     Registers the event.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="handler">The handler.</param>
    public static void SubscribeEvent(string name, Delegate handler)
    {
        if (handler == null)
            return;

        _listeners.Add((name, handler));
    }

    /// <summary>
    ///     Registers the event.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="handler">The handler.</param>
    public static void SubscribeEvent(string name, Action handler)
    {
        if (handler == null)
            return;

        _listeners.Add((name, handler));
    }

    /// <summary>
    ///     Fires the event.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="parameters">The parameters.</param>
    public static void FireEvent(string name, params object[] parameters)
    {
        try
        {
            var targets = (
                from o in _listeners
                where o.name == name && o.handler.Method.GetParameters().Length == parameters.Length
                select o.handler
            ).ToArray();

            foreach (var target in targets)
                if (Thread.CurrentThread.Name == "Network.PacketProcessor")
                    Task.Run(() => InvokeSafely(target, parameters, name));
                else
                    InvokeSafely(target, parameters, name);
        }
        catch (Exception e)
        {
            Log.Fatal(e);
        }
    }

    /// <summary>
    ///     Invokes a single subscriber, catching per-target instead of only around the whole
    ///     dispatch loop so one bad handler can't stop the others firing.
    /// </summary>
    private static void InvokeSafely(Delegate target, object[] parameters, string name)
    {
        try
        {
            target.DynamicInvoke(parameters);
        }
        catch (Exception e)
        {
            // Log.Fatal calls Log.Warn, which fires "OnAddLog" again. If the handler that
            // just threw IS an "OnAddLog" subscriber (the log window's AppendLog, which
            // builds a file path from data that can be attacker/server-controlled, e.g.
            // a corrupted player name), calling Log.Fatal here re-fires "OnAddLog" with
            // the exact same input that just failed - which fails again, which fires it
            // again, forever, ending in an uncatchable native stack overflow that kills
            // the whole process. Write straight to the exception file for this one case
            // instead of going through the event system again.
            if (name == "OnAddLog")
                Log.FatalFileOnly(e);
            else
                Log.Fatal(e);
        }
    }
}
