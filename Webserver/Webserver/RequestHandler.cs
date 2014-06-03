using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Webserver
{
    public class RequestHandler 
    {
        private Socket socket;

        public RequestHandler(Socket sock)
        {
            this.socket = sock;
            Thread thread = new Thread(start);
            thread.Start();
        }
        public void start()
        {
            Console.WriteLine("Start method called");
        }
    }
}
