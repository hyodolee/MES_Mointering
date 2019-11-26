using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace mentor_20191026_01
{
    class Program
    {
        private static UDP_Server udpServer;
        private static TCP_Client tcpClient;
        private static ILog log = LogManager.GetLogger("Program");

        private static List<Machine> machineList = new List<Machine>();
        static void Main(string[] args)
        {
            //Console.WriteLine("The current console title is: \"{0}\"", Console.Title);
            //Console.Title = "SMART FACTORY MES PROGRAMM";
            //Console.WriteLine(Console.Title);

            do
            {                
                Console.Write('#');
                String sCommand = Console.ReadLine();
                switch(sCommand.Trim())
                {
                    case "EXIT":
                        goto EXIT;
                    default:
                        sExcuteCmd(sCommand);
                        break;
                }

            } while (true);

            EXIT:
            log.Info("Good Bye !");
        }

        static string sExcuteCmd(string sMessage)
        {
            string[] splitCommand = sMessage.ToUpper().Split(new String[] { " " }, StringSplitOptions.None);

            switch (splitCommand[0])
            {
                case "EXIT":
                    return "FAIL";

                case "SET":
                    if (splitCommand[1].Equals("MACHINE"))
                    {
                        String setMachineName = splitCommand[2];
                        if (setMachineName.Length > 0)
                        {
                            Machine newMachine = new Machine();
                            newMachine.OnMachineChanged += newMachine_OnMachineEvent;
                            newMachine.OnMachineStateChanged += newMachine_OnMachineStateChangedEvent;
                            newMachine.set(setMachineName);

                            machineList.Add(newMachine);

                            return "SUCCESS";
                        }
                    }
                    else if (splitCommand[1].Equals("MACHINESTATE"))
                    {
                        string findMachineName = splitCommand[2];
                        string stateName = splitCommand[3];

                        foreach (Machine machine in machineList)
                        {
                            //세팅할 machine과 state 값이 있을때
                            if (machine.MachineName.Equals(findMachineName) && stateName.Length > 0)
                            {
                                machine.SetStateName(splitCommand[3]);
                            }
                        }
                    }
                    else
                    {
                        goto default;
                    }
                    break;

                case "VIEW":
                    if (splitCommand[1].Equals("MACHINE"))
                    {
                        //string[] split_Messages = sMessage.Split(new String[] { " " }, StringSplitOptions.None);

                        //String findMachineName = split_Messages[2]; 
                        String findMachineName = splitCommand[2]; 

                        foreach (Machine machine in machineList)
                        {
                            if (machine.MachineName.Equals(findMachineName))
                            {
                                //Console.WriteLine(" MachineName : {0}", machine.MachineName);
                                //Console.WriteLine(" LastEventTime : {0}", machine.LastEventTime);
                                //Console.WriteLine(" LastEventName : {0}", machine.LastEventName);

                                machine.view();
                                return "SUCCESS";
                            }
                            else
                            {
                                Console.WriteLine(" nomachine");
                            }
                        }
                    }
                    else if (splitCommand[1].Equals("MACHINELIST"))
                    {
                        //Console.WriteLine(" machinename : {0}", machineList.Count);
                        foreach (Machine machine in machineList)
                        {
                            machine.view();
                            Console.WriteLine();
                            //return "SUCCESS";
                        }
                    }
                    else
                    {
                        goto default;
                    }
                    break;
                case "SAVE":
                    if (splitCommand[1].Equals("MACHINE"))
                    {
                        String findMachineName = splitCommand[2];
                        foreach (Machine machine in machineList)
                        {
                            if (machine.MachineName.Equals(findMachineName))
                            {
                                machine.save();
                            }
                        }
                    }
                    else
                    {
                        goto default;
                    }
                    break;

                case "LOAD":
                    if (splitCommand[1].Equals("MACHINE"))
                    {
                        var reader = new StreamReader(@"C:\hd\mentor\xmlfile\machine.xml");

                        XmlSerializer xs = new XmlSerializer(typeof(Machine));
                        Machine machine = (Machine)xs.Deserialize(reader);
                        machineList.Add(machine);

                        machine.view();
                    }
                    else
                    {
                        goto default;
                    }
                    break;

                case "START": /*  TCP/UDP 통신 Start  */
                    if (splitCommand[1].Equals("TCP"))
                    {
                        //tcpConnect = new TcpConnect();
                        //tcpConnect.StartListening(splitCommand[2]);
                        //tcpConnect.OnReceiveMessage += OnReceiveMeaasge;
                        //return "SUCCESS";
                    }
                    else if (splitCommand[1].Equals("UDP"))
                    {
                        udpServer = new UDP_Server();
                        udpServer.OnMessage += server_OnMessageEvent;
                        udpServer.StartAsServer(splitCommand[2], splitCommand[3]);
                        udpServer.MessageEvent += OnReceiveMeaasge;
                        return "SUCCESS";
                    }

                    break;

                case "SEND":
                    //if (splitCommand.Length < 4)
                    //{
                    //    Console.WriteLine("사용법 : {0} <BInd IP> <Bind Port> <Server IP> <Server Port>");
                    //    goto default;
                    //}
                    
                    if (splitCommand[1].Equals("TCP"))
                    {
                        if(splitCommand[2].Equals("CONNECT"))
                        {
                            tcpClient = new TCP_Client("127.0.0.1", 8889, "127.0.0.1", 8888);
                            tcpClient.serverConnect();
                        }
                        else if (splitCommand[2].Equals("GETMACHINE"))
                        {
                            string return_Value = tcpClient.sendMsg(splitCommand[2]);
                            string[] split = return_Value.Split(new String[] { " " }, StringSplitOptions.None);
                            string setMachineName = split[0];

                            if (setMachineName.Length > 0)
                            {
                                Machine newMachine = new Machine();
                                newMachine.OnMachineChanged += newMachine_OnMachineEvent;
                                newMachine.OnMachineStateChanged += newMachine_OnMachineStateChangedEvent;
                                newMachine.set(setMachineName);

                                newMachine.SetStateName(split[1]);
                                newMachine.setFactoryName = (split[2]);
                                newMachine.setAreaName = split[3];

                                machineList.Add(newMachine);

                                return "SUCCESS";
                            }
                        }
                        else
                        {
                            tcpClient.sendMsg(splitCommand[2]);
                        }
                        
                    }
                    else if (splitCommand[1].Equals("UDP"))
                    {
                        UDP_Client client = new UDP_Client();
                        client.OnMessage += client_OnMessageEvent;
                        client.StartAsClient(splitCommand[1], splitCommand[2]); //ip , port
                        client.SendData(Encoding.UTF8.GetBytes(splitCommand[3]));
                    }
                    
                    break;

                default:
                    log.Info("다시 입력바랍니다.");
                    break;
            }

            return "FAIL";
        }

        static void newMachine_OnMachineEvent(object sender, MachineEventArgs e)
        {
            //Console.WriteLine(string.Format("[{0}][{1}][{2}][{3}][{4}]", e.EventTime, e.MachineName, e.MachineEventName, e.EventUser));
            Console.WriteLine(string.Format("[{0}][{1}][{2}][{3}]", e.EventTime, e.MachineName, e.MachineEventName, e.EventUser));
            log.Debug("newMachine_OnMachineEvent started");
        }

        static void newMachine_OnMachineStateChangedEvent(object sender, MachineEventArgs e)
        {
            Console.WriteLine(string.Format("[{0}][{1}][{2}] {3}", e.EventTime, e.MachineName, e.MachineEventName, e.StateName));
        }

        static void server_OnMessageEvent(object sender, SoketEventArgs e)
        {
            log.Info(string.Format("[{0}][{1}]{2}", e.ProtocolType, e.EndPoint, e.Message));
        }

        static void client_OnMessageEvent(object sender, SoketEventArgs e)
        {
            log.Info(string.Format("[{0}][{1}]{2}", e.ProtocolType, e.EndPoint, e.Message));
        }
        static void OnReceiveMeaasge(object sender, SoketEventArgs e)
        {
            log.Info(string.Format("[{0}][{1}]{2}"
                                     , e.ProtocolType, e.EndPoint, e.Message));

            string sReplyMessage = sExcuteCmd(e.Message.ToUpper());

            if (sReplyMessage.Equals(string.Empty) == false)
            {
                switch (e.ProtocolType.ToUpper())
                {
                    case "TCP":
                        string[] EndPoint = e.EndPoint.Split(':');
                        //tcpConnect.SendMessage(EndPoint[0], EndPoint[1], sReplyMessage);

                        break;

                    case "UDP":
                        udpServer.SendResponse(Encoding.UTF8.GetBytes(sReplyMessage));
                        break;
                }
            }

        }
    }
}
