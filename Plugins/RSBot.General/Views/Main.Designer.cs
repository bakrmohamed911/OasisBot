namespace RSBot.General.Views
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
            label1 = new SDUI.Controls.Label();
            lblVersion = new SDUI.Controls.Label();
            comboBoxClientType = new SDUI.Controls.ComboBox();
            bodyScrollPanel = new System.Windows.Forms.Panel();
            bodyLayout = new System.Windows.Forms.TableLayoutPanel();
            leftColumnPanel = new System.Windows.Forms.TableLayoutPanel();
            groupBox2 = new SDUI.Controls.GroupBox();
            btnClientHideShow = new SDUI.Controls.Button();
            btnStartClient = new SDUI.Controls.Button();
            btnStartClientless = new SDUI.Controls.Button();
            btnGoClientless = new SDUI.Controls.Button();
            groupBoxResurrection = new SDUI.Controls.GroupBox();
            lblResSkill = new SDUI.Controls.Label();
            comboResurrectionSkill = new SDUI.Controls.ComboBox();
            checkAcceptResurrection = new SDUI.Controls.CheckBox();
            checkResurrectParty = new SDUI.Controls.CheckBox();
            lblResDelay = new SDUI.Controls.Label();
            numResDelay = new SDUI.Controls.NumUpDown();
            lblResDelaySec = new SDUI.Controls.Label();
            lblResRadius = new SDUI.Controls.Label();
            numResRadius = new SDUI.Controls.NumUpDown();
            groupBox1 = new SDUI.Controls.GroupBox();
            panel3 = new System.Windows.Forms.Panel();
            lblWaitAfterDC = new SDUI.Controls.Label();
            numWaitAfterDC = new SDUI.Controls.NumUpDown();
            checkWaitAfterDC = new SDUI.Controls.CheckBox();
            separator1 = new SDUI.Controls.Separator();
            radioAutoSelectHigher = new SDUI.Controls.Radio();
            radioAutoSelectFirst = new SDUI.Controls.Radio();
            checkCharAutoSelect = new SDUI.Controls.CheckBox();
            checkHideClient = new SDUI.Controls.CheckBox();
            lblLoginDelaySeconds = new SDUI.Controls.Label();
            numLoginDelay = new SDUI.Controls.NumUpDown();
            checkEnableLoginDelay = new SDUI.Controls.CheckBox();
            checkStartBot = new SDUI.Controls.CheckBox();
            checkUseReturnScroll = new SDUI.Controls.CheckBox();
            captchaPanel = new System.Windows.Forms.Panel();
            separator2 = new SDUI.Controls.Separator();
            label6 = new SDUI.Controls.Label();
            label5 = new SDUI.Controls.Label();
            txtStaticCaptcha = new SDUI.Controls.TextBox();
            checkEnableStaticCaptcha = new SDUI.Controls.CheckBox();
            autoLoginTopPanel = new System.Windows.Forms.Panel();
            comboAccounts = new SDUI.Controls.ComboBox();
            label7 = new SDUI.Controls.Label();
            label4 = new SDUI.Controls.Label();
            checkEnableAutoLogin = new SDUI.Controls.CheckBox();
            comboCharacter = new SDUI.Controls.ComboBox();
            btnAutoLoginSettings = new SDUI.Controls.Button();
            groupBoxMastery = new SDUI.Controls.GroupBox();
            checkLearnMastery = new SDUI.Controls.CheckBox();
            btnMasterySelect = new SDUI.Controls.Button();
            lblMasteryGap = new SDUI.Controls.Label();
            numMasteryGap = new SDUI.Controls.NumUpDown();
            checkLearnMasteryBotStopped = new SDUI.Controls.CheckBox();
            groupBoxStatPoints = new SDUI.Controls.GroupBox();
            checkIncInt = new SDUI.Controls.CheckBox();
            numIncInt = new SDUI.Controls.NumUpDown();
            checkIncStr = new SDUI.Controls.CheckBox();
            numIncStr = new SDUI.Controls.NumUpDown();
            checkIncBotStopped = new SDUI.Controls.CheckBox();
            buttonRun = new SDUI.Controls.Button();
            btnBrowseSilkroadPath = new SDUI.Controls.Button();
            txtSilkroadPath = new SDUI.Controls.TextBox();
            bodyScrollPanel.SuspendLayout();
            bodyLayout.SuspendLayout();
            leftColumnPanel.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBoxResurrection.SuspendLayout();
            groupBox1.SuspendLayout();
            panel3.SuspendLayout();
            captchaPanel.SuspendLayout();
            autoLoginTopPanel.SuspendLayout();
            groupBoxMastery.SuspendLayout();
            groupBoxStatPoints.SuspendLayout();
            SuspendLayout();
            //
            // label1
            //
            label1.ApplyGradient = false;
            label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            label1.AutoSize = false;
            label1.Font = new System.Drawing.Font("Segoe UI", 11F);
            label1.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            label1.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            label1.GradientAnimation = false;
            label1.Location = new System.Drawing.Point(24, 18);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(260, 25);
            label1.TabIndex = 0;
            label1.Text = "Silkroad executable path:";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblVersion
            //
            lblVersion.ApplyGradient = false;
            lblVersion.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblVersion.Font = new System.Drawing.Font("Segoe UI", 11F);
            lblVersion.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblVersion.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            lblVersion.GradientAnimation = false;
            lblVersion.Location = new System.Drawing.Point(775, 18);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new System.Drawing.Size(80, 25);
            lblVersion.TabIndex = 3;
            lblVersion.Text = "v1.000";
            lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // comboBoxClientType
            //
            comboBoxClientType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            comboBoxClientType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            comboBoxClientType.DropDownHeight = 120;
            comboBoxClientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxClientType.FormattingEnabled = true;
            comboBoxClientType.IntegralHeight = false;
            comboBoxClientType.ItemHeight = 22;
            comboBoxClientType.Location = new System.Drawing.Point(629, 49);
            comboBoxClientType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            comboBoxClientType.Name = "comboBoxClientType";
            comboBoxClientType.Radius = 5;
            comboBoxClientType.ShadowDepth = 4F;
            comboBoxClientType.Size = new System.Drawing.Size(160, 34);
            comboBoxClientType.TabIndex = 18;
            comboBoxClientType.SelectedIndexChanged += comboBoxClientType_SelectedIndexChanged;
            //
            // btnBrowseSilkroadPath
            //
            btnBrowseSilkroadPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnBrowseSilkroadPath.Color = System.Drawing.Color.Transparent;
            btnBrowseSilkroadPath.Location = new System.Drawing.Point(799, 48);
            btnBrowseSilkroadPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnBrowseSilkroadPath.Name = "btnBrowseSilkroadPath";
            btnBrowseSilkroadPath.Radius = 6;
            btnBrowseSilkroadPath.ShadowDepth = 4F;
            btnBrowseSilkroadPath.Size = new System.Drawing.Size(46, 36);
            btnBrowseSilkroadPath.TabIndex = 2;
            btnBrowseSilkroadPath.Text = "...";
            btnBrowseSilkroadPath.UseVisualStyleBackColor = true;
            btnBrowseSilkroadPath.Click += btnBrowseSilkroadPath_Click;
            //
            // txtSilkroadPath
            //
            txtSilkroadPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtSilkroadPath.Location = new System.Drawing.Point(24, 48);
            txtSilkroadPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtSilkroadPath.MaxLength = 32767;
            txtSilkroadPath.MultiLine = false;
            txtSilkroadPath.Name = "txtSilkroadPath";
            txtSilkroadPath.PassFocusShow = false;
            txtSilkroadPath.Radius = 3;
            txtSilkroadPath.Size = new System.Drawing.Size(595, 36);
            txtSilkroadPath.TabIndex = 1;
            txtSilkroadPath.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            txtSilkroadPath.UseSystemPasswordChar = false;
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
            bodyLayout.Controls.Add(leftColumnPanel, 0, 0);
            bodyLayout.Controls.Add(groupBoxResurrection, 1, 0);
            bodyLayout.Controls.Add(groupBoxMastery, 1, 1);
            bodyLayout.Controls.Add(groupBoxStatPoints, 1, 2);
            bodyLayout.Location = new System.Drawing.Point(0, 0);
            bodyLayout.Name = "bodyLayout";
            bodyLayout.Padding = new System.Windows.Forms.Padding(24, 0, 24, 20);
            bodyLayout.RowCount = 3;
            bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            bodyLayout.Size = new System.Drawing.Size(865, 476);
            bodyLayout.TabIndex = 30;
            bodyLayout.SetRowSpan(leftColumnPanel, 3);
            //
            // leftColumnPanel
            //
            leftColumnPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            leftColumnPanel.AutoSize = true;
            leftColumnPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            leftColumnPanel.BackColor = System.Drawing.Color.Transparent;
            leftColumnPanel.ColumnCount = 1;
            leftColumnPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            leftColumnPanel.Controls.Add(groupBox2, 0, 0);
            leftColumnPanel.Controls.Add(groupBox1, 0, 1);
            leftColumnPanel.Location = new System.Drawing.Point(24, 3);
            leftColumnPanel.Margin = new System.Windows.Forms.Padding(0, 3, 12, 18);
            leftColumnPanel.Name = "leftColumnPanel";
            leftColumnPanel.RowCount = 2;
            leftColumnPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            leftColumnPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            leftColumnPanel.Size = new System.Drawing.Size(398, 500);
            leftColumnPanel.TabIndex = 29;
            //
            // bodyScrollPanel
            //
            bodyScrollPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            bodyScrollPanel.AutoScroll = true;
            bodyScrollPanel.BackColor = System.Drawing.Color.Transparent;
            bodyScrollPanel.Controls.Add(bodyLayout);
            bodyScrollPanel.Location = new System.Drawing.Point(0, 100);
            bodyScrollPanel.Name = "bodyScrollPanel";
            bodyScrollPanel.Size = new System.Drawing.Size(869, 476);
            bodyScrollPanel.TabIndex = 31;
            //
            // groupBox2
            //
            groupBox2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox2.BackColor = System.Drawing.Color.Transparent;
            groupBox2.Controls.Add(btnClientHideShow);
            groupBox2.Controls.Add(btnStartClient);
            groupBox2.Controls.Add(btnStartClientless);
            groupBox2.Controls.Add(btnGoClientless);
            groupBox2.Location = new System.Drawing.Point(0, 0);
            groupBox2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 18);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(4, 18, 4, 8);
            groupBox2.Radius = 10;
            groupBox2.ShadowDepth = 4;
            groupBox2.Size = new System.Drawing.Size(398, 140);
            groupBox2.TabIndex = 15;
            groupBox2.TabStop = false;
            groupBox2.Text = "Start game";
            //
            // btnClientHideShow
            //
            btnClientHideShow.Color = System.Drawing.Color.Transparent;
            btnClientHideShow.Enabled = false;
            btnClientHideShow.Location = new System.Drawing.Point(24, 96);
            btnClientHideShow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnClientHideShow.Name = "btnClientHideShow";
            btnClientHideShow.Radius = 6;
            btnClientHideShow.ShadowDepth = 4F;
            btnClientHideShow.Size = new System.Drawing.Size(180, 36);
            btnClientHideShow.TabIndex = 19;
            btnClientHideShow.Text = "Client Visibility";
            btnClientHideShow.UseVisualStyleBackColor = true;
            btnClientHideShow.Click += btnClientHideShow_Click;
            //
            // btnStartClient
            //
            btnStartClient.Color = System.Drawing.Color.Transparent;
            btnStartClient.Location = new System.Drawing.Point(24, 48);
            btnStartClient.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnStartClient.Name = "btnStartClient";
            btnStartClient.Radius = 6;
            btnStartClient.ShadowDepth = 4F;
            btnStartClient.Size = new System.Drawing.Size(180, 36);
            btnStartClient.TabIndex = 16;
            btnStartClient.Text = "Start Client";
            btnStartClient.UseVisualStyleBackColor = true;
            btnStartClient.Click += btnStartClient_Click;
            //
            // btnStartClientless
            //
            btnStartClientless.Color = System.Drawing.Color.Transparent;
            btnStartClientless.Location = new System.Drawing.Point(214, 48);
            btnStartClientless.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnStartClientless.Name = "btnStartClientless";
            btnStartClientless.Radius = 6;
            btnStartClientless.ShadowDepth = 4F;
            btnStartClientless.Size = new System.Drawing.Size(180, 36);
            btnStartClientless.TabIndex = 18;
            btnStartClientless.Text = "Start Clientless";
            btnStartClientless.UseVisualStyleBackColor = false;
            btnStartClientless.Click += btnStartClientless_Click;
            //
            // btnGoClientless
            //
            btnGoClientless.Color = System.Drawing.Color.Transparent;
            btnGoClientless.Enabled = false;
            btnGoClientless.Location = new System.Drawing.Point(214, 96);
            btnGoClientless.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnGoClientless.Name = "btnGoClientless";
            btnGoClientless.Radius = 6;
            btnGoClientless.ShadowDepth = 4F;
            btnGoClientless.Size = new System.Drawing.Size(180, 36);
            btnGoClientless.TabIndex = 17;
            btnGoClientless.Text = "Go Clientless";
            btnGoClientless.UseVisualStyleBackColor = true;
            btnGoClientless.Click += btnGoClientless_Click;
            //
            // groupBoxResurrection
            //
            groupBoxResurrection.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBoxResurrection.BackColor = System.Drawing.Color.Transparent;
            groupBoxResurrection.Controls.Add(lblResSkill);
            groupBoxResurrection.Controls.Add(comboResurrectionSkill);
            groupBoxResurrection.Controls.Add(checkAcceptResurrection);
            groupBoxResurrection.Controls.Add(checkResurrectParty);
            groupBoxResurrection.Controls.Add(lblResDelay);
            groupBoxResurrection.Controls.Add(numResDelay);
            groupBoxResurrection.Controls.Add(lblResDelaySec);
            groupBoxResurrection.Controls.Add(lblResRadius);
            groupBoxResurrection.Controls.Add(numResRadius);
            groupBoxResurrection.Location = new System.Drawing.Point(446, 3);
            groupBoxResurrection.Margin = new System.Windows.Forms.Padding(12, 3, 0, 18);
            groupBoxResurrection.Name = "groupBoxResurrection";
            groupBoxResurrection.Padding = new System.Windows.Forms.Padding(4, 18, 4, 8);
            groupBoxResurrection.Radius = 10;
            groupBoxResurrection.ShadowDepth = 4;
            groupBoxResurrection.Size = new System.Drawing.Size(399, 260);
            groupBoxResurrection.TabIndex = 16;
            groupBoxResurrection.TabStop = false;
            groupBoxResurrection.Text = "Automated resurrection";
            //
            // lblResSkill
            //
            lblResSkill.ApplyGradient = false;
            lblResSkill.AutoSize = false;
            lblResSkill.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblResSkill.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            lblResSkill.GradientAnimation = false;
            lblResSkill.Location = new System.Drawing.Point(24, 48);
            lblResSkill.Name = "lblResSkill";
            lblResSkill.Size = new System.Drawing.Size(80, 28);
            lblResSkill.TabIndex = 0;
            lblResSkill.Text = "Res. skill:";
            //
            // comboResurrectionSkill
            //
            comboResurrectionSkill.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            comboResurrectionSkill.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            comboResurrectionSkill.DropDownHeight = 120;
            comboResurrectionSkill.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboResurrectionSkill.FormattingEnabled = true;
            comboResurrectionSkill.IntegralHeight = false;
            comboResurrectionSkill.ItemHeight = 22;
            comboResurrectionSkill.Location = new System.Drawing.Point(108, 44);
            comboResurrectionSkill.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            comboResurrectionSkill.Name = "comboResurrectionSkill";
            comboResurrectionSkill.Radius = 5;
            comboResurrectionSkill.ShadowDepth = 4F;
            comboResurrectionSkill.Size = new System.Drawing.Size(267, 32);
            comboResurrectionSkill.TabIndex = 1;
            comboResurrectionSkill.SelectedIndexChanged += comboResurrectionSkill_SelectedIndexChanged;
            //
            // checkAcceptResurrection
            //
            checkAcceptResurrection.AutoSize = true;
            checkAcceptResurrection.BackColor = System.Drawing.Color.Transparent;
            checkAcceptResurrection.Checked = true;
            checkAcceptResurrection.CheckState = System.Windows.Forms.CheckState.Checked;
            checkAcceptResurrection.Depth = 0;
            checkAcceptResurrection.Location = new System.Drawing.Point(20, 84);
            checkAcceptResurrection.Margin = new System.Windows.Forms.Padding(0);
            checkAcceptResurrection.MouseLocation = new System.Drawing.Point(-1, -1);
            checkAcceptResurrection.Name = "checkAcceptResurrection";
            checkAcceptResurrection.Ripple = true;
            checkAcceptResurrection.Size = new System.Drawing.Size(220, 34);
            checkAcceptResurrection.TabIndex = 2;
            checkAcceptResurrection.Text = "Auto accept resurrection";
            checkAcceptResurrection.UseVisualStyleBackColor = false;
            checkAcceptResurrection.CheckedChanged += checkAcceptResurrection_CheckedChanged;
            //
            // checkResurrectParty
            //
            checkResurrectParty.AutoSize = true;
            checkResurrectParty.BackColor = System.Drawing.Color.Transparent;
            checkResurrectParty.Depth = 0;
            checkResurrectParty.Location = new System.Drawing.Point(20, 120);
            checkResurrectParty.Margin = new System.Windows.Forms.Padding(0);
            checkResurrectParty.MouseLocation = new System.Drawing.Point(-1, -1);
            checkResurrectParty.Name = "checkResurrectParty";
            checkResurrectParty.Ripple = true;
            checkResurrectParty.Size = new System.Drawing.Size(260, 34);
            checkResurrectParty.TabIndex = 3;
            checkResurrectParty.Text = "Auto resurrect party members";
            checkResurrectParty.UseVisualStyleBackColor = false;
            checkResurrectParty.CheckedChanged += checkResurrectParty_CheckedChanged;
            //
            // lblResDelay
            //
            lblResDelay.ApplyGradient = false;
            lblResDelay.AutoSize = false;
            lblResDelay.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            lblResDelay.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblResDelay.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            lblResDelay.GradientAnimation = false;
            lblResDelay.Location = new System.Drawing.Point(24, 166);
            lblResDelay.Name = "lblResDelay";
            lblResDelay.Size = new System.Drawing.Size(160, 24);
            lblResDelay.TabIndex = 4;
            lblResDelay.Text = "Resurrect cooldown";
            //
            // numResDelay
            //
            numResDelay.BackColor = System.Drawing.Color.Transparent;
            numResDelay.Font = new System.Drawing.Font("Segoe UI", 11F);
            numResDelay.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            numResDelay.Location = new System.Drawing.Point(190, 158);
            numResDelay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            numResDelay.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            numResDelay.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numResDelay.MinimumSize = new System.Drawing.Size(90, 36);
            numResDelay.Name = "numResDelay";
            numResDelay.Size = new System.Drawing.Size(90, 36);
            numResDelay.TabIndex = 5;
            numResDelay.Value = new decimal(new int[] { 120, 0, 0, 0 });
            numResDelay.ValueChanged += numResDelay_ValueChanged;
            //
            // lblResDelaySec
            //
            lblResDelaySec.ApplyGradient = false;
            lblResDelaySec.AutoSize = false;
            lblResDelaySec.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            lblResDelaySec.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblResDelaySec.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            lblResDelaySec.GradientAnimation = false;
            lblResDelaySec.Location = new System.Drawing.Point(286, 166);
            lblResDelaySec.Name = "lblResDelaySec";
            lblResDelaySec.Size = new System.Drawing.Size(40, 24);
            lblResDelaySec.TabIndex = 6;
            lblResDelaySec.Text = "sec";
            //
            // lblResRadius
            //
            lblResRadius.ApplyGradient = false;
            lblResRadius.AutoSize = false;
            lblResRadius.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            lblResRadius.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblResRadius.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            lblResRadius.GradientAnimation = false;
            lblResRadius.Location = new System.Drawing.Point(24, 210);
            lblResRadius.Name = "lblResRadius";
            lblResRadius.Size = new System.Drawing.Size(160, 24);
            lblResRadius.TabIndex = 7;
            lblResRadius.Text = "Resurrection radius";
            //
            // numResRadius
            //
            numResRadius.BackColor = System.Drawing.Color.Transparent;
            numResRadius.Font = new System.Drawing.Font("Segoe UI", 11F);
            numResRadius.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            numResRadius.Location = new System.Drawing.Point(190, 202);
            numResRadius.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            numResRadius.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            numResRadius.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numResRadius.MinimumSize = new System.Drawing.Size(90, 36);
            numResRadius.Name = "numResRadius";
            numResRadius.Size = new System.Drawing.Size(90, 36);
            numResRadius.TabIndex = 8;
            numResRadius.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numResRadius.ValueChanged += numResRadius_ValueChanged;
            //
            // groupBox1
            //
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.AutoSize = true;
            groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            groupBox1.BackColor = System.Drawing.Color.Transparent;
            groupBox1.Controls.Add(panel3);
            groupBox1.Controls.Add(captchaPanel);
            groupBox1.Controls.Add(autoLoginTopPanel);
            groupBox1.Location = new System.Drawing.Point(0, 158);
            groupBox1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 18, 4, 8);
            groupBox1.Radius = 10;
            groupBox1.ShadowDepth = 4;
            groupBox1.Size = new System.Drawing.Size(398, 464);
            groupBox1.TabIndex = 14;
            groupBox1.TabStop = false;
            groupBox1.Text = "Automated login";
            //
            // panel3
            //
            panel3.Controls.Add(lblWaitAfterDC);
            panel3.Controls.Add(numWaitAfterDC);
            panel3.Controls.Add(checkWaitAfterDC);
            panel3.Controls.Add(separator1);
            panel3.Controls.Add(radioAutoSelectHigher);
            panel3.Controls.Add(radioAutoSelectFirst);
            panel3.Controls.Add(checkCharAutoSelect);
            panel3.Controls.Add(checkHideClient);
            panel3.Controls.Add(lblLoginDelaySeconds);
            panel3.Controls.Add(numLoginDelay);
            panel3.Controls.Add(checkEnableLoginDelay);
            panel3.Controls.Add(checkStartBot);
            panel3.Controls.Add(checkUseReturnScroll);
            panel3.Dock = System.Windows.Forms.DockStyle.Top;
            panel3.Location = new System.Drawing.Point(4, 303);
            panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(390, 200);
            panel3.TabIndex = 1;
            //
            // lblWaitAfterDC
            //
            lblWaitAfterDC.ApplyGradient = false;
            lblWaitAfterDC.AutoSize = false;
            lblWaitAfterDC.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            lblWaitAfterDC.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblWaitAfterDC.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            lblWaitAfterDC.GradientAnimation = false;
            lblWaitAfterDC.Location = new System.Drawing.Point(398, 56);
            lblWaitAfterDC.Name = "lblWaitAfterDC";
            lblWaitAfterDC.Size = new System.Drawing.Size(80, 24);
            lblWaitAfterDC.TabIndex = 39;
            lblWaitAfterDC.Text = "minutes";
            //
            // numWaitAfterDC
            //
            numWaitAfterDC.BackColor = System.Drawing.Color.Transparent;
            numWaitAfterDC.Font = new System.Drawing.Font("Segoe UI", 11F);
            numWaitAfterDC.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            numWaitAfterDC.Location = new System.Drawing.Point(280, 50);
            numWaitAfterDC.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            numWaitAfterDC.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numWaitAfterDC.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numWaitAfterDC.MinimumSize = new System.Drawing.Size(112, 36);
            numWaitAfterDC.Name = "numWaitAfterDC";
            numWaitAfterDC.Size = new System.Drawing.Size(112, 36);
            numWaitAfterDC.TabIndex = 41;
            numWaitAfterDC.Value = new decimal(new int[] { 3, 0, 0, 0 });
            numWaitAfterDC.ValueChanged += numWaitAfterDC_ValueChanged;
            //
            // checkWaitAfterDC
            //
            checkWaitAfterDC.AutoSize = true;
            checkWaitAfterDC.BackColor = System.Drawing.Color.Transparent;
            checkWaitAfterDC.Depth = 0;
            checkWaitAfterDC.Location = new System.Drawing.Point(75, 51);
            checkWaitAfterDC.Margin = new System.Windows.Forms.Padding(0);
            checkWaitAfterDC.MouseLocation = new System.Drawing.Point(-1, -1);
            checkWaitAfterDC.Name = "checkWaitAfterDC";
            checkWaitAfterDC.Ripple = true;
            checkWaitAfterDC.Size = new System.Drawing.Size(124, 34);
            checkWaitAfterDC.TabIndex = 40;
            checkWaitAfterDC.Text = "Wait after DC";
            checkWaitAfterDC.UseVisualStyleBackColor = false;
            checkWaitAfterDC.CheckedChanged += checkWaitAfterDC_CheckedChanged;
            //
            // separator1
            //
            separator1.Dock = System.Windows.Forms.DockStyle.Top;
            separator1.IsVertical = false;
            separator1.Location = new System.Drawing.Point(0, 0);
            separator1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            separator1.Name = "separator1";
            separator1.Size = new System.Drawing.Size(390, 3);
            separator1.TabIndex = 19;
            //
            // radioAutoSelectHigher
            //
            radioAutoSelectHigher.AutoSize = true;
            radioAutoSelectHigher.Enabled = false;
            radioAutoSelectHigher.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            radioAutoSelectHigher.Location = new System.Drawing.Point(280, 152);
            radioAutoSelectHigher.Margin = new System.Windows.Forms.Padding(0);
            radioAutoSelectHigher.Name = "radioAutoSelectHigher";
            radioAutoSelectHigher.Ripple = true;
            radioAutoSelectHigher.Size = new System.Drawing.Size(180, 34);
            radioAutoSelectHigher.TabIndex = 38;
            radioAutoSelectHigher.Text = "Auto Select (Higher)";
            radioAutoSelectHigher.UseVisualStyleBackColor = true;
            radioAutoSelectHigher.CheckedChanged += radioAutoSelectHigher_CheckedChanged;
            //
            // radioAutoSelectFirst
            //
            radioAutoSelectFirst.AutoSize = true;
            radioAutoSelectFirst.Checked = true;
            radioAutoSelectFirst.Enabled = false;
            radioAutoSelectFirst.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            radioAutoSelectFirst.Location = new System.Drawing.Point(75, 152);
            radioAutoSelectFirst.Margin = new System.Windows.Forms.Padding(0);
            radioAutoSelectFirst.Name = "radioAutoSelectFirst";
            radioAutoSelectFirst.Ripple = true;
            radioAutoSelectFirst.Size = new System.Drawing.Size(160, 34);
            radioAutoSelectFirst.TabIndex = 37;
            radioAutoSelectFirst.TabStop = true;
            radioAutoSelectFirst.Text = "Auto Select (First)";
            radioAutoSelectFirst.UseVisualStyleBackColor = true;
            radioAutoSelectFirst.CheckedChanged += radioAutoSelectFirst_CheckedChanged;
            //
            // checkCharAutoSelect
            //
            checkCharAutoSelect.AutoSize = true;
            checkCharAutoSelect.BackColor = System.Drawing.Color.Transparent;
            checkCharAutoSelect.Depth = 0;
            checkCharAutoSelect.Enabled = false;
            checkCharAutoSelect.Location = new System.Drawing.Point(75, 118);
            checkCharAutoSelect.Margin = new System.Windows.Forms.Padding(0);
            checkCharAutoSelect.MouseLocation = new System.Drawing.Point(-1, -1);
            checkCharAutoSelect.Name = "checkCharAutoSelect";
            checkCharAutoSelect.Ripple = true;
            checkCharAutoSelect.Size = new System.Drawing.Size(155, 34);
            checkCharAutoSelect.TabIndex = 36;
            checkCharAutoSelect.Text = "Auto Char Select";
            checkCharAutoSelect.UseVisualStyleBackColor = false;
            checkCharAutoSelect.CheckedChanged += checkCharAutoSelect_CheckedChanged;
            //
            // checkHideClient
            //
            checkHideClient.AutoSize = true;
            checkHideClient.BackColor = System.Drawing.Color.Transparent;
            checkHideClient.Depth = 0;
            checkHideClient.Location = new System.Drawing.Point(280, 118);
            checkHideClient.Margin = new System.Windows.Forms.Padding(0);
            checkHideClient.MouseLocation = new System.Drawing.Point(-1, -1);
            checkHideClient.Name = "checkHideClient";
            checkHideClient.Ripple = true;
            checkHideClient.Size = new System.Drawing.Size(155, 34);
            checkHideClient.TabIndex = 31;
            checkHideClient.Text = "Auto Hide Client";
            checkHideClient.UseVisualStyleBackColor = false;
            checkHideClient.CheckedChanged += checkHideClient_CheckedChanged;
            //
            // lblLoginDelaySeconds
            //
            lblLoginDelaySeconds.ApplyGradient = false;
            lblLoginDelaySeconds.AutoSize = false;
            lblLoginDelaySeconds.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            lblLoginDelaySeconds.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblLoginDelaySeconds.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            lblLoginDelaySeconds.GradientAnimation = false;
            lblLoginDelaySeconds.Location = new System.Drawing.Point(398, 20);
            lblLoginDelaySeconds.Name = "lblLoginDelaySeconds";
            lblLoginDelaySeconds.Size = new System.Drawing.Size(80, 24);
            lblLoginDelaySeconds.TabIndex = 22;
            lblLoginDelaySeconds.Text = "seconds";
            //
            // numLoginDelay
            //
            numLoginDelay.BackColor = System.Drawing.Color.Transparent;
            numLoginDelay.Font = new System.Drawing.Font("Segoe UI", 11F);
            numLoginDelay.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            numLoginDelay.Location = new System.Drawing.Point(280, 14);
            numLoginDelay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            numLoginDelay.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            numLoginDelay.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numLoginDelay.MinimumSize = new System.Drawing.Size(112, 36);
            numLoginDelay.Name = "numLoginDelay";
            numLoginDelay.Size = new System.Drawing.Size(112, 36);
            numLoginDelay.TabIndex = 30;
            numLoginDelay.Value = new decimal(new int[] { 3, 0, 0, 0 });
            numLoginDelay.ValueChanged += numLoginDelay_ValueChanged;
            //
            // checkEnableLoginDelay
            //
            checkEnableLoginDelay.AutoSize = true;
            checkEnableLoginDelay.BackColor = System.Drawing.Color.Transparent;
            checkEnableLoginDelay.Depth = 0;
            checkEnableLoginDelay.Location = new System.Drawing.Point(75, 15);
            checkEnableLoginDelay.Margin = new System.Windows.Forms.Padding(0);
            checkEnableLoginDelay.MouseLocation = new System.Drawing.Point(-1, -1);
            checkEnableLoginDelay.Name = "checkEnableLoginDelay";
            checkEnableLoginDelay.Ripple = true;
            checkEnableLoginDelay.Size = new System.Drawing.Size(178, 34);
            checkEnableLoginDelay.TabIndex = 25;
            checkEnableLoginDelay.Text = "Enable login delay";
            checkEnableLoginDelay.UseVisualStyleBackColor = false;
            checkEnableLoginDelay.CheckedChanged += checkEnableLoginDelay_CheckedChanged;
            //
            // checkStartBot
            //
            checkStartBot.AutoSize = true;
            checkStartBot.BackColor = System.Drawing.Color.Transparent;
            checkStartBot.Depth = 0;
            checkStartBot.Location = new System.Drawing.Point(75, 86);
            checkStartBot.Margin = new System.Windows.Forms.Padding(0);
            checkStartBot.MouseLocation = new System.Drawing.Point(-1, -1);
            checkStartBot.Name = "checkStartBot";
            checkStartBot.Ripple = true;
            checkStartBot.Size = new System.Drawing.Size(145, 34);
            checkStartBot.TabIndex = 24;
            checkStartBot.Text = "Auto start bot";
            checkStartBot.UseVisualStyleBackColor = false;
            checkStartBot.CheckedChanged += checkAutoStartBot_CheckedChanged;
            //
            // checkUseReturnScroll
            //
            checkUseReturnScroll.AutoSize = true;
            checkUseReturnScroll.BackColor = System.Drawing.Color.Transparent;
            checkUseReturnScroll.Depth = 0;
            checkUseReturnScroll.Location = new System.Drawing.Point(280, 86);
            checkUseReturnScroll.Margin = new System.Windows.Forms.Padding(0);
            checkUseReturnScroll.MouseLocation = new System.Drawing.Point(-1, -1);
            checkUseReturnScroll.Name = "checkUseReturnScroll";
            checkUseReturnScroll.Ripple = true;
            checkUseReturnScroll.Size = new System.Drawing.Size(160, 34);
            checkUseReturnScroll.TabIndex = 16;
            checkUseReturnScroll.Text = "Use return scroll";
            checkUseReturnScroll.UseVisualStyleBackColor = false;
            checkUseReturnScroll.CheckedChanged += checkUseReturnScroll_CheckedChanged;
            //
            // captchaPanel
            //
            captchaPanel.Controls.Add(separator2);
            captchaPanel.Controls.Add(label6);
            captchaPanel.Controls.Add(label5);
            captchaPanel.Controls.Add(txtStaticCaptcha);
            captchaPanel.Controls.Add(checkEnableStaticCaptcha);
            captchaPanel.Dock = System.Windows.Forms.DockStyle.Top;
            captchaPanel.Location = new System.Drawing.Point(4, 168);
            captchaPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            captchaPanel.Name = "captchaPanel";
            captchaPanel.Size = new System.Drawing.Size(390, 110);
            captchaPanel.TabIndex = 0;
            captchaPanel.Visible = false;
            //
            // separator2
            //
            separator2.Dock = System.Windows.Forms.DockStyle.Top;
            separator2.IsVertical = false;
            separator2.Location = new System.Drawing.Point(0, 0);
            separator2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            separator2.Name = "separator2";
            separator2.Size = new System.Drawing.Size(390, 3);
            separator2.TabIndex = 29;
            //
            // label6
            //
            label6.ApplyGradient = false;
            label6.AutoSize = false;
            label6.Enabled = false;
            label6.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            label6.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            label6.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            label6.GradientAnimation = false;
            label6.Location = new System.Drawing.Point(90, 82);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(420, 24);
            label6.TabIndex = 21;
            label6.Text = "Please use this only if the captcha never changes.";
            //
            // label5
            //
            label5.ApplyGradient = false;
            label5.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            label5.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            label5.GradientAnimation = false;
            label5.Location = new System.Drawing.Point(12, 14);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(72, 28);
            label5.TabIndex = 18;
            label5.Text = "Captcha:";
            //
            // txtStaticCaptcha
            //
            txtStaticCaptcha.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtStaticCaptcha.Location = new System.Drawing.Point(90, 12);
            txtStaticCaptcha.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtStaticCaptcha.MaxLength = 32767;
            txtStaticCaptcha.MultiLine = false;
            txtStaticCaptcha.Name = "txtStaticCaptcha";
            txtStaticCaptcha.PassFocusShow = false;
            txtStaticCaptcha.Radius = 3;
            txtStaticCaptcha.Size = new System.Drawing.Size(288, 32);
            txtStaticCaptcha.TabIndex = 3;
            txtStaticCaptcha.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            txtStaticCaptcha.UseSystemPasswordChar = false;
            txtStaticCaptcha.TextChanged += txtStaticCaptcha_TextChanged;
            //
            // checkEnableStaticCaptcha
            //
            checkEnableStaticCaptcha.AutoSize = true;
            checkEnableStaticCaptcha.BackColor = System.Drawing.Color.Transparent;
            checkEnableStaticCaptcha.Depth = 0;
            checkEnableStaticCaptcha.Location = new System.Drawing.Point(90, 46);
            checkEnableStaticCaptcha.Margin = new System.Windows.Forms.Padding(0);
            checkEnableStaticCaptcha.MouseLocation = new System.Drawing.Point(-1, -1);
            checkEnableStaticCaptcha.Name = "checkEnableStaticCaptcha";
            checkEnableStaticCaptcha.Ripple = true;
            checkEnableStaticCaptcha.Size = new System.Drawing.Size(230, 34);
            checkEnableStaticCaptcha.TabIndex = 4;
            checkEnableStaticCaptcha.Text = "Enable static captcha solve";
            checkEnableStaticCaptcha.UseVisualStyleBackColor = false;
            checkEnableStaticCaptcha.CheckedChanged += checkEnableStaticCaptcha_CheckedChanged;
            //
            // autoLoginTopPanel
            //
            autoLoginTopPanel.BackColor = System.Drawing.Color.Transparent;
            autoLoginTopPanel.Controls.Add(comboAccounts);
            autoLoginTopPanel.Controls.Add(label7);
            autoLoginTopPanel.Controls.Add(label4);
            autoLoginTopPanel.Controls.Add(checkEnableAutoLogin);
            autoLoginTopPanel.Controls.Add(comboCharacter);
            autoLoginTopPanel.Controls.Add(btnAutoLoginSettings);
            autoLoginTopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            autoLoginTopPanel.Location = new System.Drawing.Point(4, 18);
            autoLoginTopPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            autoLoginTopPanel.Name = "autoLoginTopPanel";
            autoLoginTopPanel.Size = new System.Drawing.Size(390, 128);
            autoLoginTopPanel.TabIndex = 28;
            //
            // comboAccounts
            //
            comboAccounts.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            comboAccounts.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            comboAccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboAccounts.FormattingEnabled = true;
            comboAccounts.ItemHeight = 22;
            comboAccounts.Location = new System.Drawing.Point(90, 12);
            comboAccounts.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            comboAccounts.Name = "comboAccounts";
            comboAccounts.Radius = 5;
            comboAccounts.ShadowDepth = 4F;
            comboAccounts.Size = new System.Drawing.Size(288, 32);
            comboAccounts.TabIndex = 0;
            comboAccounts.SelectedIndexChanged += comboAccounts_SelectedIndexChanged;
            //
            // label7
            //
            label7.ApplyGradient = false;
            label7.AutoSize = false;
            label7.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            label7.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            label7.GradientAnimation = false;
            label7.Location = new System.Drawing.Point(20, 50);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(70, 24);
            label7.TabIndex = 23;
            label7.Text = "Player:";
            //
            // label4
            //
            label4.ApplyGradient = false;
            label4.AutoSize = false;
            label4.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            label4.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            label4.GradientAnimation = false;
            label4.Location = new System.Drawing.Point(4, 12);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(85, 24);
            label4.TabIndex = 0;
            label4.Text = "Account:";
            //
            // checkEnableAutoLogin
            //
            checkEnableAutoLogin.AutoSize = true;
            checkEnableAutoLogin.BackColor = System.Drawing.Color.Transparent;
            checkEnableAutoLogin.Depth = 0;
            checkEnableAutoLogin.Location = new System.Drawing.Point(90, 86);
            checkEnableAutoLogin.Margin = new System.Windows.Forms.Padding(0);
            checkEnableAutoLogin.MouseLocation = new System.Drawing.Point(-1, -1);
            checkEnableAutoLogin.Name = "checkEnableAutoLogin";
            checkEnableAutoLogin.Ripple = true;
            checkEnableAutoLogin.Size = new System.Drawing.Size(230, 34);
            checkEnableAutoLogin.TabIndex = 1;
            checkEnableAutoLogin.Text = "Enable automated login";
            checkEnableAutoLogin.UseVisualStyleBackColor = false;
            checkEnableAutoLogin.CheckedChanged += checkEnableAutoLogin_CheckedChanged;
            //
            // comboCharacter
            //
            comboCharacter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            comboCharacter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            comboCharacter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboCharacter.FormattingEnabled = true;
            comboCharacter.ItemHeight = 22;
            comboCharacter.Items.AddRange(new object[] { "Not Selected" });
            comboCharacter.Location = new System.Drawing.Point(90, 48);
            comboCharacter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            comboCharacter.Name = "comboCharacter";
            comboCharacter.Radius = 5;
            comboCharacter.ShadowDepth = 4F;
            comboCharacter.Size = new System.Drawing.Size(288, 32);
            comboCharacter.TabIndex = 22;
            comboCharacter.SelectedIndexChanged += comboCharacter_SelectedIndexChanged;
            //
            // btnAutoLoginSettings
            //
            btnAutoLoginSettings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnAutoLoginSettings.Color = System.Drawing.Color.Transparent;
            btnAutoLoginSettings.Location = new System.Drawing.Point(268, 84);
            btnAutoLoginSettings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnAutoLoginSettings.Name = "btnAutoLoginSettings";
            btnAutoLoginSettings.Radius = 6;
            btnAutoLoginSettings.ShadowDepth = 4F;
            btnAutoLoginSettings.Size = new System.Drawing.Size(110, 38);
            btnAutoLoginSettings.TabIndex = 2;
            btnAutoLoginSettings.Text = "Setup";
            btnAutoLoginSettings.UseVisualStyleBackColor = true;
            btnAutoLoginSettings.Click += btnAutoLoginSettings_Click;
            //
            // groupBoxMastery
            //
            groupBoxMastery.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBoxMastery.BackColor = System.Drawing.Color.Transparent;
            groupBoxMastery.Controls.Add(checkLearnMastery);
            groupBoxMastery.Controls.Add(btnMasterySelect);
            groupBoxMastery.Controls.Add(lblMasteryGap);
            groupBoxMastery.Controls.Add(numMasteryGap);
            groupBoxMastery.Controls.Add(checkLearnMasteryBotStopped);
            groupBoxMastery.Location = new System.Drawing.Point(446, 281);
            groupBoxMastery.Margin = new System.Windows.Forms.Padding(12, 0, 0, 18);
            groupBoxMastery.Name = "groupBoxMastery";
            groupBoxMastery.Padding = new System.Windows.Forms.Padding(4, 18, 4, 8);
            groupBoxMastery.Radius = 10;
            groupBoxMastery.ShadowDepth = 4;
            groupBoxMastery.Size = new System.Drawing.Size(399, 230);
            groupBoxMastery.TabIndex = 17;
            groupBoxMastery.TabStop = false;
            groupBoxMastery.Text = "Mastery update";
            //
            // checkLearnMastery
            //
            checkLearnMastery.AutoSize = true;
            checkLearnMastery.BackColor = System.Drawing.Color.Transparent;
            checkLearnMastery.Depth = 0;
            checkLearnMastery.Location = new System.Drawing.Point(20, 44);
            checkLearnMastery.Margin = new System.Windows.Forms.Padding(0);
            checkLearnMastery.MouseLocation = new System.Drawing.Point(-1, -1);
            checkLearnMastery.Name = "checkLearnMastery";
            checkLearnMastery.Ripple = true;
            checkLearnMastery.Size = new System.Drawing.Size(110, 34);
            checkLearnMastery.TabIndex = 0;
            checkLearnMastery.Text = "Mastery";
            checkLearnMastery.UseVisualStyleBackColor = false;
            checkLearnMastery.CheckedChanged += checkLearnMastery_CheckedChanged;
            //
            // btnMasterySelect
            //
            btnMasterySelect.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            btnMasterySelect.Color = System.Drawing.Color.Transparent;
            btnMasterySelect.Location = new System.Drawing.Point(20, 84);
            btnMasterySelect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnMasterySelect.Name = "btnMasterySelect";
            btnMasterySelect.Radius = 6;
            btnMasterySelect.ShadowDepth = 4F;
            btnMasterySelect.Size = new System.Drawing.Size(355, 36);
            btnMasterySelect.TabIndex = 1;
            btnMasterySelect.Text = "Select masteries...  ▾";
            btnMasterySelect.UseVisualStyleBackColor = true;
            btnMasterySelect.Click += btnMasterySelect_Click;
            //
            // lblMasteryGap
            //
            lblMasteryGap.ApplyGradient = false;
            lblMasteryGap.AutoSize = false;
            lblMasteryGap.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            lblMasteryGap.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblMasteryGap.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            lblMasteryGap.GradientAnimation = false;
            lblMasteryGap.Location = new System.Drawing.Point(24, 138);
            lblMasteryGap.Name = "lblMasteryGap";
            lblMasteryGap.Size = new System.Drawing.Size(40, 24);
            lblMasteryGap.TabIndex = 2;
            lblMasteryGap.Text = "Gap";
            //
            // numMasteryGap
            //
            numMasteryGap.BackColor = System.Drawing.Color.Transparent;
            numMasteryGap.Font = new System.Drawing.Font("Segoe UI", 11F);
            numMasteryGap.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            numMasteryGap.Location = new System.Drawing.Point(70, 132);
            numMasteryGap.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            numMasteryGap.Maximum = new decimal(new int[] { 9, 0, 0, 0 });
            numMasteryGap.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            numMasteryGap.MinimumSize = new System.Drawing.Size(90, 36);
            numMasteryGap.Name = "numMasteryGap";
            numMasteryGap.Size = new System.Drawing.Size(90, 36);
            numMasteryGap.TabIndex = 3;
            numMasteryGap.Value = new decimal(new int[] { 0, 0, 0, 0 });
            numMasteryGap.ValueChanged += numMasteryGap_ValueChanged;
            //
            // checkLearnMasteryBotStopped
            //
            checkLearnMasteryBotStopped.AutoSize = true;
            checkLearnMasteryBotStopped.BackColor = System.Drawing.Color.Transparent;
            checkLearnMasteryBotStopped.Depth = 0;
            checkLearnMasteryBotStopped.Location = new System.Drawing.Point(20, 176);
            checkLearnMasteryBotStopped.Margin = new System.Windows.Forms.Padding(0);
            checkLearnMasteryBotStopped.MouseLocation = new System.Drawing.Point(-1, -1);
            checkLearnMasteryBotStopped.Name = "checkLearnMasteryBotStopped";
            checkLearnMasteryBotStopped.Ripple = true;
            checkLearnMasteryBotStopped.Size = new System.Drawing.Size(240, 34);
            checkLearnMasteryBotStopped.TabIndex = 4;
            checkLearnMasteryBotStopped.Text = "Increase even if bot is stopped";
            checkLearnMasteryBotStopped.UseVisualStyleBackColor = false;
            checkLearnMasteryBotStopped.CheckedChanged += checkLearnMasteryBotStopped_CheckedChanged;
            //
            // groupBoxStatPoints
            //
            groupBoxStatPoints.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBoxStatPoints.BackColor = System.Drawing.Color.Transparent;
            groupBoxStatPoints.Controls.Add(checkIncInt);
            groupBoxStatPoints.Controls.Add(numIncInt);
            groupBoxStatPoints.Controls.Add(checkIncStr);
            groupBoxStatPoints.Controls.Add(numIncStr);
            groupBoxStatPoints.Controls.Add(checkIncBotStopped);
            groupBoxStatPoints.Controls.Add(buttonRun);
            groupBoxStatPoints.Location = new System.Drawing.Point(446, 515);
            groupBoxStatPoints.Margin = new System.Windows.Forms.Padding(12, 0, 0, 16);
            groupBoxStatPoints.Name = "groupBoxStatPoints";
            groupBoxStatPoints.Padding = new System.Windows.Forms.Padding(4, 18, 4, 12);
            groupBoxStatPoints.Radius = 10;
            groupBoxStatPoints.ShadowDepth = 4;
            groupBoxStatPoints.Size = new System.Drawing.Size(399, 230);
            groupBoxStatPoints.TabIndex = 24;
            groupBoxStatPoints.TabStop = false;
            groupBoxStatPoints.Text = "Stat points";
            //
            // checkIncInt
            //
            checkIncInt.AutoSize = true;
            checkIncInt.BackColor = System.Drawing.Color.Transparent;
            checkIncInt.Depth = 0;
            checkIncInt.Location = new System.Drawing.Point(24, 44);
            checkIncInt.Margin = new System.Windows.Forms.Padding(0);
            checkIncInt.MouseLocation = new System.Drawing.Point(-1, -1);
            checkIncInt.Name = "checkIncInt";
            checkIncInt.Ripple = true;
            checkIncInt.Size = new System.Drawing.Size(150, 34);
            checkIncInt.TabIndex = 0;
            checkIncInt.Text = "Increase INT";
            checkIncInt.UseVisualStyleBackColor = false;
            checkIncInt.CheckedChanged += settings_CheckedChanged;
            //
            // numIncInt
            //
            numIncInt.BackColor = System.Drawing.Color.Transparent;
            numIncInt.Font = new System.Drawing.Font("Segoe UI", 11F);
            numIncInt.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            numIncInt.Location = new System.Drawing.Point(200, 40);
            numIncInt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            numIncInt.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            numIncInt.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            numIncInt.MinimumSize = new System.Drawing.Size(90, 36);
            numIncInt.Name = "numIncInt";
            numIncInt.Size = new System.Drawing.Size(90, 36);
            numIncInt.TabIndex = 1;
            numIncInt.Value = new decimal(new int[] { 0, 0, 0, 0 });
            numIncInt.ValueChanged += numIncInt_ValueChanged;
            //
            // checkIncStr
            //
            checkIncStr.AutoSize = true;
            checkIncStr.BackColor = System.Drawing.Color.Transparent;
            checkIncStr.Depth = 0;
            checkIncStr.Location = new System.Drawing.Point(24, 88);
            checkIncStr.Margin = new System.Windows.Forms.Padding(0);
            checkIncStr.MouseLocation = new System.Drawing.Point(-1, -1);
            checkIncStr.Name = "checkIncStr";
            checkIncStr.Ripple = true;
            checkIncStr.Size = new System.Drawing.Size(150, 34);
            checkIncStr.TabIndex = 2;
            checkIncStr.Text = "Increase STR";
            checkIncStr.UseVisualStyleBackColor = false;
            checkIncStr.CheckedChanged += settings_CheckedChanged;
            //
            // numIncStr
            //
            numIncStr.BackColor = System.Drawing.Color.Transparent;
            numIncStr.Font = new System.Drawing.Font("Segoe UI", 11F);
            numIncStr.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            numIncStr.Location = new System.Drawing.Point(200, 84);
            numIncStr.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            numIncStr.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            numIncStr.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            numIncStr.MinimumSize = new System.Drawing.Size(90, 36);
            numIncStr.Name = "numIncStr";
            numIncStr.Size = new System.Drawing.Size(90, 36);
            numIncStr.TabIndex = 3;
            numIncStr.Value = new decimal(new int[] { 0, 0, 0, 0 });
            numIncStr.ValueChanged += numIncStr_ValueChanged;
            //
            // checkIncBotStopped
            //
            checkIncBotStopped.AutoSize = true;
            checkIncBotStopped.BackColor = System.Drawing.Color.Transparent;
            checkIncBotStopped.Checked = true;
            checkIncBotStopped.CheckState = System.Windows.Forms.CheckState.Checked;
            checkIncBotStopped.Depth = 0;
            checkIncBotStopped.Location = new System.Drawing.Point(24, 132);
            checkIncBotStopped.Margin = new System.Windows.Forms.Padding(0);
            checkIncBotStopped.MouseLocation = new System.Drawing.Point(-1, -1);
            checkIncBotStopped.Name = "checkIncBotStopped";
            checkIncBotStopped.Ripple = true;
            checkIncBotStopped.Size = new System.Drawing.Size(300, 34);
            checkIncBotStopped.TabIndex = 4;
            checkIncBotStopped.Text = "Increase even if bot is stopped";
            checkIncBotStopped.UseVisualStyleBackColor = false;
            checkIncBotStopped.CheckedChanged += settings_CheckedChanged;
            //
            // buttonRun
            //
            buttonRun.Color = System.Drawing.Color.Transparent;
            buttonRun.Location = new System.Drawing.Point(24, 174);
            buttonRun.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            buttonRun.Name = "buttonRun";
            buttonRun.Radius = 6;
            buttonRun.ShadowDepth = 4F;
            buttonRun.Size = new System.Drawing.Size(150, 36);
            buttonRun.TabIndex = 5;
            buttonRun.Text = "Run";
            buttonRun.UseVisualStyleBackColor = true;
            buttonRun.Click += buttonRun_Click;
            //
            // Main
            //
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoScroll = true;
            Controls.Add(bodyScrollPanel);
            Controls.Add(comboBoxClientType);
            Controls.Add(lblVersion);
            Controls.Add(btnBrowseSilkroadPath);
            Controls.Add(txtSilkroadPath);
            Controls.Add(label1);
            Font = new System.Drawing.Font("Segoe UI", 10.5F);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "Main";
            Size = new System.Drawing.Size(869, 576);
            bodyLayout.ResumeLayout(false);
            bodyScrollPanel.ResumeLayout(false);
            leftColumnPanel.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBoxResurrection.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            captchaPanel.ResumeLayout(false);
            captchaPanel.PerformLayout();
            autoLoginTopPanel.ResumeLayout(false);
            autoLoginTopPanel.PerformLayout();
            groupBoxMastery.ResumeLayout(false);
            groupBoxStatPoints.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SDUI.Controls.Label label1;
        private SDUI.Controls.TextBox txtSilkroadPath;
        private SDUI.Controls.Button btnBrowseSilkroadPath;
        private SDUI.Controls.Label lblVersion;
        private System.Windows.Forms.Panel bodyScrollPanel;
        private System.Windows.Forms.TableLayoutPanel bodyLayout;
        private System.Windows.Forms.TableLayoutPanel leftColumnPanel;
        private SDUI.Controls.GroupBox groupBox1;
        private SDUI.Controls.Label label4;
        private SDUI.Controls.ComboBox comboAccounts;
        private SDUI.Controls.CheckBox checkEnableAutoLogin;
        private SDUI.Controls.Button btnAutoLoginSettings;
        private SDUI.Controls.TextBox txtStaticCaptcha;
        private SDUI.Controls.Label label5;
        private SDUI.Controls.CheckBox checkEnableStaticCaptcha;
        private SDUI.Controls.Label label6;
        private SDUI.Controls.Label label7;
        private SDUI.Controls.ComboBox comboCharacter;
        private SDUI.Controls.GroupBox groupBox2;
        private SDUI.Controls.Button btnGoClientless;
        private SDUI.Controls.Button btnStartClientless;
        private SDUI.Controls.Button btnStartClient;
        private SDUI.Controls.CheckBox checkUseReturnScroll;
        private SDUI.Controls.CheckBox checkStartBot;
        private SDUI.Controls.GroupBox groupBoxResurrection;
        private SDUI.Controls.Label lblResSkill;
        private SDUI.Controls.ComboBox comboResurrectionSkill;
        private SDUI.Controls.CheckBox checkAcceptResurrection;
        private SDUI.Controls.CheckBox checkResurrectParty;
        private SDUI.Controls.Label lblResDelay;
        private SDUI.Controls.NumUpDown numResDelay;
        private SDUI.Controls.Label lblResDelaySec;
        private SDUI.Controls.Label lblResRadius;
        private SDUI.Controls.NumUpDown numResRadius;
        private SDUI.Controls.Button btnClientHideShow;
        private SDUI.Controls.GroupBox groupBoxMastery;
        private SDUI.Controls.CheckBox checkLearnMastery;
        private SDUI.Controls.Button btnMasterySelect;
        private SDUI.Controls.Label lblMasteryGap;
        private SDUI.Controls.NumUpDown numMasteryGap;
        private SDUI.Controls.CheckBox checkLearnMasteryBotStopped;
        private SDUI.Controls.ComboBox comboBoxClientType;
        private System.Windows.Forms.Panel captchaPanel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel autoLoginTopPanel;
        private SDUI.Controls.Separator separator2;
        private SDUI.Controls.Separator separator1;
        private SDUI.Controls.Label lblLoginDelaySeconds;
        private SDUI.Controls.NumUpDown numLoginDelay;
        private SDUI.Controls.CheckBox checkEnableLoginDelay;
        private SDUI.Controls.CheckBox checkHideClient;
        private SDUI.Controls.Radio radioAutoSelectHigher;
        private SDUI.Controls.Radio radioAutoSelectFirst;
        private SDUI.Controls.CheckBox checkCharAutoSelect;
        private SDUI.Controls.GroupBox groupBoxStatPoints;
        private SDUI.Controls.CheckBox checkIncInt;
        private SDUI.Controls.NumUpDown numIncInt;
        private SDUI.Controls.CheckBox checkIncStr;
        private SDUI.Controls.NumUpDown numIncStr;
        private SDUI.Controls.CheckBox checkIncBotStopped;
        private SDUI.Controls.Button buttonRun;
        private SDUI.Controls.Label lblWaitAfterDC;
        private SDUI.Controls.NumUpDown numWaitAfterDC;
        private SDUI.Controls.CheckBox checkWaitAfterDC;
    }
}
