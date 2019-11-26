using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace mentor_20191026_01
{
    public class Machine
    {
        public string FactoryName;                     // Factory명
        public string AreaName;                        // Area명
        public string SuperMachineName;                // 상위 장비명
        public string MachineGroupName;                // Machine group 명
        public int ProcessCount;                       // 처리되고 있는 수
        public string ResourceState;                   // Resource State
        public string E10State;                        // 장비 E10State [Productive|StandBy|Engineering|UnscheduledDown|ScheduledDown|NonScheduled]  
        public string CommunicationState;              // 장비 통신 상태[OffLine|OnLineLocal|OnLineRemote]  
        public string StateName;                       // Machine State명 [IDLE|RUN|DOWN|PM]
        public string EventName;                       // Machine Evnet 명
        public string LastEventName;                   // Last event 명
        public string LastEventUser;                   // Last event user
        public string LastEventComment;                // Last event Comment
        public DateTime LastEventTime;                 // Last event Time
        public string LastEventFlag;                   // Last event flag
        public string ReasonCodeType;                  // Reason Code Type
        public string ReasonCode;                      // reason code

        public event EventHandler<MachineEventArgs> OnMachineChanged;
        public event EventHandler<MachineEventArgs> OnMachineStateChanged;

        protected virtual void OnMachineEvent(MachineEventArgs e)
        {
            //EventHandler<MachineEventArgs> handler = OnMachineChanged;
            //if (handler != null)
            //{
            //    handler(this, e);
            //}

            if (OnMachineChanged != null)
                OnMachineChanged(this, e);
        }

        protected virtual void OnMachineStateChangedEvent(MachineEventArgs e)
        {
            if (OnMachineStateChanged != null)
                OnMachineStateChanged(this, e);
        }


        public void set(string machine)
        {
            String setMachineName = machine;
            if (setMachineName.Length > 0)
            {
                this.MachineName = setMachineName;
                this.EventName = "Create";
                this.LastEventTime = DateTime.Now;

                MachineEventArgs args = new MachineEventArgs();
                args.EventTime = DateTime.Now;
                args.MachineName = this.MachineName;
                args.MachineEventName = "set";

                OnMachineEvent(args);
            }
        }

        public void view()
        {
            //    Console.WriteLine(" MachineName : ", this.MachineName);
            //    Console.WriteLine(" LastEventTime : ", this.LastEventTime);
            //    Console.WriteLine(" LastEventName : ", this.LastEventName);
            getXLM();
        }

        public string viewXml()
        {
            string textValue;

            XmlSerializer xs = new XmlSerializer(typeof(Machine));
            //xs.Serialize(Console.Out, this);

            StringWriter sww = new StringWriter();
            XmlWriter writer = XmlWriter.Create(sww);
            xs.Serialize(writer, this);
            textValue = sww.ToString();

            return textValue;
        }

        public void getXLM()
        {
            XmlSerializer xs = new XmlSerializer(typeof(Machine));
            xs.Serialize(Console.Out, this);
        }

        public void save()
        {
            string savePath = @"C:\hd\mentor\xmlfile\machine.xml";
            string textValue;

            XmlSerializer xs = new XmlSerializer(typeof(Machine));
            //xs.Serialize(Console.Out, this);

            StringWriter sww = new StringWriter();
            XmlWriter writer = XmlWriter.Create(sww);
            xs.Serialize(writer, this);
            textValue = sww.ToString();

            System.IO.File.WriteAllText(savePath, textValue, System.Text.Encoding.Default);
        }

        public string MachineName { get; set; }

        public void SetStateName(string value)
        {
            this.StateName = value;

            MachineEventArgs args = new MachineEventArgs();
            args.EventTime = DateTime.Now;
            args.MachineName = this.MachineName;
            args.MachineEventName = "setMachineState";
            args.StateName = value;

            OnMachineStateChangedEvent(args);
        }
        public string setFactoryName
        {
            get { return FactoryName; }    // _data 반환
            set { FactoryName = value; }   // value 키워드 사용
        }
        public string setAreaName
        {
            get { return AreaName; }    // _data 반환
            set { AreaName = value; }   // value 키워드 사용
        }
    }

    public class MachineEventArgs : EventArgs
    {
        public DateTime EventTime;
        public string MachineName;
        public string MachineEventName;
        public string EventUser;
        public string StateName;
    }
}
