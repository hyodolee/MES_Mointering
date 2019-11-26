using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SmartFactory
{
    public class Udp_Client
    {

        Socket Client;
        IPEndPoint ep;
        IPAddress ip;
        EndPoint epRemote;

        private byte[] RecvData = new byte[1024 * 10];
        private int RecvSize = 1024 * 10;
        private Thread thisThread;
        private Dictionary<Thread, byte[]> resvMsgMap = new Dictionary<Thread, byte[]>();

        public Udp_Client()
        {

        }

        public string StartAsClient(string s_ip, string s_port)
        {
            string result = "";
            try
            {
                ip = IPAddress.Parse(s_ip);
                ep = new IPEndPoint(ip, Convert.ToInt32(s_port));
                epRemote = new IPEndPoint(IPAddress.Broadcast, Convert.ToInt32(s_port));
                //epRemote = new IPEndPoint(IPAddress.Any, 0);
                Client = new Socket(ep.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                //Client = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
                Client.BeginConnect(ep, new AsyncCallback(connectCallback), Client);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Client 연결 오류] " + ex.Message);
                result = "[Client 연결 오류] " + ex.Message;
                if (Client != null)
                    Client.Close();
            }
            return result;
        }

        public void StopAsClient()
        {
            Client.Dispose();
        }

        private void connectCallback(IAsyncResult ar)
        {
            Client = (Socket)ar.AsyncState;
            SocketError err;
            try
            {
                Client.EndConnect(ar);

                Console.WriteLine(Client.RemoteEndPoint.ToString() + "에 연결 완료");
                Client.BeginReceive(RecvData, 0, RecvSize, SocketFlags.None, out err, new AsyncCallback(receiveCollback), Client);
                //byte[] buffer = new byte[1500];
                //Client.BeginReceiveFrom(RecvData, 0, RecvSize, SocketFlags.None, ref epRemote, new AsyncCallback(receiveCollback), Client);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("[연결 실패] " + ex.Message);
            }
        }

        private void receiveCollback(IAsyncResult iar)
        {
            try
            {
                Socket remote = (Socket)iar.AsyncState;
                int i_recvSize = remote.EndReceive(iar);
                Console.WriteLine("[recv size] " + i_recvSize);
                if (i_recvSize > 0)
                {
                    string stringData = Encoding.UTF8.GetString(RecvData, 0, i_recvSize);
                    //Console.WriteLine("[recv Data] " + stringData);
                    CheckRecvMsg(RecvData, i_recvSize);

                    remote.BeginReceive(RecvData, 0, RecvSize, SocketFlags.None, new AsyncCallback(receiveCollback), remote);
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Client 수신 오류] " + ex.Message);
                if (Client != null)
                    Client.Close();
            }
        }

        public void CheckRecvMsg(byte[] RecvData, int i_recvSize)
        {
            byte[] tempRecv = RecvData.Take(i_recvSize).ToArray();

            if (thisThread != null)
            {
                if (thisThread.IsAlive)
                {
                    resvMsgMap.Add(thisThread, tempRecv);
                    thisThread.Interrupt();

                    return;
                }
            }

            //if (thisThread.IsAlive)
            //{
            //    resvMsgMap.Add(thisThread, tempRecv);
            //    thisThread.Interrupt();

            //    return;
            //}

            string sData = Encoding.UTF8.GetString(tempRecv);
            string receiveMessage = sData.Replace("\0", "").Trim();
            Console.WriteLine("CheckRecvMsg:" + receiveMessage);
        }

        public void SendData(byte[] sendByte)
        {
            try
            {
                Client.BeginSend(sendByte, 0, sendByte.Length, SocketFlags.None, new AsyncCallback(sendCallback), Client);
                //Client.BeginSend(sendByte, 0, sendByte.Length, SocketFlags.None, new AsyncCallback(receiveCollback), Client); 
                Console.WriteLine("[send]");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Client 송신 오류] " + ex.Message);
                if (Client != null)
                    Client.Close();
            }
        }

        public byte[] SendRequest(byte[] sendByte)
        {
            thisThread = Thread.CurrentThread;
            byte[] resvResponse;
            Client.BeginSend(sendByte, 0, sendByte.Length, SocketFlags.None, new AsyncCallback(sendCallback), Client);

            int lngTTL = 30000;
            try
            {
                Thread.Sleep(lngTTL);
                return null;
            }
            catch (ThreadInterruptedException e)
            {
                if (resvMsgMap.TryGetValue(thisThread, out resvResponse))
                {
                    return resvResponse;
                }
                return null;
            }
            finally
            {
                resvMsgMap.Remove(thisThread);
            }
        }

        private void sendCallback(IAsyncResult iar)
        {
            Console.WriteLine("sendCallback");
            Socket remote = (Socket)iar.AsyncState;
            int sent = remote.EndSend(iar);
        }

    }
}
