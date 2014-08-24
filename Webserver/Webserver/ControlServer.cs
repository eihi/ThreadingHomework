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
        private ControlSettings settings;
        private SendResponse sendResponse;

        public ControlSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }
        public ControlServer()
        {
            //thread starten en startserver aanroepen
            settings = new ControlSettings().Load();
            sendResponse = new SendResponse();
            Thread controlThread = new Thread(startServer);
            controlThread.Start();

        }
        public void startServer() 
        {
            TcpListener listener = new TcpListener(settings.Controlport);
            listener.Start();
            Console.WriteLine("ControlServer Started - Listening on port:" + settings.Controlport);

            ControlRequestHandler handler = new ControlRequestHandler(listener, settings);
        }
    }
}
