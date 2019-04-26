using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using static ZTCK.Lib.APMeasurementHelper.ArduinoRemote;
using static ZTCK.Lib.APMeasurementHelper.AppConfigHandler;

namespace ZTCK.Lib.APMeasurementHelper
{
    /// <summary>
    /// User Interface를 간편하게 지원하기 위한 중계 Class.
    /// 함수 노출은 최소로 한다.
    /// </summary>
    public class ModeratorForTest : IDisposable
    {
        //---------------------------------------------------
        // ENUM , CONSTANT
        //---------------------------------------------------
        public enum UPDATE_TEXTBOX
        {
            CURRENT_JOB,
            SUCCESS_COUNT,
            FAILED_COUNT
        };

        //---------------------------------------------------
        // DELEGATE DECLARATION
        //---------------------------------------------------
        public delegate void UpdateTextBox(UPDATE_TEXTBOX textbox, string message);

        //---------------------------------------------------
        // VARIABLE
        //---------------------------------------------------
        private AppConfigHandler _appConfigHandler = null;
        private MtkLogStatus _logStatus = null;
        private ArduinoRemote _rmc = null;
        private UpdateTextBox _updateMessage = null;

        /// <summary>
        /// 생성자에서 Exception이 발생을 대비하기 위한 변수
        /// </summary>
        private bool _initalError = false;

        /// <summary>
        /// Script로 정의된 내용을 실행하는 Thread
        /// </summary>
        private Thread _progressTestThread;
        public static bool _threadContinue = false;

        //---------------------------------------------------
        // PROPERTIES
        //---------------------------------------------------
        public string ErrorMessage { get; private set; } = string.Empty;
        public string ExecutionPath { get; set; } = string.Empty;
        public string MtkComPortName { get; set; } = string.Empty;
        public string RmcComPortName { get; set; } = string.Empty;
        public string ArduinoFileName { get; set; } = string.Empty;
        public string ModelName { get; private set; } = string.Empty;
        public string ULIVersion { get; private set; } = string.Empty;
        public string MCUVersion { get; private set; } = string.Empty;
        public string DSPVersion { get; private set; } = string.Empty;
        public string HDMIVersion { get; private set; } = string.Empty;
        public string eARCVersion { get; private set; } = string.Empty;
        public UpdateTextBox UpdateMessage
        {
            get { return this._updateMessage; }
            set { this._updateMessage = value; }
        }
        public ModeratorForTest()
        {
            try
            {
                this._appConfigHandler = new AppConfigHandler();
                this._logStatus = new MtkLogStatus();
                this._rmc = new ArduinoRemote();

                this._progressTestThread = new Thread(new ThreadStart(this.ProgressTestThreadFunction))
                {
                    Priority = ThreadPriority.Normal
                };
                this._progressTestThread.Name = "ProcessQueue" + this._progressTestThread.ManagedThreadId.ToString();

                if (this._appConfigHandler.MakeKeys() == false)
                    throw new Exception(this._appConfigHandler.ErrorMessage);

                this.MtkComPortName = this._appConfigHandler.Get(APP_CONFIG_KEY.mtkComPort);
                this.RmcComPortName = this._appConfigHandler.Get(APP_CONFIG_KEY.rmcComPort);
                this.ArduinoFileName = this._appConfigHandler.Get(APP_CONFIG_KEY.arduinoFileName);
            }
            catch (Exception ex)
            {
                this.ErrorMessage = "Construct error - " + ex.Message;
                this._initalError = true;
            }
        }
        public void Dispose()
        {
            if( _threadContinue == true )
                Close();
        }
        public void Close()
        {
            this._appConfigHandler.Save(APP_CONFIG_KEY.mtkComPort, this.MtkComPortName);
            this._appConfigHandler.Save(APP_CONFIG_KEY.rmcComPort, this.RmcComPortName);
            this._appConfigHandler.Save(APP_CONFIG_KEY.arduinoFileName, this.ArduinoFileName);
            this._appConfigHandler.SaveToConfig();

            _threadContinue = false;

            if (this._progressTestThread.ThreadState != ThreadState.Stopped)
            {
                this._progressTestThread.Abort();
            }

            this._logStatus.Dispose();
            this._rmc.Dispose();
        }
        /// <summary>
        /// 각종 설정값들을 확인하고 하부 class들을 실행 시킨다.
        /// </summary>
        /// <returns>
        /// true - 모든 설정값과 class들이 정상적으로 실행되었다.
        /// false - 문제 발생. ErrorMessage에 상세 내용을 저장한다.
        /// </returns>
        public bool Start()
        {
            bool isResult = false;

            do
            {
                if( this._initalError == true )
                    break;

                if (CheckPresetString() == false)
                    break;

                _threadContinue = true;

                // OPEN MTK
                _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "MTK log parser start");
                if (MtkLogParserStart() == false)
                    break;

                // OPEN RMC
                _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "vizio RMC arduino start");
                if (VizioRmcStart() == false)
                    break;

