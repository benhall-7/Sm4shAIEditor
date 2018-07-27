using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sm4shAIEditor.Static;
using System.IO;
using System.Windows.Forms;

namespace Sm4shAIEditor
{
    public partial class Config : Form
    {
        string workDir
        {
            get { return work_tB.Text; }
            set { work_tB.Text = value; }
        }
        string compileDir
        {
            get { return compile_tB.Text; }
            set { compile_tB.Text = value; }
        }
        string gameFighterDir
        {
            get { return game_tB.Text; }
            set { game_tB.Text = value; }
        }
        public bool check
        {
            get
            {
                return (workDir != ""
                    && compileDir != ""
                    && gameFighterDir != ""
                    && Directory.Exists(workDir)
                    && Directory.Exists(compileDir)
                    && Directory.Exists(gameFighterDir));
            }
        }

        public Config()
        {
            InitializeComponent();
            CheckOK();
            workDir = util.workDirectory;
            compileDir = util.compileDirectory;
            gameFighterDir = util.gameFighterDirectory;
        }
        public Config(bool disableWork)
        {
            InitializeComponent();
            CheckOK();
            workDir = util.workDirectory;
            compileDir = util.compileDirectory;
            gameFighterDir = util.gameFighterDirectory;
            if (disableWork)
            {
                work_tB.Enabled = false;
                browseProjectDir_button.Enabled = false;
            }
        }

        private void CheckOK()
        {
            if (check) OK_button.Enabled = true;
            else OK_button.Enabled = false;
        }

        private void tB_TextChanged(object sender, EventArgs e)
        {
            CheckOK();
        }

        private void apply_button_Click(object sender, EventArgs e)
        {
            util.workDirectory = workDir;
            util.compileDirectory = compileDir;
            util.gameFighterDirectory = gameFighterDir;
        }

        private void OK_button_Click(object sender, EventArgs e)
        {
            apply_button_Click(sender, e);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void browseProjectDir_button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                workDir = dialog.SelectedPath;
            }
        }

        private void browseCompileDir_button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                compileDir = dialog.SelectedPath;
            }
        }

        private void browseGameDir_button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                gameFighterDir = dialog.SelectedPath;
            }
        }

        private void work_tB_DoubleClick(object sender, EventArgs e)
        {
            work_tB.SelectionStart = 0;
            work_tB.SelectionLength = workDir.Length;
        }

        private void compile_tB_DoubleClick(object sender, EventArgs e)
        {
            compile_tB.SelectionStart = 0;
            compile_tB.SelectionLength = compileDir.Length;
        }

        private void game_tB_DoubleClick(object sender, EventArgs e)
        {
            game_tB.SelectionStart = 0;
            game_tB.SelectionLength = gameFighterDir.Length;
        }
    }
}
