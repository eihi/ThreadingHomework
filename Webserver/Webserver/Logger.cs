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
        private static Logger _instance;
        private string[] logFile = new string[CONST.LOGLENGTH];
        private Semaphore getter;
        private Semaphore setter;
        private Semaphore synchronizer;

        protected Logger()
        {
            getter = new Semaphore(1,1);
            setter = new Semaphore(0,5);
            synchronizer = new Semaphore(1, 1);
        }
        public static Logger Instance()
        {
            if(_instance == null)
            {
                _instance = new Logger();
            }
            return _instance;
        }

        public void set(string line)
        {
            setter.WaitOne();
            synchronizer.WaitOne();
            //add line to logFile
            synchronizer.Release();
            getter.Release();
        }

        public void get()
        {
            getter.WaitOne();
            
            //get logFile

            setter.Release();
        }
    }
}
