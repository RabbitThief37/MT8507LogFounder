using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Collections.Concurrent;
using System.Threading;

namespace MtkLogParserTest
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            string[] portNames = SerialPort.GetPortNames();
            string portName = string.Empty;

            if (portNames.Count() == 0)
            {
                MessageBox.Show(this, "COM PORT가 없습니다.\n\n장치관리자에서 확인해 주세요.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            if (portNames.Count() > 1)
            {
                frmSelectComPort selectComport = new frmSelectComPort();
                selectComport.comportNames = portNames;
                selectComport.ShowDialog();

                portName = selectComport.selectPortName;
            }
            else
            {
                portName = portNames[0];
            }

            this._logStatus = new LogStatus();

            this.txtPortName.Text = portName;

            this.timer1.Interval = 500;
            this.timer1.Start();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._logStatus.Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(isTimerInit == false)
            {
                this.timer1.Stop();

                if (this._logStatus.Start(this.txtPortName.Text) == false)
                {
                    MessageBox.Show(this, this._logStatus.ErrorMessage, "Log Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                isTimerInit = true;
                this.timer1.Interval = 1000;
                this.timer1.Start();
            }
            else
            {
                UpdateInformation();
            }
        }


        private void UpdateInformation()
        {
            if (this._logStatus.IsNewData() == false)
                return;

            this.lstResult.Items.Clear();
            
            this.lstResult.Items.Add("POWER_STATUS      : " + this._logStatus.POWER_STATUS);
            this.lstResult.Items.Add("MODEL_NAME        : " + this._logStatus.MODEL_NAME);
            this.lstResult.Items.Add("INPUT_SOURCE      : " + this._logStatus.INPUT_SOURCE);
            this.lstResult.Items.Add("VOLUME            : " + this._logStatus.VOLUME.ToString());
            this.lstResult.Items.Add("SUBWOOFER         : " + this._logStatus.SUBWOOFER.ToString());
            this.lstResult.Items.Add("BASS              : " + this._logStatus.BASS.ToString());
            this.lstResult.Items.Add("TREBLE            : " + this._logStatus.TREBLE.ToString());
            this.lstResult.Items.Add("CENTER            : " + this._logStatus.CENTER.ToString());
            this.lstResult.Items.Add("SURROUND_VOLUME   : " + this._logStatus.SURROUND_VOLUME.ToString());
            this.lstResult.Items.Add("SURROUND_BALANCE  : " + this._logStatus.SURROUND_BALANCE.ToString());
            this.lstResult.Items.Add("AV_DELAY          : " + this._logStatus.AV_DELAY.ToString());
            this.lstResult.Items.Add("MUTE              : " + this._logStatus.MUTE_STATUS);
            this.lstResult.Items.Add("HEIGHT_SPEAKERS   : " + this._logStatus.HEIGHT_SPEAKERS_STATUS);
            this.lstResult.Items.Add("HEIGHT_LEVEL      : " + this._logStatus.HEIGHT_LEVEL.ToString());
            this.lstResult.Items.Add("SURROUND          : " + this._logStatus.SURROUND_STATUS);
            this.lstResult.Items.Add("VOLUME_LEVELER    : " + this._logStatus.VOLUME_LEVELER_STATUS);
            this.lstResult.Items.Add("NIGHT_MODE        : " + this._logStatus.NIGHT_MODE_STATUS);
            this.lstResult.Items.Add("EQ                : " + this._logStatus.EQ_STATUS);
            this.lstResult.Items.Add("DEMO_MODE         : " + this._logStatus.DEMO_MODE);
        }

        public SerialPort _serialPort;
        public LogStatus _logStatus;
        private bool isTimerInit = false;
    }
}
