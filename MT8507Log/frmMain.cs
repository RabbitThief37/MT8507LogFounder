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

namespace MT8507Log
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

                this.portName = selectComport.selectPortName;
            }
            else
            {
                this.portName = portNames[0];
            }

            this.txtPortName.Text = this.portName;
            this.receiveQueue = new ConcurrentQueue<byte[]>();
            this._serialClient = new SerialClient(this.portName, this.receiveQueue);
            this._processQueue = new ConcurrentQueue<string>();

            this.timer1.Interval = 500;
            this.timer1.Start();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._processContinue = false;
            this._serialClient.CloseConn();
            this._serialClient.Dispose();

            Thread.Sleep(10);

            if ( this._processPortDataThread.ThreadState != ThreadState.Stopped )
            {
                this._processPortDataThread.Abort();
            }

            if (this._processStringThread.ThreadState != ThreadState.Stopped)
            {
                this._processStringThread.Abort();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();

            if (!this._serialClient.OpenConn())
            {
                MessageBox.Show(this, this._serialClient.ErrorMessage, "Serial Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            this._processContinue = true;
            this._processPortDataThread = new Thread(new ThreadStart(ProcessPortDataInQueue))
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
                if( this.receiveQueue.TryDequeue(out portData) == false )
                {
                    Thread.Sleep(10);
                    continue;
                }

                foreach (byte uartChar in portData)
                {
                    if(isCommand)
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
            string result = string.Empty;
            int pos = 0;

            while (this._processContinue)
            {
                if (this._processQueue.TryDequeue(out commandLine) == false)
                {
                    Thread.Sleep(10);
                    continue;
                }

                if(commandLine.Contains(COMMAND_PREFIX) == false)
                {
                    continue;
                }

                do
                {
                    pos = commandLine.IndexOf(COMMAND_ID);
                    if (pos < COMMAND_PREFIX.Length)
                    {
                        result = "ERR - No CommandID , " + commandLine;
                        break;
                    }

                    //System.Diagnostics.Debug.Print(commandLine);
                    //System.Diagnostics.Debug.Print("\r\n");

                    commandID = commandLine.Substring(pos + COMMAND_ID.Length, 6);

                    if (commandLine.Contains(COMMAND_DATA))
                    {
                        pos = commandLine.IndexOf(COMMAND_DATA);
                        commandArgument = commandLine.Substring(pos + COMMAND_DATA.Length, 4);
                    }
                    else
                    {
                        pos = commandLine.IndexOf(COMMAND_DATA_FIRST);
                        if (pos == -1)
                        {
                            commandArgument = string.Empty;
                            //result = string.Format("COMMAND ID:{0}", Convert.ToInt32(commandID, 16));
                        }
                        else
                        {
                            commandArgument = commandLine.Substring(pos + COMMAND_DATA_FIRST.Length, 4);
                            //result = string.Format("COMMAND ID:{0},DATA:{1}", Convert.ToInt32(commandID, 16), Convert.ToInt32(commandArgument, 16));
                        }
                    }

                    //System.Diagnostics.Debug.Print(result);
                    //System.Diagnostics.Debug.Print("\r\n");

                    result = MakeResultString(ref commandID, ref commandArgument);

                } while (false);

                this.lstResult.Invoke(new Action(() => this.lstResult.Items.Insert(0, result)));
            }
        }

        public string MakeResultString(ref string id, ref string arg)
        {
            string result = string.Empty;
            int valueID = Convert.ToInt32(id, 16);
            int valueArg = 0;

            if( arg.Length !=0 )
                valueArg = Convert.ToInt32(arg, 16);

            switch( valueID )
            {
                case 0x0001:
                    if (valueArg == 1 || valueArg == 2 || valueArg == 5)
                        result = "POWER_CTRL : ON";
                    else
                        result = "POWER_CTRL : OFF";
                    break;

                case 0x0010: result = string.Format("MASTER VOLUME : {0}", valueArg);    break;
                case 0x0012: result = string.Format("SUBWOOFER_VOLUME : {0}", valueArg - 10); break;
                case 0x0013: result = string.Format("BASS_CONTROL : {0}", valueArg - 10); break;
                case 0x0014: result = string.Format("TREBLE_CONTROL : {0}", valueArg - 10); break;
                case 0x0015: result = string.Format("CENTER_CONTROL : {0}", valueArg - 10); break;
                case 0x0016: result = string.Format("SURROUND_VOLUME : {0}", valueArg - 10); break;
                case 0x0017: result = string.Format("SURROUND_BALANCE : {0}", valueArg - 10); break;
                case 0x0018: result = string.Format("AV_DELAY_CONTROL : {0}", valueArg); break;

                case 0x0019:
                    result = (valueArg == 0) ? "MUTE_CONTROL : MUTE" : "MUTE_CONTROL : UN-MUTE";
                    break;

                case 0x001A:
                    if (valueArg == 0 )
                        result = "HEIGHT_SPEAKERS : OFF";
                    else if (valueArg == 1)
                        result = "HEIGHT_SPEAKERS : ON";
                    else
                        result = "HEIGHT_SPEAKERS : VRT";
                    break;

                case 0x001B: result = string.Format("HEIGHT_LEVEL : {0}", valueArg - 5); break;

                case 0x0020:
                    {
                        switch(valueArg)
                        {
                            case 1: result = "INPUT_SELECT : AUX"; break;
                            case 2: result = "INPUT_SELECT : COAX DIGITAL"; break;
                            case 3: result = "INPUT_SELECT : OPTICAL DIGITAL"; break;
                            case 4: result = "INPUT_SELECT : HDMI"; break;
                            case 5: result = "INPUT_SELECT : HDMI-ARC"; break;
                            case 6: result = "INPUT_SELECT : BLUETOOTH"; break;
                            case 7: result = "INPUT_SELECT : USB"; break;
                            case 8: result = "INPUT_SELECT : GOOGLE CAST"; break;
                        }
                    }
                    break;

                case 0x0040: result = (valueArg == 0) ? "SURROUND_CONTROL : OFF" : "SURROUND_CONTROL : ON"; break;
                case 0x0041: result = (valueArg == 0) ? "VOLUME LEVELER : OFF" : "VOLUME LEVELER : ON"; break;
                case 0x0042: result = (valueArg == 0) ? "NIGHT_MODE_CONTROL : OFF" : "NIGHT_MODE_CONTROL : ON"; break;

                case 0x0043:
                    if (valueArg == 0)
                        result = "EQ : MUSIC";
                    else if (valueArg == 1)
                        result = "EQ : MOVIE";
                    else
                        result = "EQ : DIRECT";
                    break;

                case 0x0050: result = "SOFTWARE DETAIL VERSION"; break;

                case 0x8100: result = "RESPONSE_REQUEST_INIT_VAL";  break;

                default:
                    result = string.Format("---> REQUEST TO KS.KIM...ID:0x{0:X4}", id);
                    break;
            }


            return string.Format("[{0}] {1}",DateTime.Now.ToString("HH:mm:ss"), result);
        }


        public const string COMMAND_PREFIX = "<misc_uart_mcu_execute_send_cmd>";
        public const string COMMAND_ID = "UART_MCU_COMMAND_ID:{";
        public const string COMMAND_DATA = "},Data{";
        public const string COMMAND_DATA_FIRST = "},0{";

        public string portName;
        public SerialPort serialPort;

        public SerialClient _serialClient;

        public ConcurrentQueue<byte[]> receiveQueue;
        private Thread _processPortDataThread;
        private bool _processContinue;

        private ConcurrentQueue<string> _processQueue;
        private Thread _processStringThread;
    }
}
