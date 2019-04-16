using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;
using System.IO;

using ZTCK.Lib.APMeasurementHelper;

using static ZTCK.Lib.APMeasurementHelper.ArduinoRemote;
using static ZTCK.Lib.APMeasurementHelper.AppConfigHandler;
using System.Threading;

namespace AutoTestATS_524
{
    public partial class Form1 : Form
    {
        public const int MTK_LOG_CONFIRM_COUNT = 20;
        public const int MTK_LOG_RETRY_COUNT = 3;

        private AppConfigHandler _appConfigHandler = null;
        private MtkLogStatus _logStatus = null;
        private ArduinoRemote _rmc = null;

        private Thread _progressTestThread;
        private bool _progressThreadContinue = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void DisplayErrorMessageBox(string message)
        {
            MessageBox.Show(this, message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] portNames = SerialPort.GetPortNames();
            string portName = string.Empty;

            try
            {
                this._rmc = new ArduinoRemote();
                this._logStatus = new MtkLogStatus(Application.StartupPath);
                this._appConfigHandler = new AppConfigHandler();

                this._progressThreadContinue = true;
                this._progressTestThread = new Thread(new ThreadStart(this.ProgressTestThreadFunction))
                {
                    Priority = ThreadPriority.Normal
                };
                this._progressTestThread.Name = "ProcessQueue" + this._progressTestThread.ManagedThreadId.ToString();

                this._logStatus.SetVerifyStatusCounter = MTK_LOG_CONFIRM_COUNT;
                this._logStatus.SetCommandRetryCounter = MTK_LOG_RETRY_COUNT;
            }
            catch (Exception ex)
            {
                DisplayErrorMessageBox(string.Format("Obejct가 생성되지 않았습니다.\n\n{0}", ex.Message));
                this.Close();
                return;
            }

            if (portNames.Count() == 0)
            {
                DisplayErrorMessageBox("COM PORT가 없습니다.\n\n장치관리자에서 확인해 주세요.");
                this.Close();
                return;
            }

            if (portNames.Count() == 1)
            {
                if (MessageBox.Show(this, "COM PORT가 1개 뿐이 없습니다.\n\n계속 진행하시겠습니까?", "WARNING"
                                        , MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                {
                    this.Close();
                    return;
                }
            }

            if (this._appConfigHandler.MakeKeys() == false)
            {
                DisplayErrorMessageBox(string.Format("app.config 환경을 구성하는데 실패했습니다.\n\n{0}", this._appConfigHandler.ErrorMessage));
                this.Close();
                return;
            }

            if (portNames.Count() == 1)
            {
                this.cboMtkComport.Items.AddRange(portNames);
                this.cboMtkComport.SelectedIndex = 0;

                this.cboRmcComport.Items.AddRange(portNames);
                this.cboRmcComport.SelectedIndex = 0;
            }
            else
            {
                string mtkPortName = this._appConfigHandler.Get(APP_CONFIG_KEY.mtkComPort);
                string rmcPortName = this._appConfigHandler.Get(APP_CONFIG_KEY.rmcComPort);

                this.cboMtkComport.Items.AddRange(portNames);
                this.cboRmcComport.Items.AddRange(portNames);

                this.cboMtkComport.SelectedIndex = 0;
                this.cboRmcComport.SelectedIndex = 1;

                if (string.IsNullOrEmpty(rmcPortName) == false || string.IsNullOrEmpty(mtkPortName) == false)
                {
                    for (int i = 0; i < portNames.Length; i++)
                    {
                        if (portNames[i] == mtkPortName)
                        {
                            this.cboMtkComport.SelectedIndex = i;
                            continue;
                        }

                        if (portNames[i] == rmcPortName)
                        {
                            this.cboRmcComport.SelectedIndex = i;
                            continue;
                        }
                    }
                }
            }

            // App.config 안에 있는 내용을 적용한다.
            this.txtArduinoFileName.Text = this._appConfigHandler.Get(APP_CONFIG_KEY.arduinoFileName);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._progressThreadContinue = false;
            if (this._progressTestThread.ThreadState != ThreadState.Stopped)
            {
                this._progressTestThread.Abort();
            }

            this._logStatus.Dispose();
            this._rmc.Dispose();
        }

        private void BtnSelectArduinoFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtArduinoFileName.Text))
            {
                this.openFileDialog1.InitialDirectory = Application.StartupPath;
            }
            else
            {
                this.openFileDialog1.InitialDirectory = Path.GetDirectoryName(this.txtArduinoFileName.Text);
                this.openFileDialog1.FileName = Path.GetFileName(this.txtArduinoFileName.Text);
            }

