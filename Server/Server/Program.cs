using System;


namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerTcpWorker tcpW = new ServerTcpWorker();
            tcpW.Listen();
            
            Console.ReadKey();
        }
    }
}
