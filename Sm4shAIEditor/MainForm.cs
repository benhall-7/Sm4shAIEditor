using Sm4shAIEditor.Static;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
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
        //directories
        public static string fighterDirectory = ConfigurationManager.AppSettings.Get("fighter_directory");
        public static string workDirectory = ConfigurationManager.AppSettings.Get("work_directory");
        public static string exportDirectory = ConfigurationManager.AppSettings.Get("export_directory");

        public static Stopwatch timer = new Stopwatch();

        public MainForm()
        {
            InitializeComponent();
            this.Text = Properties.Resources.Title;
            this.Icon = Properties.Resources.FoxLogo;

            if (workDirectory == "")
                workDirectory = "workspace";
            if (exportDirectory == "")
                exportDirectory = "export";
        }
        
        private void LoadATKD(string directory)
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
                    nodeName = util.GetFileName(key);
                    TreeNode child = new TreeNode(nodeName);
                    child.Tag = nodeTag;
                    fighterNode.Nodes.Add(child);
                }
            }
        }

        private void openFighterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderSelectDialog openFighter = new FolderSelectDialog(true);
            if (Directory.Exists(fighterDirectory) && fighterDirectory != "")
                openFighter.InitialDirectory = fighterDirectory;
            if (openFighter.ShowDialog() == DialogResult.OK)
            {
                tree.AddFighters(openFighter.SelectedPaths, ref status_TB);
                UpdateTreeView();
            }
        }

        private void openAllFightersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderSelectDialog openAllFighters = new FolderSelectDialog(false);
            if (Directory.Exists(fighterDirectory) && fighterDirectory != "")
            {
                string[] fighterDirectories = Directory.EnumerateDirectories(fighterDirectory).ToArray();
                tree.AddFighters(fighterDirectories, ref status_TB);
                UpdateTreeView();
            }
            else if (openAllFighters.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] fighterDirectories = Directory.EnumerateDirectories(openAllFighters.SelectedPath).ToArray();
                tree.AddFighters(fighterDirectories, ref status_TB);
                UpdateTreeView();
            }
        }
        
        private void openWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(workDirectory))
            {
                string[] fighters = Directory.EnumerateDirectories(workDirectory).ToArray();
                foreach (string fighter in fighters)
                {
                    
                }
                UpdateTreeView();
            }
            else
                status_TB.Text += string.Format("The workspace folder '{0}' does not exist. By saving and/or diassembling files, you can create a workspace.", workDirectory) + "\r\n";
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //only the main files have a tag attribute; it stores the file directory and uses it as a unique identifier
            string nodeDirectory = (string)treeView.SelectedNode.Tag;
            if (nodeDirectory != null && !fileTabContainer.TabPages.ContainsKey(nodeDirectory))
            {
                string nodeFileName = util.GetFileName(nodeDirectory);
                if (nodeFileName == util.fileMagic.ElementAt(0).Key)
                    LoadATKD(nodeDirectory);
                else if (nodeFileName == util.fileMagic.ElementAt(1).Key ||
                    nodeFileName == util.fileMagic.ElementAt(2).Key)
                    LoadAIPD(nodeDirectory);
                else if (nodeFileName == util.fileMagic.ElementAt(3).Key)
                    LoadScript(nodeDirectory);
            }
            if (fileTabContainer.TabPages.ContainsKey(nodeDirectory))
            {
                fileTabContainer.SelectedIndex = fileTabContainer.TabPages.IndexOfKey(nodeDirectory);
            }
        }

        private void treeView_DoubleClick(object sender, EventArgs e)
        {
            //double clicking a fighter opens all their files
            TreeNode selectedNode = treeView.SelectedNode;
            if ((string)selectedNode.Tag == null)
            {
                for(int i=0; i < selectedNode.Nodes.Count; i++)
                {
                    TreeNode subNode = selectedNode.Nodes[i];
                    string subNodeDir = (string)subNode.Tag;
                    if (subNodeDir != null && !fileTabContainer.TabPages.ContainsKey(subNodeDir))
                    {
                        string nodeFileName = util.GetFileName(subNodeDir);
                        if (nodeFileName == util.fileMagic.ElementAt(0).Key)
                            LoadATKD(subNodeDir);
                        else if (nodeFileName == util.fileMagic.ElementAt(1).Key ||
                            nodeFileName == util.fileMagic.ElementAt(2).Key)
                            LoadAIPD(subNodeDir);
                        else if (nodeFileName == util.fileMagic.ElementAt(3).Key)
                            LoadScript(subNodeDir);
                    }
                    if (i == selectedNode.Nodes.Count - 1)
                    {
                        fileTabContainer.SelectedIndex = fileTabContainer.TabPages.IndexOfKey(subNodeDir);
                    }
                }
            }
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

        private void everyATKDToCSV_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            RecursiveTreeArray(treeView.Nodes, "attack_data.bin", ref nodes);
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
                        //leave out the second Uint16 because it's always 0 and probably never read
                        text += attack.FirstFrame + ",";
                        text += attack.LastFrame + ",";
                        text += attack.X1 + ",";
                        text += attack.X2 + ",";
                        text += attack.Y1 + ",";
                        text += attack.Y2 + "\r\n";
                    }
                    File.WriteAllText(path + node.Parent.Name + "_atkd.csv", text);
                }
            }
            status_TB.Text += "Disassembled attack_data to folder" + "\r\n";
        }

        //totally unnecessary since the tree never goes past a second level but idc
        private void RecursiveTreeArray(TreeNodeCollection nodes, string fileName, ref List<TreeNode> collection_added_to)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null)
                {
                    if (util.GetFileName((string)node.Tag) == fileName)
                        collection_added_to.Add(node);
                }
                if (node.Nodes.Count != 0)
                    RecursiveTreeArray(node.Nodes, fileName, ref collection_added_to);
            }
        }

        private void asmDialog_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssemblyDialog asmDialog = new AssemblyDialog();
            if (asmDialog.ShowDialog() == DialogResult.OK)
            {
                if (asmDialog.asmChoice == AssemblyDialog.Type.Assemble)
                    Assemble(asmDialog.DoATKD, asmDialog.DoAIPD, asmDialog.DoScript, asmDialog.asmScope);
                else if (asmDialog.asmChoice == AssemblyDialog.Type.Disassemble)
                    Disassemble(asmDialog.DoATKD, asmDialog.DoAIPD, asmDialog.DoScript, asmDialog.disasmScope);
            }
            asmDialog.Dispose();
        }

        private void Assemble(bool doATKD, bool doAIPD, bool doScript, AssemblyDialog.AsmScope scope)
        {
            if (!doATKD && !doAIPD && !doScript)
            {
                status_TB.Text += "Returned without assembling. No filetypes given" + "\r\n";
                return;
            }
            if (scope == AssemblyDialog.AsmScope.FromTabs)
            {
                timer.Start();

                List<TabPage> ATKDTabs = new List<TabPage>();
                List<TabPage> AIPDTabs = new List<TabPage>();
                List<TabPage> ScriptTabs = new List<TabPage>();
                //just a quick way to count the tabs for each file type
                foreach (TabPage tab in fileTabContainer.TabPages)
                {
                    string fileName = util.GetFileName(tab.Name);
                    if (fileName == util.fileMagic.ElementAt(0).Key) { ATKDTabs.Add(tab); }
                    else if (fileName == util.fileMagic.ElementAt(1).Key ||
                        fileName == util.fileMagic.ElementAt(2).Key) { AIPDTabs.Add(tab); }
                    else if (fileName == util.fileMagic.ElementAt(3).Key) { ScriptTabs.Add(tab); }
                }
                if (doATKD)
                {
                    if (ATKDTabs.Count > 0 && doATKD)
                    {
                        int exceptionCount = 0;
                        foreach (TabPage tab in ATKDTabs)
                        {
                            try
                            {
                                DataGridView dataGrid = ((DataGridView)tab.Controls[0]);
                                Tuple<UInt32, UInt32> dataTag = (Tuple<UInt32, UInt32>)dataGrid.Tag;
                                UInt32 specialMoveIndex = dataTag.Item1;
                                UInt32 specialIndexCount = dataTag.Item2;
                                //tab.Name is the file's directory, and we can get the fighter from the ai tree
                                string fighterName = tree.aiFiles[tab.Name];
                                string fileType = util.fileMagic.Keys.ElementAt(0);
                                string folderDirectory = exportDirectory + @"\" + fighterName;
                                if (!Directory.Exists(folderDirectory))
                                    Directory.CreateDirectory(folderDirectory);
                                string fileDirectory = folderDirectory + @"\" + fileType;

                                BinaryWriter binWriter = new BinaryWriter(File.Create(fileDirectory));
                                binWriter.Write(util.fileMagic[fileType]);
                                util.WriteReverseUInt32(ref binWriter, (UInt32)dataGrid.RowCount);
                                util.WriteReverseUInt32(ref binWriter, specialMoveIndex);
                                util.WriteReverseUInt32(ref binWriter, specialIndexCount);
                                foreach (DataGridViewRow attack in dataGrid.Rows)
                                {
                                    util.WriteReverseUInt16(ref binWriter, UInt16.Parse(attack.Cells[0].Value.ToString()));
                                    util.WriteReverseUInt16(ref binWriter, 0);//always 0
                                    util.WriteReverseUInt16(ref binWriter, UInt16.Parse(attack.Cells[1].Value.ToString()));
                                    util.WriteReverseUInt16(ref binWriter, UInt16.Parse(attack.Cells[2].Value.ToString()));
                                    util.WriteReverseFloat(ref binWriter, float.Parse(attack.Cells[3].Value.ToString()));
                                    util.WriteReverseFloat(ref binWriter, float.Parse(attack.Cells[4].Value.ToString()));
                                    util.WriteReverseFloat(ref binWriter, float.Parse(attack.Cells[5].Value.ToString()));
                                    util.WriteReverseFloat(ref binWriter, float.Parse(attack.Cells[6].Value.ToString()));
                                }
                                binWriter.Dispose();
                            }
                            catch (Exception e)
                            {
                                exceptionCount++;
                                status_TB.Text += string.Format("ERROR ({0}): {1}", tab.Text, e.Message) + "\r\n";
                            }
                        }
                        status_TB.Text += string.Format("Assembled {0} ATKD files to '{1}' (encountered {2} exceptions)", ATKDTabs.Count - exceptionCount, exportDirectory, exceptionCount) + "\r\n";
                    }
                    else
                        status_TB.Text += string.Format("No ATKD files to assemble.") + "\r\n";
                }
                if (doAIPD)
                {
                    if (AIPDTabs.Count > 0 && doAIPD)
                    {
                        assembleAIPD();
                    }
                    else
                        status_TB.Text += "No AIPD files to assemble." + "\r\n";
                }
                if (doScript)
                {
                    if (ScriptTabs.Count > 0 && doScript)
                    {
                        //get all the text stuff
                        foreach (TabPage tab in ScriptTabs)
                        {
                            Dictionary<UInt32, string> acts = new Dictionary<uint, string>();
                            string fighterName = tree.aiFiles[tab.Name];
                            string folderDirectory = exportDirectory + @"\" + fighterName;
                            if (!Directory.Exists(folderDirectory))
                                Directory.CreateDirectory(folderDirectory);
                            string fileDirectory = folderDirectory + @"\" + util.fileMagic.Keys.ElementAt(3);

                            TabControl actContainer = ((TabControl)tab.Controls[0]);
                            foreach (TabPage actTab in actContainer.TabPages)
                            {
                                UInt32 id = UInt32.Parse(actTab.Text, NumberStyles.HexNumber);
                                string text = ((RichTextBox)actTab.Controls[0]).Text;
                                acts.Add(id, text);
                            }
                            assembleScript(acts, fileDirectory);
                        }
                    }
                    else
                        status_TB.Text += string.Format("No Script files to assemble.") + "\r\n";
                }
                status_TB.Text += string.Format("Operation ended in {0} milliseconds", timer.Elapsed.Milliseconds) + "\r\n";
                timer.Stop();
            }
            else if (scope == AssemblyDialog.AsmScope.FromFolder)
            {
                if (Directory.Exists(workDirectory))
                {
                    timer.Start();
                    foreach (string fighterFolder in Directory.EnumerateDirectories(workDirectory).ToArray())
                    {
                        string fighterName = util.GetFileName(fighterFolder);
                        if (doATKD)
                        {
                            if (Directory.Exists(fighterFolder + @"\atkd"))
                            {
                                status_TB.Text += "I still need to add ATKD reassembly through folders" + "\r\n";
                            }
                        }
                        if (doAIPD)
                        {
                            if (Directory.Exists(fighterFolder + @"\aipd"))
                            {

                            }
                        }
                        if (doScript)
                        {
                            if (Directory.Exists(fighterFolder + @"\script"))
                            {
                                //try
                                //{
                                    string[] actIDs = File.ReadAllLines(fighterFolder + @"\script\acts.txt");
                                    Dictionary<uint, string> actData = new Dictionary<uint, string>();
                                    foreach (string actID in actIDs)
                                    {
                                        uint id = uint.Parse(actID, NumberStyles.HexNumber);
                                        actData.Add(id, File.ReadAllText(fighterFolder + @"\script\" + actID + ".txt"));
                                    }
                                    string outDir = exportDirectory + @"\" + util.GetFileName(fighterFolder);
                                    if (!Directory.Exists(outDir))
                                        Directory.CreateDirectory(outDir);
                                    outDir += @"\script.bin";
                                    assembleScript(actData, outDir);
                                //}
                                //catch (Exception e)
                                //{
                                //    status_TB.Text += string.Format("ERROR in {0}: {1}", fighterName, e.Message) + "\r\n";
                                //}
                            }
                        }
                    }
                    status_TB.Text += string.Format("Operation ended in {0} milliseconds", timer.Elapsed.Milliseconds) + "\r\n";
                    timer.Stop();
                }
                else
                    status_TB.Text += string.Format("The workspace folder '{0}' does not exist. By saving and/or diassembling files, you can create a workspace.", workDirectory) + "\r\n";
            }
        }

        private void assembleAIPD()
        {

        }

        private void assembleScript(Dictionary<UInt32, string> decompiledActs, string outDirectory)
        {
            BinaryWriter binWriter = new BinaryWriter(File.Create(outDirectory));
            //header data
            binWriter.Write((UInt32)0);//pad
            util.WriteReverseUInt32(ref binWriter, (UInt32)decompiledActs.Count);
            binWriter.Write((UInt64)0);//pad

            List<script.Act> acts = new List<script.Act>();
            foreach (UInt32 actID in decompiledActs.Keys)
            {
                acts.Add(new script.Act(actID, decompiledActs[actID]));
            }
            int positionInHeader = 0x10;
            for (int i = 0; i < acts.Count; i++)
            {
                uint actOffset = (((uint)acts.Count + 3) / 4) * 0x10 + 0x10;
                if (i > 0)
                    actOffset = (uint)binWriter.BaseStream.Length;
                actOffset = Align0x10(actOffset);
                binWriter.BaseStream.Seek(positionInHeader, SeekOrigin.Begin);
                util.WriteReverseUInt32(ref binWriter, actOffset);
                //start writing the actual script here
                binWriter.BaseStream.Seek(actOffset, SeekOrigin.Begin);
                //act header data
                util.WriteReverseUInt32(ref binWriter, acts[i].ID);
                util.WriteReverseUInt32(ref binWriter, acts[i].ScriptOffset);
                util.WriteReverseUInt32(ref binWriter, acts[i].ScriptFloatOffset);
                util.WriteReverseUInt16(ref binWriter, acts[i].VarCount);
                util.WriteReverseUInt16(ref binWriter, 0);//padding. We could have done this via 0x10 alignment but it doesn't matter
                //act commands data
                foreach (script.Act.Cmd cmd in acts[i].CmdList)
                {
                    binWriter.Write(cmd.ID);
                    binWriter.Write(cmd.ParamCount);
                    util.WriteReverseUInt16(ref binWriter, cmd.Size);
                    foreach (UInt32 param in cmd.ParamList)
                    {
                        util.WriteReverseUInt32(ref binWriter, param);
                    }
                }
                foreach (float value in acts[i].ScriptFloats.Values)
                {
                    util.WriteReverseFloat(ref binWriter, value);
                }
                positionInHeader += 4;
            }
            binWriter.Dispose();
        }
        
        private uint Align0x10(uint position)
        {
            return ((position + 0xf) / 0x10) * 0x10;
        }

        private void Disassemble(bool doATKD, bool doAIPD, bool doScript, AssemblyDialog.DisasmScope scope)
        {
            if (!doATKD && !doAIPD && !doScript)
            {
                status_TB.Text += "Returned without disassembling. No filetypes given" + "\r\n";
                return;
            }
            if (scope == AssemblyDialog.DisasmScope.FromTabs)
            {
                List<TabPage> ATKDTabs = new List<TabPage>();
                List<TabPage> AIPDTabs = new List<TabPage>();
                List<TabPage> ScriptTabs = new List<TabPage>();
                //just a quick way to count the tabs for each file type
                foreach (TabPage tab in fileTabContainer.TabPages)
                {
                    string fileName = util.GetFileName(tab.Name);
                    if (fileName == util.fileMagic.ElementAt(0).Key) { ATKDTabs.Add(tab); }
                    else if (fileName == util.fileMagic.ElementAt(1).Key ||
                        fileName == util.fileMagic.ElementAt(2).Key) { AIPDTabs.Add(tab); }
                    else if (fileName == util.fileMagic.ElementAt(3).Key) { ScriptTabs.Add(tab); }
                }
                if (doATKD && ATKDTabs.Count > 0)
                {
                    foreach(TabPage tab in ATKDTabs)
                    {

                    }
                }
                if (doAIPD && AIPDTabs.Count > 0)
                {

                }
                if (doScript && ScriptTabs.Count > 0)
                {
                    
                }
            }
            else if (scope == AssemblyDialog.DisasmScope.FromTree)
            {
                if (tree.aiFiles.Count == 0)
                {
                    status_TB.Text += "Returned without disassembling. No files in tree" + "\r\n";
                    return;
                }
                if (doScript)
                {
                    List<TreeNode> nodes = new List<TreeNode>();
                    RecursiveTreeArray(treeView.Nodes, "script.bin", ref nodes);
                    foreach (TreeNode node in nodes)
                    {
                        string path = workDirectory;
                        string fileDirectory = (string)node.Tag;
                        path += @"\" + node.Parent.Name + @"\script\";
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        script scriptFile = new script(fileDirectory);
                        StreamWriter streamWriter = new StreamWriter(File.Create(path + "acts.txt"));
                        foreach (script.Act act in scriptFile.acts.Keys)
                        {
                            streamWriter.WriteLine(act.ID.ToString("X4"));
                            string text = act.DecompAct();
                            File.WriteAllText(path + act.ID.ToString("X4") + ".txt", text);
                        }
                        streamWriter.Dispose();
                    }
                    status_TB.Text += string.Format("Disassembled scripts from tree to {0}", workDirectory) + "\r\n";
                }
            }
        }
    }
}
