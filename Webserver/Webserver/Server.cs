using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Webserver
{
    public class Server
    {

        public Server()
        {
            
        }
        public void startServer(int port)
        {
            TcpListener serverSocket = new TcpListener(port);
            int requestCount = 0;
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            while (true)
            {
                Console.WriteLine("WebServer Started - Listening on port:" + port);
                Socket sock = serverSocket.AcceptSocket();

                RequestHandler handler = new RequestHandler(sock);

            }
            //clientSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine("WebServer: Accept connection from client");

            /* clientSocket.Close();
             serverSocket.Stop();
             Console.WriteLine(" >> exit");*/
            Console.ReadLine();
        }
    }
}
