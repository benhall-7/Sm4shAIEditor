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
            //based on the way I set this up, only the main files have a tag attribute
            //the tag contains the file info (directory/name) for each file which will be used to open the file
            string[] nodeTag = (string[])treeView.SelectedNode.Tag;
            if (nodeTag != null)
            {
                if (nodeTag[1] == fileTypes[0])
                {

                }
                else if (nodeTag[1] == fileTypes[1] || nodeTag[1] == fileTypes[2])
                {

                }
                else if (nodeTag[1] == fileTypes[3])
                {

                }
            }
        }
    }
}
