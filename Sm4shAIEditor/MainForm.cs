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
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = Properties.Resources.OpenFileFilter;
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadFile(openFile.FileName);
            }
        }

        private void openFighterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFighter = new FolderBrowserDialog();
            if (openFighter.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadFighter(openFighter.SelectedPath);
            }
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
            UpdateTreeView();
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
            UpdateTreeView();
        }

        private void UpdateTreeView()
        {
            treeView.Nodes.Clear();
            string[] fighters = tree.GetFighterNames();
            foreach (string fighter in fighters)
            {
                string[][] fighterFileInfo = tree.GetFighterFileInfoFromName(fighter);
                int fileCount = fighterFileInfo.Length;
                TreeNode[] children = new TreeNode[fileCount];
                for (int i=0; i < fileCount; i++)
                {
                    children[i] = new TreeNode(fighterFileInfo[i][1]);
                }
                TreeNode node = new TreeNode(fighter, children);
                treeView.Nodes.Add(node);
            }
        }
    }
}
