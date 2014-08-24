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
        private int numberBytes;
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
                
                //get http request
                int index = buffer.IndexOf("HTTP", 1);
                string HttpVersion = buffer.Substring(index, 8);
                string request = buffer.Substring(0, index - 1);
                request.Replace("\\", "/");


                //extract requested file name
                index = request.LastIndexOf("/") + 1;
                string requestedFile = "/" + request.Substring(index);

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
                        numberBytes = 0;
                        string response = processFile(CONST.ERROR_400);
                        sendResponse.SendHeader("HTTP/1.1", getMIMEtype(CONST.ERROR_400), numberBytes, " 400 Bad Request", ref socket);
                        sendResponse.SendToBrowser(response, ref socket);
                        log(requestType, requestedFile, socket.RemoteEndPoint);
                        break;
                }


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
                    numberBytes = 0;
                    string response = processFile(CONST.ERROR_404);
                    sendResponse.SendHeader(HttpVersion, getMIMEtype(CONST.ERROR_404), numberBytes, " 404 Not Found", ref socket);
                    sendResponse.SendToBrowser(response, ref socket);
                    log(requestType, requestedFile, socket.RemoteEndPoint);
                }
                else
                {
                    numberBytes = 0;
                    string response = processFile(requestedFile);
                    sendResponse.SendHeader(HttpVersion, getMIMEtype(requestedFile), numberBytes, " 200 OK ", ref socket);
                    sendResponse.SendToBrowser(response, ref socket);
                    log(requestType, requestedFile, socket.RemoteEndPoint);
                }
                socket.Close();
            }
            limit.Release();
        }
        private string processFile(string requestedFile)
        {
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
            return response;
        }
        private string getMIMEtype(string requestedFile)
        {
            requestedFile = requestedFile.ToLower();
            int startpos = requestedFile.IndexOf(".");

            if (startpos > 0)
            {
                string fileExt = requestedFile.Substring(startpos);

                switch (fileExt)
                {
                    case ".html":
                        return "text/html";
                        break;
                    case ".htm":
                        return "text/html";
                        break;
                    case ".png":
                        return "image/png";
                        break;
                    case ".jpg":
                        return "image/jpg";
                        break;
                }
            }
            return "text/html";
        }

        private byte[] ReceiveClientData()
        {
            Console.WriteLine("WebServer: Accept connection from client: " + socket.RemoteEndPoint);
            Byte[] receivedBytes = new Byte[1024];
            int i = socket.Receive(receivedBytes, receivedBytes.Length, 0);
            return receivedBytes;
        }
        public void log(string requestType, string url, EndPoint ip)
        {
            Logger logger = new Logger();
            string line = "RequestType: " + requestType + "\r\n";
            line += "URL: " + url + "\r\n";
            line += "Ip adres: " + ip + "\r\n";
            line += "Datetime: " + DateTime.Now.ToString() + "\r\n";
            logger.logMessage(line);
        }
    }
}
