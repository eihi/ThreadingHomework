using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Webserver
{
    public class WebServer
    {
        ControlSettings controlData;
        private Logger logger;
        public WebServer(ControlSettings controlData)
        {
            this.controlData = controlData;
            logger = new Logger();
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

                RequestHandler handler = new RequestHandler(listener, controlData, logger);

            }
            catch(Exception e)
            {
                Console.WriteLine("Exception occured on webserver");
            }
        }
    }
}
