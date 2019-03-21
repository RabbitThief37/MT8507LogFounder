using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MT8507Log
{
    public class LogStatus : IDisposable
    {
        public LogStatus(string portName)
        {
            this._portName = portName;
            this._receiveQueue = new ConcurrentQueue<byte[]>();
            this._serialClient = new SerialClient(this._portName, this._receiveQueue);
            this._processQueue = new ConcurrentQueue<string>();
        }

        public void Dispose()
        {
            this._processContinue = false;
            this._serialClient.CloseConn();
            this._serialClient.Dispose();

            Thread.Sleep(10);

            if (this._processPortDataThread.ThreadState != ThreadState.Stopped)
            {
                this._processPortDataThread.Abort();
            }

            if (this._processStringThread.ThreadState != ThreadState.Stopped)
            {
                this._processStringThread.Abort();
            }
        }

        public bool Start()
        {
            if (!this._serialClient.OpenConn())
            {
                this.ErrorMessage = this._serialClient.ErrorMessage;
                return false;
            }

            this._processContinue = true;
            this._processPortDataThread = new Thread(new ThreadStart(this.ProcessPortDataInQueue))
            {
                Priority = ThreadPriority.Normal
            };
            this._processPortDataThread.Name = "ProcessQueue" + this._processPortDataThread.ManagedThreadId.ToString();
            this._processPortDataThread.Start();

            this._processStringThread = new Thread(new ThreadStart(ProcessStringInQueue))
            {
                Priority = ThreadPriority.Normal
            };
            this._processStringThread.Name = "ProcessQueue" + this._processStringThread.ManagedThreadId.ToString();
            this._processStringThread.Start();

            return true;
        }

        //---------------------------------//
        //  RECEIVE QUEUE PROCESS THREAD 
        //---------------------------------//
        private void ProcessPortDataInQueue()
        {
            byte[] portData;
            StringBuilder accData;
            bool isCommand;

            accData = new StringBuilder(4096);
            isCommand = false;

            while (this._processContinue)
            {
                if (this._receiveQueue.TryDequeue(out portData) == false)
                {
                    Thread.Sleep(10);
                    continue;
                }

                foreach (byte uartChar in portData)
                {
                    if (isCommand)
                    {
                        if (uartChar == 0x0D || uartChar == 0x0A)
                        {
                            isCommand = false;
                            if (accData.Length != 0)
                            {
                                this._processQueue.Enqueue(accData.ToString());
                                accData.Clear();
                            }

                            continue;
                        }

                        accData.Append(Convert.ToChar(uartChar));
                    }
                    else
                    {
                        if (uartChar == 0x3C)
                        {
                            accData.Append('<');
                            isCommand = true;
                        }
                    }
                }
            }
        }

        //---------------------------------//
        //  STRING QUEUE PROCESS THREAD 
        //---------------------------------//
        private void ProcessStringInQueue()
        {
            string commandLine = string.Empty;
            string commandID = string.Empty;
            string commandArgument = string.Empty;
            string commandArgumentList = string.Empty;
            int pos = 0;

            while (this._processContinue)
            {
                if (this._processQueue.TryDequeue(out commandLine) == false)
                {
                    Thread.Sleep(10);
                    continue;
                }

                if (commandLine.Contains(COMMAND_PREFIX) == false)
                {
                    continue;
                }

                do
                {
                    pos = commandLine.IndexOf(COMMAND_ID);
                    if (pos < COMMAND_PREFIX.Length)
                    {
                        break;
                    }

                    commandID = commandLine.Substring(pos + COMMAND_ID.Length, 6);

                    if (commandLine.Contains(COMMAND_DATA))
                    {
                        pos = commandLine.IndexOf(COMMAND_DATA);
                        commandArgument = commandLine.Substring(pos + COMMAND_DATA.Length, 4);
                        commandArgumentList = string.Empty;
                    }
                    else
                    {
                        pos = commandLine.IndexOf(COMMAND_DATA_FIRST);
                        if (pos == -1)
                        {
                            commandArgument = string.Empty;
                            commandArgumentList = string.Empty;
                        }
                        else
                        {
                            commandArgument = commandLine.Substring(pos + COMMAND_DATA_FIRST.Length, 4);
                            commandArgumentList = commandLine.Substring(pos + 3);
                        }
                    }

                    //System.Diagnostics.Debug.Print(result);
                    //System.Diagnostics.Debug.Print("\r\n");

                    UpdateStatus(ref commandID, ref commandArgument, ref commandArgumentList);

                } while (false);
            }
        }

        public void UpdateStatus(ref string id, ref string arg, ref string argList)
        {
            int valueID = Convert.ToInt32(id, 16);
            int valueArg = 0;

            if (arg.Length != 0)
                valueArg = Convert.ToInt32(arg, 16);

            setNewData();

            switch (valueID)
            {
                case 0x0001:
                    if (valueArg == 1 || valueArg == 2 || valueArg == 5)
                        this.POWER_STATUS = "ON";
                    else
                        this.POWER_STATUS = "OFF";
                    break;

                case 0x0010: this.VOLUME = valueArg; break;
                case 0x0012: this.SUBWOOFER_INDEX = valueArg; break;
                case 0x0013: this.BASS_INDEX = valueArg; break;
                case 0x0014: this.TREBLE_INDEX = valueArg; break;
                case 0x0015: this.CENTER_INDEX = valueArg; break;
                case 0x0016: this.SURROUND_VOLUME_INDEX = valueArg; break;
                case 0x0017: this.SURROUND_BALANCE_INDEX = valueArg; break;
                case 0x0018: this.AV_DELAY = valueArg; break;

                case 0x0019:
                    this.MUTE_STATUS = (valueArg == 0) ? "UNMUTE" : "MUTE";
                    break;

                case 0x001A:
                    if (valueArg == 0)
                        this.HEIGHT_SPEAKERS_STATUS = "OFF";
                    else if (valueArg == 1)
                        this.HEIGHT_SPEAKERS_STATUS = "ON";
                    else
                        this.HEIGHT_SPEAKERS_STATUS = "VRT";
                    break;

                case 0x001B: this.HEIGHT_LEVEL_INDEX = valueArg; break;

                case 0x0020:
                    {
                        switch (valueArg)
                        {
                            case 1: this.INPUT_SOURCE = "AUX"; break;
                            case 2: this.INPUT_SOURCE = "COAXIAL"; break;
                            case 3: this.INPUT_SOURCE = "OPTICAL"; break;
                            case 4: this.INPUT_SOURCE = "HDMI"; break;
                            case 5: this.INPUT_SOURCE = "ARC"; break;
                            case 6: this.INPUT_SOURCE = "BLUETOOTH"; break;
                            case 7: this.INPUT_SOURCE = "USB"; break;
                            case 8: this.INPUT_SOURCE = "CAST"; break;
                        }
                    }
                    break;

                case 0x0040: this.SURROUND_STATUS = (valueArg == 0) ? "OFF" : "ON"; break;
                case 0x0041: this.VOLUME_LEVELER_STATUS = (valueArg == 0) ? "OFF" : "ON"; break;
                case 0x0042: this.NIGHT_MODE_STATUS = (valueArg == 0) ? "OFF" : "ON"; break;

                //{0x00},1{0x0a},2{0x0a},3{0x0a},4{0x0a}
                case 0x0043:
                    {
                        string value = string.Empty;
                        int bracePos = 0;
                        int index = 0;

                        do
                        {
                            value = argList.Substring(bracePos + 1, 4);
                            valueArg = Convert.ToInt32(arg, 16);

                            switch (index)
                            {
                                case 0:
                                    {
                                        if (valueArg == 0)
                                            this.EQ_STATUS = "MUSIC";
                                        else if (valueArg == 1)
                                            this.EQ_STATUS = "MOVIE";
                                        else
                                            this.EQ_STATUS = "DIRECT";
                                    }
                                    break;

                                case 1: this.BASS_INDEX = valueArg; break;
                                case 2: this.TREBLE_INDEX = valueArg; break;
                                case 3: this.CENTER_INDEX = valueArg; break;
                                case 4: this.SUBWOOFER_INDEX = valueArg; break;
                            }

                            bracePos = argList.IndexOf("{", bracePos + 1);
                            index++;

                        } while (bracePos != -1);
                    }
                    break;

                //{0x0f},1{0x0a},2{0x0a},3{0x0a},4{0x0a},5{0x0a},6{0x0a},7{0x00},8{0x00},9{0x01},10{0x01},11{0x00},12{0x00},13{0x01},14{0x14},15{0x01},16{0x00}
                case 0x8100:
                    {
                        string value = string.Empty;
                        int bracePos = 0;
                        int index = 0;

                        do
                        {
                            value = argList.Substring(bracePos + 1, 4);
                            valueArg = Convert.ToInt32(value, 16);

                            switch (index)
                            {
                                case 0: this.VOLUME = valueArg; break;
                                case 1: this.SUBWOOFER_INDEX = valueArg; break;
                                case 2: this.BASS_INDEX = valueArg; break;
                                case 3: this.TREBLE_INDEX = valueArg; break;
                                case 4: this.CENTER_INDEX = valueArg; break;
                                case 5: this.SURROUND_VOLUME_INDEX = valueArg; break;
                                case 6: this.SURROUND_BALANCE_INDEX = valueArg; break;
                                case 7: this.AV_DELAY = valueArg; break;
                                case 8: this.MUTE_STATUS = (valueArg == 0) ? "UNMUTE" : "MUTE"; break;
                                case 9:
                                    {
                                        switch (valueArg)
                                        {
                                            case 1: this.INPUT_SOURCE = "AUX"; break;
                                            case 2: this.INPUT_SOURCE = "COAXIAL"; break;
                                            case 3: this.INPUT_SOURCE = "OPTICAL"; break;
                                            case 4: this.INPUT_SOURCE = "HDMI"; break;
                                            case 5: this.INPUT_SOURCE = "ARC"; break;
                                            case 6: this.INPUT_SOURCE = "BLUETOOTH"; break;
                                            case 7: this.INPUT_SOURCE = "USB"; break;
                                            case 8: this.INPUT_SOURCE = "CAST"; break;
                                        }
                                    }
                                    break;

                                case 10: this.SURROUND_STATUS = (valueArg == 0) ? "OFF" : "ON"; break;
                                case 11: this.VOLUME_LEVELER_STATUS = (valueArg == 0) ? "OFF" : "ON"; break;
                                case 12: this.NIGHT_MODE_STATUS = (valueArg == 0) ? "OFF" : "ON"; break;
                                case 13:
                                    {
                                        if (valueArg == 0)
                                            this.EQ_STATUS = "MUSIC";
                                        else if (valueArg == 1)
                                            this.EQ_STATUS = "MOVIE";
                                        else
                                            this.EQ_STATUS = "DIRECT";
                                    }
                                    break;

                                case 14:
                                    {
                                        switch( valueArg )
                                        {
                                            case 18: this.MODEL_NAME = "SB46514-F6"; break;
                                            case 19: this.MODEL_NAME = "SB46312-F6"; break;
                                            case 20: this.MODEL_NAME = "SB36512-F6"; break;
                                            case 21: this.MODEL_NAME = "SB36312-G6"; break;
                                            case 22: this.MODEL_NAME = "SB36514-G6"; break;
                                            case 23: this.MODEL_NAME = "SB36512-F6E"; break;
                                            default: this.MODEL_NAME = string.Format("UNKNOWN MODEL NUMBER:{0}", valueArg); break;
                                        }
                                    }
                                    break;

                                case 15:
                                    {
                                        Byte btValue = (Byte)valueArg;
                                        Byte heightValue = (Byte)((btValue & 0xf0) >> 4);

                                        if (heightValue == 0)
                                            this.HEIGHT_SPEAKERS_STATUS = "OFF";
                                        else if (heightValue == 1)
                                            this.HEIGHT_SPEAKERS_STATUS = "ON";
                                        else
                                            this.HEIGHT_SPEAKERS_STATUS = "VRT";

                                        this.HEIGHT_LEVEL_INDEX = (btValue & 0x0f);
                                    }
                                    break;

                                case 16:
                                    {
                                        switch (valueArg)
                                        {
                                            case 0: this.DEMO_MODE = "OFF"; break;
                                            case 1: this.DEMO_MODE = "DEMO MODE 1(AUX)"; break;
                                            case 2: this.DEMO_MODE = "DEMO MODE 2(OPTICAL)"; break;
                                            case 3: this.DEMO_MODE = "DEMO MODE 3(HDMI/ARC)"; break;
                                            default: this.DEMO_MODE = string.Format("UNKNOWN NUMBER:{0}", valueArg); break;
                                        }
                                    }
                                    break;
                            }

                            bracePos = argList.IndexOf("{", bracePos + 1);
                            index++;

                        } while (  bracePos != -1);
                    }
                    break;

                default:
                    //result = string.Format("---> REQUEST TO KS.KIM...ID:0x{0:X4}", id);
                    break;
            }
        }

        public bool IsNewData()
        {
            if( this.changeData )
            {
                this.changeData = false;
                return true;
            }

            return false;
        }

        private void setNewData() {  this.changeData = true;  }

        public const string COMMAND_PREFIX = "<misc_uart_mcu_execute_send_cmd>";
        public const string COMMAND_ID = "UART_MCU_COMMAND_ID:{";
        public const string COMMAND_DATA = "},Data{";
        public const string COMMAND_DATA_FIRST = "},0{";

        public string ErrorMessage { get; private set; }

        //------------------------------------------------------------------
        public string POWER_STATUS { get; private set; } = "ON";
        public string INPUT_SOURCE { get; private set; } = "AUX";
        public int VOLUME { get; private set; } = 15;
        public int SUBWOOFER_INDEX { get; private set; } = 10;
        public int SUBWOOFER
        {
            get { return this.SUBWOOFER_INDEX - 10; }
        }
        public int BASS_INDEX { get; private set; } = 10;
        public int BASS
        {
            get { return this.BASS_INDEX - 10; }
        }
        public int TREBLE_INDEX { get; private set; } = 10;
        public int TREBLE
        {
            get { return this.TREBLE_INDEX - 10; }
        }
        public int CENTER_INDEX { get; private set; } = 10;
        public int CENTER
        {
            get { return this.CENTER_INDEX - 10; }
        }
        public int SURROUND_VOLUME_INDEX { get; private set; } = 10;
        public int SURROUND_VOLUME
        {
            get { return this.SURROUND_VOLUME_INDEX - 10; }
        }
        public int SURROUND_BALANCE_INDEX { get; private set; } = 10;
        public int SURROUND_BALANCE
        {
            get { return this.SURROUND_BALANCE_INDEX - 10; }
        }
        public int AV_DELAY { get; private set; } = 0;
        public string MUTE_STATUS { get; private set; } = "UNMUTE";
        public string HEIGHT_SPEAKERS_STATUS { get; private set; } = "ON";
        public int HEIGHT_LEVEL_INDEX { get; private set; } = 5;
        public int HEIGHT_LEVEL
        {
            get { return this.HEIGHT_LEVEL_INDEX - 5; }
        }
        public string SURROUND_STATUS { get; private set; } = "ON";
        public string VOLUME_LEVELER_STATUS { get; private set; } = "OFF";
        public string NIGHT_MODE_STATUS { get; private set; } = "OFF";
        public string EQ_STATUS { get; private set; } = "MOVIE";
        public string MODEL_NAME { get; private set; } = "SB46514-F6";
        public string DEMO_MODE { get; private set; } = "OFF";
        //------------------------------------------------------------------

        public string _portName;
        public SerialClient _serialClient;

        public ConcurrentQueue<byte[]> _receiveQueue;
        private Thread _processPortDataThread;
        private bool _processContinue;

        private ConcurrentQueue<string> _processQueue;
        private Thread _processStringThread;

        private bool changeData = false;
    }
}
