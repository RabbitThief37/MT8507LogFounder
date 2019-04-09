using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZTCK.Lib.APMeasurementHelper
{
    public class ArduinoRemote : IDisposable
    {
        public enum VIZIO_RMC_CMD : int
        {
            VIZIO_RMC_CMD_POWER = 0,
            VIZIO_RMC_CMD_AUX,
            VIZIO_RMC_CMD_COXIAL,
            VIZIO_RMC_CMD_OPT,
            VIZIO_RMC_CMD_HDMI,
            VIZIO_RMC_CMD_ARC,
            VIZIO_RMC_CMD_BT,
            VIZIO_RMC_CMD_USB,
            VIZIO_RMC_CMD_RESET_ALL,
            VIZIO_RMC_CMD_RESET_AUDIO,
            VIZIO_RMC_CMD_VOLUME_UP,
            VIZIO_RMC_CMD_VOLUME_DOWN,
            VIZIO_RMC_CMD_LEFT_CURSOR,
            VIZIO_RMC_CMD_RIGHT_CURSOR,
            VIZIO_RMC_CMD_BASS_UP,
            VIZIO_RMC_CMD_BASS_DOWN,
            VIZIO_RMC_CMD_TREBLE_UP,
            VIZIO_RMC_CMD_TREBLE_DOWN,
            VIZIO_RMC_CMD_CENTER_UP,
            VIZIO_RMC_CMD_CENTER_DOWN,
            VIZIO_RMC_CMD_HEIGHT_LEVEL_UP,
            VIZIO_RMC_CMD_HEIGHT_LEVEL_DOWN,
            VIZIO_RMC_CMD_SURROUND_UP,
            VIZIO_RMC_CMD_SURROUND_DOWN,
            VIZIO_RMC_CMD_BALANCE_UP,
            VIZIO_RMC_CMD_BALANCE_DOWN,
            VIZIO_RMC_CMD_SUBWOOFER_UP,
            VIZIO_RMC_CMD_SUBWOOFER_DOWN,
            VIZIO_RMC_CMD_EQ_MOVIE,
            VIZIO_RMC_CMD_EQ_MUSIC,
            VIZIO_RMC_CMD_EQ_DIRECT,
            VIZIO_RMC_CMD_HEIGHT_ON,
            VIZIO_RMC_CMD_HEIGHT_OFF,
            VIZIO_RMC_CMD_HEIGHT_VRT,
            VIZIO_RMC_CMD_MAX
        }

        public ArduinoRemote()
        {
            this._rmcSerialPort = new SerialPort();
            this._cmd = new Dictionary<VIZIO_RMC_CMD, string>();
        }

        public void Dispose()
        {
            Close();

            this._rmcSerialPort.Dispose();
            this._rmcSerialPort = null;

            this.IsOpen = false;
        }

        public bool Start(string portName)
        {
            this._portName = portName;

            try
            {
                this._rmcSerialPort.PortName = this._portName;
                this._rmcSerialPort.BaudRate = 9600;
                this._rmcSerialPort.Parity = Parity.None;
                this._rmcSerialPort.DataBits = 8;
                this._rmcSerialPort.StopBits = StopBits.One;

                this._rmcSerialPort.Open();
                this.IsOpen = true;
            }
            catch(Exception ex)
            {
                this.ErrorMessage = ex.Message;
                this.IsOpen = false;
            }

            return this.IsOpen;
        }
        public void Close()
        {
            try
            {
                this._rmcSerialPort.Close();
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }

            this.IsOpen = false;
        }
        public bool SendCommand(VIZIO_RMC_CMD interCommand)
        {
            if (this.IsOpen == false)
            {
                this.ErrorMessage = "RMC TX가 연결되지 않았습니다.\nSerial Port 연결을 확인해 주세요.";
                return false;
            }

            try
            {
                this._rmcSerialPort.WriteLine(this._cmd[interCommand]);
                Thread.Sleep(WAIT_COMMAND_EXECUTION_TIME);
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        public bool LoadCommandFromIno(string inoFileName)
        {
            bool isResult = false;

            try
            {
                StreamReader file = new StreamReader(inoFileName);
                string line = string.Empty;
                bool isFoundSendIR = false;
                bool isFoundCommand = false;
                int posFront = 0;
                int posRear = 0;
                string arduinoCommand = string.Empty;
                VIZIO_RMC_CMD internalCommand = VIZIO_RMC_CMD.VIZIO_RMC_CMD_MAX;
                string internalCommandString = string.Empty;

                this._cmd.Clear();

                while ((line = file.ReadLine()) != null)
                {
                    if(isFoundSendIR)
                    {
                        //if (inputString == "Power")				//VIZIO_RMC_CMD_POWER
                        posFront = line.IndexOf(ARDUINO_REAL_COMMAND_PREFIX);
                        if (posFront <= 0)
                            continue;

                        posFront += ARDUINO_REAL_COMMAND_PREFIX.Length;
                        posRear = line.IndexOf(ARDUINO_REAL_COMMAND_TAIL, posFront );
                        if (posRear <= 0)
                            continue;

                        arduinoCommand = line.Substring(posFront, posRear - posFront);

                        posRear += ARDUINO_REAL_COMMAND_TAIL.Length;
                        posFront = line.IndexOf(ARDUINO_ENUM_TOKEN, posRear);
                        if (posFront <= 0)
                            continue;

                        posFront += ARDUINO_ENUM_TOKEN.Length;
                        internalCommandString = line.Substring(posFront).Trim();

                        internalCommand = SearchingInternalCommand(internalCommandString);
                        if(internalCommand == VIZIO_RMC_CMD.VIZIO_RMC_CMD_MAX)
                            continue;

                        this._cmd.Add(internalCommand, arduinoCommand);
                        isFoundCommand = true;
                    }
                    else
                    {
                        //void SendIR() {
                        posFront = line.IndexOf(ARDUINO_SEND_IR);

                        if (posFront > 0)
                            isFoundSendIR = true;
                    }
                }

                file.Close();

                if (isFoundSendIR == false || isFoundCommand == false)
                    throw new Exception("SendIR() 함수 또는 Command 주석이 없습니다.");

                isResult = true;
            }
            catch(Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }

            this.IsReady = isResult;
            return isResult;
        }

        VIZIO_RMC_CMD SearchingInternalCommand(string cmdstring)
        {
            VIZIO_RMC_CMD result = VIZIO_RMC_CMD.VIZIO_RMC_CMD_MAX;

            for(VIZIO_RMC_CMD index = 0; index < VIZIO_RMC_CMD.VIZIO_RMC_CMD_MAX; index++)
            {
                if(cmdstring == index.ToString())
                {
                    result = index;
                    break;
                }
            }

            return result;
        }

        public void LoadCommandDefault()
        {
            this._cmd.Clear();

            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_POWER, "Power");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_AUX, "AUX");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_OPT, "Optical");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_HDMI, "HDMI");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_ARC, "HDMIARC");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_BT, "BT");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_USB, "USB");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_RESET_ALL, "ResetAll");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_VOLUME_UP, "VolUp");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_VOLUME_DOWN, "VolDown");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_LEFT_CURSOR, "LeftCursor");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_RIGHT_CURSOR, "RightCursor");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_BASS_UP, "BassUp");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_BASS_DOWN, "BassDown");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_TREBLE_UP, "TrebleUp");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_TREBLE_DOWN, "TrebleDown");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_CENTER_UP, "CenterUp");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_CENTER_DOWN, "CenterDown");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_HEIGHT_LEVEL_UP, "HeightUp");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_HEIGHT_LEVEL_DOWN, "HeightDown");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_SURROUND_UP, "SurroundUp");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_SURROUND_DOWN, "SurroundDown");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_BALANCE_UP, "BalanceUp");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_BALANCE_DOWN, "BalanceDown");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_SUBWOOFER_UP, "SubUp");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_SUBWOOFER_DOWN, "SubDown");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_EQ_MOVIE, "Movie");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_EQ_MUSIC, "Music");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_EQ_DIRECT, "Direct");
            this._cmd.Add(VIZIO_RMC_CMD.VIZIO_RMC_CMD_HEIGHT_ON, "HeightOn");
        }

        public const string ARDUINO_SEND_IR = "SendIR() {";
        public const string ARDUINO_REAL_COMMAND_PREFIX = "inputString == \"";
        public const string ARDUINO_REAL_COMMAND_TAIL = "\")";
        public const string ARDUINO_ENUM_TOKEN = "//";
        public const int WAIT_COMMAND_EXECUTION_TIME = 500;

        public string ErrorMessage { get; private set; } = string.Empty;
        public bool IsOpen { get; private set; } = false;
        public bool IsReady { get; private set; } = false;

        private SerialPort _rmcSerialPort;
        private string _portName;
        private Dictionary<VIZIO_RMC_CMD, string> _cmd;
    }
}
