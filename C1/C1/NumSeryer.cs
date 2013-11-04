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
        public int Mnum { get; set; }
        public bool IsSendNum { get; set; }
        public Socket client { get; set; }
        public int Mpoint { get; set; }
        public int MNJoinGame { get; set; }
        public int MNWin { get; set; }
        public int MNlose { get; set; }
    }

    class NumSeryer
    {

        public static DateTime StartTime =new DateTime();
        public static DateTime EndTime = new DateTime();

        public static List<ClientData> ClientList = new List<ClientData>();


        public static int Nround=40;
        public static int NGetPoint = 10;
        public static int NotInTime = 5;
        public static int NLosePoint = 1;

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
            Thread.Sleep(1000);
            Console.WriteLine("等待客户端连接....");
            //通过Clientsoket发送数据

            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
            Console.ReadLine();
        }

        
        /// 监听客户端连接
        
        /// 




        public static void sendstart()
        {
            Console.WriteLine();
            Console.WriteLine("所有客户端均已经连接到服务器，游戏将在3秒后开始");
            for (int i = 3; i >= 1; i--)
            {
                Thread.Sleep(1 * 1000);
                Console.WriteLine("游戏将在{0}秒后开始", i);
            }

            Thread.Sleep(1 * 1000);
            Console.WriteLine("游戏开始");
            Console.WriteLine();
            for (int i = 0; i < Nround; i++)
            {
                foreach (ClientData xx in ClientList)
                {
                    xx.IsSendNum = false;
                }
                sendmessage();

                Thread.Sleep(1000);

                double sum = 0, ave, min, max;
                int n = 0;

                double[] dis = new double[100];

                foreach (ClientData xx in ClientList)
                {
                    if (xx.IsSendNum)
                    {
                        sum += xx.Mnum;
                        xx.MNJoinGame++;
                        n++;
                    }
                }


                ave = sum / n * 0.618;

                for (int j = 0; j < ClientList.Count; j++)
                {
                    ClientData xx = ClientList[j];
                    dis[j] = Math.Abs(xx.Mnum - ave);
                }

                max = min = dis[0];

                for (int j = 1; j < ClientList.Count; j++)
                {
                    if (max < dis[j]) max = dis[j];
                    if (min > dis[j]) min = dis[j];
                }

                List<int> ThisRoundResultVE = new List<int>();
                List<int> ThisRoundResultLOSE = new List<int>();
                ThisRoundResultVE.Clear();
                ThisRoundResultLOSE.Clear();
              
                for (int j = 0; j < ClientList.Count; j++)
                {
                    if (dis[j] == max)
                    {
                        ThisRoundResultLOSE.Add(j);
                        ClientList[j].Mpoint -= NLosePoint;
                        ClientList[j].MNlose++;
                    }
                    if (dis[j] == min)
                    {
                        ThisRoundResultVE.Add(j);
                        ClientList[j].Mpoint += NGetPoint;
                        ClientList[j].MNWin++;
                    }
                }
                foreach(ClientData xx in ClientList)
                {
                    if(!xx.IsSendNum) xx.Mpoint-=NotInTime;
                }

                Console.WriteLine("第{0}轮结果", i);
                Console.WriteLine("参与客户端数：{0}，G-number：{1}", n, ave);
                Console.WriteLine("最优预估" + (char)9 + "数值");
                for (int j = 0; j < ThisRoundResultVE.Count; j++)
                    Console.WriteLine("{0}" + (char)9 + (char)9 + "{1}", ClientList[ThisRoundResultVE[j]].Mname, ClientList[ThisRoundResultVE[j]].Mnum);

                Console.WriteLine("最差预估" + (char)9 + "数值");
                for (int j = 0; j < ThisRoundResultLOSE.Count; j++)
                    Console.WriteLine("{0}" + (char)9 + (char)9 + "{1}", ClientList[ThisRoundResultLOSE[j]].Mname, ClientList[ThisRoundResultLOSE[j]].Mnum);
                Console.WriteLine();


            }

            Thread.Sleep(1000);
            Console.WriteLine("游戏结束，下面给出游戏结果");
            Thread.Sleep(1000);
            Console.WriteLine("名次" + (char)9 + "名字" + (char)9 + "总分" + (char)9 + "参与次数" + (char)9 + "获胜次数" + (char)9 + "失败次数");

            ClientList = ClientList.OrderByDescending(x => x.Mpoint).ToList<ClientData>();

            int nkey=0,nt=1,f=-5*100;

            for (int i = 0; i < ClientList.Count; i++)
            {
                if (f != ClientList[i].Mpoint)
                {
                    nkey += nt;
                    nt = 1;
                    f = ClientList[i].Mpoint;
                }
                else nt++;
                Console.WriteLine("{0}" + (char)9 + "{1}" + (char)9 + "{2}" + (char)9 + "{3}" + (char)9 + (char)9  +"{4}" + (char)9 + (char)9 + "{5}", nkey, ClientList[i].Mname, ClientList[i].Mpoint, ClientList[i].MNJoinGame, ClientList[i].MNWin, ClientList[i].MNlose);
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


                    int tnum = 0;

                    if (s[0] == "Regester")
                    {
                        string[] stemp = s[1].Trim().Split('-');
                        int index = int.Parse(stemp[1]);
                        ClientList[index].Mname = s[1];
                        ClientList[index].Mcode = s[2];
                        Console.WriteLine("客户端{0}已连入", s[1]);
                        continue;
                    }


                    string tname = s[0];
                    try
                    {
                        tnum = int.Parse(s[1]);

                    }
                    catch
                    {
                        continue;
                    } 

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