            this.openFileDialog1.Filter = "Arduino File (*.ino)|*.ino|All files (*.*)|*.*";
            this.openFileDialog1.FilterIndex = 1;

            if (this.openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.txtArduinoFileName.Text = this.openFileDialog1.FileName;
            this._appConfigHandler.Save(APP_CONFIG_KEY.arduinoFileName, this.txtArduinoFileName.Text);
        }

        //----------------------------------------------------------------------------------------------
        // START !!!
        private void BtnTestStart_Click(object sender, EventArgs e)
        //----------------------------------------------------------------------------------------------
        {
            if (this.btnTestStart.Text == "START")
            {
                this.btnTestStart.Text = "STOP";

                this.txtModelName.Text = string.Empty;
                this.txtULIVersion.Text = string.Empty;
                this.txtCurrrentJob.Text = string.Empty;
                this.txtStartTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                // OPEN MTK
                this.txtCurrrentJob.Text = "MTK log parser start.";
                Application.DoEvents();
                if (MtkLogParserStart() == false)
                    return;

                // OPEN RMC
                this.txtCurrrentJob.Text = "Vizio RMC arduino start.";
                Application.DoEvents();
                if (VizioRmcStart() == false)
                    return;

                // LOAD RMC COMMAND 
                if (LoadVizioRmcCommand() == false)
                    return;

                // Check Ready -> Send Power
                if (ReadyForTest() == false)
                    return;

                this.txtModelName.Text = this._logStatus.MODEL_NAME;
                this.txtULIVersion.Text = this._logStatus.ULI_VERSION;
                this.txtCurrrentJob.Text = string.Format("MCU:{0},DSP:{1},HDMI:{2},eARC{3}", this._logStatus.MODULE_VERSION[0]
                    , this._logStatus.MODULE_VERSION[1], this._logStatus.MODULE_VERSION[2], this._logStatus.MODULE_VERSION[3]);

                this._progressThreadContinue = true;
                this._progressTestThread.Start();
            }
            else
            {
                this._progressThreadContinue = false;

                if (this._logStatus.IsOpen)
                {
                    this._logStatus.Close();
                }

                if (this._rmc.IsOpen)
                {
                    this._rmc.Close();
                }

                this.cboMtkComport.Enabled = true;
                this.cboRmcComport.Enabled = true;
                this.btnTestStart.Text = "START";
            }
        }

        public bool MtkLogParserStart()
        {
            if (this._logStatus.IsOpen)
            {
                this._logStatus.Close();
            }

            if (this._logStatus.Start(this.cboMtkComport.Text) == false)
            {
                DisplayErrorMessageBox(string.Format("{1} Port에 연결을 실패하였습니다.\n\n{0}", this.cboMtkComport.Text, this._logStatus.ErrorMessage));
                return false;
            }

            this.cboMtkComport.Enabled = false;
            this._appConfigHandler.Save(APP_CONFIG_KEY.mtkComPort, this.cboMtkComport.Text);

            return true;
        }

        public bool VizioRmcStart()
        {
            if (this._rmc.IsOpen)
            {
                this._rmc.Close();
            }

            if (this._rmc.Start(this.cboRmcComport.Text) == false)
            {
                DisplayErrorMessageBox(string.Format("{1} Port에 연결을 실패하였습니다.\n\n{0}", this.cboRmcComport.Text, this._rmc.ErrorMessage));
                return false;
            }

            this.cboRmcComport.Enabled = false;
            this._appConfigHandler.Save(APP_CONFIG_KEY.rmcComPort, this.cboRmcComport.Text);

            return true;
        }

