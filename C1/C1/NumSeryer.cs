using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace C1
{


    class ClientData
    {
        public string Mname { get; set; }
        public string Mcode { get; set; }
        public double Mnum { get; set; }
        public bool IsSendNum { get; set; }
        public Socket client { get; set; }
    }

    class NumSeryer
    {

        public static DateTime StartTime =new DateTime();
        public static DateTime EndTime = new DateTime();

        public static List<ClientData> ClientList = new List<ClientData>();


        public static int Nround=40;

        private static byte[] result = new byte[1024];
        private static int myProt = 8885;   //端口
        static Socket serverSocket;
        public static int nsum;
        static void Main(string[] args)
        {
            //服务器IP地址
            IPAddress ip = IPAddress.Parse("192.168.12.169");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口
            serverSocket.Listen(50);    
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
            //通过Clientsoket发送数据

            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
            Console.ReadLine();
        }

        
        /// 监听客户端连接
        
        /// 



        public static void sendstart()
        {

            for (int i = 0; i < Nround; i++)
            {
                sendmessage();
                Thread.Sleep(1200);

                double sum = 0, ave, jiange = 1111111111;
                int n = 0, pos = 0;

                foreach (ClientData xx in ClientList)
                {
                    if (xx.IsSendNum)
                    {
                        sum += xx.Mnum;
                        n++;
                    }
                }


                ave = sum / n * 0.618;

                for (int j = 0; j < ClientList.Count; j++)
                {
                    ClientData xx = ClientList[j];
                    if (xx.IsSendNum && jiange > Math.Abs(xx.Mnum - ave))
                    {
                        jiange = Math.Abs(xx.Mnum - ave); pos = j;
                    }
                }

                Console.WriteLine("第{0}轮结果", i);
                Console.WriteLine("参与客户端数：{0}，G-number：{1}", n, ave);
                Console.WriteLine("获胜客户端名字:{0},数值：{1}", ClientList[pos].Mname, ClientList[pos].Mnum);
                Console.WriteLine();
            }
        }

        public static void sendmessage()
        {
            foreach(ClientData xx in ClientList)
            xx.client.Send(Encoding.ASCII.GetBytes("Start"));
        }



        private static void ListenClientConnect()
        {
            while (true)
            {
                nsum++;


                StartTime = DateTime.Now;

                Socket clientSocket=serverSocket.Accept();

                ClientData _client = new ClientData { client =clientSocket  };

                ClientList.Add(_client);

                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);

                if (nsum >= 20)
                {
                    sendstart();
                    //        return;
                }


            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="clientSocket"></param>
        private static void ReceiveMessage(object clientSocket)
         {
            Socket myClientSocket = (Socket)clientSocket;


            while (true)
            {
                try
                {
                    //通过clientSocket接收数据
                    int receiveNumber = myClientSocket.Receive(result);

                    string ss = Encoding.ASCII.GetString(result, 0, receiveNumber);

                    string[] s = ss.Trim().Split('+');


                    double tnum = 0;

                    if (s[0] == "Regester")
                    {
                        string[] stemp = s[1].Trim().Split('-');
                        int index = int.Parse(stemp[1]);
                        ClientList[index].Mname = s[1];
                        ClientList[index].Mcode = s[2];
                        continue;
                    }



                    string tname = s[0];
                    tnum = int.Parse(s[1]);
                    int n = ClientList.FindIndex(x => x.Mname == s[0]);


                    ClientList[n].Mnum = tnum;
                    ClientList[n].IsSendNum = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }
    }
}

