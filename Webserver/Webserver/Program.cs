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
                    WebServer server = new WebServer(controlserver.Settings);
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