                // LOAD RMC COMMAND 
                LoadVizioRmcCommand();

                // Check Ready -> Send Power
                if (ReadyForTest() == false)
                    break;

                //this._progressTestThread.Start();

                isResult = true;
            } while (false);

            return isResult;
        }
        /// <summary>
        /// 처리를 멈춘다.
        /// </summary>
        public void Stop()
        {
            _threadContinue = false;

            if (this._logStatus.IsOpen)
            {
                this._logStatus.Close();
            }

            if (this._rmc.IsOpen)
            {
                this._rmc.Close();
            }
        }
        private bool CheckPresetString()
        {
            if (string.IsNullOrEmpty(this.ExecutionPath) == true)
            {
                this.ErrorMessage = "ExecutionPath를 지정하지 않으면, 로그를 남길 수가 없습니다!";
                return false;
            }

            this._logStatus.ConfigPath = this.ExecutionPath;

            this.MtkComPortName = this.MtkComPortName.Trim();
            if (string.IsNullOrEmpty(this.MtkComPortName) == true)
            {
                this.ErrorMessage = "MTK LOG를 취득해야 하는데, ComPort 지정하지 않는 의도는?";
                return false;
            }

            this.RmcComPortName = this.RmcComPortName.Trim();
            if (string.IsNullOrEmpty(this.RmcComPortName) == true)
            {
                this.ErrorMessage = "RMC 명령을 내려야 하는데, ComPort 지정하지 않는 의도는?";
                return false;
            }

            return true;
        }
        private bool MtkLogParserStart()
        {
            if (this._logStatus.IsOpen)
            {
                this._logStatus.Close();
            }

            if (this._logStatus.Start(this.MtkComPortName) == false)
            {
                _updateMessage( UPDATE_TEXTBOX.CURRENT_JOB, string.Format("{1}에 연결을 실패하였습니다.{0}", this.MtkComPortName, this._logStatus.ErrorMessage));
                return false;
            }

            return true;
        }
        private bool VizioRmcStart()
        {
            if (this._rmc.IsOpen)
            {
                this._rmc.Close();
            }

            if (this._rmc.Start(this.RmcComPortName) == false)
            {
                _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, string.Format("{1}에 연결을 실패하였습니다.{0}", this.RmcComPortName, this._rmc.ErrorMessage));
                return false;
            }

            return true;
        }
        /// <summary>
        /// Arduino File 이름이 비워져 있으면 기본 명령어 셋트를 이용하고, 있으면 Parsing 해서 사용
        /// </summary>
        private void LoadVizioRmcCommand()
        {
            if (string.IsNullOrEmpty(this.ArduinoFileName) == true)
            {
                this._rmc.LoadCommandDefault();
            }
            else
            {
                if(this._rmc.LoadCommandFromIno(this.ArduinoFileName) == false)
                    this._rmc.LoadCommandDefault();
            }
        }
        /// <summary>
        /// 테스트를 하기 전에 초기 상태로 Power ON 하여 대기 할 수 있도록 한다.
        /// </summary>
        /// <returns>Error 여부</returns>
        private bool ReadyForTest()
        {
            int count = 0;
            bool isResult = false;

            do
            {
                // power off 되었을때는 고려 해야 한다...젠장...다시 넣어~~~~~

                _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "READY[1 / 5] - FACTORY RESET");
                if (this._rmc.SendCommand(VIZIO_RMC_CMD.VIZIO_RMC_CMD_RESET_ALL) == false)
                {
                    _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, string.Format("Arduino를 통한 FACTORY-RESET 전송에 실패했습니다.\n\n{0}", this._rmc.ErrorMessage));
                    break;
                }

                count = 0;
                while( this._logStatus.RESET_ALL == false || this._logStatus.POWER_DC_FINAL_OFF == false )
                {
                    count++;
                    Thread.Sleep(100);

                    if (count == 1000)
                        break;
                }

                if (this._logStatus.POWER_DC_FINAL_OFF == false)
                {
                    _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "RESET ALL과 DC OFF가 완전하게 되지 않았다. 확인요망!!!");
                    break;
                }

                Thread.Sleep(2000);

                _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "READY[2 / 5] - POWER ON");
                if (this._rmc.SendCommand(VIZIO_RMC_CMD.VIZIO_RMC_CMD_POWER) == false)
                {
                    _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, string.Format("Arduino를 통한 POWER 전송에 실패했습니다.\n\n{0}", this._rmc.ErrorMessage));
                    break;
                }

                _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "READY[3 / 5] - AUTO DETECTION");

                count = 0;
                while (this._logStatus.AUTO_DETECTION == "OFF")
                {
                    count++;
                    Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

                    if (count == 3000)
                        break;
                }

                if (this._logStatus.AUTO_DETECTION == "OFF")
                {
                    _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "AUTO DETECTION 시작이 없다.");
                    break;
                }

                _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "READY[4 / 5] - STOP AUTO DETECTION");
                if (this._rmc.SendCommand(VIZIO_RMC_CMD.VIZIO_RMC_CMD_AUX) == false)
                {
                    _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, string.Format("Arduino를 통한 AUX 전송에 실패했습니다.{0}", this._rmc.ErrorMessage));
                    break;
                }

                _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "READY[5 / 5] - CHECK VERSION");

                count = 0;
                while (this._logStatus.POWER_DC_FINAL_ON == false)
                {
                    count++;
                    Thread.Sleep(100); //////////////////////////////////////////////////////////////////////////////////////////

                    if (count == 1000)
                        break;
                }

                if (this._logStatus.POWER_DC_FINAL_ON == false)
                {
                    _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "DC ON가 완전하게 되지 않았다. VERSION이 없다.");
                    break;
                }

                // VERSION 확인
                if (this._logStatus.MODULE_VERSION[0] == "{0}.{0}.{0}"
                    || this._logStatus.MODULE_VERSION[1] == "{0}.{0}.{0}"
                    || this._logStatus.MODULE_VERSION[2] == "{0}.{0}.{0}"
                    || this._logStatus.MODULE_VERSION[3] == "{0}.{0}.{0}")
                {
                    if (this._logStatus.MODULE_VERSION[0] == "{0}.{0}.{0}")
                        _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "ERROR - MCU VERSION {0}.{0}.{0}");
                    else if (this._logStatus.MODULE_VERSION[1] == "{0}.{0}.{0}")
                        _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "ERROR - DSP VERSION {0}.{0}.{0}");
                    else if (this._logStatus.MODULE_VERSION[2] == "{0}.{0}.{0}")
                        _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "ERROR - HDMI VERSION {0}.{0}.{0}");
                    else if (this._logStatus.MODULE_VERSION[3] == "{0}.{0}.{0}")
                        _updateMessage(UPDATE_TEXTBOX.CURRENT_JOB, "ERROR - eARC VERSION {0}.{0}.{0}");
                    break;
                }

                this.ULIVersion = this._logStatus.ULI_VERSION;
                this.ModelName = this._logStatus.MODEL_NAME;
                this.MCUVersion = this._logStatus.MODULE_VERSION[0];
                this.DSPVersion = this._logStatus.MODULE_VERSION[1];
                this.HDMIVersion = this._logStatus.MODULE_VERSION[2];
                this.eARCVersion = this._logStatus.MODULE_VERSION[3];

                isResult = true;
            } while (false);

            return isResult;
        }

        /// <summary>
        /// Script를 수행하는 실제적인 Thread Function
        /// </summary>
        private void ProgressTestThreadFunction()
        {
            //while (_threadContinue)
            //{
            //}
        }

    }
}
