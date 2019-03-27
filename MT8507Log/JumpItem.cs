using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT8507Log
{
    //This class represents a measurement and signal path
    //These are items in a drop-down box which allows the user to jump to a specific measurement

    public class JumpItem
    {
        public JumpItem(string signalPath, string measurement)
        {
            this.SignalPath = signalPath;
            this.Measurement = measurement;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", this.SignalPath, this.Measurement);
        }

        public string Measurement { get; private set; } = string.Empty;
        public string SignalPath { get; private set; } = string.Empty;
    }
}
