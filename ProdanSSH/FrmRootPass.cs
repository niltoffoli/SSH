using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProdanSSH
{
    public partial class FrmRootPass : Form
    {
        public string RootPass { get; set; }

        public FrmRootPass()
        {
            InitializeComponent();
            this.ActiveControl = textRoot;
            textRoot.Focus();
        }

        public void GravaSenha()
        {
            RootPass = textRoot.Text;
            if (checkPass.Checked)
            {
                Config.Default.rootpass = RootPass;
                Config.Default.Save();
            }
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            GravaSenha();
        }

        private void textRoot_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) //se tecla pressionada for enter
            {
                GravaSenha();
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) 
        {
            if (keyData == Keys.Escape) //ESC fecha a tela
            {
                this.Hide();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
