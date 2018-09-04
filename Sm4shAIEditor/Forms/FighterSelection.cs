using Sm4shAIEditor.Static;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Sm4shAIEditor
{
    public partial class FighterSelection : Form
    {
        string wDir { get { return util.workDir; } }
        string gDir { get { return util.gameFtDir; } }
        string cDir { get { return util.compDir; } }
        string[] fighters { get; set; }

        public AITree.AIType[] selTypes
        {
            get
            {
                bool[] checks = new bool[] { atkd_cB.Checked, aipd_cB.Checked, aipd_nfp_cB.Checked, script_cB.Checked };
                if (all_cB.Checked)
                    checks = new bool[] { true, true, true, true };
                List<AITree.AIType> types = new List<AITree.AIType>();
                for (int i = 0; i < checks.Length; i++)
                {
                    if (checks[i])
                        types.Add((AITree.AIType)i);
                }
                return types.ToArray();
            }
        }
        public string[] selFighters
        {
            get
            {
                List<string> sel = new List<string>();
                for (int i = 0; i < fighters.Length; i++)
                {
                    if (ft_cBList.GetItemChecked(i))
                        sel.Add(fighters[i]);
                }
                return sel.ToArray();
            }
        }

        public FighterSelection(AITree.AISource source)
        {
            InitializeComponent();
            string[] dirs;
            if (source == AITree.AISource.work) dirs = Directory.EnumerateDirectories(wDir).ToArray();
            else if (source == AITree.AISource.game_file) dirs = Directory.EnumerateDirectories(gDir).ToArray();
            else dirs = Directory.EnumerateDirectories(cDir).ToArray();
            fighters = new string[dirs.Length];
            for (int i = 0; i < dirs.Length; i++)
            {
                string name = util.GetFolderName(util.CorrectFormatFolderPath(dirs[i]));
                fighters[i] = name;
            }
            for (int i = 0; i < fighters.Length; i++)
            {
                ft_cBList.Items.Add(fighters[i], false);
            }
        }

        private void all_cB_CheckedChanged(object sender, EventArgs e)
        {
            atkd_cB.Enabled = !all_cB.Checked;
            aipd_cB.Enabled = !all_cB.Checked;
            aipd_nfp_cB.Enabled = !all_cB.Checked;
            script_cB.Enabled = !all_cB.Checked;
        }

        private void allFt_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ft_cBList.Items.Count; i++)
            {
                ft_cBList.SetItemCheckState(i, CheckState.Checked);
            }
        }

        private void noFt_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ft_cBList.Items.Count; i++)
            {
                ft_cBList.SetItemCheckState(i, CheckState.Unchecked);
            }
        }

        private void OK_button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
