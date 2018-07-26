namespace Sm4shAIEditor
{
    partial class FighterSelection
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.all_cB = new System.Windows.Forms.CheckBox();
            this.script_cB = new System.Windows.Forms.CheckBox();
            this.aipd_nfp_cB = new System.Windows.Forms.CheckBox();
            this.aipd_cB = new System.Windows.Forms.CheckBox();
            this.atkd_cB = new System.Windows.Forms.CheckBox();
            this.allFt_button = new System.Windows.Forms.Button();
            this.noFt_button = new System.Windows.Forms.Button();
            this.cancel_button = new System.Windows.Forms.Button();
            this.OK_button = new System.Windows.Forms.Button();
            this.ft_cBList = new System.Windows.Forms.CheckedListBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.all_cB);
            this.groupBox1.Controls.Add(this.script_cB);
            this.groupBox1.Controls.Add(this.aipd_nfp_cB);
            this.groupBox1.Controls.Add(this.aipd_cB);
            this.groupBox1.Controls.Add(this.atkd_cB);
            this.groupBox1.Location = new System.Drawing.Point(182, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(90, 135);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File types";
            // 
            // all_cB
            // 
            this.all_cB.AutoSize = true;
            this.all_cB.Location = new System.Drawing.Point(6, 19);
            this.all_cB.Name = "all_cB";
            this.all_cB.Size = new System.Drawing.Size(37, 17);
            this.all_cB.TabIndex = 4;
            this.all_cB.Text = "All";
            this.all_cB.UseVisualStyleBackColor = true;
            this.all_cB.CheckedChanged += new System.EventHandler(this.all_cB_CheckedChanged);
            // 
            // script_cB
            // 
            this.script_cB.AutoSize = true;
            this.script_cB.Location = new System.Drawing.Point(6, 114);
            this.script_cB.Name = "script_cB";
            this.script_cB.Size = new System.Drawing.Size(51, 17);
            this.script_cB.TabIndex = 3;
            this.script_cB.Text = "script";
            this.script_cB.UseVisualStyleBackColor = true;
            // 
            // aipd_nfp_cB
            // 
            this.aipd_nfp_cB.AutoSize = true;
            this.aipd_nfp_cB.Location = new System.Drawing.Point(6, 90);
            this.aipd_nfp_cB.Name = "aipd_nfp_cB";
            this.aipd_nfp_cB.Size = new System.Drawing.Size(76, 17);
            this.aipd_nfp_cB.TabIndex = 2;
            this.aipd_nfp_cB.Text = "param_nfp";
            this.aipd_nfp_cB.UseVisualStyleBackColor = true;
            // 
            // aipd_cB
            // 
            this.aipd_cB.AutoSize = true;
            this.aipd_cB.Location = new System.Drawing.Point(6, 66);
            this.aipd_cB.Name = "aipd_cB";
            this.aipd_cB.Size = new System.Drawing.Size(55, 17);
            this.aipd_cB.TabIndex = 1;
            this.aipd_cB.Text = "param";
            this.aipd_cB.UseVisualStyleBackColor = true;
            // 
            // atkd_cB
            // 
            this.atkd_cB.AutoSize = true;
            this.atkd_cB.Location = new System.Drawing.Point(6, 42);
            this.atkd_cB.Name = "atkd_cB";
            this.atkd_cB.Size = new System.Drawing.Size(83, 17);
            this.atkd_cB.TabIndex = 0;
            this.atkd_cB.Text = "attack_data";
            this.atkd_cB.UseVisualStyleBackColor = true;
            // 
            // allFt_button
            // 
            this.allFt_button.Location = new System.Drawing.Point(188, 154);
            this.allFt_button.Name = "allFt_button";
            this.allFt_button.Size = new System.Drawing.Size(75, 23);
            this.allFt_button.TabIndex = 5;
            this.allFt_button.Text = "All fighters";
            this.allFt_button.UseVisualStyleBackColor = true;
            this.allFt_button.Click += new System.EventHandler(this.allFt_button_Click);
            // 
            // noFt_button
            // 
            this.noFt_button.Location = new System.Drawing.Point(188, 184);
            this.noFt_button.Name = "noFt_button";
            this.noFt_button.Size = new System.Drawing.Size(75, 23);
            this.noFt_button.TabIndex = 6;
            this.noFt_button.Text = "No fighters";
            this.noFt_button.UseVisualStyleBackColor = true;
            this.noFt_button.Click += new System.EventHandler(this.noFt_button_Click);
            // 
            // cancel_button
            // 
            this.cancel_button.Location = new System.Drawing.Point(188, 324);
            this.cancel_button.Name = "cancel_button";
            this.cancel_button.Size = new System.Drawing.Size(75, 23);
            this.cancel_button.TabIndex = 8;
            this.cancel_button.Text = "Cancel";
            this.cancel_button.UseVisualStyleBackColor = true;
            this.cancel_button.Click += new System.EventHandler(this.cancel_button_Click);
            // 
            // OK_button
            // 
            this.OK_button.Location = new System.Drawing.Point(188, 295);
            this.OK_button.Name = "OK_button";
            this.OK_button.Size = new System.Drawing.Size(75, 23);
            this.OK_button.TabIndex = 9;
            this.OK_button.Text = "OK";
            this.OK_button.UseVisualStyleBackColor = true;
            this.OK_button.Click += new System.EventHandler(this.OK_button_Click);
            // 
            // ft_cBList
            // 
            this.ft_cBList.CheckOnClick = true;
            this.ft_cBList.FormattingEnabled = true;
            this.ft_cBList.Location = new System.Drawing.Point(13, 13);
            this.ft_cBList.Name = "ft_cBList";
            this.ft_cBList.Size = new System.Drawing.Size(163, 334);
            this.ft_cBList.TabIndex = 10;
            // 
            // FighterSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 361);
            this.Controls.Add(this.ft_cBList);
            this.Controls.Add(this.OK_button);
            this.Controls.Add(this.cancel_button);
            this.Controls.Add(this.noFt_button);
            this.Controls.Add(this.allFt_button);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FighterSelection";
            this.Text = "FighterSelection";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox script_cB;
        private System.Windows.Forms.CheckBox aipd_nfp_cB;
        private System.Windows.Forms.CheckBox aipd_cB;
        private System.Windows.Forms.CheckBox atkd_cB;
        private System.Windows.Forms.CheckBox all_cB;
        private System.Windows.Forms.Button allFt_button;
        private System.Windows.Forms.Button noFt_button;
        private System.Windows.Forms.Button cancel_button;
        private System.Windows.Forms.Button OK_button;
        private System.Windows.Forms.CheckedListBox ft_cBList;
    }
}