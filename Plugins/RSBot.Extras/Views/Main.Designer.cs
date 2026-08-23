namespace RSBot.Extras.Views
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            bodyLayout = new System.Windows.Forms.TableLayoutPanel();
            groupBoxClientSettings = new SDUI.Controls.GroupBox();
            label2 = new SDUI.Controls.Label();
            checkStayConnected = new SDUI.Controls.CheckBox();
            groupBoxBotSettings = new SDUI.Controls.GroupBox();
            label8 = new SDUI.Controls.Label();
            checkBoxBotTrayMinimized = new SDUI.Controls.CheckBox();
            groupBoxServerPending = new SDUI.Controls.GroupBox();
            checkAutoHidePendingWindow = new SDUI.Controls.CheckBox();
            checkEnableQueueLogs = new SDUI.Controls.CheckBox();
            checkEnableQueueNotification = new SDUI.Controls.CheckBox();
            numQueueLeft = new SDUI.Controls.NumUpDown();
            lblPeopleLeft = new SDUI.Controls.Label();
            btnShowPending = new SDUI.Controls.Button();
            groupBoxSoundNotification = new SDUI.Controls.GroupBox();
            btnSoundSettingSetup = new SDUI.Controls.Button();
            bodyLayout.SuspendLayout();
            groupBoxClientSettings.SuspendLayout();
            groupBoxBotSettings.SuspendLayout();
            groupBoxServerPending.SuspendLayout();
            groupBoxSoundNotification.SuspendLayout();
            SuspendLayout();
            //
            // bodyLayout
            //
            bodyLayout.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            bodyLayout.AutoSize = true;
            bodyLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            bodyLayout.BackColor = System.Drawing.Color.Transparent;
            bodyLayout.ColumnCount = 2;
            bodyLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            bodyLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            bodyLayout.Controls.Add(groupBoxClientSettings, 0, 0);
            bodyLayout.Controls.Add(groupBoxBotSettings, 1, 0);
            bodyLayout.Controls.Add(groupBoxServerPending, 0, 1);
            bodyLayout.Controls.Add(groupBoxSoundNotification, 1, 1);
            bodyLayout.Location = new System.Drawing.Point(0, 0);
            bodyLayout.Name = "bodyLayout";
            bodyLayout.Padding = new System.Windows.Forms.Padding(24);
            bodyLayout.RowCount = 2;
            bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            bodyLayout.Size = new System.Drawing.Size(869, 420);
            bodyLayout.TabIndex = 0;
            //
            // groupBoxClientSettings
            //
            groupBoxClientSettings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBoxClientSettings.BackColor = System.Drawing.Color.Transparent;
            groupBoxClientSettings.Controls.Add(label2);
            groupBoxClientSettings.Controls.Add(checkStayConnected);
            groupBoxClientSettings.Location = new System.Drawing.Point(24, 24);
            groupBoxClientSettings.Margin = new System.Windows.Forms.Padding(0, 0, 12, 18);
            groupBoxClientSettings.Name = "groupBoxClientSettings";
            groupBoxClientSettings.Padding = new System.Windows.Forms.Padding(4, 18, 4, 8);
            groupBoxClientSettings.Radius = 10;
            groupBoxClientSettings.ShadowDepth = 4;
            groupBoxClientSettings.Size = new System.Drawing.Size(399, 150);
            groupBoxClientSettings.TabIndex = 0;
            groupBoxClientSettings.TabStop = false;
            groupBoxClientSettings.Text = "Client settings";
            //
            // label2
            //
            label2.ApplyGradient = false;
            label2.AutoSize = false;
            label2.Enabled = false;
            label2.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            label2.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            label2.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            label2.GradientAnimation = false;
            label2.Location = new System.Drawing.Point(24, 82);
            label2.Name = "label2";
            label2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            label2.Size = new System.Drawing.Size(363, 50);
            label2.TabIndex = 1;
            label2.Text = "If the client exits due to a crash, the bot will automatically switch\r\nto clientless mode and continue its tasks";
            //
            // checkStayConnected
            //
            checkStayConnected.AutoSize = true;
            checkStayConnected.BackColor = System.Drawing.Color.Transparent;
            checkStayConnected.Depth = 0;
            checkStayConnected.Location = new System.Drawing.Point(20, 44);
            checkStayConnected.Margin = new System.Windows.Forms.Padding(0);
            checkStayConnected.MouseLocation = new System.Drawing.Point(-1, -1);
            checkStayConnected.Name = "checkStayConnected";
            checkStayConnected.Ripple = true;
            checkStayConnected.Size = new System.Drawing.Size(315, 34);
            checkStayConnected.TabIndex = 0;
            checkStayConnected.Text = "Stay connected if client exits unexpectedly";
            checkStayConnected.UseVisualStyleBackColor = false;
            checkStayConnected.CheckedChanged += checkStayConnected_CheckedChanged;
            //
            // groupBoxBotSettings
            //
            groupBoxBotSettings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBoxBotSettings.BackColor = System.Drawing.Color.Transparent;
            groupBoxBotSettings.Controls.Add(label8);
            groupBoxBotSettings.Controls.Add(checkBoxBotTrayMinimized);
            groupBoxBotSettings.Location = new System.Drawing.Point(446, 24);
            groupBoxBotSettings.Margin = new System.Windows.Forms.Padding(12, 0, 0, 18);
            groupBoxBotSettings.Name = "groupBoxBotSettings";
            groupBoxBotSettings.Padding = new System.Windows.Forms.Padding(4, 18, 4, 8);
            groupBoxBotSettings.Radius = 10;
            groupBoxBotSettings.ShadowDepth = 4;
            groupBoxBotSettings.Size = new System.Drawing.Size(399, 150);
            groupBoxBotSettings.TabIndex = 1;
            groupBoxBotSettings.TabStop = false;
            groupBoxBotSettings.Text = "Bot Settings";
            //
            // label8
            //
            label8.ApplyGradient = false;
            label8.AutoSize = false;
            label8.Enabled = false;
            label8.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            label8.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            label8.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            label8.GradientAnimation = false;
            label8.Location = new System.Drawing.Point(24, 82);
            label8.Name = "label8";
            label8.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            label8.Size = new System.Drawing.Size(363, 50);
            label8.TabIndex = 1;
            label8.Text = "If activated, when the bot is minimized, the bot will\r\nautomatically switch to tray mode and continue to run there.";
            //
            // checkBoxBotTrayMinimized
            //
            checkBoxBotTrayMinimized.AutoSize = true;
            checkBoxBotTrayMinimized.BackColor = System.Drawing.Color.Transparent;
            checkBoxBotTrayMinimized.Depth = 0;
            checkBoxBotTrayMinimized.Location = new System.Drawing.Point(20, 44);
            checkBoxBotTrayMinimized.Margin = new System.Windows.Forms.Padding(0);
            checkBoxBotTrayMinimized.MouseLocation = new System.Drawing.Point(-1, -1);
            checkBoxBotTrayMinimized.Name = "checkBoxBotTrayMinimized";
            checkBoxBotTrayMinimized.Ripple = true;
            checkBoxBotTrayMinimized.Size = new System.Drawing.Size(340, 34);
            checkBoxBotTrayMinimized.TabIndex = 0;
            checkBoxBotTrayMinimized.Text = "Move bot to system tray when minimized";
            checkBoxBotTrayMinimized.UseVisualStyleBackColor = false;
            checkBoxBotTrayMinimized.CheckedChanged += checkBoxBotTrayMinimized_CheckedChanged;
            //
            // groupBoxServerPending
            //
            groupBoxServerPending.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBoxServerPending.BackColor = System.Drawing.Color.Transparent;
            groupBoxServerPending.Controls.Add(checkAutoHidePendingWindow);
            groupBoxServerPending.Controls.Add(checkEnableQueueLogs);
            groupBoxServerPending.Controls.Add(checkEnableQueueNotification);
            groupBoxServerPending.Controls.Add(numQueueLeft);
            groupBoxServerPending.Controls.Add(lblPeopleLeft);
            groupBoxServerPending.Controls.Add(btnShowPending);
            groupBoxServerPending.Location = new System.Drawing.Point(24, 192);
            groupBoxServerPending.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            groupBoxServerPending.Name = "groupBoxServerPending";
            groupBoxServerPending.Padding = new System.Windows.Forms.Padding(4, 18, 4, 12);
            groupBoxServerPending.Radius = 10;
            groupBoxServerPending.ShadowDepth = 4;
            groupBoxServerPending.Size = new System.Drawing.Size(399, 264);
            groupBoxServerPending.TabIndex = 2;
            groupBoxServerPending.TabStop = false;
            groupBoxServerPending.Text = "Server Pending";
            //
            // checkAutoHidePendingWindow
            //
            checkAutoHidePendingWindow.AutoSize = true;
            checkAutoHidePendingWindow.BackColor = System.Drawing.Color.Transparent;
            checkAutoHidePendingWindow.Depth = 0;
            checkAutoHidePendingWindow.Location = new System.Drawing.Point(24, 44);
            checkAutoHidePendingWindow.Margin = new System.Windows.Forms.Padding(0);
            checkAutoHidePendingWindow.MouseLocation = new System.Drawing.Point(-1, -1);
            checkAutoHidePendingWindow.Name = "checkAutoHidePendingWindow";
            checkAutoHidePendingWindow.Ripple = true;
            checkAutoHidePendingWindow.Size = new System.Drawing.Size(270, 34);
            checkAutoHidePendingWindow.TabIndex = 0;
            checkAutoHidePendingWindow.Text = "Auto hide the pending window";
            checkAutoHidePendingWindow.UseVisualStyleBackColor = false;
            checkAutoHidePendingWindow.CheckedChanged += checkAutoHidePendingWindow_CheckedChanged;
            //
            // checkEnableQueueLogs
            //
            checkEnableQueueLogs.AutoSize = true;
            checkEnableQueueLogs.BackColor = System.Drawing.Color.Transparent;
            checkEnableQueueLogs.Depth = 0;
            checkEnableQueueLogs.Location = new System.Drawing.Point(24, 88);
            checkEnableQueueLogs.Margin = new System.Windows.Forms.Padding(0);
            checkEnableQueueLogs.MouseLocation = new System.Drawing.Point(-1, -1);
            checkEnableQueueLogs.Name = "checkEnableQueueLogs";
            checkEnableQueueLogs.Ripple = true;
            checkEnableQueueLogs.Size = new System.Drawing.Size(250, 34);
            checkEnableQueueLogs.TabIndex = 1;
            checkEnableQueueLogs.Text = "Enable pending queue logs";
            checkEnableQueueLogs.UseVisualStyleBackColor = false;
            checkEnableQueueLogs.CheckedChanged += checkEnableQueueLogs_CheckedChanged;
            //
            // checkEnableQueueNotification
            //
            checkEnableQueueNotification.AutoSize = true;
            checkEnableQueueNotification.BackColor = System.Drawing.Color.Transparent;
            checkEnableQueueNotification.Depth = 0;
            checkEnableQueueNotification.Location = new System.Drawing.Point(24, 132);
            checkEnableQueueNotification.Margin = new System.Windows.Forms.Padding(0);
            checkEnableQueueNotification.MouseLocation = new System.Drawing.Point(-1, -1);
            checkEnableQueueNotification.Name = "checkEnableQueueNotification";
            checkEnableQueueNotification.Ripple = true;
            checkEnableQueueNotification.Size = new System.Drawing.Size(260, 34);
            checkEnableQueueNotification.TabIndex = 2;
            checkEnableQueueNotification.Text = "Enable queue notification on ";
            checkEnableQueueNotification.UseVisualStyleBackColor = false;
            checkEnableQueueNotification.CheckedChanged += checkEnableQueueNotification_CheckedChanged;
            //
            // numQueueLeft
            //
            numQueueLeft.BackColor = System.Drawing.Color.Transparent;
            numQueueLeft.Font = new System.Drawing.Font("Segoe UI", 11F);
            numQueueLeft.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            numQueueLeft.Location = new System.Drawing.Point(310, 126);
            numQueueLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            numQueueLeft.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            numQueueLeft.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            numQueueLeft.MinimumSize = new System.Drawing.Size(112, 40);
            numQueueLeft.Name = "numQueueLeft";
            numQueueLeft.Size = new System.Drawing.Size(112, 40);
            numQueueLeft.TabIndex = 3;
            numQueueLeft.Value = new decimal(new int[] { 0, 0, 0, 0 });
            numQueueLeft.ValueChanged += numQueueLeft_ValueChanged;
            //
            // lblPeopleLeft
            //
            lblPeopleLeft.ApplyGradient = false;
            lblPeopleLeft.AutoSize = false;
            lblPeopleLeft.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            lblPeopleLeft.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblPeopleLeft.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            lblPeopleLeft.GradientAnimation = false;
            lblPeopleLeft.Location = new System.Drawing.Point(24, 172);
            lblPeopleLeft.Name = "lblPeopleLeft";
            lblPeopleLeft.Size = new System.Drawing.Size(120, 28);
            lblPeopleLeft.TabIndex = 4;
            lblPeopleLeft.Text = "people left";
            //
            // btnShowPending
            //
            btnShowPending.AutoSize = false;
            btnShowPending.Color = System.Drawing.Color.Transparent;
            btnShowPending.Location = new System.Drawing.Point(24, 208);
            btnShowPending.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnShowPending.Name = "btnShowPending";
            btnShowPending.Radius = 6;
            btnShowPending.ShadowDepth = 4F;
            btnShowPending.Size = new System.Drawing.Size(270, 36);
            btnShowPending.TabIndex = 5;
            btnShowPending.Text = "Toggle Pending Window";
            btnShowPending.UseVisualStyleBackColor = true;
            btnShowPending.Click += btnShowPending_Click;
            //
            // groupBoxSoundNotification
            //
            groupBoxSoundNotification.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBoxSoundNotification.BackColor = System.Drawing.Color.Transparent;
            groupBoxSoundNotification.Controls.Add(btnSoundSettingSetup);
            groupBoxSoundNotification.Location = new System.Drawing.Point(446, 192);
            groupBoxSoundNotification.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            groupBoxSoundNotification.Name = "groupBoxSoundNotification";
            groupBoxSoundNotification.Padding = new System.Windows.Forms.Padding(4, 20, 4, 8);
            groupBoxSoundNotification.Radius = 10;
            groupBoxSoundNotification.ShadowDepth = 4;
            groupBoxSoundNotification.Size = new System.Drawing.Size(399, 95);
            groupBoxSoundNotification.TabIndex = 3;
            groupBoxSoundNotification.TabStop = false;
            groupBoxSoundNotification.Text = "Sound Notifications";
            //
            // btnSoundSettingSetup
            //
            btnSoundSettingSetup.Color = System.Drawing.Color.Transparent;
            btnSoundSettingSetup.Location = new System.Drawing.Point(24, 40);
            btnSoundSettingSetup.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnSoundSettingSetup.Name = "btnSoundSettingSetup";
            btnSoundSettingSetup.Radius = 6;
            btnSoundSettingSetup.ShadowDepth = 4F;
            btnSoundSettingSetup.Size = new System.Drawing.Size(210, 36);
            btnSoundSettingSetup.TabIndex = 0;
            btnSoundSettingSetup.Text = "Open Settings...";
            btnSoundSettingSetup.UseVisualStyleBackColor = true;
            btnSoundSettingSetup.Click += btnSoundSettingSetup_Click;
            //
            // Main
            //
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoScroll = true;
            Controls.Add(bodyLayout);
            Font = new System.Drawing.Font("Segoe UI", 10.5F);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "Main";
            Size = new System.Drawing.Size(869, 468);
            bodyLayout.ResumeLayout(false);
            groupBoxClientSettings.ResumeLayout(false);
            groupBoxBotSettings.ResumeLayout(false);
            groupBoxServerPending.ResumeLayout(false);
            groupBoxSoundNotification.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel bodyLayout;
        private SDUI.Controls.GroupBox groupBoxClientSettings;
        private SDUI.Controls.Label label2;
        private SDUI.Controls.CheckBox checkStayConnected;
        private SDUI.Controls.GroupBox groupBoxBotSettings;
        private SDUI.Controls.Label label8;
        private SDUI.Controls.CheckBox checkBoxBotTrayMinimized;
        private SDUI.Controls.GroupBox groupBoxServerPending;
        private SDUI.Controls.CheckBox checkAutoHidePendingWindow;
        private SDUI.Controls.CheckBox checkEnableQueueLogs;
        private SDUI.Controls.CheckBox checkEnableQueueNotification;
        private SDUI.Controls.NumUpDown numQueueLeft;
        private SDUI.Controls.Label lblPeopleLeft;
        private SDUI.Controls.Button btnShowPending;
        private SDUI.Controls.GroupBox groupBoxSoundNotification;
        private SDUI.Controls.Button btnSoundSettingSetup;
    }
}
