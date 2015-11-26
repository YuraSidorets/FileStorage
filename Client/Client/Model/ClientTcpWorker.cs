using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Data;

namespace Client
{
    internal class ClientTcpWorker
    { 
        TcpClient client { get; set; }

        public ClientTcpWorker(int port, string ipAddr)
        {
            this.client = new TcpClient(ipAddr, port);
        }

        private Dictionary<string, object> CreateDictToSendId(string id)
        {
            Dictionary<string, object> sendingContent = new Dictionary<string, object>();
            sendingContent.Add("Command", "Download");
            sendingContent.Add("Id", id);
            return sendingContent;
        }

        public void SendIdDict(string id)
        {
            NetworkStream netStream = client.GetStream();
            BinaryFormatter bf = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, CreateDictToSendId(id));
                byte[] data = ms.ToArray();
                netStream.Write(data, 0, data.Length);
                RecieveFile(netStream);
            }
        }   

        public void RecieveFile(NetworkStream netStream)
        { 
            byte[] data = new byte[1024];
            int dataCitit;
            int totalBytes = 0;

            FileStream fs = new FileStream("$temp", FileMode.Create, FileAccess.Write);

            do
            {
                dataCitit = netStream.Read(data, 0, data.Length);
                fs.Write(data, 0, dataCitit);
                totalBytes += dataCitit;

            } while (netStream.DataAvailable);

            fs.Close();
            netStream.Close();

            BinaryFormatter binFormatter = new BinaryFormatter();
            DataTable contentOfFile;
            byte[] outData;
         
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes("$temp")))
            {
                contentOfFile = (DataTable) binFormatter.Deserialize(ms);
                outData = (byte[]) contentOfFile.Rows[0].ItemArray.GetValue(2);
            }

            File.WriteAllBytes(contentOfFile.Rows[0].ItemArray.GetValue(0).ToString()+contentOfFile.Rows[0].ItemArray.GetValue(1), outData);
            File.Delete("$temp");
        }

        private Dictionary<string, object> CreateDictToSendFile(string path)
        {
            Dictionary<string, object> fileInformation = new Dictionary<string, object>();
            fileInformation.Add("Command", "Add");
            fileInformation.Add("Name", FileWorkerCl.GetFileInfo(path)["Name"]);
            fileInformation.Add("Type", FileWorkerCl.GetFileInfo(path)["Type"]);
            fileInformation.Add("Date", FileWorkerCl.GetFileInfo(path)["Date"]);
            fileInformation.Add("Size", FileWorkerCl.GetFileInfo(path)["Size"]);
            fileInformation.Add("Data", FileWorkerCl.GetBytes(path));
            fileInformation.Add("Description", string.Empty); //!!!
            return fileInformation;
        }

        public void SendFileDict(string path)
        {
            NetworkStream netStream = client.GetStream();
            BinaryFormatter bf = new BinaryFormatter();
                                               
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, CreateDictToSendFile(path));
                
                MessageBox.Show(string.Format("Размер отправляемых данных: {0}", ms.ToArray().Length));  //Убрать

                byte[] data = ms.ToArray();
                netStream.Write(data, 0, data.Length);
                netStream.Flush();
                netStream.Close();
            }

        }
    }
}

