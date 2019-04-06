using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT8507Log
{
    public class APxInputChannelInfo
    {
        public enum APX_TOTAL_INPUT_CHANNEL : int
        {
            CH_3 = 0,   //"FL,FR,Sub 3ch (2.1)"
            CH_6,       //"FL,FR,Ctr,FTL,FTR,Sub 6ch (3.1.2)"
            CH_7,       //"FL,Ctr,FTL,SL,SR,RTL,Sub 7ch (5.1.4)"
            CH_8        //"FL,FR,Ctr,FTL,FTR,SL,SR,Sub 8ch (5.1.2)"
        }

        public APX_TOTAL_INPUT_CHANNEL TotalOutputChannel { get; set; }

        public int FrontLeft { get; set; }
        public int FrontRight { get; set; }
        public int FrontCenter { get; set; }
        public int FrontTopLeft { get; set; }
        public int FrontTopRight { get; set; }
        public int SurroundLeft { get; set; }
        public int SurroundRight { get; set; }
        public int SurroundTopLeft { get; set; }
        public int SurroundTopRight { get; set; }
        public int Subwoofer { get; set; }
        
    }
}
