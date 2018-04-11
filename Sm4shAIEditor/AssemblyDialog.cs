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
        public enum asmOption_AsmChoice
        {
            Assemble,
            Disassemble
        }
        public enum asmOption_AsmScope
        {
            FromTabs,
            FromFolder
        }
        public enum asmOption_DisasmScope
        {
            FromTabs,
            FromTree
        }
        public asmOption_AsmChoice asmChoice { get; set; }
        public asmOption_AsmScope asmScope { get; set; }
        public asmOption_DisasmScope disasmScope { get; set; }
        public bool asmOption_DisasmTreeAndTabs { get; set; }
        public bool asmOption_ATKD { get; set; }
        public bool asmOption_AIPD { get; set; }
        public bool asmOption_Script { get; set; }
        public AssemblyDialog()
        {
            asmChoice = asmOption_AsmChoice.Assemble;
            asmScope = asmOption_AsmScope.FromTabs;
            disasmScope = asmOption_DisasmScope.FromTree;
            asmOption_DisasmTreeAndTabs = false;
            asmOption_ATKD = false;
            asmOption_AIPD = false;
            asmOption_Script = false;
            InitializeComponent();
        }

        private void AssemblyDialog_Load(object sender, EventArgs e)
        {
            rB_asmFromTabs.Checked = true;
            rB_disasmFromTabs.Checked = true;
            cB_disasmTreeAndTabs.Enabled = false;

            if (asmChoice == asmOption_AsmChoice.Assemble)
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
                asmChoice = asmOption_AsmChoice.Assemble;
                gB_assemblyScope.Enabled = true;
                gB_disassemblyScope.Enabled = false;
            }
            else
            {
                asmChoice = asmOption_AsmChoice.Disassemble;
                gB_assemblyScope.Enabled = false;
                gB_disassemblyScope.Enabled = true;
            }
        }

        private void rB_asmFromTabs_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_asmFromTabs.Checked)
                asmScope = asmOption_AsmScope.FromTabs;
            else
                asmScope = asmOption_AsmScope.FromFolder;
        }

        private void rB_disasmFromTabs_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_disasmFromTabs.Checked)
            {
                disasmScope = asmOption_DisasmScope.FromTabs;
                cB_disasmTreeAndTabs.Enabled = false;
            }
            else
            {
                disasmScope = asmOption_DisasmScope.FromTree;
                cB_disasmTreeAndTabs.Enabled = true;
            }
        }

        private void cB_disasmTreeAndTabs_CheckedChanged(object sender, EventArgs e)
        {
            if (cB_disasmTreeAndTabs.Checked)
                asmOption_DisasmTreeAndTabs = true;
            else
                asmOption_DisasmTreeAndTabs = false;
        }

        private void button_accept_Click(object sender, EventArgs e)
        {
            button_accept.DialogResult = DialogResult.OK;
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
