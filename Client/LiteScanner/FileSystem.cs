using System;
using System.Collections.Generic;
using System.IO;

namespace LiteScanner
{

    [Serializable]
    public class FileSystemException : Exception
    {
        public FileSystemException()
        { }

        public FileSystemException(string message)
            : base(message)
        { }

        public FileSystemException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }


    public class FileSystem
    {

        public static List<string> ReadFromFile(string filename)
        {

            List<string> list_ready = new List<string>();

            string[] filelines = ReadFile(filename);

            foreach (string line in filelines) { list_ready.Add(line); }
            return list_ready;
        }


        public static List<string> ReadStatisticsFromFile(string filename, bool remove_one = false)
        {
            List<string> list_ready = new List<string>();

            string[] filelines = ReadFile(filename);

            foreach (string line in filelines)
            {
                string[] two_parts = line.Split(':');

                if (two_parts.Length < 2)
                {
                    throw new FileSystemException($"File {filename} is in the wrong format");
                }

                int repeats = Int32.Parse(two_parts[1]);
                if (remove_one & repeats > 1) repeats--;

                for (int i = 0; i < repeats; i++)
                {
                    list_ready.Add(two_parts[0]);
                }

            }

            return list_ready;
        }

        static string[] ReadFile(string filename)
        {
            string[] filelines;

            try
            {
                filelines = File.ReadAllLines(filename);
            }
            catch (Exception e)
            {
                throw new FileSystemException($"Cant't find {filename}", e);
            }

            return filelines;
        }



    }
}
