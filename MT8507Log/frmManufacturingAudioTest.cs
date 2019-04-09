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
using System.Threading;
using System.Configuration;
using System.IO;

using AudioPrecision.API;
using ZTCK.Lib.APMeasurementHelper;

using static MT8507Log.APxInputChannelInfo;
using static ZTCK.Lib.APMeasurementHelper.ArduinoRemote;

namespace MT8507Log
{
    public partial class frmManufacturingAudioTest : Form
    {
        public frmManufacturingAudioTest()
        {
            InitializeComponent();
        }

        private void DisplayErrorMessageBox(string message)
        {
            MessageBox.Show(this, message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // 초기화 
        private void frmManufacturingAudioTest_Load(object sender, EventArgs e)
        {
            string[] portNames = SerialPort.GetPortNames();
            string portName = string.Empty;

            try
            {
                this._rmc = new ArduinoRemote();
                this._logStatus = new MtkLogStatus();

                this._config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                this._apx = new APx500();
                this._excel = new Microsoft.Office.Interop.Excel.Application();
                this._apxInputChannel = new APxInputChannelInfo();
            }
            catch (Exception ex)
            {
                DisplayErrorMessageBox(string.Format("Obejct가 생성되지 않았습니다.\n\n{0}", ex.Message));
                this.Close();
            }

            if (portNames.Count() == 0)
            {
                DisplayErrorMessageBox("COM PORT가 없습니다.\n\n장치관리자에서 확인해 주세요.");
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
                string rmcPortName = ConfigurationManager.AppSettings["rmcComPort"];
                string mtkPortName = ConfigurationManager.AppSettings["mtkComPort"];

                this.cboRmcTxSerialPorts.Items.AddRange(portNames);
                this.cboMtkLogSerialPorts.Items.AddRange(portNames);

                this.cboRmcTxSerialPorts.SelectedIndex = 0;
                this.cboMtkLogSerialPorts.SelectedIndex = 1;

                if ( rmcPortName.Length > 0 || mtkPortName.Length > 0 )
                {
                    for(int i = 0; i < portNames.Length; i++)
                    {
                        if( portNames[i] == rmcPortName )
                        {
                            this.cboRmcTxSerialPorts.SelectedIndex = i;
                            continue;
                        }

                        if (portNames[i] == mtkPortName)
                        {
                            this.cboMtkLogSerialPorts.SelectedIndex = i;
                            continue;
                        }
                    }
                }

                this.cboInputMode.Text = string.Empty;
                this.cboApxInputChannel.Text = string.Empty;
            }

            // App.config 안에 있는 내용을 적용한다.
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["arduinoFileName"]))
            {
                this.txtArduinoSourceFile.Text = ConfigurationManager.AppSettings["arduinoFileName"];
            }

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["apxFileName"]))
            {
                this.txtApxProjectFile.Text = ConfigurationManager.AppSettings["apxFileName"];
            }

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["excelFileName"]))
            {
                this.txtSpecSheetFile.Text = ConfigurationManager.AppSettings["excelFileName"];
            }
        }

        // 정상 종료
        private void frmManufacturingAudioTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._logStatus.Dispose();
            this._rmc.Dispose();

            if(this._isOpenApx)
            {
                this._apx.Exit();
            }

            if (this._isOpenExcel)
            {
                this._excelWorkbook.Close(0);
                this._excel.Application.Quit();
            }
        }

        // 리모콘 출력을 위한 포트 연결
        private void btnConnectRmcTxSerialPort_Click(object sender, EventArgs e)
        {
            if (this._rmc.IsOpen)
            {
                this._rmc.Close();

                this.btnConnectRmcTxSerialPort.Text = "CONNECT";
                this.cboRmcTxSerialPorts.Enabled = true;
                return;
            }

            if(this._rmc.Start(this.cboRmcTxSerialPorts.Text) == false)
            {
                DisplayErrorMessageBox(string.Format("{1} Port에 연결을 실패하였습니다.\n\n{0}", this.cboRmcTxSerialPorts.Text, this._rmc.ErrorMessage));
                return;
            }

            this.btnConnectRmcTxSerialPort.Text = "DISCONNECT";
            this.cboRmcTxSerialPorts.Enabled = false;

            this._config.AppSettings.Settings["rmcComPort"].Value = this.cboRmcTxSerialPorts.Text;
            this._config.Save(ConfigurationSaveMode.Modified);
        }

        // MTK log 분석을 위한 클래스 
        private void btnConnectMtkSerialPort_Click(object sender, EventArgs e)
        {
            if (this._logStatus.IsOpen)
            {
                this._logStatus.Close();

                this.btnConnectMtkSerialPort.Text = "CONNECT";
                this.cboMtkLogSerialPorts.Enabled = true;
                return;
            }

            if (this._logStatus.Start(this.cboMtkLogSerialPorts.Text) == false)
            {
                DisplayErrorMessageBox(string.Format("{1} Port에 연결을 실패하였습니다.\n\n{0}", this.cboMtkLogSerialPorts.Text, this._logStatus.ErrorMessage));
                return;
            }

            this.btnConnectMtkSerialPort.Text = "DISCONNECT";
            this.cboMtkLogSerialPorts.Enabled = false;

            this._config.AppSettings.Settings["mtkComPort"].Value = this.cboMtkLogSerialPorts.Text;
            this._config.Save(ConfigurationSaveMode.Modified);
        }

        // Arduino Source File 선택
        private void BtnOpenArduinoSourceFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtArduinoSourceFile.Text))
            {
                this.openFileDialog1.InitialDirectory = Application.StartupPath;
            }
            else
            {
                this.openFileDialog1.InitialDirectory = Path.GetDirectoryName(this.txtArduinoSourceFile.Text);
                this.openFileDialog1.FileName = Path.GetFileName(this.txtArduinoSourceFile.Text);
            }

            this.openFileDialog1.Filter = "Arduino File (*.ino)|*.ino|All files (*.*)|*.*";
            this.openFileDialog1.FilterIndex = 1;

            if (this.openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.txtArduinoSourceFile.Text = this.openFileDialog1.FileName;

            if( this._rmc.LoadCommandFromIno(this.txtArduinoSourceFile.Text) == false )
            {
                this._isArduino = false;
                DisplayErrorMessageBox(string.Format("{0} 내용이 올바르지 않습니다.\n\n{1}", Path.GetFileName(this.txtArduinoSourceFile.Text), this._rmc.ErrorMessage));
                return;
            }

            this._config.AppSettings.Settings["arduinoFileName"].Value = this.txtArduinoSourceFile.Text;
            this._config.Save(ConfigurationSaveMode.Modified);

            this._isArduino = true;
        }

        // APx Project 파일을 열고 준비
        private void btnOpenApxProjectFile_Click(object sender, EventArgs e)
        {
            int count = 0;

            if( string.IsNullOrEmpty(this.txtApxProjectFile.Text) )
            {
                this.openFileDialog1.InitialDirectory = Application.StartupPath;
            }
            else
            {
                this.openFileDialog1.InitialDirectory = Path.GetDirectoryName(this.txtApxProjectFile.Text);
                this.openFileDialog1.FileName = Path.GetFileName(this.txtApxProjectFile.Text);
            }

            this.openFileDialog1.Filter = "All APx Projects (*.approj,*.apporojx)|*.approj*|APx Projects (*.approjx)|*.approjx|APx XML Projects (*.approj)|*.approj|All files (*.*)|*.*";
            this.openFileDialog1.FilterIndex = 1;

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

                        DisplayErrorMessageBox(string.Format("Signal Path & Measurement가 없다.\n\n{0}", this.txtApxProjectFile.Text));
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
                DisplayErrorMessageBox(string.Format("{0}", ex.Message));
                return;
            }

            this._config.AppSettings.Settings["apxFileName"].Value = this.txtApxProjectFile.Text;
            this._config.Save(ConfigurationSaveMode.Modified);

            this._apx.Visible = true;
            this.Cursor = Cursors.Default;

            this.Activate();
            this._isOpenApx = true;
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

        //Spec Excel 파일
        private void btnOpenSpecSheetFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtSpecSheetFile.Text))
            {
                this.openFileDialog1.InitialDirectory = Application.StartupPath;
            }
            else
            {
                this.openFileDialog1.InitialDirectory = Path.GetDirectoryName(this.txtSpecSheetFile.Text);
                this.openFileDialog1.FileName = Path.GetFileName(this.txtSpecSheetFile.Text);
            }

            this.openFileDialog1.Filter = "Excel Files (*.xls?)|*.xls?|All files (*.*)|*.*";
            this.openFileDialog1.FilterIndex = 1;

            if (this.openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.txtSpecSheetFile.Text = this.openFileDialog1.FileName;

            try
            {
                this._excelWorkbook = this._excel.Workbooks.Open(this.txtSpecSheetFile.Text);
                this._excel.Visible = true;
                this._excel.UserControl = true;
                this._excelSheet = this._excelWorkbook.ActiveSheet;
            }
            catch (Exception ex)
            {
                DisplayErrorMessageBox(string.Format("Excel Ojbect에서 문제 발생.\n\n{0}", ex.Message));
                return;
            }

            this._config.AppSettings.Settings["excelFileName"].Value = this.txtSpecSheetFile.Text;
            this._config.Save(ConfigurationSaveMode.Modified);

            this.Activate();
            this._isOpenExcel = true;
        }

        // INPUT SOURCE 변경
        private void cboInputMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( this._isArduino == false )
            {
                SelectArduinoCommandSet();
            }

            switch ( this.cboInputMode.SelectedIndex )
            {
                case 0: this._currentInputMode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_AUX;  break;
                case 1: this._currentInputMode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_OPT;  break;
                case 2: this._currentInputMode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_HDMI; break;
                case 3: this._currentInputMode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_ARC;  break;
                case 4: this._currentInputMode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_BT;   break;
                case 5: this._currentInputMode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_USB;  break;
                default:
                    DisplayErrorMessageBox(string.Format("알 수 없는 INPUT MODE. AUX로 강제.\n\n{0} - {1}", this.cboInputMode.SelectedIndex, this.cboInputMode.Text));
                    this._currentInputMode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_AUX;
                    break;
            }

            // Arduino를 통해서 IR 명령을 전송
            if (this._rmc.SendCommand(this._currentInputMode) == false)
            {
                DisplayErrorMessageBox(string.Format("Arduino를 통한 {0} 전송에 실패했습니다.\n\n{1}", this._currentInputMode.ToString(), this._rmc.ErrorMessage));
                cboApxInputChannel.Text = string.Empty;
                return;
            }

            // MTK Log를 계속적으로 확인해서 변경될 때까지 
            if( this._logStatus.CheckMtkStatus(this._currentInputMode) == false )
            {
                DisplayErrorMessageBox(string.Format("MTK LOG에서 {0} 변경이 없습니다.\n\n{1}", this._currentInputMode.ToString(), this._logStatus.ErrorMessage));
                cboApxInputChannel.Text = string.Empty;
                return;
            }
        }

        private void SelectArduinoCommandSet()
        {
            if (string.IsNullOrEmpty(this.txtArduinoSourceFile.Text))
            {
                DialogResult result = MessageBox.Show(this, "Arduino Source Code가 지정되지 않았습니다.\n\n파일을 선택하시겠습니까?\n\n(YES - 파일 선택 , NO - 기본 명령어)"
                                                     , "SELECTION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if( result == DialogResult.Yes)
                {
                    BtnOpenArduinoSourceFile_Click(null, null);

                    if (this._rmc.LoadCommandFromIno(this.txtArduinoSourceFile.Text) == false)
                    {
                        DisplayErrorMessageBox(string.Format("{0} 내용이 올바르지 않습니다.\n\n{1}\n\n기본 명령어로 설정합니다."
                                              , Path.GetFileName(this.txtArduinoSourceFile.Text), this._rmc.ErrorMessage));

                        this._rmc.LoadCommandDefault();
                    }
                }
                else
                {
                    this._rmc.LoadCommandDefault();
                }
            }

            this._isArduino = true;
        }

        private void cboApxInputChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(this.cboApxInputChannel.SelectedIndex)
            {
                case 0:
                    this._apxInputChannel.TotalOutputChannel = APX_TOTAL_INPUT_CHANNEL.CH_3;
                    this._apxInputChannel.FrontLeft = 0;
                    this._apxInputChannel.FrontRight = 1;
                    this._apxInputChannel.Subwoofer = 2;
                    break;

                case 1:
                    this._apxInputChannel.TotalOutputChannel = APX_TOTAL_INPUT_CHANNEL.CH_6;
                    this._apxInputChannel.FrontLeft = 0;
                    this._apxInputChannel.FrontRight = 1;
                    this._apxInputChannel.FrontCenter = 2;
                    this._apxInputChannel.FrontTopLeft = 3;
                    this._apxInputChannel.FrontTopRight = 4;
                    this._apxInputChannel.Subwoofer = 5;
                    break;

                case 2:
                    this._apxInputChannel.TotalOutputChannel = APX_TOTAL_INPUT_CHANNEL.CH_8;
                    break;

                case 3: this._apxInputChannel.TotalOutputChannel = APX_TOTAL_INPUT_CHANNEL.CH_7;
                    break;

                default:
                    DisplayErrorMessageBox(string.Format("알 수 없는 CHANNELS. 강제로 FL,FR,Sub 3ch (2.1).\n\n{0} - {1}", this.cboApxInputChannel.SelectedIndex, this.cboApxInputChannel.Text));
                    this._apxInputChannel.TotalOutputChannel = APX_TOTAL_INPUT_CHANNEL.CH_3;
                    break;
            }
        }

        private Configuration _config;

        public MtkLogStatus _logStatus = null;
        public ArduinoRemote _rmc = null;
        public APx500 _apx = null;

        public Microsoft.Office.Interop.Excel.Application _excel;
        public Microsoft.Office.Interop.Excel.Workbook _excelWorkbook;
        public Microsoft.Office.Interop.Excel.Worksheet _excelSheet;

        private bool _isOpenApx = false;
        private bool _isOpenExcel = false;
        private bool _isArduino = false;

        private VIZIO_RMC_CMD _currentInputMode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_AUX;
        private APxInputChannelInfo _apxInputChannel;

    }
}