        public bool LoadVizioRmcCommand()
        {
            if (string.IsNullOrEmpty(this.txtArduinoFileName.Text))
            {
                DialogResult result = MessageBox.Show(this, "Arduino Source Code가 지정되지 않았습니다.\n\n파일을 선택하시겠습니까?\n\n(YES - 파일 선택 , NO - 기본 명령어)"
                                                        , "SELECTION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    this._rmc.LoadCommandDefault();
                    return true;
                }

                BtnSelectArduinoFile_Click(null, null);
            }

            if (string.IsNullOrEmpty(this.txtArduinoFileName.Text))
                return false;

            if (this._rmc.LoadCommandFromIno(this.txtArduinoFileName.Text) == false)
            {
                DialogResult result = MessageBox.Show(this
                                       , string.Format("{0} 내용이 올바르지 않습니다.\n\n{1}\n\n파일을 선택하시겠습니까?\n\n(YES - 파일 선택 , NO - 기본 명령어)"
                                       , Path.GetFileName(this.txtArduinoFileName.Text), this._rmc.ErrorMessage), "SELECTION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    this._rmc.LoadCommandDefault();
                    return true;
                }

                return false;
            }

            return true;
        }

        public bool ReadyForTest()
        {
            int count = 0;

            this.txtFailedCounter.Text = "0";
            this.txtFailedLastTime.Text = string.Empty;
            this.txtSuccessCounter.Text = "0";

            this.txtCurrrentJob.Text = "READY[1 / 4] - POWER";
            Application.DoEvents();

            if (this._rmc.SendCommand(VIZIO_RMC_CMD.VIZIO_RMC_CMD_POWER) == false)
            {
                DisplayErrorMessageBox(string.Format("Arduino를 통한 POWER 전송에 실패했습니다.\n\n{0}", this._rmc.ErrorMessage));
                return false;
            }

            if (this._logStatus.CheckMtkStatus(ref this._progressThreadContinue) == false)
            {
                DisplayErrorMessageBox(string.Format("MTK LOG에서 POWER 변경이 없습니다.\n\n{0}", this._logStatus.ErrorMessage));
                return false;
            }

            if (this._logStatus.POWER_STATUS == "OFF")
            {
                count = 0;
                while (this._logStatus.POWER_DC_FINAL_OFF == false && this._logStatus.POWER_DC_FAKE_STANDBY_OFF == false)
                {
                    count++;
                    Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

                    if (count == 200)
                        break;
                }

                this.txtCurrrentJob.Text = "READY[2 / 4] - CHECK POWER OFF";
                Application.DoEvents();

                if (this._logStatus.POWER_DC_FINAL_OFF == false)
                {
                    if (this._logStatus.POWER_DC_FAKE_STANDBY_OFF == false)
                    {
                        DisplayErrorMessageBox("DC OFF가 완전하게 되지 않았다. 확인요망!!!");
                        return false;
                    }
                }
                else
                {
                    Thread.Sleep(2000);
                }

                this.txtCurrrentJob.Text = "READY[2 / 4] - POWER ON";
                Application.DoEvents();

                if (this._rmc.SendCommand(VIZIO_RMC_CMD.VIZIO_RMC_CMD_POWER) == false)
                {
                    DisplayErrorMessageBox(string.Format("Arduino를 통한 POWER 전송에 실패했습니다.\n\n{0}", this._rmc.ErrorMessage));
                    return false;
                }
            }

            this.txtCurrrentJob.Text = "READY[3 / 4] - CHECK POWER ON & VERSION";
            Application.DoEvents();

            count = 0;
            while (this._logStatus.POWER_DC_FINAL_ON == false)
            {
                count++;
                Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

                if (count == 200)
                    break;
            }

            if (this._logStatus.POWER_DC_FINAL_ON == false)
            {
                DisplayErrorMessageBox("DC ON가 완전하게 되지 않았다. VERSION이 없다.");
                return false;
            }

            this.txtCurrrentJob.Text = "READY[4 / 4] - ECO MODE ON";
            Application.DoEvents();

            if (this._rmc.SendCommand(VIZIO_RMC_CMD.VIZIO_RMC_CMD_ECO_ON) == false)
            {
                DisplayErrorMessageBox(string.Format("Arduino를 통한 POWER 전송에 실패했습니다.\n\n{0}", this._rmc.ErrorMessage));
                return false;
            }

            Thread.Sleep(2000); //////////////////////////////////////////////////////////////////////////////////////////

            if (this._rmc.SendCommand(VIZIO_RMC_CMD.VIZIO_RMC_CMD_ECO_ON) == false)
            {
                DisplayErrorMessageBox(string.Format("Arduino를 통한 POWER 전송에 실패했습니다.\n\n{0}", this._rmc.ErrorMessage));
                return false;
            }

            Thread.Sleep(2000); //////////////////////////////////////////////////////////////////////////////////////////

            return true;
        }

