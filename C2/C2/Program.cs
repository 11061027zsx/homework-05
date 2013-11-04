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


    class saaaa
    {
        public static void Main()
        {


            client[] a =new client[20];
            for (int i = 0; i < 10; i++)
            {
                a[i] = new client { sname = "c-" + i.ToString(), scode = "123" };
                a[i].connnectserver();
                a[i].Sign_Receive();
                a[i].Regester();
            }

            for (int i = 10; i < 20; i++)
            {
                Thread.Sleep(1000);
                a[i] = new client { sname = "c-" + i.ToString(), scode = "123" };
                a[i].connnectserver();
                a[i].Sign_Receive();
                a[i].Regester();
            }
            
            Console.ReadLine();
        }
    }
    }


