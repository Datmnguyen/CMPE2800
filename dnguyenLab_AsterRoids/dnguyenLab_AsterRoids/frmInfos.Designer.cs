namespace dnguyenLab_AsterRoids
{
    partial class frmInfos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInfos));
            this.txtHelp = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtHelp
            // 
            this.txtHelp.Location = new System.Drawing.Point(43, 92);
            this.txtHelp.Multiline = true;
            this.txtHelp.Name = "txtHelp";
            this.txtHelp.ReadOnly = true;
            this.txtHelp.Size = new System.Drawing.Size(219, 175);
            this.txtHelp.TabIndex = 0;
            this.txtHelp.Text = resources.GetString("txtHelp.Text");
            this.txtHelp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // frmInfos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::dnguyenLab_AsterRoids.Properties.Resources.balloon1;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(305, 431);
            this.Controls.Add(this.txtHelp);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmInfos";
            this.Text = "Help Desk";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmInfos_FormClosing);
            this.Load += new System.EventHandler(this.frmInfos_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtHelp;
    }
}