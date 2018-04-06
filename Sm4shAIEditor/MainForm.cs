using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using Sm4shAIEditor.Filetypes;

namespace Sm4shAIEditor
{
    public partial class MainForm : Form
    {
        public static AITree tree = new AITree();
        //Runtime variable
        public static Font scriptFont = new Font(
                    new FontFamily(ConfigurationManager.AppSettings.Get("script_font")),
                    float.Parse(ConfigurationManager.AppSettings.Get("script_font_size")));
        public static string fighterDirectory = ConfigurationManager.AppSettings.Get("fighter_directory");

        public MainForm()
        {
            InitializeComponent();
            this.Text = Properties.Resources.Title;
            this.Icon = Properties.Resources.FoxLogo;
        }
        
        private void LoadFiles(string[] fileDirectories)
        {
            tree.AddFiles(fileDirectories, ref status_TB);
        }
        
        private void LoadFighters(string[] fighterDirectories)
        {
            tree.AddFighters(fighterDirectories, ref status_TB);
        }

        //maybe turn these into delegates instead of using if/else statements
        private void LoadATKD(string directory)
        {
            attack_data atkdFile = new attack_data(directory);

            TabPage atkdTab = new TabPage();
            atkdTab.Name = directory;
            string fileName = task_helper.GetFileName(directory);
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
            atkdTabData.Parent = atkdTab;
            atkdTabData.Dock = DockStyle.Fill;
            atkdTabData.ReadOnly = true;
            atkdTabData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            atkdTabData.AutoResizeColumns();
            fileTabContainer.TabPages.Add(atkdTab);
            fileTabContainer.SelectedTab = atkdTab;
        }

        private void LoadAIPD(string directory)
        {

        }

        private void LoadScript(string directory)
        {
            script scriptFile = new script(directory);

            TabPage entireScript = new TabPage();
            entireScript.Name = directory;
            string fileName = task_helper.GetFileName(directory);
            entireScript.Text = tree.aiFiles[directory] + "/" + fileName;

            TabControl actTabContainer = new TabControl();

            foreach (script.Act act in scriptFile.acts.Keys)
            {
                TabPage actTab = new TabPage();
                actTab.Text = act.ID.ToString("X4");

                RichTextBox act_TB = new RichTextBox();
                
                string text = WriteScript(act);
                act_TB.Text = text;
                act_TB.Font = scriptFont;
                act_TB.Parent = actTab;
                act_TB.Dock = DockStyle.Fill;
                actTabContainer.TabPages.Add(actTab);
            }
            actTabContainer.Parent = entireScript;
            actTabContainer.Dock = DockStyle.Fill;
            fileTabContainer.TabPages.Add(entireScript);
            fileTabContainer.SelectedTab = entireScript;
        }

