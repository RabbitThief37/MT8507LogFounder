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

namespace AutoTestATS_524
{
    public partial class Form1 : Form
    {
        ModeratorForTest _moderator;

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
                this._moderator = new ModeratorForTest();
                this._moderator.UpdateMessage = new ModeratorForTest.UpdateTextBox(UpdateTextBox);
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

            if (portNames.Count() == 1)
            {
                this.cboMtkComport.Items.AddRange(portNames);
                this.cboMtkComport.SelectedIndex = 0;

                this.cboRmcComport.Items.AddRange(portNames);
                this.cboRmcComport.SelectedIndex = 0;
            }
            else
            {
                string mtkPortName = this._moderator.MtkComPortName;
                string rmcPortName = this._moderator.RmcComPortName;

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
            this.txtArduinoFileName.Text = this._moderator.ArduinoFileName;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._moderator.Close();
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

            this._moderator.ArduinoFileName = this.txtArduinoFileName.Text = this.openFileDialog1.FileName;
        }

        //----------------------------------------------------------------------------------------------
        // START !!!
        private void BtnTestStart_Click(object sender, EventArgs e)
        //----------------------------------------------------------------------------------------------
        {
            if (this.btnTestStart.Text == "START")
            {
                if (string.IsNullOrEmpty(this.txtArduinoFileName.Text) == true)
                {
                    DialogResult result = MessageBox.Show(this, "Arduino Source Code가 지정되지 않았습니다.\n\n파일을 선택하시겠습니까?\n\n(YES - 파일 선택 , NO - 기본 명령어)"
                                                            , "SELECTION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        BtnSelectArduinoFile_Click(null, null);
                    }
                }

                this.cboMtkComport.Enabled = false;
                this.cboRmcComport.Enabled = false;
                this.btnTestStart.Text = "STOP";

                this._moderator.ExecutionPath = Application.StartupPath;
                this._moderator.MtkComPortName = this.cboMtkComport.Text;
                this._moderator.RmcComPortName = this.cboRmcComport.Text;

                this.txtModelName.Text = string.Empty;
                this.txtULIVersion.Text = string.Empty;
                this.txtCurrrentJob.Text = string.Empty;
                this.txtStartTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                this.txtFailedCounter.Text = "0";
                this.txtFailedLastTime.Text = string.Empty;
                this.txtSuccessCounter.Text = "0";

                Cursor.Current = Cursors.WaitCursor;
                if (this._moderator.Start() == true)
                {
                    Cursor.Current = Cursors.Default;
                    this.txtModelName.Text = this._moderator.ModelName;
                    this.txtULIVersion.Text = this._moderator.ULIVersion;
                    this.txtCurrrentJob.Text = string.Format("MCU:{0},DSP:{1},HDMI:{2},eARC{3}", this._moderator.MCUVersion
                        , this._moderator.DSPVersion, this._moderator.HDMIVersion, this._moderator.eARCVersion);

                }
                else
                {
                    Cursor.Current = Cursors.Default;
                    DisplayErrorMessageBox(this._moderator.ErrorMessage);
                }
            }
            else
            {
                this._moderator.Stop();

                this.cboMtkComport.Enabled = true;
                this.cboRmcComport.Enabled = true;
                this.btnTestStart.Text = "START";
            }
        }


        //-------------------------------------//
        // TEST PROCESS THREAD 
        //------------------------------------//
        //private void ProgressTestThreadFunction()
        //{
        //    VIZIO_RMC_CMD ecomode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_ECO_OFF;
        //    int count = 0;
        //    int countFailed = 0;
        //    int countSuccess = 0;

        //    while ( this._progressThreadContinue )
        //    {
        //        if (ecomode == VIZIO_RMC_CMD.VIZIO_RMC_CMD_ECO_ON)
        //        {
        //            ecomode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_ECO_OFF;
        //            UpdateCurrentJobTextbox("( 1 / 5 ) ECO MODE OFF");
        //        }
        //        else
        //        {
        //            ecomode = VIZIO_RMC_CMD.VIZIO_RMC_CMD_ECO_ON;
        //            UpdateCurrentJobTextbox("( 1 / 5 ) ECO MODE ON");
        //        }

        //        if (this._rmc.SendCommand(ecomode) == false)
        //        {
        //            UpdateCurrentJobTextbox("ERR-" + this._rmc.ErrorMessage);
        //            break;
        //        }

        //        count = 0;
        //        while (this._progressThreadContinue == true)
        //        {
        //            count++;
        //            Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

        //            if (count == 20)
        //                break;
        //        }

        //        if (this._progressThreadContinue == false)
        //            break;

        //        if (this._rmc.SendCommand(ecomode) == false)
        //        {
        //            UpdateCurrentJobTextbox("ERR-" + this._rmc.ErrorMessage);
        //            break;
        //        }

        //        if (this._logStatus.CheckMtkStatus(ref this._progressThreadContinue) == false)
        //        {
        //            UpdateCurrentJobTextbox("ERR-" + this._logStatus.ErrorMessage);
        //            break;
        //        }

        //        // DC OFF
        //        UpdateCurrentJobTextbox("( 2 / 5 ) DC OFF");
        //        if (this._rmc.SendCommand(VIZIO_RMC_CMD.VIZIO_RMC_CMD_POWER) == false)
        //        {
        //            UpdateCurrentJobTextbox("ERR-" + this._rmc.ErrorMessage);
        //            break;
        //        }

        //        if (this._logStatus.CheckMtkStatus(ref this._progressThreadContinue) == false)
        //        {
        //            UpdateCurrentJobTextbox("ERR-" + this._logStatus.ErrorMessage);
        //            break;
        //        }

