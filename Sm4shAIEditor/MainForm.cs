﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Sm4shAIEditor
{
    public partial class MainForm : Form
    {
        public static AITree tree = new AITree();
        private static string[] fileTypes =
        {
            "attack_data.bin",
            "param.bin",
            "param_nfp.bin",
            "script.bin"
        };

        public MainForm()
        {
            InitializeComponent();
            this.Text = Properties.Resources.Title;
            this.Icon = Properties.Resources.FoxLogo;
        }

        private void LoadFile(string fileDirectory)
        {
            string parent = Directory.GetParent(fileDirectory).FullName;
            string fileName = fileDirectory.Remove(0, parent.Length + 1);
            if (File.Exists(fileDirectory))
            {
                try
                {
                    tree.AddFile(fileDirectory, fileName);
                }
                catch (ProgramException exception)
                {
                    status_TB.Text += exception.Message + Environment.NewLine;
                }
            }
        }

        private void LoadFighter(string fighterDirectory)
        {
            string parent = Directory.GetParent(fighterDirectory).FullName;
            string fighterName = fighterDirectory.Remove(0, parent.Length + 1);
            if (Directory.Exists(fighterDirectory))
            {
                try
                {
                    tree.AddFighter(fighterDirectory, fighterName);
                }
                catch (ProgramException exception)
                {
                    status_TB.Text += exception.Message + Environment.NewLine;
                }
            }
        }

        //maybe a method to load the tab data would be better suited for the class itself
        private void LoadATKD(string directory, string fileName)
        {
            attack_data atkdFile = new attack_data(directory);

            TabPage atkdTab = new TabPage();
            atkdTab.Tag = atkdFile;
            atkdTab.Text = fileName;

            DataGridView atkdData = new DataGridView();
            atkdData.RowCount = atkdFile.EntryCount;
            atkdData.ColumnCount = 7;
            atkdData.Columns[0].HeaderText = "SubactionID";
            atkdData.Columns[1].HeaderText = "First Frame";
            atkdData.Columns[2].HeaderText = "Last Frame";
            atkdData.Columns[3].HeaderText = "X1";
            atkdData.Columns[4].HeaderText = "X2";
            atkdData.Columns[5].HeaderText = "Y1";
            atkdData.Columns[6].HeaderText = "Y2";
            foreach (DataGridViewRow row in atkdData.Rows)
            {
                row.Cells[0].Value = atkdFile.attacks[row.Index].SubactionID;
                row.Cells[1].Value = atkdFile.attacks[row.Index].FirstFrame;
                row.Cells[2].Value = atkdFile.attacks[row.Index].LastFrame;
                row.Cells[3].Value = atkdFile.attacks[row.Index].X1;
                row.Cells[4].Value = atkdFile.attacks[row.Index].X2;
                row.Cells[5].Value = atkdFile.attacks[row.Index].Y1;
                row.Cells[6].Value = atkdFile.attacks[row.Index].Y2;
            }
            atkdData.AutoSize = true;
            atkdData.Parent = atkdTab;
            atkdData.Dock = DockStyle.Fill;
            fileTabContainer.TabPages.Add(atkdTab);
        }

        private void LoadAIPD(string directory)
        {

        }

        private void LoadScript(string directory)
        {

        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = Properties.Resources.OpenFileFilter;
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadFile(openFile.FileName);
            }

            UpdateTreeView();
        }

        private void openFighterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFighter = new FolderBrowserDialog();
            if (openFighter.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadFighter(openFighter.SelectedPath);
            }

            UpdateTreeView();
        }

        private void openAllFightersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openAllFighters = new FolderBrowserDialog();
            if (openAllFighters.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] fighterDirectories = Directory.EnumerateDirectories(openAllFighters.SelectedPath).ToArray();
                foreach (string fighter in fighterDirectories)
                {
                    LoadFighter(fighter);
                }
            }

            UpdateTreeView();
        }

        private void UpdateTreeView()
        {
            treeView.Nodes.Clear();

            //basic files first
            string[][] fileInfo = tree.GetFileInfo();
            int fileCount = fileInfo.Length;
            for (int i = 0; i < fileCount; i++)
            {
                string directory = fileInfo[i][0];
                TreeNode node = new TreeNode(fileInfo[i][0]);
                node.Tag = fileInfo[i];
                treeView.Nodes.Add(node);
            }

            //fighters after
            string[] fighters = tree.GetFighterNames();
            foreach (string fighter in fighters)
            {
                string[][] fighterFileInfo = tree.GetFighterFileInfoFromName(fighter);
                int fighterFileCount = fighterFileInfo.Length;
                TreeNode[] children = new TreeNode[fighterFileCount];
                for (int i=0; i < fighterFileCount; i++)
                {
                    children[i] = new TreeNode(fighterFileInfo[i][1]);
                    children[i].Tag = fighterFileInfo[i];
                }
                TreeNode node = new TreeNode(fighter, children);
                treeView.Nodes.Add(node);
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //TODO: unload any files previously selected here (will this be automatic?)

            //based on the way I set this up, only the main files have a tag attribute
            //the tag contains the file info (directory/name) for each file which will be used to open the file
            string[] nodeTag = (string[])treeView.SelectedNode.Tag;
            if (nodeTag != null)
            {
                if (nodeTag[1] == fileTypes[0])
                    LoadATKD(nodeTag[0], nodeTag[1]);
                else if (nodeTag[1] == fileTypes[1] || nodeTag[1] == fileTypes[2])
                    LoadAIPD(nodeTag[0]);
                else if (nodeTag[1] == fileTypes[3])
                    LoadScript(nodeTag[0]);
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
    }
}
