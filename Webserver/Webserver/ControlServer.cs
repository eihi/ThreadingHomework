using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Webserver
{
    public class ControlServer 
    {
        public ControlSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }
        private ControlSettings settings;

        public ControlServer()
        {
            // Load settings
            settings = new ControlSettings().Load();

            // Start server on a new thread
            Thread controlThread = new Thread(startServer);
            controlThread.Start();

        }
        public void startServer() 
        {
            // Start listening
            TcpListener listener = new TcpListener(settings.Controlport);
            listener.Start();
            Console.WriteLine("ControlServer Started - Listening on port:" + settings.Controlport);

            // Handle incoming requests
            ControlRequestHandler handler = new ControlRequestHandler(listener, settings);
        }
    }
}
