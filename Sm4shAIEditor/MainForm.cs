using Sm4shAIEditor.Static;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Sm4shAIEditor
{
    public partial class MainForm : Form
    {
        public static AITree tree = new AITree();
        //font
        public static Font scriptFont = new Font(
                    new FontFamily(ConfigurationManager.AppSettings.Get("script_font")),
                    float.Parse(ConfigurationManager.AppSettings.Get("script_font_size")));

        public MainForm()
        {
            InitializeComponent();
            this.Text = Properties.Resources.Title;
            this.Icon = Properties.Resources.FoxLogo;
        }
        
        /*private void LoadATKD(string directory)
        {
            attack_data atkdFile = new attack_data(directory);

            TabPage atkdTab = new TabPage();
            //SPECIAL DATA
            atkdTab.Name = directory;
            string fileName = util.GetFileName(directory);
            atkdTab.Text = tree.aiFiles[directory]+"/"+fileName;

            DataGridView atkdTabData = new DataGridView();
            atkdTabData.RowCount = (int)atkdFile.EntryCount;
            atkdTabData.ColumnCount = 7;
            atkdTabData.Columns[0].HeaderText = "Subaction Index";
            atkdTabData.Columns[1].HeaderText = "First Frame";
            atkdTabData.Columns[2].HeaderText = "Last Frame";
            atkdTabData.Columns[3].HeaderText = "X1";
            atkdTabData.Columns[4].HeaderText = "X2";
            atkdTabData.Columns[5].HeaderText = "Y1";
            atkdTabData.Columns[6].HeaderText = "Y2";
            foreach (DataGridViewRow row in atkdTabData.Rows)
            {
                row.Cells[0].Value = atkdFile.attacks[row.Index].SubactionID;
                row.Cells[1].Value = atkdFile.attacks[row.Index].FirstFrame;
                row.Cells[2].Value = atkdFile.attacks[row.Index].LastFrame;
                row.Cells[3].Value = atkdFile.attacks[row.Index].X1;
                row.Cells[4].Value = atkdFile.attacks[row.Index].X2;
                row.Cells[5].Value = atkdFile.attacks[row.Index].Y1;
                row.Cells[6].Value = atkdFile.attacks[row.Index].Y2;
            }
            //SPECIAL DATA
            atkdTabData.Tag = new Tuple<UInt32, UInt32>(atkdFile.SpecialMoveIndex, atkdFile.SpecialIndexCount);

            atkdTabData.Parent = atkdTab;
            atkdTabData.Dock = DockStyle.Fill;
            atkdTabData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            atkdTabData.AutoResizeColumns();
            fileTabContainer.TabPages.Add(atkdTab);
        }

        private void LoadAIPD(string directory)
        {

        }

        private void LoadScript(string directory)
        {
            script scriptFile = new script(directory);

            TabPage entireScript = new TabPage();
            entireScript.Name = directory;
            string fileName = util.GetFileName(directory);
            entireScript.Text = tree.aiFiles[directory] + "/" + fileName;

            TabControl actTabContainer = new TabControl();

            foreach (script.Act act in scriptFile.acts.Keys)
            {
                TabPage actTab = new TabPage();
                actTab.Text = act.ID.ToString("X4");

                RichTextBox act_TB = new RichTextBox();
                
                string text = act.DecompAct();
                act_TB.Text = text;
                act_TB.Font = scriptFont;
                act_TB.Parent = actTab;
                act_TB.Dock = DockStyle.Fill;
                actTabContainer.TabPages.Add(actTab);
            }
            actTabContainer.Parent = entireScript;
            actTabContainer.Dock = DockStyle.Fill;
            fileTabContainer.TabPages.Add(entireScript);
        }*/

        private void UpdateTreeView()
        {
            treeView.Nodes.Clear();
            foreach (AITree.AIFighter ft in tree.fighters)
            {
                TreeNode ftNode = new TreeNode(ft.name);
                foreach (AITree.AIFighter.AIFile file in ft.files)
                {
                    ftNode.Nodes.Add(AITree.AITypeToString[file.type]);
                }
                treeView.Nodes.Add(ftNode);
            }
        }

        private void openFighterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void openAllFightersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckConfig())
                return;
            FighterSelection selector = new FighterSelection();
            if (selector.ShowDialog() != DialogResult.OK)
                return;
            tree.InitNewProject(selector.selFighters, selector.selTypes);
            //all files in project tree are disassembled immediately, then the tree is refreshed with files from workspace
            foreach (var ft in tree.fighters)
            {
                foreach (var file in ft.files)
                {
                    string pathIn = file.folder_address + AITree.AITypeToString[file.type] + ".bin";
                    string pathOut = util.workDirectory + ft.name + "\\" + AITree.AITypeToString[file.type] + "\\";
                    aism.DisassembleFile(pathIn, pathOut);
                }
            }
            tree.onDisasm();
            UpdateTreeView();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //basically just want to load the proper filetype with this method
        }

        private void treeView_DoubleClick(object sender, EventArgs e)
        {
            //double clicking a fighter opens all their files
        }

        private void fileTabContainer_ControlAdded(object sender, ControlEventArgs e)
        {
            if (fileTabContainer.TabCount != 0)
                fileTabContainer.Visible = true;
        }

        private void fileTabContainer_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (fileTabContainer.TabCount == 0)
                fileTabContainer.Visible = false;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config config = new Config();
            config.ShowDialog();
        }

        private bool CheckConfig()
        {
            Config config = new Config();
            if (!config.check)
            {
                if (config.ShowDialog() != DialogResult.OK)
                    return false;
            }
            util.workDirectory = util.CorrectFormatFolderPath(util.workDirectory);
            util.compileDirectory = util.CorrectFormatFolderPath(util.compileDirectory);
            util.gameFighterDirectory = util.CorrectFormatFolderPath(util.gameFighterDirectory);
            return true;
        }
    }
}
