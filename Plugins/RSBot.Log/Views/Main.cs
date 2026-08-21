using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using RSBot.Core;
using RSBot.Core.Event;
using SDUI.Controls;

namespace RSBot.Log.Views;

[ToolboxItem(false)]
public partial class Main : DoubleBufferedControl
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Main" /> class.
    /// </summary>
    public Main()
    {
        InitializeComponent();
        LoadConfig();

        EventManager.SubscribeEvent("OnAddLog", new Action<string, LogLevel>(AppendLog));

        if (!Kernel.Debug)
        {
            checkDebug.Checked = false;
            checkError.Visible = false;
            checkNormal.Visible = false;
            checkWarning.Visible = false;
            checkDebug.Visible = false;
        }

        // Fully-qualified: this file lives under the RSBot.Log namespace, which would
        // otherwise shadow the RSBot.Core.Log class for an unqualified "Log".
        RSBot.Core.Log.DebugEnabled = checkDebug.Checked;
        checkDebug.CheckedChanged += (_, _) => RSBot.Core.Log.DebugEnabled = checkDebug.Checked;
    }

    /// <summary>
    ///     Appends the log.
    /// </summary>
    /// <param name="message">The message.</param>
    public void AppendLog(string message, LogLevel level = LogLevel.Notify)
    {
        if (this.InvokeRequired)
        {
            // Non-blocking: this is called via a fire-and-forget Task.Run for every event
            // fired while processing a packet (see EventManager.FireEvent), so a burst of
            // log lines (e.g. a character load) could otherwise pile up many ThreadPool
            // threads all blocked here waiting on the UI thread at once.
            if (IsHandleCreated)
                this.BeginInvoke(new Action<string, LogLevel>(AppendLog), message, level);

            return;
        }

        if (!checkEnabled.Checked)
            return;

        // Game.Player.Name comes straight off the wire - a misparsed/corrupted packet
        // field (this session chased exactly one) can turn it into something containing
        // characters that are illegal in a Windows path. Path.Combine won't catch that;
        // Directory.CreateDirectory/File.AppendText further down would throw, and an
        // exception thrown by an "OnAddLog" subscriber used to be able to crash the
        // whole process (see EventManager.InvokeSafely). Sanitize defensively so this
        // can't throw regardless of whether that recursion is also fixed.
        var playerName = Game.Player == null ? "Environment" : SanitizeForPath(Game.Player.Name);

        var logFile = Path.Combine(Kernel.BasePath, "User", "Logs", playerName, $"{DateTime.Now:dd-MM-yyyy}.txt");

        if (level == LogLevel.Debug && !checkDebug.Checked)
            return;

        if (level == LogLevel.Error && !checkError.Checked)
            return;

        if (level == LogLevel.Notify && !checkNormal.Checked)
            return;

        if (level == LogLevel.Warning && !checkWarning.Checked)
            return;

        txtLog.Write($"<{level}> \t{message}", true, Kernel.Debug, logFile);
    }

    /// <summary>
    ///     Replaces characters that are illegal in a Windows file/directory name, and caps
    ///     the length, so an untrusted value can safely be used as a single path segment.
    /// </summary>
    /// <param name="value">The value.</param>
    private static string SanitizeForPath(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "Unknown";

        var invalid = Path.GetInvalidFileNameChars();
        var chars = value.Length > 64 ? value.Substring(0, 64).ToCharArray() : value.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
            if (Array.IndexOf(invalid, chars[i]) >= 0)
                chars[i] = '_';

        var sanitized = new string(chars).Trim();

        return string.IsNullOrEmpty(sanitized) ? "Unknown" : sanitized;
    }

    /// <summary>
    ///     Loads the configuration.
    /// </summary>
    private void LoadConfig()
    {
        checkEnabled.Checked = GlobalConfig.Get("RSBot.Log.logEnabled", true);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkEnabled control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void checkEnabled_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.Log.logEnabled", checkEnabled.Checked.ToString());
    }

    /// <summary>
    ///     Handles the Click event of the btnReset control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void btnReset_Click(object sender, EventArgs e)
    {
        txtLog.Text = string.Empty;
    }
}
