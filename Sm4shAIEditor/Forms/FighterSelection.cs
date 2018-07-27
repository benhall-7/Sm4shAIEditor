using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sm4shAIEditor.Static;
using System.IO;

namespace Sm4shAIEditor
{
    public partial class FighterSelection : Form
    {
        string ftDir { get { return util.gameFtDir; } }
        string[] fighters { get; set; }

        public AITree.AIType[] selTypes
        {
            get
            {
                bool[] checks = new bool[] { atkd_cB.Checked, aipd_cB.Checked, aipd_nfp_cB.Checked, script_cB.Checked };
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

        public FighterSelection()
        {
            InitializeComponent();
            string[] dirs = Directory.EnumerateDirectories(ftDir).ToArray();
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
            if (all_cB.Checked)
            {
                atkd_cB.Checked = true;
                aipd_cB.Checked = true;
                aipd_nfp_cB.Checked = true;
                script_cB.Checked = true;
                atkd_cB.Enabled = false;
                aipd_cB.Enabled = false;
                aipd_nfp_cB.Enabled = false;
                script_cB.Enabled = false;
            }
            else
            {
                atkd_cB.Enabled = true;
                aipd_cB.Enabled = true;
                aipd_nfp_cB.Enabled = true;
                script_cB.Enabled = true;
            }
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