        //-------------------------------------//
        // TEST PROCESS THREAD 
        //------------------------------------//
        private void ProgressTestThreadFunction()
        {
            VIZIO_RMC_CMD ecomode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_ECO_OFF;
            int count = 0;
            int countFailed = 0;
            int countSuccess = 0;

            while ( this._progressThreadContinue )
            {
                if (ecomode == VIZIO_RMC_CMD.VIZIO_RMC_CMD_ECO_ON)
                {
                    ecomode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_ECO_OFF;
                    UpdateCurrentJobTextbox("( 1 / 5 ) ECO MODE OFF");
                }
                else
                {
                    ecomode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_ECO_ON;
                    UpdateCurrentJobTextbox("( 1 / 5 ) ECO MODE ON");
                }

                if (this._rmc.SendCommand(ecomode) == false)
                {
                    UpdateCurrentJobTextbox("ERR-" + this._rmc.ErrorMessage);
                    break;
                }

                count = 0;
                while (this._progressThreadContinue == true)
                {
                    count++;
                    Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

                    if (count == 20)
                        break;
                }

                if (this._progressThreadContinue == false)
                    break;

                if (this._rmc.SendCommand(ecomode) == false)
                {
                    UpdateCurrentJobTextbox("ERR-" + this._rmc.ErrorMessage);
                    break;
                }

                if (this._logStatus.CheckMtkStatus(ref this._progressThreadContinue) == false)
                {
                    UpdateCurrentJobTextbox("ERR-" + this._logStatus.ErrorMessage);
                    break;
                }

                // DC OFF
                UpdateCurrentJobTextbox("( 2 / 5 ) DC OFF");
                if (this._rmc.SendCommand(VIZIO_RMC_CMD.VIZIO_RMC_CMD_POWER) == false)
                {
                    UpdateCurrentJobTextbox("ERR-" + this._rmc.ErrorMessage);
                    break;
                }

                if (this._logStatus.CheckMtkStatus(ref this._progressThreadContinue) == false)
                {
                    UpdateCurrentJobTextbox("ERR-" + this._logStatus.ErrorMessage);
                    break;
                }

                // DC ON
                if( ecomode == VIZIO_RMC_CMD.VIZIO_RMC_CMD_ECO_OFF )
                {
                    UpdateCurrentJobTextbox("( 3 / 5 ) CHECK FAKE STANDBY");
                    count = 0;
                    while (this._logStatus.POWER_DC_FAKE_STANDBY_OFF == false && this._progressThreadContinue == true)
                    {
                        count++;
                        Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

                        if (count == 100)
                            break;
                    }

                    if (this._progressThreadContinue == false)
                        break;

                    if (this._logStatus.POWER_DC_FAKE_STANDBY_OFF == false)
                    {
                        UpdateCurrentJobTextbox("ERR - FAKE STANDBY가 완전하게 되지 않았다. 확인요망!!!");
                        break;
                    }
                }
                else
                {
                    UpdateCurrentJobTextbox("( 3 / 5 ) CHECK DC OFF");
                    count = 0;
                    while (this._logStatus.POWER_DC_FINAL_OFF == false && this._progressThreadContinue == true)
                    {
                        count++;
                        Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

                        if (count == 300)
                            break;
                    }
                    if (this._progressThreadContinue == false)
                        break;

                    count = 0;
                    while (this._progressThreadContinue == true)
                    {
                        count++;
                        Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

                        if (count == 30)
                            break;
                    }
                    if (this._progressThreadContinue == false)
                        break;

                    if (this._logStatus.POWER_DC_FINAL_OFF == false)
                    {
                        UpdateCurrentJobTextbox("ERR - DC OFF가 완전하게 되지 않았다. 확인요망!!!");
                        break;
                    }
                }

                UpdateCurrentJobTextbox("( 4 / 5 ) DC ON");
                if (this._rmc.SendCommand(VIZIO_RMC_CMD.VIZIO_RMC_CMD_POWER) == false)
                {
                    UpdateCurrentJobTextbox("ERR-" + this._rmc.ErrorMessage);
                    break;
                }

                if (this._logStatus.CheckMtkStatus(ref this._progressThreadContinue) == false)
                {
                    UpdateCurrentJobTextbox("ERR-" + this._logStatus.ErrorMessage);
                    break;
                }

                UpdateCurrentJobTextbox("( 5 / 5 ) CHECK DC ON & VERSION");

                count = 0;
                while (this._logStatus.POWER_DC_FINAL_ON == false)
                {
                    count++;
                    Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

                    if (count == 300)
                        break;
                }

                if (this._logStatus.POWER_DC_FINAL_ON == false)
                {
                    UpdateCurrentJobTextbox("DC ON가 완전하게 되지 않았다. VERSION이 없다.");
                    break;
                }

                // VERSION 확인
                if (    this._logStatus.MODULE_VERSION[0] == "{0}.{0}.{0}" 
                    || this._logStatus.MODULE_VERSION[1] == "{0}.{0}.{0}" 
                    || this._logStatus.MODULE_VERSION[2] == "{0}.{0}.{0}"
                    || this._logStatus.MODULE_VERSION[3] == "{0}.{0}.{0}" )
                {
                    countFailed++;
                    UpdateFailedCounterTextbox(countFailed.ToString());

                    if(this._logStatus.MODULE_VERSION[0] == "{0}.{0}.{0}")
                        UpdateCurrentJobTextbox("ERROR - MCU VERSION {0}.{0}.{0}");
                    else if (this._logStatus.MODULE_VERSION[1] == "{0}.{0}.{0}")
                        UpdateCurrentJobTextbox("ERROR - DSP VERSION {0}.{0}.{0}");
                    else if (this._logStatus.MODULE_VERSION[2] == "{0}.{0}.{0}")
                        UpdateCurrentJobTextbox("ERROR - HDMI VERSION {0}.{0}.{0}");
                    else if (this._logStatus.MODULE_VERSION[3] == "{0}.{0}.{0}")
                        UpdateCurrentJobTextbox("ERROR - eARC VERSION {0}.{0}.{0}");

                    count = 0;
                    while (this._progressThreadContinue == true)
                    {
                        count++;
                        Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

                        if( count == 300)
                            UpdateCurrentJobTextbox("WAIT FOR F/W UPGRADE DONW");

                        if (count == 3600)
                            break;
                    }
                    if (this._progressThreadContinue == false)
                        break;
                }
                else
                {
                    countSuccess++;
                    UpdateSuccessCounterTextbox(countSuccess.ToString());
                }
            }
        }

        public void UpdateCurrentJobTextbox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateCurrentJobTextbox), new object[] { value });
                return;
            }

            this.txtCurrrentJob.Text = value;
        }
        public void UpdateFailedCounterTextbox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateFailedCounterTextbox), new object[] { value });
                return;
            }

            this.txtFailedCounter.Text = value;
        }
        public void UpdateSuccessCounterTextbox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateSuccessCounterTextbox), new object[] { value });
                return;
            }

            this.txtSuccessCounter.Text = value;
        }

        private void TxtFailedCounter_TextChanged(object sender, EventArgs e)
        {
            this.txtFailedLastTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
