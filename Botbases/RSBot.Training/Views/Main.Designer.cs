namespace RSBot.Training.Views
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            groupBox2 = new SDUI.Controls.GroupBox();
            avoidanceListPanel = new System.Windows.Forms.Panel();
            groupBoxTrainingPlace = new SDUI.Controls.GroupBox();
            checkBoxUseReverse = new SDUI.Controls.CheckBox();
            checkUseSpeedDrug = new SDUI.Controls.CheckBox();
            checkCastBuffs = new SDUI.Controls.CheckBox();
            checkUseMount = new SDUI.Controls.CheckBox();
            lstTrainingPlaces = new System.Windows.Forms.ListBox();
            btnClearPlace = new SDUI.Controls.Button();
            txtSearchPlace = new SDUI.Controls.TextBox();
            labelSearchPlace = new SDUI.Controls.Label();
            groupBoxCreateScript = new SDUI.Controls.GroupBox();
            btnUpdateNavLink = new SDUI.Controls.Button();
            btnImportScript = new SDUI.Controls.Button();
            btnRecord = new SDUI.Controls.Button();
            labelCreateScriptHint = new SDUI.Controls.Label();
            checkBerzerkWhenFull = new SDUI.Controls.CheckBox();
            checkBerserkOnMonsterRarity = new SDUI.Controls.CheckBox();
            groupBoxBerserk = new SDUI.Controls.GroupBox();
            label7 = new SDUI.Controls.Label();
            numBerzerkMonsterAmount = new SDUI.Controls.NumUpDown();
            checkBerzerkAvoidance = new SDUI.Controls.CheckBox();
            checkBerzerkMonsterAmount = new SDUI.Controls.CheckBox();
            groupBoxArea = new SDUI.Controls.GroupBox();
            btnApplyArea = new SDUI.Controls.Button();
            label8 = new SDUI.Controls.Label();
            txtRegion = new SDUI.Controls.TextBox();
            buttonSelectTrainingArea = new SDUI.Controls.Button();
            label6 = new SDUI.Controls.Label();
            label5 = new SDUI.Controls.Label();
            radioWalkAround = new SDUI.Controls.Radio();
            radioCenter = new SDUI.Controls.Radio();
            btnGetCurrent = new SDUI.Controls.Button();
            label3 = new SDUI.Controls.Label();
            label2 = new SDUI.Controls.Label();
            label1 = new SDUI.Controls.Label();
            txtRadius = new SDUI.Controls.TextBox();
            txtYCoord = new SDUI.Controls.TextBox();
            txtXCoord = new SDUI.Controls.TextBox();
            groupBoxAdvanced = new SDUI.Controls.GroupBox();
            checkBoxDontFollowMobs = new SDUI.Controls.CheckBox();
            linkAttackWeakerMobsHelp = new System.Windows.Forms.LinkLabel();
            checkAttackWeakerFirst = new SDUI.Controls.CheckBox();
            checkBoxDimensionPillar = new SDUI.Controls.CheckBox();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            groupBox2.SuspendLayout();
            groupBoxTrainingPlace.SuspendLayout();
            groupBoxCreateScript.SuspendLayout();
            groupBoxBerserk.SuspendLayout();
            groupBoxArea.SuspendLayout();
            groupBoxAdvanced.SuspendLayout();
            SuspendLayout();
            //
            // groupBox2
            //
            groupBox2.BackColor = System.Drawing.Color.Transparent;
            groupBox2.Controls.Add(avoidanceListPanel);
            groupBox2.Location = new System.Drawing.Point(29, 352);
            groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(2, 9, 2, 4);
            groupBox2.Radius = 10;
            groupBox2.ShadowDepth = 4;
            groupBox2.Size = new System.Drawing.Size(380, 330);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Avoidance";
            //
            // avoidanceListPanel
            //
            // Populated in code (see Main.cs BuildAvoidanceChecklist) with one row per monster
            // rarity, each carrying 3 mutually-exclusive checkboxes (Avoid/Prefer/Berserk) -
            // replaces the old ListView + right-click-to-assign context menu.
            avoidanceListPanel.AutoScroll = true;
            avoidanceListPanel.BackColor = System.Drawing.Color.Transparent;
            avoidanceListPanel.Location = new System.Drawing.Point(2, 29);
            avoidanceListPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            avoidanceListPanel.Name = "avoidanceListPanel";
            avoidanceListPanel.Size = new System.Drawing.Size(376, 297);
            avoidanceListPanel.TabIndex = 5;
            //
            // groupBoxTrainingPlace
            //
            groupBoxTrainingPlace.BackColor = System.Drawing.Color.Transparent;
            groupBoxTrainingPlace.Controls.Add(labelSearchPlace);
            groupBoxTrainingPlace.Controls.Add(txtSearchPlace);
            groupBoxTrainingPlace.Controls.Add(btnClearPlace);
            groupBoxTrainingPlace.Controls.Add(lstTrainingPlaces);
            groupBoxTrainingPlace.Controls.Add(checkUseMount);
            groupBoxTrainingPlace.Controls.Add(checkCastBuffs);
            groupBoxTrainingPlace.Controls.Add(checkUseSpeedDrug);
            groupBoxTrainingPlace.Controls.Add(checkBoxUseReverse);
            groupBoxTrainingPlace.Location = new System.Drawing.Point(418, 20);
            groupBoxTrainingPlace.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            groupBoxTrainingPlace.Name = "groupBoxTrainingPlace";
            groupBoxTrainingPlace.Padding = new System.Windows.Forms.Padding(4, 12, 4, 4);
            groupBoxTrainingPlace.Radius = 10;
            groupBoxTrainingPlace.ShadowDepth = 4;
            groupBoxTrainingPlace.Size = new System.Drawing.Size(598, 260);
            groupBoxTrainingPlace.TabIndex = 2;
            groupBoxTrainingPlace.TabStop = false;
            groupBoxTrainingPlace.Text = "Select training place";
            //
            // labelSearchPlace
            //
            labelSearchPlace.ApplyGradient = false;
            labelSearchPlace.AutoSize = true;
            labelSearchPlace.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            labelSearchPlace.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            labelSearchPlace.GradientAnimation = false;
            labelSearchPlace.Location = new System.Drawing.Point(26, 38);
            labelSearchPlace.Name = "labelSearchPlace";
            labelSearchPlace.Size = new System.Drawing.Size(180, 20);
            labelSearchPlace.TabIndex = 0;
            labelSearchPlace.Text = "Search (name or level):";
            //
            // txtSearchPlace
            //
            txtSearchPlace.Location = new System.Drawing.Point(26, 60);
            txtSearchPlace.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            txtSearchPlace.MaxLength = 128;
            txtSearchPlace.MultiLine = false;
            txtSearchPlace.Name = "txtSearchPlace";
            txtSearchPlace.PassFocusShow = false;
            txtSearchPlace.Radius = 2;
            txtSearchPlace.Size = new System.Drawing.Size(390, 28);
            txtSearchPlace.TabIndex = 1;
            txtSearchPlace.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            txtSearchPlace.UseSystemPasswordChar = false;
            txtSearchPlace.TextChanged += txtSearchPlace_TextChanged;
            //
            // btnClearPlace
            //
            btnClearPlace.Color = System.Drawing.Color.Transparent;
            btnClearPlace.Location = new System.Drawing.Point(424, 60);
            btnClearPlace.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnClearPlace.Name = "btnClearPlace";
            btnClearPlace.Radius = 6;
            btnClearPlace.ShadowDepth = 4F;
            btnClearPlace.Size = new System.Drawing.Size(148, 29);
            btnClearPlace.TabIndex = 2;
            btnClearPlace.Text = "Clear selection";
            btnClearPlace.UseVisualStyleBackColor = true;
            btnClearPlace.Click += btnClearPlace_Click;
            //
            // lstTrainingPlaces
            //
            lstTrainingPlaces.BackColor = System.Drawing.Color.White;
            lstTrainingPlaces.BorderStyle = System.Windows.Forms.BorderStyle.None;
            lstTrainingPlaces.IntegralHeight = false;
            lstTrainingPlaces.ItemHeight = 20;
            lstTrainingPlaces.Location = new System.Drawing.Point(26, 96);
            lstTrainingPlaces.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            lstTrainingPlaces.Name = "lstTrainingPlaces";
            lstTrainingPlaces.Size = new System.Drawing.Size(546, 86);
            lstTrainingPlaces.TabIndex = 3;
            lstTrainingPlaces.SelectedIndexChanged += lstTrainingPlaces_SelectedIndexChanged;
            //
            // checkUseMount
            //
            checkUseMount.AutoSize = true;
            checkUseMount.BackColor = System.Drawing.Color.Transparent;
            checkUseMount.Checked = true;
            checkUseMount.CheckState = System.Windows.Forms.CheckState.Checked;
            checkUseMount.Depth = 0;
            checkUseMount.Location = new System.Drawing.Point(26, 192);
            checkUseMount.Margin = new System.Windows.Forms.Padding(0);
            checkUseMount.MouseLocation = new System.Drawing.Point(-1, -1);
            checkUseMount.Name = "checkUseMount";
            checkUseMount.Ripple = true;
            checkUseMount.Size = new System.Drawing.Size(240, 30);
            checkUseMount.TabIndex = 4;
            checkUseMount.Text = "Use mount if available";
            checkUseMount.UseVisualStyleBackColor = false;
            checkUseMount.CheckedChanged += settings_CheckedChanged;
            //
            // checkCastBuffs
            //
            // Left disabled for now - cast-buffs handling here will be reworked later.
            checkCastBuffs.AutoSize = true;
            checkCastBuffs.BackColor = System.Drawing.Color.Transparent;
            checkCastBuffs.Checked = true;
            checkCastBuffs.CheckState = System.Windows.Forms.CheckState.Checked;
            checkCastBuffs.Depth = 0;
            checkCastBuffs.Enabled = false;
            checkCastBuffs.Location = new System.Drawing.Point(290, 192);
            checkCastBuffs.Margin = new System.Windows.Forms.Padding(0);
            checkCastBuffs.MouseLocation = new System.Drawing.Point(-1, -1);
            checkCastBuffs.Name = "checkCastBuffs";
            checkCastBuffs.Ripple = true;
            checkCastBuffs.Size = new System.Drawing.Size(240, 30);
            checkCastBuffs.TabIndex = 5;
            checkCastBuffs.Text = "Cast buffs (coming soon)";
            checkCastBuffs.UseVisualStyleBackColor = false;
            checkCastBuffs.CheckedChanged += settings_CheckedChanged;
            //
            // checkUseSpeedDrug
            //
            checkUseSpeedDrug.AutoSize = true;
            checkUseSpeedDrug.BackColor = System.Drawing.Color.Transparent;
            checkUseSpeedDrug.Checked = true;
            checkUseSpeedDrug.CheckState = System.Windows.Forms.CheckState.Checked;
            checkUseSpeedDrug.Depth = 0;
            checkUseSpeedDrug.Location = new System.Drawing.Point(26, 222);
            checkUseSpeedDrug.Margin = new System.Windows.Forms.Padding(0);
            checkUseSpeedDrug.MouseLocation = new System.Drawing.Point(-1, -1);
            checkUseSpeedDrug.Name = "checkUseSpeedDrug";
            checkUseSpeedDrug.Ripple = true;
            checkUseSpeedDrug.Size = new System.Drawing.Size(240, 30);
            checkUseSpeedDrug.TabIndex = 6;
            checkUseSpeedDrug.Text = "Use speed drug";
            checkUseSpeedDrug.UseVisualStyleBackColor = false;
            checkUseSpeedDrug.CheckedChanged += settings_CheckedChanged;
            //
            // checkBoxUseReverse
            //
            checkBoxUseReverse.AutoSize = true;
            checkBoxUseReverse.BackColor = System.Drawing.Color.Transparent;
            checkBoxUseReverse.Depth = 0;
            checkBoxUseReverse.Location = new System.Drawing.Point(290, 222);
            checkBoxUseReverse.Margin = new System.Windows.Forms.Padding(0);
            checkBoxUseReverse.MouseLocation = new System.Drawing.Point(-1, -1);
            checkBoxUseReverse.Name = "checkBoxUseReverse";
            checkBoxUseReverse.Ripple = true;
            checkBoxUseReverse.Size = new System.Drawing.Size(240, 30);
            checkBoxUseReverse.TabIndex = 7;
            checkBoxUseReverse.Text = "Use Reverse";
            checkBoxUseReverse.UseVisualStyleBackColor = false;
            checkBoxUseReverse.CheckedChanged += settings_CheckedChanged;
            //
            // groupBoxCreateScript
            //
            groupBoxCreateScript.BackColor = System.Drawing.Color.Transparent;
            groupBoxCreateScript.Controls.Add(labelCreateScriptHint);
            groupBoxCreateScript.Controls.Add(btnRecord);
            groupBoxCreateScript.Controls.Add(btnImportScript);
            groupBoxCreateScript.Controls.Add(btnUpdateNavLink);
            groupBoxCreateScript.Location = new System.Drawing.Point(418, 300);
            groupBoxCreateScript.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            groupBoxCreateScript.Name = "groupBoxCreateScript";
            groupBoxCreateScript.Padding = new System.Windows.Forms.Padding(4, 12, 4, 4);
            groupBoxCreateScript.Radius = 10;
            groupBoxCreateScript.ShadowDepth = 4;
            groupBoxCreateScript.Size = new System.Drawing.Size(598, 110);
            groupBoxCreateScript.TabIndex = 3;
            groupBoxCreateScript.TabStop = false;
            groupBoxCreateScript.Text = "Create a walk script";
            //
            // labelCreateScriptHint
            //
            labelCreateScriptHint.ApplyGradient = false;
            labelCreateScriptHint.AutoSize = false;
            labelCreateScriptHint.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            labelCreateScriptHint.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            labelCreateScriptHint.GradientAnimation = false;
            labelCreateScriptHint.Location = new System.Drawing.Point(26, 34);
            labelCreateScriptHint.Name = "labelCreateScriptHint";
            labelCreateScriptHint.Size = new System.Drawing.Size(546, 36);
            labelCreateScriptHint.TabIndex = 0;
            labelCreateScriptHint.Text = "Record a new patrol route, or import an existing .rbs script - either way you\'ll be asked to name it and set its level.";
            //
            // btnRecord
            //
            btnRecord.Color = System.Drawing.Color.Transparent;
            btnRecord.Location = new System.Drawing.Point(26, 74);
            btnRecord.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnRecord.Name = "btnRecord";
            btnRecord.Radius = 6;
            btnRecord.ShadowDepth = 4F;
            btnRecord.Size = new System.Drawing.Size(160, 32);
            btnRecord.TabIndex = 1;
            btnRecord.Text = "Record...";
            btnRecord.UseVisualStyleBackColor = true;
            btnRecord.Click += btnRecord_Click;
            //
            // btnImportScript
            //
            btnImportScript.Color = System.Drawing.Color.Transparent;
            btnImportScript.Location = new System.Drawing.Point(200, 74);
            btnImportScript.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnImportScript.Name = "btnImportScript";
            btnImportScript.Radius = 6;
            btnImportScript.ShadowDepth = 4F;
            btnImportScript.Size = new System.Drawing.Size(160, 32);
            btnImportScript.TabIndex = 2;
            btnImportScript.Text = "Import file...";
            btnImportScript.UseVisualStyleBackColor = true;
            btnImportScript.Click += btnImportScript_Click;
            //
            // btnUpdateNavLink
            //
            // Declared here (rather than added at runtime, as it originally was inside the
            // removed "Back to training" box) so it goes through the same DPI auto-scale pass as
            // its siblings - a runtime-added control at a hand-picked pixel position doesn't, and
            // ends up mispositioned relative to a parent that DID get rescaled.
            btnUpdateNavLink.Color = System.Drawing.Color.Transparent;
            btnUpdateNavLink.Location = new System.Drawing.Point(390, 74);
            btnUpdateNavLink.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnUpdateNavLink.Name = "btnUpdateNavLink";
            btnUpdateNavLink.Radius = 6;
            btnUpdateNavLink.ShadowDepth = 4F;
            btnUpdateNavLink.Size = new System.Drawing.Size(160, 32);
            btnUpdateNavLink.TabIndex = 3;
            btnUpdateNavLink.Text = "Update NavLink";
            btnUpdateNavLink.UseVisualStyleBackColor = true;
            //
            // checkBerzerkWhenFull
            //
            checkBerzerkWhenFull.AutoSize = true;
            checkBerzerkWhenFull.BackColor = System.Drawing.Color.Transparent;
            checkBerzerkWhenFull.Depth = 0;
            checkBerzerkWhenFull.Location = new System.Drawing.Point(26, 42);
            checkBerzerkWhenFull.Margin = new System.Windows.Forms.Padding(0);
            checkBerzerkWhenFull.MouseLocation = new System.Drawing.Point(-1, -1);
            checkBerzerkWhenFull.Name = "checkBerzerkWhenFull";
            checkBerzerkWhenFull.Ripple = true;
            checkBerzerkWhenFull.Size = new System.Drawing.Size(229, 30);
            checkBerzerkWhenFull.TabIndex = 4;
            checkBerzerkWhenFull.Text = "Enter berzerk mode when full";
            checkBerzerkWhenFull.UseVisualStyleBackColor = false;
            checkBerzerkWhenFull.CheckedChanged += settings_CheckedChanged;
            //
            // checkBerserkOnMonsterRarity
            //
            checkBerserkOnMonsterRarity.AutoSize = true;
            checkBerserkOnMonsterRarity.BackColor = System.Drawing.Color.Transparent;
            checkBerserkOnMonsterRarity.Depth = 0;
            checkBerserkOnMonsterRarity.Location = new System.Drawing.Point(26, 140);
            checkBerserkOnMonsterRarity.Margin = new System.Windows.Forms.Padding(0);
            checkBerserkOnMonsterRarity.MouseLocation = new System.Drawing.Point(-1, -1);
            checkBerserkOnMonsterRarity.Name = "checkBerserkOnMonsterRarity";
            checkBerserkOnMonsterRarity.Ripple = true;
            checkBerserkOnMonsterRarity.Size = new System.Drawing.Size(392, 30);
            checkBerserkOnMonsterRarity.TabIndex = 4;
            checkBerserkOnMonsterRarity.Text = "Enter berserk mode when attack specific monster type";
            checkBerserkOnMonsterRarity.UseVisualStyleBackColor = false;
            checkBerserkOnMonsterRarity.CheckedChanged += settings_CheckedChanged;
            //
            // groupBoxBerserk
            //
            groupBoxBerserk.BackColor = System.Drawing.Color.Transparent;
            groupBoxBerserk.Controls.Add(label7);
            groupBoxBerserk.Controls.Add(numBerzerkMonsterAmount);
            groupBoxBerserk.Controls.Add(checkBerzerkAvoidance);
            groupBoxBerserk.Controls.Add(checkBerzerkMonsterAmount);
            groupBoxBerserk.Controls.Add(checkBerzerkWhenFull);
            groupBoxBerserk.Controls.Add(checkBerserkOnMonsterRarity);
            groupBoxBerserk.Location = new System.Drawing.Point(418, 430);
            groupBoxBerserk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            groupBoxBerserk.Name = "groupBoxBerserk";
            groupBoxBerserk.Padding = new System.Windows.Forms.Padding(4, 12, 4, 4);
            groupBoxBerserk.Radius = 10;
            groupBoxBerserk.ShadowDepth = 4;
            groupBoxBerserk.Size = new System.Drawing.Size(598, 188);
            groupBoxBerserk.TabIndex = 5;
            groupBoxBerserk.TabStop = false;
            groupBoxBerserk.Text = "Berserk";
            //
            // label7
            //
            label7.ApplyGradient = false;
            label7.AutoSize = true;
            label7.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            label7.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            label7.GradientAnimation = false;
            label7.Location = new System.Drawing.Point(375, 84);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(69, 20);
            label7.TabIndex = 7;
            label7.Text = "monsters";
            //
            // numBerzerkMonsterAmount
            //
            numBerzerkMonsterAmount.BackColor = System.Drawing.Color.Transparent;
            numBerzerkMonsterAmount.Font = new System.Drawing.Font("Segoe UI", 9.25F);
            numBerzerkMonsterAmount.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            numBerzerkMonsterAmount.Location = new System.Drawing.Point(268, 78);
            numBerzerkMonsterAmount.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            numBerzerkMonsterAmount.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numBerzerkMonsterAmount.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            numBerzerkMonsterAmount.MinimumSize = new System.Drawing.Size(100, 31);
            numBerzerkMonsterAmount.Name = "numBerzerkMonsterAmount";
            numBerzerkMonsterAmount.Size = new System.Drawing.Size(100, 31);
            numBerzerkMonsterAmount.TabIndex = 6;
            numBerzerkMonsterAmount.Value = new decimal(new int[] { 5, 0, 0, 0 });
            numBerzerkMonsterAmount.ValueChanged += numSettings_ValueChanged;
            //
            // checkBerzerkAvoidance
            //
            checkBerzerkAvoidance.AutoSize = true;
            checkBerzerkAvoidance.BackColor = System.Drawing.Color.Transparent;
            checkBerzerkAvoidance.Depth = 0;
            checkBerzerkAvoidance.Location = new System.Drawing.Point(26, 108);
            checkBerzerkAvoidance.Margin = new System.Windows.Forms.Padding(0);
            checkBerzerkAvoidance.MouseLocation = new System.Drawing.Point(-1, -1);
            checkBerzerkAvoidance.Name = "checkBerzerkAvoidance";
            checkBerzerkAvoidance.Ripple = true;
            checkBerzerkAvoidance.Size = new System.Drawing.Size(427, 30);
            checkBerzerkAvoidance.TabIndex = 5;
            checkBerzerkAvoidance.Text = "If being attacked by a monster type that should be avoided";
            checkBerzerkAvoidance.UseVisualStyleBackColor = false;
            checkBerzerkAvoidance.CheckedChanged += settings_CheckedChanged;
            //
            // checkBerzerkMonsterAmount
            //
            checkBerzerkMonsterAmount.AutoSize = true;
            checkBerzerkMonsterAmount.BackColor = System.Drawing.Color.Transparent;
            checkBerzerkMonsterAmount.Depth = 0;
            checkBerzerkMonsterAmount.Location = new System.Drawing.Point(26, 75);
            checkBerzerkMonsterAmount.Margin = new System.Windows.Forms.Padding(0);
            checkBerzerkMonsterAmount.MouseLocation = new System.Drawing.Point(-1, -1);
            checkBerzerkMonsterAmount.Name = "checkBerzerkMonsterAmount";
            checkBerzerkMonsterAmount.Ripple = true;
            checkBerzerkMonsterAmount.Size = new System.Drawing.Size(226, 30);
            checkBerzerkMonsterAmount.TabIndex = 4;
            checkBerzerkMonsterAmount.Text = "Being attacked by more than";
            checkBerzerkMonsterAmount.UseVisualStyleBackColor = false;
            checkBerzerkMonsterAmount.CheckedChanged += settings_CheckedChanged;
            //
            // groupBoxArea
            //
            groupBoxArea.BackColor = System.Drawing.Color.Transparent;
            groupBoxArea.Controls.Add(btnApplyArea);
            groupBoxArea.Controls.Add(label8);
            groupBoxArea.Controls.Add(txtRegion);
            groupBoxArea.Controls.Add(buttonSelectTrainingArea);
            groupBoxArea.Controls.Add(label6);
            groupBoxArea.Controls.Add(label5);
            groupBoxArea.Controls.Add(radioWalkAround);
            groupBoxArea.Controls.Add(radioCenter);
            groupBoxArea.Controls.Add(btnGetCurrent);
            groupBoxArea.Controls.Add(label3);
            groupBoxArea.Controls.Add(label2);
            groupBoxArea.Controls.Add(label1);
            groupBoxArea.Controls.Add(txtRadius);
            groupBoxArea.Controls.Add(txtYCoord);
            groupBoxArea.Controls.Add(txtXCoord);
            groupBoxArea.Location = new System.Drawing.Point(29, 20);
            groupBoxArea.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            groupBoxArea.Name = "groupBoxArea";
            groupBoxArea.Padding = new System.Windows.Forms.Padding(4, 12, 4, 4);
            groupBoxArea.Radius = 10;
            groupBoxArea.ShadowDepth = 4;
            groupBoxArea.Size = new System.Drawing.Size(276, 312);
            groupBoxArea.TabIndex = 0;
            groupBoxArea.TabStop = false;
            groupBoxArea.Text = "Area";
            //
            // btnApplyArea
            //
            btnApplyArea.Color = System.Drawing.Color.Transparent;
            btnApplyArea.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            btnApplyArea.Location = new System.Drawing.Point(56, 174);
            btnApplyArea.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnApplyArea.Name = "btnApplyArea";
            btnApplyArea.Radius = 6;
            btnApplyArea.ShadowDepth = 4F;
            btnApplyArea.Size = new System.Drawing.Size(30, 30);
            btnApplyArea.TabIndex = 10;
            btnApplyArea.Text = "v";
            btnApplyArea.UseVisualStyleBackColor = true;
            btnApplyArea.Click += btnApplyArea_Click;
            //
            // label8
            //
            label8.ApplyGradient = false;
            label8.AutoSize = true;
            label8.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            label8.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            label8.GradientAnimation = false;
            label8.Location = new System.Drawing.Point(34, 109);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(59, 20);
            label8.TabIndex = 9;
            label8.Text = "Region:";
            //
            // txtRegion
            //
            txtRegion.Location = new System.Drawing.Point(95, 105);
            txtRegion.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            txtRegion.MaxLength = 32767;
            txtRegion.MultiLine = false;
            txtRegion.Name = "txtRegion";
            txtRegion.PassFocusShow = false;
            txtRegion.Radius = 2;
            txtRegion.Size = new System.Drawing.Size(121, 25);
            txtRegion.TabIndex = 8;
            txtRegion.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            txtRegion.UseSystemPasswordChar = false;
            txtRegion.TextChanged += txtRegion_TextChanged;
            //
            // buttonSelectTrainingArea
            //
            buttonSelectTrainingArea.Color = System.Drawing.Color.Transparent;
            buttonSelectTrainingArea.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            buttonSelectTrainingArea.Location = new System.Drawing.Point(224, 174);
            buttonSelectTrainingArea.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            buttonSelectTrainingArea.Name = "buttonSelectTrainingArea";
            buttonSelectTrainingArea.Radius = 6;
            buttonSelectTrainingArea.ShadowDepth = 4F;
            buttonSelectTrainingArea.Size = new System.Drawing.Size(30, 30);
            buttonSelectTrainingArea.TabIndex = 7;
            buttonSelectTrainingArea.Text = "...";
            buttonSelectTrainingArea.UseVisualStyleBackColor = true;
            buttonSelectTrainingArea.Click += buttonSelectTrainingArea_Click;
            //
            // label6
            //
            label6.ApplyGradient = false;
            label6.AutoSize = true;
            label6.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            label6.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            label6.GradientAnimation = false;
            label6.Location = new System.Drawing.Point(18, 221);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(207, 20);
            label6.TabIndex = 7;
            label6.Text = "If there is no monster nearby...";
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
            label5.Location = new System.Drawing.Point(8, 208);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(250, 2);
            label5.TabIndex = 6;
            //
            // radioWalkAround
            //
            radioWalkAround.AutoSize = true;
            radioWalkAround.Checked = true;
            radioWalkAround.Location = new System.Drawing.Point(44, 280);
            radioWalkAround.Margin = new System.Windows.Forms.Padding(0);
            radioWalkAround.Name = "radioWalkAround";
            radioWalkAround.Ripple = true;
            radioWalkAround.Size = new System.Drawing.Size(118, 30);
            radioWalkAround.TabIndex = 5;
            radioWalkAround.TabStop = true;
            radioWalkAround.Text = "Walk around";
            radioWalkAround.CheckedChanged += settings_CheckedChanged;
            //
            // radioCenter
            //
            radioCenter.AutoSize = true;
            radioCenter.Location = new System.Drawing.Point(44, 248);
            radioCenter.Margin = new System.Windows.Forms.Padding(0);
            radioCenter.Name = "radioCenter";
            radioCenter.Ripple = true;
            radioCenter.Size = new System.Drawing.Size(153, 30);
            radioCenter.TabIndex = 4;
            radioCenter.Text = "Go back to center";
            radioCenter.CheckedChanged += settings_CheckedChanged;
            //
            // btnGetCurrent
            //
            btnGetCurrent.Color = System.Drawing.Color.Transparent;
            btnGetCurrent.Location = new System.Drawing.Point(95, 174);
            btnGetCurrent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btnGetCurrent.Name = "btnGetCurrent";
            btnGetCurrent.Radius = 6;
            btnGetCurrent.ShadowDepth = 4F;
            btnGetCurrent.Size = new System.Drawing.Size(121, 29);
            btnGetCurrent.TabIndex = 3;
            btnGetCurrent.Text = "Current";
            btnGetCurrent.UseVisualStyleBackColor = true;
            btnGetCurrent.Click += btnGetCurrent_Click;
            //
            // label3
            //
            label3.ApplyGradient = false;
            label3.AutoSize = true;
            label3.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            label3.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            label3.GradientAnimation = false;
            label3.Location = new System.Drawing.Point(34, 145);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(56, 20);
            label3.TabIndex = 1;
            label3.Text = "Radius:";
            //
            // label2
            //
            label2.ApplyGradient = false;
            label2.AutoSize = true;
            label2.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            label2.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            label2.GradientAnimation = false;
            label2.Location = new System.Drawing.Point(66, 72);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(20, 20);
            label2.TabIndex = 1;
            label2.Text = "Y:";
            toolTip1.SetToolTip(label2, "The coordinates of the dungeons should be divided by 10");
            //
            // label1
            //
            label1.ApplyGradient = false;
            label1.AutoSize = true;
            label1.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            label1.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            label1.GradientAnimation = false;
            label1.Location = new System.Drawing.Point(66, 40);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(21, 20);
            label1.TabIndex = 1;
            label1.Text = "X:";
            toolTip1.SetToolTip(label1, "The coordinates of the dungeons should be divided by 10");
            //
            // txtRadius
            //
            txtRadius.Location = new System.Drawing.Point(95, 141);
            txtRadius.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            txtRadius.MaxLength = 32767;
            txtRadius.MultiLine = false;
            txtRadius.Name = "txtRadius";
            txtRadius.PassFocusShow = false;
            txtRadius.Radius = 2;
            txtRadius.Size = new System.Drawing.Size(121, 25);
            txtRadius.TabIndex = 0;
            txtRadius.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            txtRadius.UseSystemPasswordChar = false;
            txtRadius.TextChanged += txtRadius_TextChanged;
            //
            // txtYCoord
            //
            txtYCoord.Location = new System.Drawing.Point(95, 69);
            txtYCoord.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            txtYCoord.MaxLength = 32767;
            txtYCoord.MultiLine = false;
            txtYCoord.Name = "txtYCoord";
            txtYCoord.PassFocusShow = false;
            txtYCoord.Radius = 2;
            txtYCoord.Size = new System.Drawing.Size(121, 25);
            txtYCoord.TabIndex = 0;
            txtYCoord.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            txtYCoord.UseSystemPasswordChar = false;
            txtYCoord.TextChanged += txtYCoord_TextChanged;
            //
            // txtXCoord
            //
            txtXCoord.Location = new System.Drawing.Point(95, 36);
            txtXCoord.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            txtXCoord.MaxLength = 32767;
            txtXCoord.MultiLine = false;
            txtXCoord.Name = "txtXCoord";
            txtXCoord.PassFocusShow = false;
            txtXCoord.Radius = 2;
            txtXCoord.Size = new System.Drawing.Size(121, 25);
            txtXCoord.TabIndex = 0;
            txtXCoord.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            txtXCoord.UseSystemPasswordChar = false;
            txtXCoord.TextChanged += txtXCoord_TextChanged;
            //
            // groupBoxAdvanced
            //
            groupBoxAdvanced.BackColor = System.Drawing.Color.Transparent;
            groupBoxAdvanced.Controls.Add(checkBoxDontFollowMobs);
            groupBoxAdvanced.Controls.Add(linkAttackWeakerMobsHelp);
            groupBoxAdvanced.Controls.Add(checkAttackWeakerFirst);
            groupBoxAdvanced.Controls.Add(checkBoxDimensionPillar);
            groupBoxAdvanced.Location = new System.Drawing.Point(418, 638);
            groupBoxAdvanced.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            groupBoxAdvanced.Name = "groupBoxAdvanced";
            groupBoxAdvanced.Padding = new System.Windows.Forms.Padding(4, 10, 4, 4);
            groupBoxAdvanced.Radius = 10;
            groupBoxAdvanced.ShadowDepth = 4;
            groupBoxAdvanced.Size = new System.Drawing.Size(598, 158);
            groupBoxAdvanced.TabIndex = 6;
            groupBoxAdvanced.TabStop = false;
            groupBoxAdvanced.Text = "Advanced";
            //
            // checkBoxDontFollowMobs
            //
            checkBoxDontFollowMobs.AutoSize = false;
            checkBoxDontFollowMobs.BackColor = System.Drawing.Color.Transparent;
            checkBoxDontFollowMobs.Depth = 0;
            checkBoxDontFollowMobs.Location = new System.Drawing.Point(26, 116);
            checkBoxDontFollowMobs.Margin = new System.Windows.Forms.Padding(0);
            checkBoxDontFollowMobs.MouseLocation = new System.Drawing.Point(-1, -1);
            checkBoxDontFollowMobs.Name = "checkBoxDontFollowMobs";
            checkBoxDontFollowMobs.Ripple = true;
            checkBoxDontFollowMobs.Size = new System.Drawing.Size(420, 30);
            checkBoxDontFollowMobs.TabIndex = 8;
            checkBoxDontFollowMobs.Text = "Don't follow mobs outside the training area";
            checkBoxDontFollowMobs.UseVisualStyleBackColor = false;
            checkBoxDontFollowMobs.CheckedChanged += settings_CheckedChanged;
            //
            // linkAttackWeakerMobsHelp
            //
            linkAttackWeakerMobsHelp.AutoSize = true;
            linkAttackWeakerMobsHelp.Location = new System.Drawing.Point(454, 84);
            linkAttackWeakerMobsHelp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            linkAttackWeakerMobsHelp.Name = "linkAttackWeakerMobsHelp";
            linkAttackWeakerMobsHelp.Size = new System.Drawing.Size(16, 20);
            linkAttackWeakerMobsHelp.TabIndex = 7;
            linkAttackWeakerMobsHelp.TabStop = true;
            linkAttackWeakerMobsHelp.Text = "?";
            linkAttackWeakerMobsHelp.LinkClicked += linkAttackWeakerMobsHelp_LinkClicked;
            //
            // checkAttackWeakerFirst
            //
            checkAttackWeakerFirst.AutoSize = false;
            checkAttackWeakerFirst.BackColor = System.Drawing.Color.Transparent;
            checkAttackWeakerFirst.Depth = 0;
            checkAttackWeakerFirst.Location = new System.Drawing.Point(26, 76);
            checkAttackWeakerFirst.Margin = new System.Windows.Forms.Padding(0);
            checkAttackWeakerFirst.MouseLocation = new System.Drawing.Point(-1, -1);
            checkAttackWeakerFirst.Name = "checkAttackWeakerFirst";
            checkAttackWeakerFirst.Ripple = true;
            checkAttackWeakerFirst.Size = new System.Drawing.Size(420, 30);
            checkAttackWeakerFirst.TabIndex = 1;
            checkAttackWeakerFirst.Text = "If avoided: counter attack weaker mobs first";
            checkAttackWeakerFirst.UseVisualStyleBackColor = false;
            checkAttackWeakerFirst.CheckedChanged += settings_CheckedChanged;
            //
            // checkBoxDimensionPillar
            //
            checkBoxDimensionPillar.AutoSize = false;
            checkBoxDimensionPillar.BackColor = System.Drawing.Color.Transparent;
            checkBoxDimensionPillar.Depth = 0;
            checkBoxDimensionPillar.Location = new System.Drawing.Point(26, 36);
            checkBoxDimensionPillar.Margin = new System.Windows.Forms.Padding(0);
            checkBoxDimensionPillar.MouseLocation = new System.Drawing.Point(-1, -1);
            checkBoxDimensionPillar.Name = "checkBoxDimensionPillar";
            checkBoxDimensionPillar.Ripple = true;
            checkBoxDimensionPillar.Size = new System.Drawing.Size(300, 30);
            checkBoxDimensionPillar.TabIndex = 0;
            checkBoxDimensionPillar.Text = "Ignore Dimension Pillar";
            checkBoxDimensionPillar.UseVisualStyleBackColor = false;
            checkBoxDimensionPillar.CheckedChanged += settings_CheckedChanged;
            //
            // Main
            //
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoScroll = true;
            Controls.Add(groupBoxAdvanced);
            Controls.Add(groupBoxCreateScript);
            Controls.Add(groupBoxTrainingPlace);
            Controls.Add(groupBox2);
            Controls.Add(groupBoxArea);
            Controls.Add(groupBoxBerserk);
            Font = new System.Drawing.Font("Segoe UI", 9F);
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Name = "Main";
            Size = new System.Drawing.Size(1059, 820);
            Load += Main_Load;
            groupBox2.ResumeLayout(false);
            groupBoxTrainingPlace.ResumeLayout(false);
            groupBoxTrainingPlace.PerformLayout();
            groupBoxCreateScript.ResumeLayout(false);
            groupBoxBerserk.ResumeLayout(false);
            groupBoxBerserk.PerformLayout();
            groupBoxArea.ResumeLayout(false);
            groupBoxArea.PerformLayout();
            groupBoxAdvanced.ResumeLayout(false);
            groupBoxAdvanced.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private SDUI.Controls.GroupBox groupBox2;
        private System.Windows.Forms.Panel avoidanceListPanel;
        private SDUI.Controls.GroupBox groupBoxTrainingPlace;
        private SDUI.Controls.Label labelSearchPlace;
        private SDUI.Controls.TextBox txtSearchPlace;
        private SDUI.Controls.Button btnClearPlace;
        private System.Windows.Forms.ListBox lstTrainingPlaces;
        private SDUI.Controls.CheckBox checkUseMount;
        private SDUI.Controls.CheckBox checkUseSpeedDrug;
        private SDUI.Controls.CheckBox checkCastBuffs;
        private SDUI.Controls.CheckBox checkBoxUseReverse;
        private SDUI.Controls.GroupBox groupBoxCreateScript;
        private SDUI.Controls.Label labelCreateScriptHint;
        private SDUI.Controls.Button btnRecord;
        private SDUI.Controls.Button btnImportScript;
        private SDUI.Controls.Button btnUpdateNavLink;
        private SDUI.Controls.CheckBox checkBerzerkWhenFull;
        private SDUI.Controls.CheckBox checkBerserkOnMonsterRarity;
        private SDUI.Controls.GroupBox groupBoxBerserk;
        private SDUI.Controls.NumUpDown numBerzerkMonsterAmount;
        private SDUI.Controls.CheckBox checkBerzerkAvoidance;
        private SDUI.Controls.CheckBox checkBerzerkMonsterAmount;
        private SDUI.Controls.Label label7;
        private SDUI.Controls.GroupBox groupBoxArea;
        private SDUI.Controls.Label label6;
        private SDUI.Controls.Label label5;
        private SDUI.Controls.Radio radioWalkAround;
        private SDUI.Controls.Radio radioCenter;
        private SDUI.Controls.Button btnGetCurrent;
        private SDUI.Controls.Label label3;
        private SDUI.Controls.Label label2;
        private SDUI.Controls.Label label1;
        private SDUI.Controls.TextBox txtRadius;
        private SDUI.Controls.TextBox txtYCoord;
        private SDUI.Controls.TextBox txtXCoord;
        private SDUI.Controls.Button buttonSelectTrainingArea;
        private SDUI.Controls.Radio radioStand;
        private SDUI.Controls.GroupBox groupBoxAdvanced;
        private SDUI.Controls.CheckBox checkBoxDimensionPillar;
        private SDUI.Controls.CheckBox checkAttackWeakerFirst;
        private System.Windows.Forms.LinkLabel linkAttackWeakerMobsHelp;
        private SDUI.Controls.Label label8;
        private SDUI.Controls.TextBox txtRegion;
        private SDUI.Controls.Button btnApplyArea;
        private System.Windows.Forms.ToolTip toolTip1;
        private SDUI.Controls.CheckBox checkBoxDontFollowMobs;
    }
}
