using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MT8507Log
{
    public partial class frmSelectComPort : Form
    {
        public frmSelectComPort()
        {
            InitializeComponent();
        }

        private void frmSelectComPort_Load(object sender, EventArgs e)
        {
            this.cboComport.Items.AddRange(this.comportNames);
            this.cboComport.SelectedIndex = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.selectPortName = this.cboComport.SelectedItem.ToString();
            this.Close();
        }

        public string[] comportNames;
        public string selectPortName;
    }
}
