namespace CMPE2800_MMCalc_DanDat
{
    partial class Form1
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
            this.dgvMain = new System.Windows.Forms.DataGridView();
            this.bttnSortName = new System.Windows.Forms.Button();
            this.bttnSortChar = new System.Windows.Forms.Button();
            this.bttnSortAtomic = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtbxFormula = new System.Windows.Forms.TextBox();
            this.txtbxMolarMass = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvMain
            // 
            this.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMain.Location = new System.Drawing.Point(12, 12);
            this.dgvMain.Name = "dgvMain";
            this.dgvMain.Size = new System.Drawing.Size(536, 347);
            this.dgvMain.TabIndex = 0;
            // 
            // bttnSortName
            // 
            this.bttnSortName.Location = new System.Drawing.Point(554, 12);
            this.bttnSortName.Name = "bttnSortName";
            this.bttnSortName.Size = new System.Drawing.Size(153, 23);
            this.bttnSortName.TabIndex = 1;
            this.bttnSortName.Text = "Sort By Name";
            this.bttnSortName.UseVisualStyleBackColor = true;
            this.bttnSortName.Click += new System.EventHandler(this.bttnSortName_Click);
            // 
            // bttnSortChar
            // 
            this.bttnSortChar.Location = new System.Drawing.Point(554, 41);
            this.bttnSortChar.Name = "bttnSortChar";
            this.bttnSortChar.Size = new System.Drawing.Size(153, 23);
            this.bttnSortChar.TabIndex = 2;
            this.bttnSortChar.Text = "Single Character Symbols";
            this.bttnSortChar.UseVisualStyleBackColor = true;
            this.bttnSortChar.Click += new System.EventHandler(this.bttnSortChar_Click);
            // 
            // bttnSortAtomic
            // 
            this.bttnSortAtomic.Location = new System.Drawing.Point(554, 70);
            this.bttnSortAtomic.Name = "bttnSortAtomic";
            this.bttnSortAtomic.Size = new System.Drawing.Size(153, 23);
            this.bttnSortAtomic.TabIndex = 3;
            this.bttnSortAtomic.Text = "Sort By Atomic #";
            this.bttnSortAtomic.UseVisualStyleBackColor = true;
            this.bttnSortAtomic.Click += new System.EventHandler(this.bttnSortAtomic_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 378);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Chemical Formula:";
            // 
            // txtbxFormula
            // 
            this.txtbxFormula.Location = new System.Drawing.Point(111, 375);
            this.txtbxFormula.Name = "txtbxFormula";
            this.txtbxFormula.Size = new System.Drawing.Size(297, 20);
            this.txtbxFormula.TabIndex = 5;
            this.txtbxFormula.TextChanged += new System.EventHandler(this.txtbxFormula_TextChanged);
            // 
            // txtbxMolarMass
            // 
            this.txtbxMolarMass.Location = new System.Drawing.Point(554, 378);
            this.txtbxMolarMass.Name = "txtbxMolarMass";
            this.txtbxMolarMass.Size = new System.Drawing.Size(153, 20);
            this.txtbxMolarMass.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(445, 381);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Approx. Molar Mass:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 412);
            this.Controls.Add(this.txtbxMolarMass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtbxFormula);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bttnSortAtomic);
            this.Controls.Add(this.bttnSortChar);
            this.Controls.Add(this.bttnSortName);
            this.Controls.Add(this.dgvMain);
            this.Name = "Form1";
            this.Text = "LINQ ICA";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMain;
        private System.Windows.Forms.Button bttnSortName;
        private System.Windows.Forms.Button bttnSortChar;
        private System.Windows.Forms.Button bttnSortAtomic;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtbxFormula;
        private System.Windows.Forms.TextBox txtbxMolarMass;
        private System.Windows.Forms.Label label2;
    }
}

