using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MT8507Log
{
    public class ArduinoRemote : IDisposable
    {
        public ArduinoRemote()
        {
            this._rmcSerialPort = new SerialPort();
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
        public bool SendCommand(string command)
        {
            if (this.IsOpen == false)
            {
                this.ErrorMessage = "RMC TX가 연결되지 않았습니다.\nSerial Port 연결을 확인해 주세요.";
                return false;
            }

            try
            {
                this._rmcSerialPort.WriteLine(command);
                Thread.Sleep(WAIT_COMMAND_EXECUTION_TIME);
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        private const int WAIT_COMMAND_EXECUTION_TIME = 500;

        public string ErrorMessage { get; private set; } = string.Empty;
        public bool IsOpen { get; private set; } = false;

        private SerialPort _rmcSerialPort;
        private string _portName;
    }
}
