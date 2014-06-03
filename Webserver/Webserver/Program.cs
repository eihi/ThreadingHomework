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
        static void Main(string[] args)
        {
            //TODO: start logger in thread
            Logger logger = Logger.Instance();
            //Thread loggerThread = new Thread(logger.get);


            ControlServer controlserver = null;
           try
            {
               //TODO: start server in thread
               controlserver = new ControlServer();
               if(controlserver.Data.Controlport == null)
               {
                   controlserver.Data.Controlport = 8081;
               }
               controlserver.startServer(controlserver.Data.Controlport);
            }
            catch(Exception e)
            {
                Console.WriteLine("controlserver connection failed: " + e);
            }
            try
            {
                //TODO: start server in thread
                Server server = new Server();
                if (controlserver.Data.Webport != null)
                {
                    server.startServer(controlserver.Data.Webport);
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine("webserver connection failed: " + e);
            }
            Console.ReadLine();
        }
    }
}
