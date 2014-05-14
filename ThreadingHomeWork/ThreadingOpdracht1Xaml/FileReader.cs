using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadingOpdracht1
{
    class FileReader
    {
        public int[] top10;
        public List<String> files;

        public FileReader()
        {
            file("C:/Users/school/Documents/GitHub/ThreadingHomework/Bestanden Opdr1/Bestanden.txt");
            for(int i = 0; i < files.Count; i++)
            {
                Counter readfile = new Counter();

                Thread thread = new Thread(new ThreadStart(new Counter(readfile)));
                thread.Start();

                while (!thread.IsAlive) ;

                Thread.Sleep(1);

                thread.Abort();

                thread.Join();

                Console.WriteLine("Thread has finished");
            }
 
        }
        public void file(string fileName)
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        files.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Er ging iets mis.");
                Console.WriteLine(e.Message);
            }
        }
        public void showTop10()
        {

        }
        public void addResult(int newResult)
        {

        }
        public void clearTop10()
        {

        }

    }
}
