using System;
using System.ComponentModel;
using System.Windows.Forms;
using RSBot.Core;
using RSBot.Core.Event;
using SDUI.Controls;

namespace RSBot.Extras.Views;

[ToolboxItem(false)]
internal partial class Main : DoubleBufferedControl
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Main" /> class.
    /// </summary>
    public Main()
    {
        InitializeComponent();
        SubscribeEvents();
    }

    /// <summary>
    ///     Subscribes the events.
    /// </summary>
    private void SubscribeEvents()
    {
        EventManager.SubscribeEvent("OnInitialized", OnInitialized);
    }

    /// <summary>
    ///     Called when main window loaded.
    /// </summary>
    private void OnInitialized()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(OnInitialized));
            return;
        }

        checkStayConnected.Checked = GlobalConfig.Get<bool>("RSBot.General.StayConnected");
        checkBoxBotTrayMinimized.Checked = GlobalConfig.Get<bool>("RSBot.General.TrayWhenMinimize");
        checkAutoHidePendingWindow.Checked = GlobalConfig.Get<bool>("RSBot.General.AutoHidePendingWindow");
        checkEnableQueueLogs.Checked = GlobalConfig.Get<bool>("RSBot.General.PendingEnableQueueLogs");
        checkEnableQueueNotification.Checked = GlobalConfig.Get<bool>("RSBot.General.EnableQueueNotification");
        numQueueLeft.Value = GlobalConfig.Get("RSBot.General.QueueLeft", 30);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkStayConnected control.
    /// </summary>
    private void checkStayConnected_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.StayConnected", checkStayConnected.Checked);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkBoxBotTrayMinimized control.
    /// </summary>
    private void checkBoxBotTrayMinimized_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.TrayWhenMinimize", checkBoxBotTrayMinimized.Checked);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkAutoHidePendingWindow control.
    /// </summary>
    private void checkAutoHidePendingWindow_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.AutoHidePendingWindow", checkAutoHidePendingWindow.Checked);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkEnableQueueLogs control.
    /// </summary>
    private void checkEnableQueueLogs_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.PendingEnableQueueLogs", checkEnableQueueLogs.Checked);
    }

    /// <summary>
    ///     Handles the CheckedChanged event of the checkEnableQueueNotification control.
    /// </summary>
    private void checkEnableQueueNotification_CheckedChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.EnableQueueNotification", checkEnableQueueNotification.Checked);
    }

    /// <summary>
    ///     Handles the ValueChanged event of the numQueueLeft control.
    /// </summary>
    private void numQueueLeft_ValueChanged(object sender, EventArgs e)
    {
        GlobalConfig.Set("RSBot.General.QueueLeft", numQueueLeft.Value);
    }

    /// <summary>
    ///     Handles the Click event of the btnShowPending control. The pending window itself is
    ///     owned by the General plugin (it's tightly coupled to the automated-login flow there),
    ///     so this just asks General to toggle it via the event bus.
    /// </summary>
    private void btnShowPending_Click(object sender, EventArgs e)
    {
        EventManager.FireEvent("OnTogglePendingWindowRequested");
    }

    /// <summary>
    ///     Handles the Click event of the btnSoundSettingSetup control.
    /// </summary>
    private void btnSoundSettingSetup_Click(object sender, EventArgs e)
    {
        View.SoundNotificationWindow.ShowDialog();
    }
}
