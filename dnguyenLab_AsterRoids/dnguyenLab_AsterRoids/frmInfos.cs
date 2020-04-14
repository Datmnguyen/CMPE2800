using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dnguyenLab_AsterRoids
{
    public partial class frmInfos : Form
    {
        public frmInfos()
        {
            InitializeComponent();
        }

        private void frmInfos_Load(object sender, EventArgs e)
        {
            //txtHelp.BackColor = Color.Transparent;
            txtHelp.ForeColor = Color.LightBlue;
        }

        private void frmInfos_FormClosing(object sender, FormClosingEventArgs e)
        {
            //dont let user close the form by X
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;

                Hide();
            }
        }
    }
}
