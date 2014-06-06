using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Webserver
{
    class Program
    {
        private ControlServer controlserver;
        static void Main(string[] args)
        {

            Logger logger = new Logger();
            ControlServer controlserver = null;
            try
            {
               controlserver = new ControlServer();
            }
            catch (Exception e)
            {
                Console.WriteLine("controlserver connection failed: " + e.Message);
            }
            try
            {
                if (controlserver != null)
                {
                    Server server = new Server(controlserver.Data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("webserver connection failed: " + e.Message);
            }
            
            Console.ReadLine();
        }
    }
}
