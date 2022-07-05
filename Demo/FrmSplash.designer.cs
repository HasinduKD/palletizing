namespace Demo
{
    partial class FrmSplash
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
            this.components = new System.ComponentModel.Container();
            this.lblV1Version = new System.Windows.Forms.Label();
            this.pnlV1BorderBottom = new System.Windows.Forms.Label();
            this.tmSplash = new System.Windows.Forms.Timer(this.components);
            this.pbV1Logo = new System.Windows.Forms.PictureBox();
            this.pnlProgressbar = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbV1Logo)).BeginInit();
            this.pnlProgressbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblV1Version
            // 
            this.lblV1Version.BackColor = System.Drawing.Color.Transparent;
            this.lblV1Version.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblV1Version.ForeColor = System.Drawing.Color.White;
            this.lblV1Version.Location = new System.Drawing.Point(150, 128);
            this.lblV1Version.Name = "lblV1Version";
            this.lblV1Version.Size = new System.Drawing.Size(225, 15);
            this.lblV1Version.TabIndex = 1;
            this.lblV1Version.Text = "Version 3.5.0.0";
            this.lblV1Version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlV1BorderBottom
            // 
            this.pnlV1BorderBottom.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pnlV1BorderBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(45)))), ((int)(((byte)(50)))));
            this.pnlV1BorderBottom.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlV1BorderBottom.ForeColor = System.Drawing.Color.LightGray;
            this.pnlV1BorderBottom.Location = new System.Drawing.Point(0, 185);
            this.pnlV1BorderBottom.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.pnlV1BorderBottom.Name = "pnlV1BorderBottom";
            this.pnlV1BorderBottom.Size = new System.Drawing.Size(525, 40);
            this.pnlV1BorderBottom.TabIndex = 92;
            this.pnlV1BorderBottom.Text = "ABB™, All Rights Reserved";
            this.pnlV1BorderBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmSplash
            // 
            this.tmSplash.Enabled = true;
            this.tmSplash.Interval = 15;
            this.tmSplash.Tick += new System.EventHandler(this.TmSplash_Tick);
            // 
            // pbV1Logo
            // 
            this.pbV1Logo.BackgroundImage = global::Demo.Properties.Resources.Logo;
            this.pbV1Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbV1Logo.Location = new System.Drawing.Point(150, 25);
            this.pbV1Logo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pbV1Logo.Name = "pbV1Logo";
            this.pbV1Logo.Size = new System.Drawing.Size(225, 100);
            this.pbV1Logo.TabIndex = 0;
            this.pbV1Logo.TabStop = false;
            // 
            // pnlProgressbar
            // 
            this.pnlProgressbar.Controls.Add(this.panel2);
            this.pnlProgressbar.Location = new System.Drawing.Point(0, 181);
            this.pnlProgressbar.Name = "pnlProgressbar";
            this.pnlProgressbar.Size = new System.Drawing.Size(525, 4);
            this.pnlProgressbar.TabIndex = 94;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(171)))), ((int)(((byte)(242)))));
            this.panel2.Location = new System.Drawing.Point(0, 1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(40, 5);
            this.panel2.TabIndex = 1;
            // 
            // FrmSplash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(525, 225);
            this.Controls.Add(this.pnlProgressbar);
            this.Controls.Add(this.pnlV1BorderBottom);
            this.Controls.Add(this.lblV1Version);
            this.Controls.Add(this.pbV1Logo);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmSplash";
            this.Opacity = 0.95D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Application Login";
            this.Load += new System.EventHandler(this.FrmSplash_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbV1Logo)).EndInit();
            this.pnlProgressbar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbV1Logo;
        private System.Windows.Forms.Label lblV1Version;
        private System.Windows.Forms.Label pnlV1BorderBottom;
        private System.Windows.Forms.Timer tmSplash;
        private System.Windows.Forms.Panel pnlProgressbar;
        private System.Windows.Forms.Panel panel2;
    }
}