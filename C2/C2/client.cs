using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace C2
{
    class client
    {
        public static string ToBeginGame = "Start";
        private static byte[] result = new byte[1024];
        public string sname { get; set; }
        public string scode { get; set; }

        IPAddress ip = IPAddress.Parse("192.168.12.169");
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public void Sign_Receive()
        {
            Thread Client_thread = new Thread(new ThreadStart(ReceiveSMessage));
            Client_thread.Start();
        }

        public void ReceiveSMessage()
        {
            
            //通过clientSocket接收数据
            while (true)
            {
                
                int receiveLength = clientSocket.Receive(result);
                
                string ss = Encoding.ASCII.GetString(result, 0, receiveLength);
               
                if (ss == ToBeginGame)
                {
                    SendNum();
                }

            }
            
        }

        public void connnectserver()
        {
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 8885)); //配置服务器IP与端口
                Console.WriteLine("连接服务器成功");
            }
            catch
            {
                Console.WriteLine("连接服务器失败，请按回车键退出！");
                return;
            }
        }

        public void Regester()
        {
            SendClientMessage("Regester"+"+"+sname + "+" + scode);
        }

        public void SignIn()
        {
            SendClientMessage(sname + "" + scode);
        }

        public void SendNum()
        {
            Random r = new Random();

            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iSeed = BitConverter.ToInt32(buffer, 0);
            r = new Random(iSeed);
            int x = r.Next(1000);
            Console.WriteLine("{0}", x);
            
            SendClientMessage(sname + "+" + x.ToString());
        }

        public void SendClientMessage(string smessage)
        {
                try
                {
                    clientSocket.Send(Encoding.ASCII.GetBytes(smessage));
                }
                catch
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
         

    }
}

