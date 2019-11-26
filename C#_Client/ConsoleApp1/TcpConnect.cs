using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SmartFactory
{
    public class TcpConnect
    {
        /* TCP 통신로직 */
        private Socket tcpListener;  /* Server Socket */
        private List<Socket> m_ClientSocket;

        //delegate void Receive_Completed(object sender, SocketAsyncEventArgs e);

        public void StartListening(string sPort)
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024*4];

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse(sPort));


                // Create a TCP/IP socket.
                tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.
                try
                {
                    m_ClientSocket = new List<Socket>();
                    tcpListener.Bind(localEndPoint);
                    tcpListener.Listen(10);

                    /* 비동기방식 수신 이벤트 선언 */
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);


                    /* 비동기 수신대기 */
                    tcpListener.AcceptAsync(args);

                    Console.WriteLine("Wait in a message...");

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }

        }

        void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                Socket ClientSocket = e.AcceptSocket;
                m_ClientSocket.Add(ClientSocket);

                if (m_ClientSocket != null)
                {
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();

                    byte[] bytes = new byte[1024*4];

                    args.SetBuffer(bytes, 0, bytes.Length);
                    args.UserToken = m_ClientSocket;
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);

                    ClientSocket.ReceiveAsync(args);
                }
                e.AcceptSocket = null;
                tcpListener.AcceptAsync(e);
            }
            catch { }
        }

        void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = (Socket)sender;

            if (ClientSocket.Connected && e.BytesTransferred > 0)
            {
                EndPoint tcpEp = ((System.Net.Sockets.Socket)(sender)).RemoteEndPoint;

                byte[] receiveData = e.Buffer;    // 데이터 수신
                string sData = Encoding.UTF8.GetString(receiveData);

                string receiveMessage = sData.Replace("\0", "").Trim();
                //Console.WriteLine(string.Format("[{0}] : {1}", tcpEp.ToString(), "Test"));
                //Console.WriteLine(string.Format("{0}", receiveMessage));
                //Console.Write('#');
                
                SoketEventArgs args = new SoketEventArgs();
                args.EventTime = DateTime.Now;
                args.Message = receiveMessage;
                args.ProtocolType = ClientSocket.ProtocolType.ToString();
                args.EndPoint = tcpEp.ToString();

                ReceiveMessage(args);               // 이벤트 전달 
                

                for (int i = 0; i < receiveData.Length; i++)
                {
                    receiveData[i] = 0;
                }
                e.SetBuffer(receiveData, 0, receiveData.Length);

                ClientSocket.ReceiveAsync(e);
            }
            else
            {
                ClientSocket.Disconnect(false);
                ClientSocket.Dispose();
                m_ClientSocket.Remove(ClientSocket);
            }
        }

        public void StopListening()
        {
            tcpListener.Dispose();
        }

        /* TcpListener 사용 로직 
        private TcpListener listener;
        public void Start(String sPort, Action<object> asyncTcpProcess)
        {
            AysncEchoServer(sPort, asyncTcpProcess).Wait();
        }

        public void Stop()
        {
            listener.Stop();
        }

        async Task AysncEchoServer(String sPort, Action<object> asyncTcpProcess)
        {

            listener = new TcpListener(IPAddress.Any, int.Parse(sPort));
            listener.Start();

            Console.WriteLine("Wait in a message...");

            listener.BeginAcceptTcpClient( , null);

            while (true)
            {
                // 비동기 Accept                
                TcpClient tc = await listener.AcceptTcpClientAsync().ConfigureAwait(false);

                // 새 쓰레드에서 처리
                await Task.Factory.StartNew(asyncTcpProcess, tc);
            }
        }
        async static void AsyncTcpProcess(object o)
        {
            TcpClient tc = (TcpClient)o;

            int MAX_SIZE = 1024;  // 가정
            NetworkStream stream = tc.GetStream();

            // 비동기 수신            
            var buff = new byte[MAX_SIZE];
            var nbytes = await stream.ReadAsync(buff, 0, buff.Length).ConfigureAwait(false);
            if (nbytes > 0)
            {
                string msg = Encoding.ASCII.GetString(buff, 0, nbytes);
                Console.WriteLine($"{msg} at {DateTime.Now}");

                // 비동기 송신
                await stream.WriteAsync(buff, 0, nbytes).ConfigureAwait(false);
            }

            stream.Close();
            tc.Close();
        }
        */

        public void SendMessage(string sIP, string sPort, string sMessage)
        {
            try
            {
                using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    // 서버와 연결
                    client.Connect(new IPEndPoint(IPAddress.Parse(sIP), int.Parse(sPort)));

                    // 데이터를 전송
                    client.Send(Encoding.UTF8.GetBytes(sMessage));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        public void SendRequest_Message(string sIP, string sPort, string sMessage)
        {
            try
            {
                using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    // 서버와 연결
                    client.Connect(new IPEndPoint(IPAddress.Parse(sIP), int.Parse(sPort)));

                    // 데이터를 전송
                    client.Send(Encoding.UTF8.GetBytes(sMessage));

                    //응답대기 비동기방식
                    StartListening(client.LocalEndPoint.ToString().Split(':')[1]);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        public event EventHandler<SoketEventArgs> OnReceiveMessage;

        protected virtual void ReceiveMessage(SoketEventArgs e)
        {
            EventHandler<SoketEventArgs> handler = OnReceiveMessage;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }


}
