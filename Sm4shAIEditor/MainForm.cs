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

            }
        }

        private void LoadFile(string fileDirectory)
        {
            if (File.Exists(fileDirectory))
            {
                
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

                }
            }
        }
    }
}
