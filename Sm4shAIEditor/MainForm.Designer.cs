namespace Sm4shAIEditor
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFighterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openAllFightersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAndCloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView = new System.Windows.Forms.TreeView();
            this.status_TB = new System.Windows.Forms.RichTextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAndCloseToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem,
            this.openFighterToolStripMenuItem,
            this.openAllFightersToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.openFileToolStripMenuItem.Text = "Open File";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // openFighterToolStripMenuItem
            // 
            this.openFighterToolStripMenuItem.Name = "openFighterToolStripMenuItem";
            this.openFighterToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.openFighterToolStripMenuItem.Text = "Open Fighter";
            this.openFighterToolStripMenuItem.Click += new System.EventHandler(this.openFighterToolStripMenuItem_Click);
            // 
            // openAllFightersToolStripMenuItem
            // 
            this.openAllFightersToolStripMenuItem.Name = "openAllFightersToolStripMenuItem";
            this.openAllFightersToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.openAllFightersToolStripMenuItem.Text = "Open All Fighters";
            this.openAllFightersToolStripMenuItem.Click += new System.EventHandler(this.openAllFightersToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveAndCloseToolStripMenuItem
            // 
            this.saveAndCloseToolStripMenuItem.Name = "saveAndCloseToolStripMenuItem";
            this.saveAndCloseToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.saveAndCloseToolStripMenuItem.Text = "Save and Close";
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeView.Location = new System.Drawing.Point(12, 27);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(200, 300);
            this.treeView.TabIndex = 1;
            // 
            // status_TB
            // 
            this.status_TB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.status_TB.BackColor = System.Drawing.SystemColors.InfoText;
            this.status_TB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.status_TB.ForeColor = System.Drawing.SystemColors.Info;
            this.status_TB.Location = new System.Drawing.Point(12, 333);
            this.status_TB.Name = "status_TB";
            this.status_TB.ReadOnly = true;
            this.status_TB.Size = new System.Drawing.Size(760, 116);
            this.status_TB.TabIndex = 2;
            this.status_TB.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.status_TB);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(400, 350);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFighterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAndCloseToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ToolStripMenuItem openAllFightersToolStripMenuItem;
        private System.Windows.Forms.RichTextBox status_TB;
    }
}

