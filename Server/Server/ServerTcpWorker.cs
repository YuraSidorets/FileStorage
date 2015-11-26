using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Server
{
    class ServerTcpWorker
    {
        /// <summary>
        /// Listen to incoming data
        /// </summary>
        public void Listen()
        {

            //IPAddress ipAddr = IPAddress.Parse("10.241.129.147");
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 5050);
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);
                while (true)
                {
                    Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);
                    Socket handler = sListener.Accept();
                    RecieveDict(handler);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            finally
            {
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Display recieved dictionary. Test method.
        /// </summary>
        private void TestDisplay(Dictionary<string, object> contentOfAddingFile)
        {
            foreach (KeyValuePair<string, object> kVP in contentOfAddingFile)
            {
                Console.WriteLine("{0} - {1}", kVP.Key, kVP.Value.ToString());
            }
        }

        /// <summary>
        /// Recieve Dictionary of (string, object) that contains all info about recieved file 
        /// </summary>
        /// <param name="reciever">Socket of transfer</param>
        private void RecieveDict(Socket reciever)
        {
            NetworkStream netStream = new NetworkStream(reciever);

            FileStream fs = new FileStream("$temp", FileMode.Create, FileAccess.Write);
            byte[] data = new byte[1024];
            int dataCitit;
            int totalBytes = 0;
            
            do
            {
                Thread.Sleep(10);
                dataCitit = netStream.Read(data, 0, data.Length);
                fs.Write(data, 0, dataCitit);
                totalBytes += dataCitit;
            }
            while (netStream.DataAvailable);
            fs.Close();
            
            Console.WriteLine("Получено байт: {0}", totalBytes);

           

            BinaryFormatter bf = new BinaryFormatter();
            Dictionary<string, object> contentOfAddingFile;
            try
            {
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes("$temp")))
                {
                    contentOfAddingFile = (Dictionary<string, object>)bf.Deserialize(ms);
                }
                if (contentOfAddingFile["Command"].ToString().Equals("Add"))
                    DBWorker.SetValue(contentOfAddingFile);

                else if (contentOfAddingFile["Command"].ToString().Equals("Download"))
                {
                    object fileToSend = DBWorker.GetFileToWrite((int.Parse((string)contentOfAddingFile["Id"])));

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, fileToSend);

                        netStream.Write(ms.ToArray(), 0, ms.ToArray().Length);
                    }
                }

                else if (contentOfAddingFile["Command"].ToString().Equals("Refresh"))
                {
                    object fileToSend = DBWorker.GetDataTable();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, fileToSend);

                        netStream.Write(ms.ToArray(), 0, ms.ToArray().Length);
                    }
                }
                else
                    throw new FormatException();
                netStream.Close();
                File.Delete("$temp");
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }
    }
}