        private string WriteScript(script.Act act)
        {
            string text = "";
            byte lastCmdID = 0xff;
            int ifNestLevel = 0;
            byte relID = 0xFF;
            //Excuse the mess
            for (int cmdIndex = 0; cmdIndex < act.CmdList.Count; cmdIndex++)
            {
                script.Act.Cmd cmd = act.CmdList[cmdIndex];

                //control the nested level spaces
                string ifPadding = "";
                if (cmd.ID == 8 || cmd.ID == 9)
                    ifNestLevel--;
                for (int i = 0; i < ifNestLevel; i++)
                {
                    ifPadding += "    ";
                }
                //account for the "else if" statement, which messes up the nest level
                if (((cmd.ID == 6 || cmd.ID == 7) && lastCmdID != 8) || cmd.ID == 8)
                    ifNestLevel++;
                
                string cmdString = "";
                string cmdParams = "";
                switch (cmd.ID)
                {
                    case 0x01://SetVar, uses notation [varX = Y]
                        cmdString += "var" + cmd.ParamList[0] + " = ";
                        cmdParams += act.get_script_value(cmd.ParamList[1]);
                        cmdString += cmdParams + Environment.NewLine;
                        text += ifPadding + cmdString;
                        break;
                    case 0x02://SetVec, uses notation [vecX = Y]
                        cmdString += "vec" + cmd.ParamList[0] + " = ";
                        cmdParams += act.get_script_value(cmd.ParamList[1]);
                        cmdString += cmdParams + Environment.NewLine;
                        text += ifPadding + cmdString;
                        break;
                    case 0x06://If
                    case 0x07://IfNot
                        cmdString += script_data.CmdData[0x6].Name + "(";
                        if (cmd.ID == 0x7)
                            cmdString += "!";
                        int cmdAfterIndex = 1;
                        while (cmdIndex + cmdAfterIndex < act.CmdList.Count)
                        {
                            script.Act.Cmd cmdCurr = act.CmdList[cmdIndex + cmdAfterIndex - 1];
                            cmdParams += "(" + act.get_if_chk(cmdCurr.ParamList.ToArray()) + ")";
                            //commands 0x16 to 0x19 (Or + OrNot + And + AndNot)
                            //believe it or not this next check is actually what the source code does
                            relID = (byte)(act.CmdList[cmdIndex + cmdAfterIndex].ID - 0x16);
                            if (relID <= 3)
                            {
                                cmdParams += " ";
                                if (relID / 2 == 0)
                                    cmdParams += "|| ";
                                else
                                    cmdParams += "&& ";

                                if (relID % 2 != 0)
                                    cmdParams += "!";
                                cmdAfterIndex++;
                            }
                            else
                            {
                                cmdIndex += cmdAfterIndex - 1;
                                break;
                            }
                        }
                        cmdString += cmdParams + ") {" + Environment.NewLine;
                        if (lastCmdID != 0x8)
                            text += ifPadding;
                        text += cmdString;
                        break;
                    case 0x08://Else
                        cmdString += ifPadding + "}" + Environment.NewLine + ifPadding;
                        //if next command is an "if" or "ifNot" don't put it on a separate line
                        if (act.CmdList[cmdIndex + 1].ID == 0x6 || act.CmdList[cmdIndex + 1].ID == 0x7)
                            cmdString += script_data.CmdData[cmd.ID].Name + " ";
                        else
                            cmdString += script_data.CmdData[cmd.ID].Name + " {" + Environment.NewLine;
                        text += cmdString;
                        break;
                    case 0x09://EndIf
                        cmdString += "}" + Environment.NewLine;//use the symbol instead of the name
                        text += ifPadding + cmdString;
                        break;
                    case 0x0a://SetStickRel
                    case 0x1f://SetStickAbs
                        cmdString += script_data.CmdData[cmd.ID].Name + "(";
                        for (int i = 0; i < cmd.paramCount; i++)
                        {
                            cmdParams += act.get_script_value(cmd.ParamList[i]);

                            if (i != cmd.paramCount - 1)
                                cmdParams += ", ";
                        }
                        cmdString += cmdParams + ")" + Environment.NewLine;
                        text += ifPadding + cmdString;
                        break;
                    case 0x0c://var operators
                    case 0x0d:
                    case 0x0e:
                    case 0x0f:
                    case 0x10://vec operators
                    case 0x11:
                    case 0x12:
                    case 0x13:
                        relID = (byte)(cmd.ID - 0xc);
                        if (relID < 4)
                            cmdString += "var" + cmd.ParamList[0];
                        else
                            cmdString += "vec" + cmd.ParamList[0];

                        if (relID % 4 == 0)
                            cmdString += " += ";
                        else if (relID % 4 == 1)
                            cmdString += " -= ";
                        else if (relID % 4 == 2)
                            cmdString += " *= ";
                        else
                            cmdString += " /= ";
                        
                        for (int i = 1; i < cmd.paramCount; i++)
                        {
                            cmdParams += act.get_script_value(cmd.ParamList[i]);

                            if (i != cmd.paramCount - 1)
                                cmdParams += ", ";
                        }
                        cmdString += cmdParams + Environment.NewLine;
                        text += ifPadding + cmdString;
                        break;
                    case 0x1b://SetAct
                        cmdString += script_data.CmdData[cmd.ID].Name + "(";
                        for (int i = 0; i < cmd.paramCount; i++)
                        {
                            //never uses a "get_script_value" argument so routines numbered 0x20XX will appear correctly
                            cmdParams += "0x" + cmd.ParamList[i].ToString("X");
                            if (i != cmd.paramCount - 1)
                                cmdParams += ", ";
                        }
                        cmdString += cmdParams + ")" + Environment.NewLine;
                        text += ifPadding + cmdString;
                        break;
                    case 0x1d://cliff vector stuff
                    case 0x27:
                    case 0x31:
                        cmdString += script_data.CmdData[cmd.ID].Name + "(";
                        cmdParams += "vec" + cmd.ParamList[0].ToString();
                        cmdString += cmdParams + ")" + Environment.NewLine;
                        text += ifPadding + cmdString;
                        break;
                    default:
                        cmdString += script_data.CmdData[cmd.ID].Name + "(";
                        for (int i = 0; i < cmd.paramCount; i++)
                        {
                            if (act.ScriptFloats.ContainsKey(cmd.ParamList[i]))
                                cmdParams += act.ScriptFloats[cmd.ParamList[i]];
                            else
                                cmdParams += "0x" + cmd.ParamList[i].ToString("X");

                            if (i != cmd.paramCount - 1)
                                cmdParams += ", ";
                        }
                        cmdString += cmdParams + ")" + Environment.NewLine;
                        text += ifPadding + cmdString;
                        break;
                }
                lastCmdID = cmd.ID;
            }

            return text;
        }

