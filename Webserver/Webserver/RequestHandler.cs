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
        private ControlData controlData;
        private SendResponse sendResponse;

        public RequestHandler(Socket sock, ControlData controlData)
        {
            this.socket = sock;
            this.controlData = controlData;
            sendResponse = new SendResponse();
            start();
        }
        public void start()
        {
            if(socket.Connected)
            {
                Console.WriteLine("WebServer: Accept connection from client: " + socket.RemoteEndPoint);
                //receive data from client and put in byte array
                    Byte[] receivedBytes = new Byte[1024];
                    int i = socket.Receive(receivedBytes, receivedBytes.Length, 0);
                    string buffer = Encoding.ASCII.GetString(receivedBytes);
                    string requestType = buffer.Substring(0,3);
                    //handle GET
                    if(requestType != "GET")
                    {
                        Console.WriteLine("This is no GET request");
                        socket.Close();
                        return;
                    }

                    //get http request
                    int index = buffer.IndexOf("HTTP", 1);
                    string HttpVersion = buffer.Substring(index, 8);
                    string request = buffer.Substring(0, index - 1);
                    request.Replace("\\", "/");

                    //extract requested file name
                    index = request.LastIndexOf("/") + 1;
                    string requestedFile = request.Substring(index);
                    
                    //if requested file is nothing use default
                    if(requestedFile.Length == 0)
                    {
                        requestedFile = controlData.Webroot + "\\" + controlData.Defaultpage;
                    }
                    else
                    {
                        //set full path
                        requestedFile = controlData.Webroot + "\\" + requestedFile;
                    }

                    //TODO: if directory browsing == true : search for file in specified directory (internet tut for code)
                    //check if html file exists
                    if(File.Exists(requestedFile) == false)
                    {
                        string errorMessage = "<H2>404 Error! Page does not exists..</H2>";
                        sendResponse.SendHeader(HttpVersion, errorMessage.Length, " 404 Not Found", ref socket);
                        sendResponse.SendToBrowser(errorMessage, ref socket);
                        log(requestType, socket.RemoteEndPoint);
                    }
                    else 
                    {
                        int aantalBytes = 0;
                        string response = "";
                        //TODO: show images correctly

                        FileStream filestream = new FileStream(requestedFile, FileMode.Open, FileAccess.Read, FileShare.Read);

                        BinaryReader reader = new BinaryReader(filestream);
                        Byte[] sendBytes = new Byte[filestream.Length];
                        int n;
                        while((n = reader.Read(sendBytes,0,sendBytes.Length)) != 0)
                        {
                            response += Encoding.ASCII.GetString(sendBytes,0, n);
                            aantalBytes += n;
                        }
                        reader.Close();
                        filestream.Close();

                        sendResponse.SendHeader(HttpVersion, aantalBytes, " 200 OK ", ref socket);
                        sendResponse.SendToBrowser(sendBytes, ref socket);
                        log(requestType, socket.RemoteEndPoint);
                    }
                    socket.Close();
            }
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
