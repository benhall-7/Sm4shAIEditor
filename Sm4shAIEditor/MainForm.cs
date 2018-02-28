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

        public MainForm()
        {
            InitializeComponent();
            this.Text = Properties.Resources.Title;
            this.Icon = Properties.Resources.FoxLogo;
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
                catch
                {
                    status_TB.Text += String.Format("Error loading file '{0}' in '{1}'", fileName, parent) + Environment.NewLine;
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
                catch
                {
                    status_TB.Text += String.Format("Error loading fighter '{0}'; cannot find any AI files in the directory", fighterName) + Environment.NewLine; 
                }
            }
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
                }
                TreeNode node = new TreeNode(fighter, children);
                treeView.Nodes.Add(node);
            }
        }
    }
}