        private void UpdateTreeView()
        {
            treeView.Nodes.Clear();
            string nodeName;
            string nodeTag;

            string[] fileDirs = tree.aiFiles.Keys.ToArray();
            string[] fighters = tree.fighters.ToArray();
            foreach (string fighter in fighters)
            {
                nodeName = fighter;
                treeView.Nodes.Add(nodeName, nodeName);//given key is the fighter name, which allows the search method below
            }

            foreach (string key in fileDirs)
            {
                string owner = tree.aiFiles[key];
                nodeTag = key;
                if (owner == null)
                {
                    nodeName = key;
                    TreeNode node = new TreeNode(nodeName);
                    node.Tag = nodeTag;
                    treeView.Nodes.Add(node);
                }
                else
                {
                    TreeNode fighterNode = treeView.Nodes.Find(owner, false)[0];//returns array, but should only have 1 element
                    nodeName = task_helper.GetFileName(key);
                    TreeNode child = new TreeNode(nodeName);
                    child.Tag = nodeTag;
                    fighterNode.Nodes.Add(child);
                }
            }
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = Properties.Resources.OpenFileFilter;
            openFile.Multiselect = true;
            if (Directory.Exists(fighterDirectory) && fighterDirectory != "")
                openFile.InitialDirectory = fighterDirectory;
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadFiles(openFile.FileNames);
            }

            UpdateTreeView();
        }

        private void openFighterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderSelectDialog openFighter = new FolderSelectDialog(true);
            if (Directory.Exists(fighterDirectory) && fighterDirectory != "")
                openFighter.InitialDirectory = fighterDirectory;
            if (openFighter.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadFighters(openFighter.SelectedPaths);
            }

            UpdateTreeView();
        }

        private void openAllFightersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderSelectDialog openAllFighters = new FolderSelectDialog(false);
            if (Directory.Exists(fighterDirectory) && fighterDirectory != "")
            {
                string[] fighterDirectories = Directory.EnumerateDirectories(fighterDirectory).ToArray();
                LoadFighters(fighterDirectories);
            }
            else if (openAllFighters.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] fighterDirectories = Directory.EnumerateDirectories(openAllFighters.SelectedPath).ToArray();
                LoadFighters(fighterDirectories);
            }

            UpdateTreeView();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //TODO: unload any files previously selected here

            //only the main files have a tag attribute; it stores the file directory and uses it as a unique identifier
            string nodeDirectory = (string)treeView.SelectedNode.Tag;
            if (fileTabContainer.TabPages.ContainsKey(nodeDirectory))
            {
                fileTabContainer.SelectedIndex = fileTabContainer.TabPages.IndexOfKey(nodeDirectory);
            }
            else if (nodeDirectory != null)
            {
                //later on I might choose to make this into a dictionary
                string nodeFileName = task_helper.GetFileName(nodeDirectory);
                if (nodeFileName == task_helper.fileMagic.ElementAt(0).Key)
                    LoadATKD(nodeDirectory);
                else if (nodeFileName == task_helper.fileMagic.ElementAt(1).Key || nodeFileName == task_helper.fileMagic.ElementAt(2).Key)
                    LoadAIPD(nodeDirectory);
                else if (nodeFileName == task_helper.fileMagic.ElementAt(3).Key)
                    LoadScript(nodeDirectory);
            }
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {

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

        private void everyScriptToTXT_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            RecursiveArrayBuilder(treeView.Nodes, "script.bin", ref nodes);
            foreach (TreeNode node in nodes)
            {
                string path = @"script_disasm\";
                string fileDirectory = (string)node.Tag;
                string nodeParent = null;
                if (node.Parent != null)
                    nodeParent = node.Parent.Name;
                else
                    nodeParent = "any";
                path += nodeParent + @"\";

                script scriptFile = new script(fileDirectory);
                foreach (script.Act act in scriptFile.acts.Keys)
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    string text = WriteScript(act);
                    File.WriteAllText(path + act.ID.ToString("X4") + ".txt", text);
                }
            }
            status_TB.Text += "Disassembled scripts to folder" + Environment.NewLine;
        }

        private void everyATKDToCSV_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            RecursiveArrayBuilder(treeView.Nodes, "attack_data.bin", ref nodes);
            foreach (TreeNode node in nodes)
            {
                string path = @"atkd_disasm\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string fileDirectory = (string)node.Tag;
                if (node.Parent != null)
                {
                    attack_data atkdFile = new attack_data(fileDirectory);
                    string text = "";
                    foreach (attack_data.attack_entry attack in atkdFile.attacks)
                    {
                        text += attack.SubactionID + ",";
                        //leave out the second Uint16 because it's always 0 and probably never read; they only needed 3 ints
                        text += attack.FirstFrame + ",";
                        text += attack.LastFrame + ",";
                        text += attack.X1 + ",";
                        text += attack.X2 + ",";
                        text += attack.Y1 + ",";
                        text += attack.Y2 + Environment.NewLine;
                    }
                    File.WriteAllText(path + node.Parent.Name + "_atkd.csv", text);
                }
            }
            status_TB.Text += "Disassembled attack_data to folder" + Environment.NewLine;
        }

        private void RecursiveArrayBuilder(TreeNodeCollection nodes, string fileName, ref List<TreeNode> collection_added_to)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null)
                {
                    if (task_helper.GetFileName((string)node.Tag) == fileName)
                        collection_added_to.Add(node);
                }
                if (node.Nodes.Count != 0)
                    RecursiveArrayBuilder(node.Nodes, fileName, ref collection_added_to);
            }
        }
    }
}
