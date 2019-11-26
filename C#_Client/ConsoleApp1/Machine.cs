using System;

namespace SmartFactory
{
    public class Machine
    {

        private string machineName;                            // 장비명
        public string FactoryName ;                     // Factory명
        public string AreaName ;                        // Area명
        public string SuperMachineName ;                // 상위 장비명
        public string MachineGroupName ;                // Machine group 명
        public int    ProcessCount ;                    // 처리되고 있는 수
        public string ResourceState ;                   // Resource State
        public string E10State ;                        // 장비 E10State [Productive|StandBy|Engineering|UnscheduledDown|ScheduledDown|NonScheduled]  
        public string CommunicationState ;              // 장비 통신 상태[OffLine|OnLineLocal|OnLineRemote]  
        public string StateName ;                       // Machine State명 [IDLE|RUN|DOWN|PM]
        public string EventName ;                       // Machine Evnet 명
        public string LastEventName ;                   // Last event 명
        public string LastEventUser ;                   // Last event user
        public string LastEventComment ;                // Last event Comment
        public DateTime LastEventTime ;                 // Last event Time
        public string LastEventFlag ;                   // Last event flag
        public string ReasonCodeType ;                  // Reason Code Type
        public string ReasonCode ;                      // reason code


        public string MachineName
        {
            get
            {
                return machineName;
            }

            set
            {
                machineName = value;
            }
        }

        public void SetMachine(string machineName)
        {
            this.MachineName = machineName;
            this.LastEventTime = DateTime.Now;
            this.LastEventName = "Create";
        }

        public void SetMachineState(string stateName)
        {
            this.StateName = stateName;
            this.LastEventTime = DateTime.Now;
            this.LastEventName = "ChangeMachineState";

            MachineEventArgs e = new MachineEventArgs();
            e.MachineName = this.MachineName;
            e.EventTime = DateTime.Now;
            e.EventUser = this.MachineName;
            e.MachineEventName = this.LastEventName;
            e.EventUser = this.MachineName;
            e.EventValue = stateName;

            OnMachineEvent(e);
        }

        public string View()
        {
            //Console.WriteLine(" MachineName : {0}", this.MachineName);
            //Console.WriteLine(" LastEventTime : {0}", this.LastEventTime);
            //Console.WriteLine(" LastEventName : {0}", this.LastEventName);

            return Fomatter.XmlWriter(this);
        }

        public string ViewMachine(string machineName)
        {
            //Console.WriteLine(" MachineName : {0}", this.MachineName);
            //Console.WriteLine(" LastEventTime : {0}", this.LastEventTime);
            //Console.WriteLine(" LastEventName : {0}", this.LastEventName);

            return Fomatter.XmlWriter(this);
        }

        public event EventHandler<MachineEventArgs> MachineEvent;
        protected virtual void OnMachineEvent(MachineEventArgs e)
        {
            EventHandler<MachineEventArgs> handler = MachineEvent;
            if (handler != null)
            {
                handler(this, e);
            }
            /*
            EventHandler<MachineEventArgs> handler = MachineEvent;
            handler?.Invoke(this, e);
            */
        }

    }



}
