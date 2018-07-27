namespace Sm4shAIEditor
{
    partial class Config
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
            this.label1 = new System.Windows.Forms.Label();
            this.work_tB = new System.Windows.Forms.TextBox();
            this.browseProjectDir_button = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.compile_tB = new System.Windows.Forms.TextBox();
            this.browseCompileDir_button = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.game_tB = new System.Windows.Forms.TextBox();
            this.browseGameDir_button = new System.Windows.Forms.Button();
            this.OK_button = new System.Windows.Forms.Button();
            this.apply_button = new System.Windows.Forms.Button();
            this.cancel_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Project directory:";
            // 
            // work_tB
            // 
            this.work_tB.Location = new System.Drawing.Point(13, 30);
            this.work_tB.Name = "work_tB";
            this.work_tB.Size = new System.Drawing.Size(278, 20);
            this.work_tB.TabIndex = 1;
            this.work_tB.TextChanged += new System.EventHandler(this.tB_TextChanged);
            this.work_tB.DoubleClick += new System.EventHandler(this.work_tB_DoubleClick);
            // 
            // browseProjectDir_button
            // 
            this.browseProjectDir_button.Location = new System.Drawing.Point(297, 29);
            this.browseProjectDir_button.Name = "browseProjectDir_button";
            this.browseProjectDir_button.Size = new System.Drawing.Size(75, 22);
            this.browseProjectDir_button.TabIndex = 2;
            this.browseProjectDir_button.Text = "Browse";
            this.browseProjectDir_button.UseVisualStyleBackColor = true;
            this.browseProjectDir_button.Click += new System.EventHandler(this.browseProjectDir_button_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Export/Compile directory:";
            // 
            // compile_tB
            // 
            this.compile_tB.Location = new System.Drawing.Point(13, 74);
            this.compile_tB.Name = "compile_tB";
            this.compile_tB.Size = new System.Drawing.Size(278, 20);
            this.compile_tB.TabIndex = 4;
            this.compile_tB.TextChanged += new System.EventHandler(this.tB_TextChanged);
            this.compile_tB.DoubleClick += new System.EventHandler(this.compile_tB_DoubleClick);
            // 
            // browseCompileDir_button
            // 
            this.browseCompileDir_button.Location = new System.Drawing.Point(297, 73);
            this.browseCompileDir_button.Name = "browseCompileDir_button";
            this.browseCompileDir_button.Size = new System.Drawing.Size(75, 22);
            this.browseCompileDir_button.TabIndex = 5;
            this.browseCompileDir_button.Text = "Browse";
            this.browseCompileDir_button.UseVisualStyleBackColor = true;
            this.browseCompileDir_button.Click += new System.EventHandler(this.browseCompileDir_button_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Game Fighter directory";
            // 
            // game_tB
            // 
            this.game_tB.Location = new System.Drawing.Point(13, 118);
            this.game_tB.Name = "game_tB";
            this.game_tB.Size = new System.Drawing.Size(278, 20);
            this.game_tB.TabIndex = 7;
            this.game_tB.TextChanged += new System.EventHandler(this.tB_TextChanged);
            this.game_tB.DoubleClick += new System.EventHandler(this.game_tB_DoubleClick);
            // 
            // browseGameDir_button
            // 
            this.browseGameDir_button.Location = new System.Drawing.Point(297, 117);
            this.browseGameDir_button.Name = "browseGameDir_button";
            this.browseGameDir_button.Size = new System.Drawing.Size(75, 22);
            this.browseGameDir_button.TabIndex = 8;
            this.browseGameDir_button.Text = "Browse";
            this.browseGameDir_button.UseVisualStyleBackColor = true;
            this.browseGameDir_button.Click += new System.EventHandler(this.browseGameDir_button_Click);
            // 
            // OK_button
            // 
            this.OK_button.Location = new System.Drawing.Point(135, 157);
            this.OK_button.Name = "OK_button";
            this.OK_button.Size = new System.Drawing.Size(75, 22);
            this.OK_button.TabIndex = 9;
            this.OK_button.Text = "OK";
            this.OK_button.UseVisualStyleBackColor = true;
            this.OK_button.Click += new System.EventHandler(this.OK_button_Click);
            // 
            // apply_button
            // 
            this.apply_button.Location = new System.Drawing.Point(216, 156);
            this.apply_button.Name = "apply_button";
            this.apply_button.Size = new System.Drawing.Size(75, 23);
            this.apply_button.TabIndex = 10;
            this.apply_button.Text = "Apply";
            this.apply_button.UseVisualStyleBackColor = true;
            this.apply_button.Click += new System.EventHandler(this.apply_button_Click);
            // 
            // cancel_button
            // 
            this.cancel_button.Location = new System.Drawing.Point(297, 156);
            this.cancel_button.Name = "cancel_button";
            this.cancel_button.Size = new System.Drawing.Size(75, 23);
            this.cancel_button.TabIndex = 11;
            this.cancel_button.Text = "Cancel";
            this.cancel_button.UseVisualStyleBackColor = true;
            this.cancel_button.Click += new System.EventHandler(this.cancel_button_Click);
            // 
            // Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 191);
            this.Controls.Add(this.cancel_button);
            this.Controls.Add(this.apply_button);
            this.Controls.Add(this.OK_button);
            this.Controls.Add(this.browseGameDir_button);
            this.Controls.Add(this.game_tB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.browseCompileDir_button);
            this.Controls.Add(this.compile_tB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.browseProjectDir_button);
            this.Controls.Add(this.work_tB);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Config";
            this.Text = "Config";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox work_tB;
        private System.Windows.Forms.Button browseProjectDir_button;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox compile_tB;
        private System.Windows.Forms.Button browseCompileDir_button;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox game_tB;
        private System.Windows.Forms.Button browseGameDir_button;
        private System.Windows.Forms.Button OK_button;
        private System.Windows.Forms.Button apply_button;
        private System.Windows.Forms.Button cancel_button;
    }
}