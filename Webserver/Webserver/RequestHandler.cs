using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Webserver
{
    public class RequestHandler 
    {
        private Socket socket;
        private ControlSettings controlData;
        private SendResponse sendResponse;
        public static Semaphore limit { get; set; }

        public RequestHandler(TcpListener listener, ControlSettings controlData)
        {
            this.controlData = controlData;
            sendResponse = new SendResponse();
            limit = new Semaphore(1, 20);

            while (true)
            {
                this.socket = listener.AcceptSocket();
                Thread request = new Thread(start);
                request.Start();
            }
        }
        public void start()
        {
            limit.WaitOne();
            if (socket.Connected)
            {
                // Receive data from client and put in byte array
                Byte[] receivedBytes = ReceiveClientData();

                //byte to string
                string buffer = Encoding.ASCII.GetString(receivedBytes);
                Console.WriteLine(buffer);
                string requestType = buffer.Substring(0, 3);
                //handle GET
                int typenumber = 3;
                switch (requestType)
                {
                    case "GET":
                        typenumber = 3;
                        break;
                    case "POST":
                        typenumber = 4;
                        break;
                    default:
                        Console.WriteLine("Request Type: '"+ requestType + "' not allowed");
                        string errorMessage = "<H2>400 Error! This is no legit request</H2>";
                        sendResponse.SendHeader("HTTP/1.1", errorMessage.Length, " 400 Bad Request", ref socket);
                        sendResponse.SendToBrowser(errorMessage, ref socket);
                        log(requestType, socket.RemoteEndPoint);
                        break;
                }

                //get http request
                int index = buffer.IndexOf("HTTP", 1);
                string HttpVersion = buffer.Substring(index, 8);
                string request = buffer.Substring(0, index - 1);
                request.Replace("\\", "/");


                //extract requested file name
                index = request.LastIndexOf("/") + 1;
                string requestedFile = "/" + request.Substring(index);

                //directory browsing true
                if (controlData.Directorybrowsing == true)
                {
                    string directory = request.Substring(request.IndexOf("/"), request.LastIndexOf("/") - typenumber);
                    requestedFile = directory + requestedFile;
                }


                //if requested file is nothing use default
                if (requestedFile.Length <= 2 || requestedFile == "//favicon.ico")
                {
                    requestedFile = controlData.Webroot + "\\" + controlData.Defaultpage;
                }
                else
                {
                    //set full path
                    requestedFile = controlData.Webroot + requestedFile;
                    Console.WriteLine("requested file: " + requestedFile);
                }


                if (File.Exists(requestedFile) == false)
                {
                    string errorMessage = "<H2>404 Error! Page does not exists </H2>";
                    sendResponse.SendHeader(HttpVersion, errorMessage.Length, "404 Not Found", ref socket);
                    sendResponse.SendToBrowser(errorMessage, ref socket);
                    log(requestType, socket.RemoteEndPoint);
                }
                else
                {
                    int numberBytes = 0;
                    string response = "";
                    //TODO: show images correctly

                    FileStream filestream = new FileStream(requestedFile, FileMode.Open, FileAccess.Read, FileShare.Read);

                    BinaryReader reader = new BinaryReader(filestream);
                    Byte[] sendBytes = new Byte[filestream.Length];
                    int n;
                    while ((n = reader.Read(sendBytes, 0, sendBytes.Length)) != 0)
                    {
                        response += Encoding.ASCII.GetString(sendBytes, 0, n);
                        numberBytes += n;
                    }
                    reader.Close();
                    filestream.Close();

                    sendResponse.SendHeader(HttpVersion, numberBytes, " 200 OK ", ref socket);
                    sendResponse.SendToBrowser(response, ref socket);
                    //log(requestType, socket.RemoteEndPoint);
                }
                socket.Close();
            }
            limit.Release();
        }

        private byte[] ReceiveClientData()
        {
            Console.WriteLine("WebServer: Accept connection from client: " + socket.RemoteEndPoint);
            Byte[] receivedBytes = new Byte[1024];
            int i = socket.Receive(receivedBytes, receivedBytes.Length, 0);
            return receivedBytes;
        }
        public void log(string requestType, EndPoint ip)
        {
            Logger logger = new Logger();
            string line = "RequestType: " + requestType + "\r\n";
            line += "Ip adres: " + ip + "\r\n";
            line += "Datetime: " + DateTime.Now.ToString() + "\r\n";
            logger.logMessage(line);
        }
    }
}
