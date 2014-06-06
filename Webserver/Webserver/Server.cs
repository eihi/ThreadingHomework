﻿using System;
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
            TcpListener serverSocket = new TcpListener(controlData.Webport);
            Console.WriteLine("WebServer Started - Listening on port:" + controlData.Webport);
            serverSocket.Start();

            while (true)
            {
                Socket clientSocket = serverSocket.AcceptSocket();
                RequestHandler handler = new RequestHandler(clientSocket, controlData);
                
            }
        }
    }
}