using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Webserver
{
    public class Logger
    {
        private string[] logFile = new string[CONST.LOGLENGTH];
        private Semaphore getter;
        private Semaphore setter;
        private Semaphore synchronizer;

        public Logger()
        {
            getter = new Semaphore(1,1);
            setter = new Semaphore(0,5);
            synchronizer = new Semaphore(1, 1);
            Thread loggerThread = new Thread(writeToFile);
        }

        public void logMessage(string line)
        {
            setter.WaitOne();
            synchronizer.WaitOne();
            //add line to logFile
            synchronizer.Release();
            getter.Release();
        }

        public void writeToFile()
        {
            getter.WaitOne();
            
            //write line to a file

            setter.Release();
        }
    }
}
