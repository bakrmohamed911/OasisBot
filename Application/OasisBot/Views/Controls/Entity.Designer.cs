namespace RSBot.Views.Controls
{
    partial class Entity
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
            lblType = new SDUI.Controls.Label();
            label1 = new SDUI.Controls.Label();
            lblEntityName = new SDUI.Controls.Label();
            progressHP = new SDUI.Controls.ProgressBar();
            lblEntityHpValue = new SDUI.Controls.Label();
            SuspendLayout();
            // 
            // lblType
            // 
            lblType.ApplyGradient = false;
            lblType.BackColor = System.Drawing.Color.Transparent;
            lblType.Dock = System.Windows.Forms.DockStyle.Bottom;
            lblType.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblType.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            lblType.GradientAnimation = false;
            lblType.Location = new System.Drawing.Point(0, 64);
            lblType.Name = "lblType";
            lblType.Size = new System.Drawing.Size(250, 19);
            lblType.TabIndex = 18;
            lblType.Text = "<none>";
            lblType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            label1.Location = new System.Drawing.Point(20, 40);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(26, 15);
            label1.TabIndex = 17;
            label1.Text = "HP:";
            // 
            // lblEntityName
            // 
            lblEntityName.ApplyGradient = false;
            lblEntityName.AutoSize = true;
            lblEntityName.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblEntityName.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            lblEntityName.GradientAnimation = false;
            lblEntityName.Location = new System.Drawing.Point(49, 12);
            lblEntityName.Name = "lblEntityName";
            lblEntityName.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            lblEntityName.Size = new System.Drawing.Size(108, 15);
            lblEntityName.TabIndex = 16;
            lblEntityName.Text = "No entity selected";
            // 
            // progressHP
            // 
            progressHP.BackColor = System.Drawing.Color.Transparent;
            progressHP.DrawHatch = false;
            progressHP.ForeColor = System.Drawing.Color.Firebrick;
            progressHP.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Maroon,
    System.Drawing.Color.Red
    };
            progressHP.HatchType = System.Drawing.Drawing2D.HatchStyle.Percent10;
            progressHP.Location = new System.Drawing.Point(52, 38);
            progressHP.Maximum = 100L;
            progressHP.MaxPercentShowValue = 100F;
            progressHP.Name = "progressHP";
            progressHP.PercentIndices = 2;
            progressHP.Radius = 6;
            progressHP.ShowAsPercent = true;
            progressHP.ShowValue = true;
            progressHP.Size = new System.Drawing.Size(180, 20);
            progressHP.TabIndex = 15;
            progressHP.Text = "0,00%";
            progressHP.Value = 0L;
            //
            // lblEntityHpValue
            //
            lblEntityHpValue.ApplyGradient = false;
            lblEntityHpValue.AutoSize = false;
            lblEntityHpValue.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            lblEntityHpValue.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
            lblEntityHpValue.Gradient = new System.Drawing.Color[]
    {
    System.Drawing.Color.Gray,
    System.Drawing.Color.Black
    };
            lblEntityHpValue.GradientAnimation = false;
            lblEntityHpValue.Location = new System.Drawing.Point(52, 60);
            lblEntityHpValue.Name = "lblEntityHpValue";
            lblEntityHpValue.Size = new System.Drawing.Size(180, 18);
            lblEntityHpValue.TabIndex = 19;
            lblEntityHpValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // Entity
            //
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            Controls.Add(lblType);
            Controls.Add(lblEntityHpValue);
            Controls.Add(label1);
            Controls.Add(lblEntityName);
            Controls.Add(progressHP);
            MinimumSize = new System.Drawing.Size(250, 96);
            Name = "Entity";
            Size = new System.Drawing.Size(250, 103);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SDUI.Controls.Label label1;
        private SDUI.Controls.Label lblEntityName;
        private SDUI.Controls.ProgressBar progressHP;
        private SDUI.Controls.Label lblType;
        private SDUI.Controls.Label lblEntityHpValue;
    }
}
