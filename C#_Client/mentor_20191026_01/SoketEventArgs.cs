using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mentor_20191026_01
{
    class SoketEventArgs : EventArgs
    {
        public DateTime EventTime { get; set; }
        public String Message { get; set; }
        public String EndPoint { get; set; }
        public String ProtocolType { get; set; }
        public String TransactionID { get; set; }
        public String SourceSubject { get; set; }
        public String TargetSubject { get; set; }
        public String Body { get; set; }
    }
}
