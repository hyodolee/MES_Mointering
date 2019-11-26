using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace mentor_20191026_01
{
    class TCP_Client
    {
        string bindIp;
        int bindPort;
        string serverIp;
        int serverPort;
        TcpClient client;
        NetworkStream stream;

        public TCP_Client(string bindIp, int bindPort, string serverIp, int serverPort)
        {
            this.bindIp = bindIp;
            this.bindPort = bindPort;
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            
        }

        public string serverConnect()
        {
            try
            {
                IPEndPoint clientAddress = new IPEndPoint(IPAddress.Parse(bindIp), bindPort);
                IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

                Console.WriteLine("클라이언트: {0}, 서버:{1}", clientAddress.ToString(), serverAddress.ToString());

                client = new TcpClient(clientAddress);

                client.Connect(serverAddress);

                stream = client.GetStream();
            }
            catch(SocketException e)
            {
                Console.WriteLine(e);
                return "fail";
            }

            //Console.WriteLine("클라이언트를 종료합니다.");
            return "success";
        }

        public string sendMsg(string message)
        {
            byte[] data = System.Text.Encoding.Default.GetBytes(message);

            stream.Write(data, 0, data.Length);

            Console.WriteLine("송신: {0}", message);

            if (message.Equals("EXIT"))
            {
                stream.Close();
                client.Close();

                Console.WriteLine("bye!");

                return "disconnect";
            }

            data = new byte[256];

            string responseData = "";

            int bytes = stream.Read(data, 0, data.Length);
            responseData = Encoding.Default.GetString(data, 0, bytes);
            Console.WriteLine("수신:{0}", responseData);

            return responseData;
        }
    }
}
