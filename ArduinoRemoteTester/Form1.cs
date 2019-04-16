using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArduinoRemoteTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this._rmc.Start(this.comboBox1.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] portNames = SerialPort.GetPortNames();

            this.comboBox1.Items.AddRange(portNames);
            this.comboBox1.SelectedIndex = 0;

            this._rmc = new ArduinoRemote();
        }

        public ArduinoRemote _rmc;

        private void Button2_Click(object sender, EventArgs e)
        {
            this._rmc.SendCommand("Power");
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this._rmc.SendCommand("AUX");
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            this._rmc.SendCommand("Optical");
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            this._rmc.SendCommand("EcoOn");
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            this._rmc.SendCommand("VolUp");
        }
    }
}
