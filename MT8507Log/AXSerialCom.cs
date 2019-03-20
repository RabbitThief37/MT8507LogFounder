using System;
using System.IO.Ports;
using System.Threading;
using Diagnostics = System.Diagnostics;
using System.Collections.Concurrent;

namespace MT8507Log
{
    //-------------------------------------------------------------------
    //This AX-Fast Serial Library
    //Developer: Ahmed Mubarak - RoofMan
    //This Library Provide The Fastest & Efficient Serial Communication
    //Over The Standard C# Serial Component
    //-------------------------------------------------------------------

    public class SerialClient : IDisposable
    {
        private string _port;
        private int _baudRate;
        private string errorMessage;

        private SerialPort _serialPort;
        private Thread serThread;
        private ConcurrentQueue<byte[]> _queue;

        /*The Critical Frequency of Communication to Avoid Any Lag*/
        private const int freqCriticalLimit = 20;

        public SerialClient(string port, ConcurrentQueue<byte[]> queue)
        {
            this._port = port;
            this._baudRate = 921600;
            this._queue = queue;

            serThread = new Thread(new ThreadStart(SerialReceiving))
            {
                Priority = ThreadPriority.Normal
            };
            serThread.Name = "SerialHandle" + serThread.ManagedThreadId.ToString();
        }

        public string Port
        {
            get { return _port; }
        }
        public int BaudRate
        {
            get { return _baudRate; }
        }
        public string ConnectionString
        {
            get
            {
                return String.Format("[Serial] Port: {0} | Baudrate: {1}",
                    _serialPort.PortName, _serialPort.BaudRate.ToString());
            }
        }
        public string ErrorMessage
        {
            get { return this.errorMessage; }
        }

        public bool OpenConn()
        {
            try
            {
                if (_serialPort == null)
                {
                    _serialPort = new SerialPort(_port, _baudRate, Parity.None);
                    _serialPort.ReadBufferSize = 921600;
                }

                if (!_serialPort.IsOpen)
                {
                    _serialPort.ReadTimeout = -1;
                    _serialPort.WriteTimeout = -1;

                    _serialPort.Open();

                    if (_serialPort.IsOpen)
                        serThread.Start(); /*Start The Communication Thread*/
                }
            }
            catch (Exception ex)
            {
                this.errorMessage = ex.Message;
                return false;
            }

            return true;
        }

        public void CloseConn()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                serThread.Abort();

                if (serThread.ThreadState == ThreadState.Aborted)
                    _serialPort.Close();
            }
        }

        public bool ResetConn()
        {
            CloseConn();
            return OpenConn();
        }

        public void Dispose()
        {
            CloseConn();

            if (_serialPort != null)
            {
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        //---------------------------------//
        //            THREAD 
        //---------------------------------//
        private void SerialReceiving()
        {
            while (true)
            {
                int count = _serialPort.BytesToRead;

                if (count < 1)
                {
                    Thread.Sleep(5);
                    continue;
                }

                /*Form The Packet in The Buffer*/
                byte[] buf = new byte[count];
                _serialPort.Read(buf, 0, count);
                this._queue.Enqueue(buf);
            }
        }
    }
}
