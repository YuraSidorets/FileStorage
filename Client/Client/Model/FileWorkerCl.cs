using System;
using System.Collections.Generic;
using System.IO;

namespace Client                                                                                                                     
{
    /// <summary>
     /// Gets information about file
     /// </summary>
    internal static class FileWorkerCl
    {
        public static Dictionary<string, object> GetFileInfo(string path)
        {
            Dictionary<string, object> fileInformation = new Dictionary<string, object>();
            string fileName = Path.GetFileNameWithoutExtension(path);
            fileInformation.Add("Name", fileName);

            string fileType = Path.GetExtension(path);
            fileInformation.Add("Type", fileType);

            DateTime fileCreationDate = File.GetCreationTime(path);

            fileInformation.Add("Date", fileCreationDate);

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
