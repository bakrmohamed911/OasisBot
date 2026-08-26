using System;
using System.IO;
using RSBot.Core.Components;
using RSBot.Core.Event;

namespace RSBot.Core;

public class Log
{
    /// <summary>
    ///     Whether Debug-level log messages are dispatched. Mirrors the "Debug" checkbox in the
    ///     log view; callers that build expensive/interpolated debug messages on a hot path
    ///     should still check this before formatting, since <see cref="Debug(object)" /> itself
    ///     can only skip the dispatch, not the formatting done by the caller before calling it.
    /// </summary>
    public static bool DebugEnabled { get; set; } = true;

    /// <summary>
    ///     When true, <see cref="Notify(object)" /> drops its message instead of dispatching it.
    ///     For code that deliberately calls something many times in a tight loop purely to
    ///     probe/validate (e.g. a packet-alignment recovery scan trying hundreds of candidate
    ///     offsets), where the callee's own "not found"-style Notify calls would otherwise flood
    ///     the log UI - each one marshals onto the UI thread and creates/touches a native
    ///     RichEdit handle, and a big enough burst can exhaust the process's Windows USER handle
    ///     quota, taking down logging entirely ("Error creating window handle"). Always reset in
    ///     a finally block - this is a static flag, not scoped to one call.
    /// </summary>
    public static bool Suppressed { get; set; }

    /// <summary>
    ///     Replaces the format item in a specified string with the string
    ///     representation of a corresponding object in a specified array
    /// </summary>
    /// <param name="logLevel">The message level</param>
    /// <param name="format">The format</param>
    /// <param name="args">The args</param>
    public static void AppendFormat(LogLevel logLevel, string format, params object[] args)
    {
        EventManager.FireEvent("OnAddLog", string.Format(format, args), logLevel);
    }

    /// <summary>
    ///     Appends the given message to the log using the provided log level.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="message"></param>
    public static void Append(LogLevel logLevel, string message)
    {
        EventManager.FireEvent("OnAddLog", message, logLevel);
    }

    /// <summary>
    ///     Appends the specified message.
    /// </summary>
    /// <param name="obj">The message.</param>
    /// <param name="level">The level.</param>
    public static void Notify(object obj)
    {
        if (Suppressed)
            return;

        EventManager.FireEvent("OnAddLog", obj.ToString(), LogLevel.Notify);
    }

    /// <summary>
    ///     Appends the specified language key.
    /// </summary>
    /// <param name="obj">The message.</param>
    /// <param name="level">The level.</param>
    public static void NotifyLang(string key, params object[] args)
    {
        EventManager.FireEvent("OnAddLog", LanguageManager.GetLang(key, args), LogLevel.Notify);
    }

    /// <summary>
    ///     Append specified debug message
    /// </summary>
    /// <param name="obj">The message</param>
    public static void Debug(object obj)
    {
        if (!DebugEnabled)
            return;

        EventManager.FireEvent("OnAddLog", obj.ToString(), LogLevel.Debug);
    }

    /// <summary>
    ///     Append specified Warning message
    /// </summary>
    /// <param name="obj">The message</param>
    public static void Warn(object obj)
    {
        EventManager.FireEvent("OnAddLog", obj.ToString(), LogLevel.Warning);
    }

    /// <summary>
    ///     Appends the specified language key.
    /// </summary>
    /// <param name="obj">The message.</param>
    /// <param name="level">The level.</param>
    public static void WarnLang(string key, params object[] args)
    {
        EventManager.FireEvent("OnAddLog", LanguageManager.GetLang(key, args), LogLevel.Warning);
    }

    /// <summary>
    ///     Append specified Error message
    /// </summary>
    /// <param name="obj">The message</param>
    public static void Error(object obj)
    {
        EventManager.FireEvent("OnAddLog", obj.ToString(), LogLevel.Error);
    }

    /// <summary>
    ///     Change status message on ui
    /// </summary>
    /// <param name="obj">The message</param>
    public static void Status(object obj)
    {
        EventManager.FireEvent("OnChangeStatusText", obj.ToString());
    }

    /// <summary>
    ///     Change status message on ui by language key.
    /// </summary>
    /// <param name="obj">The message.</param>
    /// <param name="level">The level.</param>
    public static void StatusLang(string key, params object[] args)
    {
        EventManager.FireEvent("OnChangeStatusText", LanguageManager.GetLang(key, args));
    }

    /// <summary>
    ///     Append specified fatal message
    /// </summary>
    /// <param name="obj">The message</param>
    public static void Fatal(Exception obj)
    {
        Warn(obj.Message);

        FatalFileOnly(obj);
    }

    /// <summary>
    ///     Writes to the same exception file <see cref="Fatal(Exception)" /> does, but skips the
    ///     <see cref="Warn(object)" /> call - i.e. never fires "OnAddLog". EventManager.FireEvent
    ///     uses this for exceptions it catches while dispatching "OnAddLog" itself: calling the
    ///     normal Fatal(Exception) there would re-fire "OnAddLog" with the same input that just
    ///     failed, which fails again, which fires it again... - a real path to unbounded recursion
    ///     ending in an uncatchable native stack overflow, not a hypothetical one.
    /// </summary>
    /// <param name="obj">The exception.</param>
    internal static void FatalFileOnly(Exception obj)
    {
        var filePath = Path.Combine(Kernel.BasePath, "User", "Logs", "Exceptions", $"{DateTime.Now:dd-MM-yyyy}.txt");
        var directory = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        using (var stream = File.AppendText(filePath))
        {
            // Every entry in this file used to be timestamp-less, making it impossible to tell
            // whether a given crash trace happened just now or hours/builds ago - directly cost
            // real diagnostic time more than once (had to fall back on file mtime, which only
            // reflects the *last* write to the whole day's file, not any individual entry).
            stream.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            stream.WriteLine(obj.ToString());
        }
    }
}
