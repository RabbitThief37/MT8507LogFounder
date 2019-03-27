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
using AudioPrecision.API;

namespace MT8507Log
{
    public partial class frmManufacturingAudioTest : Form
    {
        public frmManufacturingAudioTest()
        {
            InitializeComponent();
        }

        // 초기화 
        private void frmManufacturingAudioTest_Load(object sender, EventArgs e)
        {
            string[] portNames = SerialPort.GetPortNames();
            string portName = string.Empty;

            if (portNames.Count() == 0)
            {
                MessageBox.Show(this, "COM PORT가 없습니다.\n\n장치관리자에서 확인해 주세요.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            if (portNames.Count() == 1)
            {
                if( MessageBox.Show(this, "COM PORT가 1개 뿐이 없습니다.\n\n계속 진행하시겠습니까?", "WARNING"
                                    , MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                this.Close();
                return;
            }

            if (portNames.Count() == 1)
            {
                this.cboRmcTxSerialPorts.Items.AddRange(portNames);
                this.cboRmcTxSerialPorts.SelectedIndex = 0;

                this.cboMtkLogSerialPorts.Items.AddRange(portNames);
                this.cboMtkLogSerialPorts.SelectedIndex = 0;
            }
            else
            {
                // 첫번째 com port
                this.cboRmcTxSerialPorts.Items.AddRange(portNames);
                this.cboRmcTxSerialPorts.SelectedIndex = 0;

                // 두번째 com port
                this.cboMtkLogSerialPorts.Items.AddRange(portNames);
                this.cboMtkLogSerialPorts.SelectedIndex = 1;
            }

            try
            {
                this._apx = new APx500();
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, string.Format("APx500 Obejct가 생성되지 않았습니다.\n\n{0}",ex.Message), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        // 정상 종료
        private void frmManufacturingAudioTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            if( this._isMtkOpen )
            {
                this._logStatus.Dispose();
                this._isMtkOpen = false;
            }

            if( this._isRmcOpen )
            {
                this._rmcSerialPort.Dispose();
                this._isRmcOpen = false;
            }
        }

        // 리모콘 출력을 위한 포트 연결
        private void btnConnectRmcTxSerialPort_Click(object sender, EventArgs e)
        {
            if (this._isRmcOpen)
            {
                try
                {
                    this._rmcSerialPort.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, string.Format("{1} Port에 닫기에 실패하였습니다.\n\n{0}", ex.Message, this.cboRmcTxSerialPorts.Text)
                        , "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this._rmcSerialPort.Dispose();

                this.btnConnectRmcTxSerialPort.Text = "CONNECT";
                this.cboRmcTxSerialPorts.Enabled = true;
                this._isRmcOpen = false;

                return;
            }

            this._rmcSerialPort = new SerialPort(this.cboRmcTxSerialPorts.Text, 9600, Parity.None, 8, StopBits.One);

            try
            {
                this._rmcSerialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("{1} Port에 연결을 실패하였습니다.\n\n{0}", ex.Message, this.cboRmcTxSerialPorts.Text)
                    , "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.btnConnectRmcTxSerialPort.Text = "DISCONNECT";
            this.cboRmcTxSerialPorts.Enabled = false;
            this._isRmcOpen = true;
        }

        // MTK log 분석을 위한 클래스 
        private void btnConnectMtkSerialPort_Click(object sender, EventArgs e)
        {
            if (this._isMtkOpen)
            {
                this._logStatus.Dispose();

                this.btnConnectMtkSerialPort.Text = "DISCONNECT";
                this.cboMtkLogSerialPorts.Enabled = true;
                this._isMtkOpen = false;

                return;
            }

            this._logStatus = new LogStatus(this.cboMtkLogSerialPorts.Text);

            if (this._logStatus.Start() == false)
            {
                MessageBox.Show(this, string.Format("{1} Port에 연결을 실패하였습니다.\n\n{0}", this.cboMtkLogSerialPorts.Text, this._logStatus.ErrorMessage)
                    , "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.btnConnectMtkSerialPort.Text = "CONNECT";
            this.cboMtkLogSerialPorts.Enabled = false;
            this._isMtkOpen = true;
        }

        // APx Project 파일을 열고 준비
        private void btnOpenApxProjectFile_Click(object sender, EventArgs e)
        {
            int count = 0;

            this.openFileDialog1.InitialDirectory = Application.StartupPath;
            this.openFileDialog1.Filter = "All APx Projects (*.approj,*.apporojx)|*.approj*|APx Projects (*.approjx)|*.approjx|APx XML Projects (*.approj)|*.approj|All files (*.*)|*.*";
            this.openFileDialog1.FilterIndex = 2;
            this.openFileDialog1.RestoreDirectory = true;

            if (this.openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            this.txtApxProjectFile.Text = this.openFileDialog1.FileName;
            try
            {
                this.cboSequenceItem.Items.Clear();

                this._apx.OpenProject(this.txtApxProjectFile.Text);

                //count all of the checked signal paths and measurements in the sequence in the project file
                foreach (ISignalPath signalpath in this._apx.Sequence)
                {
                    if (signalpath.Checked == false)
                        continue;

                    //count every checked measurement 
                    foreach (ISequenceMeasurement measurement in signalpath)
                    {
                        if (measurement.Checked == false)
                            continue;

                        count++;
                        this.cboSequenceItem.Items.Add(new JumpItem(signalpath.Name, measurement.Name));
                    }

                    //select the first item in the jump to list
                    if (count == 0)
                    {
                        UpdateSequenceStep(0);

                        MessageBox.Show(this, string.Format("Signal Path & Measurement가 없다.\n\n{0}", this.txtApxProjectFile.Text)
                            , "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    UpdateSequenceStep(1);
                    this.cboSequenceItem.SelectedIndex = 0;

                    SelectMeasurement((JumpItem)this.cboSequenceItem.Items[0]);
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(this, string.Format("{0}", ex.Message), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this._apx.Visible = true;
            this.Cursor = Cursors.Default;
        }

        public void UpdateSequenceStep(int step)
        {
            this.txtSequenceStep.Text = string.Format("{0:000} / {1:000}", step, this.cboSequenceItem.Items.Count);
        }

        public void SelectMeasurement(JumpItem item)
        {
            SelectMeasurement(item.SignalPath, item.Measurement);
        }

        //a new measurement has been selected from the drop-list.  Update the text fields in the user
        //interface and extract the prompts from the sequence and display them in the instruction box
        public void SelectMeasurement(string signalPath, string measurement)
        {
            StringBuilder builder = new StringBuilder(4096);

            this.txtMeasurement.Text = measurement;
            this.rtxtOperating.Clear();

            foreach (IPromptStep prompt in this._apx.Sequence[signalPath][measurement].SequenceSteps.PromptSteps)
            {
                //extract a prompt and append it to the instructions
                builder.Append(prompt.Text);
                builder.AppendLine();
            }

            this.rtxtOperating.Text = builder.ToString();
        }



        public SerialPort _rmcSerialPort;
        public LogStatus _logStatus;
        public APx500 _apx;

        private bool _isRmcOpen = false;
        private bool _isMtkOpen = false;

    }
}
