using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Test
{
    internal class FileWorkerDB
    {
        //string, object
        public static Dictionary<string, string> GetFileInfo(string path)
        {
            Dictionary<string, string> fileInformation = new Dictionary<string, string>();
            string fileName = Path.GetFileNameWithoutExtension(path);
            fileInformation.Add("Name", fileName);

            string fileType = Path.GetExtension(path);
            fileInformation.Add("Type", fileType);

            DateTime fileCreationDate = File.GetCreationTime(path);
            fileInformation.Add("Date", fileCreationDate.ToString());

            FileInfo fi = new FileInfo(path);
            double fileSize = fi.Length;
            fileInformation.Add("Size", fileSize.ToString());

            return fileInformation;
        }

        public static byte[] GetBytes(string path)
        {
            byte[] output = null;
            try
            {
                output = File.ReadAllBytes(path);
            }
            catch (IOException e)
            {
                ///!!!!!!!!!!
                Console.WriteLine("this file does not exist\n method will return null\n", e);
            }
            return output;
        }


    }
}
