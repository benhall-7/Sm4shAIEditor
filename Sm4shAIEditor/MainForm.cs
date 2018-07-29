using Sm4shAIEditor.Static;
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace Sm4shAIEditor
{
    public partial class MainForm : Form
    {
        public static AITree tree = new AITree();
        public static bool projectActive { get; private set; }
        //font
        public static Font scriptFont = new Font(
                    new FontFamily(ConfigurationManager.AppSettings.Get("script_font")),
                    float.Parse(ConfigurationManager.AppSettings.Get("script_font_size")));

        public MainForm()
        {
            InitializeComponent();
            this.Text = Properties.Resources.Title;
            this.Icon = Properties.Resources.FoxLogo;
            SetProjectStatus(false);
        }

        public void SetProjectStatus(bool active)
        {
            projectActive = active;
            newProjectToolStripMenuItem.Enabled = !active;
            openProjectToolStripMenuItem.Enabled = !active;
            addGameFilesToolStripMenuItem.Enabled = active;
            addCompiledFilesToolStripMenuItem.Enabled = active;
            saveToolStripMenuItem.Enabled = active;
            compileToolStripMenuItem.Enabled = active;
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
            foreach (AITree.AIFt ft in tree.fighters)
            {
                TreeNode ftNode = new TreeNode(ft.name);
                foreach (AITree.AIFt.AIFile file in ft.files)
                {
                    ftNode.Nodes.Add(AITree.AITypeToString[file.type]);
                }
                treeView.Nodes.Add(ftNode);
            }
        }
        
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckConfig())
                return;
            FighterSelection selector = new FighterSelection(false);
            if (selector.ShowDialog() != DialogResult.OK)
                return;
            tree.InitNewProject(selector.selFighters, selector.selTypes);
            UpdateTreeView();
            SetProjectStatus(true);
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckConfig())
                return;
            tree.InitOpenProject();
            UpdateTreeView();
            SetProjectStatus(true);
        }

        private void addGameFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckConfig())
                return;
            FighterSelection selector = new FighterSelection(false);
            if (selector.ShowDialog() != DialogResult.OK)
                return;
            tree.AddProjectFiles(selector.selFighters, selector.selTypes, AITree.AISource.game_file);
            UpdateTreeView();
        }

        private void addCompiledFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckConfig())
                return;
            FighterSelection selector = new FighterSelection(true);
            if (selector.ShowDialog() != DialogResult.OK)
                return;
            tree.AddProjectFiles(selector.selFighters, selector.selTypes, AITree.AISource.compiled);
            UpdateTreeView();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //NEEDS TO CHECK CHANGES AND REQUEST THE SAVE FUNCTION
            tree.fighters.Clear();
            UpdateTreeView();
            SetProjectStatus(false);
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var ft in tree.fighters)
            {
                foreach (var file in ft.files)
                {
                    string pathIn = file.folder_address;
                    string pathOut = util.compDir + ft.name + "\\";
                    aism.AssembleFolder(pathIn, pathOut);
                }
            }
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
            Config config = new Config(projectActive);
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
            util.workDir = util.CorrectFormatFolderPath(util.workDir);
            util.compDir = util.CorrectFormatFolderPath(util.compDir);
            util.gameFtDir = util.CorrectFormatFolderPath(util.gameFtDir);
            return true;
        }
    }
}