        //        // DC ON
        //        if( ecomode == VIZIO_RMC_CMD.VIZIO_RMC_CMD_ECO_OFF )
        //        {
        //            UpdateCurrentJobTextbox("( 3 / 5 ) CHECK FAKE STANDBY");
        //            count = 0;
        //            while (this._logStatus.POWER_DC_FAKE_STANDBY_OFF == false && this._progressThreadContinue == true)
        //            {
        //                count++;
        //                Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

        //                if (count == 100)
        //                    break;
        //            }

        //            if (this._progressThreadContinue == false)
        //                break;

        //            if (this._logStatus.POWER_DC_FAKE_STANDBY_OFF == false)
        //            {
        //                UpdateCurrentJobTextbox("ERR - FAKE STANDBY가 완전하게 되지 않았다. 확인요망!!!");
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            UpdateCurrentJobTextbox("( 3 / 5 ) CHECK DC OFF");
        //            count = 0;
        //            while (this._logStatus.POWER_DC_FINAL_OFF == false && this._progressThreadContinue == true)
        //            {
        //                count++;
        //                Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

        //                if (count == 1000)
        //                    break;
        //            }
        //            if (this._progressThreadContinue == false)
        //                break;

        //            count = 0;
        //            while (this._progressThreadContinue == true)
        //            {
        //                count++;
        //                Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

        //                if (count == 30)
        //                    break;
        //            }
        //            if (this._progressThreadContinue == false)
        //                break;

        //            if (this._logStatus.POWER_DC_FINAL_OFF == false)
        //            {
        //                UpdateCurrentJobTextbox("ERR - DC OFF가 완전하게 되지 않았다. 확인요망!!!");
        //                break;
        //            }
        //        }

        //        UpdateCurrentJobTextbox("( 4 / 5 ) DC ON");
        //        if (this._rmc.SendCommand(VIZIO_RMC_CMD.VIZIO_RMC_CMD_POWER) == false)
        //        {
        //            UpdateCurrentJobTextbox("ERR-" + this._rmc.ErrorMessage);
        //            break;
        //        }

        //        if (this._logStatus.CheckMtkStatus(ref this._progressThreadContinue) == false)
        //        {
        //            UpdateCurrentJobTextbox("ERR-" + this._logStatus.ErrorMessage);
        //            break;
        //        }

        //        UpdateCurrentJobTextbox("( 5 / 5 ) CHECK DC ON & VERSION");

        //        count = 0;
        //        while (this._logStatus.POWER_DC_FINAL_ON == false)
        //        {
        //            count++;
        //            Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

        //            if (count == 3000)
        //                break;
        //        }

        //        if (this._logStatus.POWER_DC_FINAL_ON == false)
        //        {
        //            UpdateCurrentJobTextbox("DC ON가 완전하게 되지 않았다. VERSION이 없다.");
        //            break;
        //        }

        //        // VERSION 확인
        //        if (    this._logStatus.MODULE_VERSION[0] == "{0}.{0}.{0}" 
        //            || this._logStatus.MODULE_VERSION[1] == "{0}.{0}.{0}" 
        //            || this._logStatus.MODULE_VERSION[2] == "{0}.{0}.{0}"
        //            || this._logStatus.MODULE_VERSION[3] == "{0}.{0}.{0}" )
        //        {
        //            countFailed++;
        //            UpdateFailedCounterTextbox(countFailed.ToString());

        //            if(this._logStatus.MODULE_VERSION[0] == "{0}.{0}.{0}")
        //                UpdateCurrentJobTextbox("ERROR - MCU VERSION {0}.{0}.{0}");
        //            else if (this._logStatus.MODULE_VERSION[1] == "{0}.{0}.{0}")
        //                UpdateCurrentJobTextbox("ERROR - DSP VERSION {0}.{0}.{0}");
        //            else if (this._logStatus.MODULE_VERSION[2] == "{0}.{0}.{0}")
        //                UpdateCurrentJobTextbox("ERROR - HDMI VERSION {0}.{0}.{0}");
        //            else if (this._logStatus.MODULE_VERSION[3] == "{0}.{0}.{0}")
        //                UpdateCurrentJobTextbox("ERROR - eARC VERSION {0}.{0}.{0}");

        //            count = 0;
        //            while (this._progressThreadContinue == true)
        //            {
        //                count++;
        //                Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

        //                if( count == 300)
        //                    UpdateCurrentJobTextbox("WAIT FOR F/W UPGRADE DONW");

        //                if (count == 3600)
        //                    break;
        //            }
        //            if (this._progressThreadContinue == false)
        //                break;
        //        }
        //        else
        //        {
        //            countSuccess++;
        //            UpdateSuccessCounterTextbox(countSuccess.ToString());
        //        }
        //    }
        //}

        public void UpdateTextBox(ModeratorForTest.UPDATE_TEXTBOX textbox, string message)
        {
            switch (textbox)
            {
                case ModeratorForTest.UPDATE_TEXTBOX.CURRENT_JOB:
                    this.txtCurrrentJob.Text = message;
                    break;

                case ModeratorForTest.UPDATE_TEXTBOX.SUCCESS_COUNT:
                    this.txtSuccessCounter.Text = message;
                    break;

                case ModeratorForTest.UPDATE_TEXTBOX.FAILED_COUNT:
                    this.txtFailedCounter.Text = message;
                    break;
            }

            Application.DoEvents();
        }

        private void TxtFailedCounter_TextChanged(object sender, EventArgs e)
        {
            this.txtFailedLastTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
