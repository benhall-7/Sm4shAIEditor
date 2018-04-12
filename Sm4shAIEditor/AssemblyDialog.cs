using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sm4shAIEditor
{
    public partial class AssemblyDialog : Form
    {
        public enum Type
        {
            Assemble,
            Disassemble
        }
        public enum AsmScope
        {
            FromTabs,
            FromFolder
        }
        public enum DisasmScope
        {
            FromTabs,
            FromTree
        }
        public Type asmChoice { get; set; }
        public AsmScope asmScope { get; set; }
        public DisasmScope disasmScope { get; set; }
        public bool DisasmTreeAndTabs { get; set; }
        public bool DoATKD { get; set; }
        public bool DoAIPD { get; set; }
        public bool DoScript { get; set; }
        public AssemblyDialog()
        {
            asmChoice = Type.Assemble;
            asmScope = AsmScope.FromTabs;
            disasmScope = DisasmScope.FromTree;
            DisasmTreeAndTabs = false;
            DoATKD = false;
            DoAIPD = false;
            DoScript = false;
            InitializeComponent();
        }

        private void AssemblyDialog_Load(object sender, EventArgs e)
        {
            rB_asmFromTabs.Checked = true;
            rB_disasmFromTabs.Checked = true;
            cB_disasmTreeAndTabs.Enabled = false;

            if (asmChoice == Type.Assemble)
            {
                rB_assemble.Checked = true;
                gB_assemblyScope.Enabled = true;
                gB_disassemblyScope.Enabled = false;
            }
            else
            {
                rB_disassemble.Checked = true;
                gB_assemblyScope.Enabled = false;
                gB_disassemblyScope.Enabled = true;
            }
        }

        private void rB_assemble_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_assemble.Checked)
            {
                asmChoice = Type.Assemble;
                gB_assemblyScope.Enabled = true;
                gB_disassemblyScope.Enabled = false;
            }
            else
            {
                asmChoice = Type.Disassemble;
                gB_assemblyScope.Enabled = false;
                gB_disassemblyScope.Enabled = true;
            }
        }

        private void rB_asmFromTabs_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_asmFromTabs.Checked)
                asmScope = AsmScope.FromTabs;
            else
                asmScope = AsmScope.FromFolder;
        }

        private void rB_disasmFromTabs_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_disasmFromTabs.Checked)
            {
                disasmScope = DisasmScope.FromTabs;
                cB_disasmTreeAndTabs.Enabled = false;
            }
            else
            {
                disasmScope = DisasmScope.FromTree;
                cB_disasmTreeAndTabs.Enabled = true;
            }
        }

        private void cB_disasmTreeAndTabs_CheckedChanged(object sender, EventArgs e)
        {
            if (cB_disasmTreeAndTabs.Checked)
                DisasmTreeAndTabs = true;
            else
                DisasmTreeAndTabs = false;
        }

        private void button_accept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
