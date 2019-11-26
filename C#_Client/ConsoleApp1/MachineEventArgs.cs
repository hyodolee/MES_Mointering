using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactory
{
    public class MachineEventArgs : EventArgs
    {
        public DateTime EventTime ;            
        public string MachineName ;            
        public string MachineEventName ;                                
        public string EventUser ;
        public string EventValue;
    }
}
