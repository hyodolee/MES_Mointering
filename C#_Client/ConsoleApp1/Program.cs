using System;
using System.Collections.Generic;
using log4net;
using System.Xml;
using System.Text;
using System.Timers;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace SmartFactory
{
    class Program
    {
        //private static ILog logger = LogManager.GetLogger("Program");
        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static List<Machine> machineList = new List<Machine>();
        private static List<MachineEventArgs> machineEventArgsList = new List<MachineEventArgs>();
        private static List<SoketEventArgs> soketEventArgsList = new List<SoketEventArgs>();

        private static TcpConnect tcpConnect;
        private static Udp_Server udpServer;
        private static Udp_Client udpClient;

        private static Timer timer = new Timer();
        static void Main(string[] args)
        {
            Console.Title = String.Format("{0}       ver {1}.{2}", "SmartEAP", "0", "1");
            Console.WriteLine("=========================================================================");
            Console.WriteLine("=                        "  + Console.Title + "                         =");
            Console.WriteLine("=========================================================================");

            CmdExcute("LOAD MACHINELIST");

            if (CmdExcute("Start udp 127.0.0.1 5001").Equals("SUCCESS")) Console.WriteLine("");

            do
            {
                Console.Write('#');
                String sCommand = Console.ReadLine().ToUpper();

                switch (sCommand.Trim())
                {
                    case "EXIT":
                        goto EXIT;

                    default:
                        string sresult = CmdExcute(sCommand);
                        logger.Info(sresult);
                        break;
                }

            } while (true);

            EXIT:
            //Console.WriteLine("Good Bye !");
            logger.Info("Good Bye !");
        }

        static string CmdExcute(string sCommand)
        {
            string[] splitCommand = sCommand.ToUpper().Split(new String[] { " " }, StringSplitOptions.None);

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
                            newMachine.SetMachine(setMachineName);

                            machineList.Add(newMachine);

                            return "SUCCESS";
                        }
                    }
                    else if (splitCommand[1].Equals("MACHINESTATE"))
                    {
                        foreach (Machine machine in machineList)
                        {
                            if (machine.MachineName.Equals(splitCommand[2]))
                            {
                                machine.SetMachineState(splitCommand[3]);
                                return machine.View();
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
                        String findMachineName = splitCommand[2];
                        foreach (Machine machine in machineList)
                        {
                            if (machine.MachineName.Equals(findMachineName))
                            {
                                logger.Info(machine.ViewMachine(machine.MachineName));
                                return "SUCCESS";
                            }
                        }
                    }
                    else if (splitCommand[1].Equals("MACHINELIST"))
                    {
                        //logger.Info(Fomatter.XmlWriter(machineList));
                        //return "SUCCESS";
                        return Fomatter.XmlWriter(machineList);
                    }
                    else
                    {
                        goto default;
                    }
                    break;

                case "SAVE":
                    if (splitCommand[1].Equals("MACHINELIST"))
                    {
                        String sFileName = @".\MachineList.xml";
                        if (sFileName.Length > 0)
                        {
                            logger.Info("Start Save MachineList ... ");

                            XmlDocument doc = new XmlDocument();
                            doc.Load(new System.IO.StringReader(Fomatter.XmlWriter(machineList)));
                            doc.Save(sFileName);

                            logger.Info("End Save MachineList ");
                        }
                    }
                    else
                    {
                        goto default;
                    }
                    break;

                case "LOAD":
                    if (splitCommand[1].Equals("MACHINELIST"))
                    {
                        String sFileName = @".\MachineList.xml";
                        if (sFileName.Length > 0)
                        {
                            logger.Info("Start Loading MachineList ... ");

                            XmlDocument doc = new XmlDocument();
                            doc.Load(sFileName);
                            machineList = (List<Machine>)Fomatter.XmlDeserialize(doc.OuterXml);

                            logger.Info("End Loading MachineList ");
                        }
                    }
                    else
                    {
                        goto default;
                    }
                    break;


                case "START": /* TCP | UDP 통신 Start  */
                    if (splitCommand[1].Equals("TCP"))
                    {
                        tcpConnect = new TcpConnect();
                        tcpConnect.StartListening(splitCommand[2]);
                        tcpConnect.OnReceiveMessage += OnReceiveMeaasge;
                        return "SUCCESS";
                    }
                    else if (splitCommand[1].Equals("UDP"))
                    {
                        udpServer = new Udp_Server();
                        udpServer.StartAsServer(splitCommand[2], splitCommand[3]);
                        udpServer.MessageEvent += OnReceiveMeaasge;
                        return "SUCCESS";
                    }
                    else if (splitCommand[1].Equals("AUTOSAVE"))
                    {
                        int interval = 60;
                        if (splitCommand.Length > 2)
                        {
                            //if (int.TryParse(splitCommand[2], out interval)) SET_AUTOSAVE(interval);
                        }
                    }
                    else if (splitCommand[1].Equals("MULTICAST"))
                    {
                        if (udpServer != null)
                        {
                            udpServer.JoinMulticastGroup();
                            return "SUCCESS";
                        }

                    }
                    else if (splitCommand[1].Equals("AUTOSEND"))
                    {
                        int interval;
                        if (int.TryParse(splitCommand[2], out interval))
                        {
                            AutoSendMessage(interval, splitCommand[3], splitCommand[4], splitCommand[5]);
                        }

                    }
                    break;

                case "STOP": /* TCP | UDP 통신 Stop  */
                    if (splitCommand[1].Equals("TCP"))
                    {
                        tcpConnect.StopListening();
                        logger.Info("Dispose of a TCP Listener ...");
                        return "SUCCESS";
                    }
                    else if (splitCommand[1].Equals("UDP"))
                    {
                        udpServer.StopAsServer();
                        logger.Info("Dispose of a UDP Listener ...");
                        return "SUCCESS";
                    }
                    break;

                case "SEND":
                    Udp_Client client = new Udp_Client();
                    client.StartAsClient(splitCommand[1], splitCommand[2]);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 3; i < splitCommand.Length; i++)
                    {
                        sb.Append(splitCommand[i] + " ");
                    }
                    client.SendData(Encoding.UTF8.GetBytes(sb.ToString().Trim()));
                    break;

                case "SENDR":
                    if (splitCommand[1].Equals("UDP"))
                    {
                        if (udpClient == null)
                        {
                            udpClient = new Udp_Client();
                            udpClient.StartAsClient(splitCommand[2], splitCommand[3]);
                        }

                        StringBuilder sb2 = new StringBuilder();
                        for (int i = 4; i < splitCommand.Length; i++)
                        {
                            sb2.Append(splitCommand[i] + " ");
                        }
                        byte[] bResponse = udpClient.SendRequest(Encoding.UTF8.GetBytes(sb2.ToString()));

                        return Encoding.UTF8.GetString(bResponse);
                    }
                    break;

                default:
                    logger.Info("다시 입력바랍니다.");
                    break;
            }

            return null;
        }

        private static void AutoSendMessage(int interval, string ProtocolType, string sIP, string sPort)
        {
            timer = new Timer();
            timer.Interval = interval * 1000;

            //timer.Elapsed += new ElapsedEventHandler(timer_Elaspsed);

            string sResponse = CmdExcute(string.Format("sendr {0} {1} {2} view machinelist", ProtocolType, sIP, sPort));
            machineList = (List<Machine>)Fomatter.XmlDeserialize(sResponse);

            Random r = new Random();
            string sChangeState = "";
            Machine machine;

            timer.Elapsed += (sender, args) =>
            {
                int i = r.Next(0, machineList.Count);
                machine = machineList[i];

                switch (machine.StateName)
                {
                    case "WAIT":
                        sChangeState = "RUN";
                        break;
                    case "RUN":
                        sChangeState = "WAIT";
                        break;
                    case "DOWN":
                        return;
                    default:
                        sChangeState = "WAIT";
                        break;
                }

                string sCmdMsg = string.Format("sendr {0} {1} {2} set machinestate {3} {4}", ProtocolType, sIP, sPort, machine.MachineName, sChangeState);
                logger.Info(sCmdMsg);

                sResponse = CmdExcute(sCmdMsg);
                if (sResponse != null)
                {
                    //logger.Info(sResponse);
                    machine = (Machine)Fomatter.XmlDeserialize(sResponse);
                    machineList[i].StateName = machine.StateName;
                }
            };
            timer.Start();
        }
        static void OnMachineEvent(object sender, MachineEventArgs e)
        {
            machineEventArgsList.Add(e);
            do
            {
                MachineEventArgs machineEventArgs = new MachineEventArgs();
                machineEventArgs = machineEventArgsList[0];
                logger.Info(string.Format("[{0}][{1}] {2}"
                                           , machineEventArgs.MachineName
                                           , machineEventArgs.MachineEventName
                                           , machineEventArgs.EventUser
                                  ));

                machineEventArgsList.Remove(machineEventArgs);
            } while (machineEventArgsList.Count > 0);
        }

        static void OnReceiveMeaasge(object sender, SoketEventArgs e)
        {
            logger.Info(string.Format("[{0}][{1}]{2}"
                                     , e.ProtocolType, e.EndPoint, e.Message));

            //string sReplyMessage = CmdExcute(e.Message.ToUpper());

            //if (sReplyMessage.Equals(string.Empty) == false)
            //{
            //    switch (e.ProtocolType.ToUpper())
            //    {
            //        case "TCP":
            //            string[] EndPoint = e.EndPoint.Split(':');
            //            tcpConnect.SendMessage(EndPoint[0], EndPoint[1], sReplyMessage);

            //            break;

            //        case "UDP":
            //            udpServer.SendResponse(Encoding.UTF8.GetBytes(sReplyMessage));
            //            break;
            //    }
            //}

        }


    }
}