using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Webserver
{
    public class Server
    {
        ControlData controlData;
        public Server(ControlData controlData)
        {
            this.controlData = controlData;
            Thread webThread = new Thread(startServer);
            webThread.Start();
        }
        public void startServer()
        {
            try
            {
                TcpListener listener = new TcpListener(controlData.Webport);
                listener.Start();
                Console.WriteLine("WebServer Started - Listening on port:" + controlData.Webport);

                RequestHandler handler = new RequestHandler(listener, controlData);

            }
            catch(Exception e)
            {
                Console.WriteLine("Exception occured on webserver");
            }
        }
    }
}
