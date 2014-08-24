using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Webserver
{
    public class Logger
    {
        private string[] logFile = new string[CONST.LOGLENGTH];
        private int positie = -1;
        private Semaphore getter;
        private Semaphore setter;
        private Semaphore synchronizer;

        public Logger()
        {
            getter = new Semaphore(0,20);
            setter = new Semaphore(20,20);
            synchronizer = new Semaphore(1, 1);            
        }

        public void logMessage(string line)
        {
            //in de que (put)
            setter.WaitOne();
            synchronizer.WaitOne();
            logFile[++positie] = line;
            synchronizer.Release();
            getter.Release();
            writeToFile();
        }

        public void writeToFile()
        {
            //uit de que (get)
            getter.WaitOne();
            if (positie != -1)
            {
                using (StreamWriter writer = File.AppendText(CONST.CONTROLSERVER_LOGFILE))
                {
                    writer.WriteLine(logFile[positie--]);
                    Console.WriteLine("write to file");
                }
            }
            setter.Release();
        }
    }
}
