using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Webserver
{
    public class SendResponse
    {
        public SendResponse()
        {

        }
        public void SendSSLResponse(SslStream sslStream, string filepath)
        {
            string response = "";

            FileStream filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);

            BinaryReader reader = new BinaryReader(filestream);
            Byte[] sendBytes = new Byte[filestream.Length];
            int n;
            while ((n = reader.Read(sendBytes, 0, sendBytes.Length)) != 0)
            {
                response += Encoding.ASCII.GetString(sendBytes, 0, n);
            }
            reader.Close();
            filestream.Close();

            sslStream.Write(sendBytes);
            sslStream.Flush();
        }
        public void SendHeader(string sHttpVersion, int iTotBytes, string sStatusCode, ref Socket mySocket)
        {

            String sBuffer = "";

            string sMIMEheader = "text/html";

            sBuffer = sBuffer + sHttpVersion + sStatusCode + "\r\n";
            sBuffer = sBuffer + "Server: cx1193719-b\r\n";
            sBuffer = sBuffer + "Content-Type: " + sMIMEheader + "\r\n";
            sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
            sBuffer = sBuffer + "Content-Length: " + iTotBytes + "\r\n\r\n";

            Byte[] sendData = Encoding.ASCII.GetBytes(sBuffer);

            SendToBrowser(sendData, ref mySocket);

            Console.WriteLine("Total Bytes : " + iTotBytes.ToString());

        }
        public void SendToBrowser(String sData, ref Socket mySocket)
        {
            SendToBrowser(Encoding.ASCII.GetBytes(sData), ref mySocket);
        }
        public void SendToBrowser(Byte[] bSendData, ref Socket mySocket)
        {
            int numBytes = 0;
            try
            {
                if (mySocket.Connected)
                {
                    if ((numBytes = mySocket.Send(bSendData,
                          bSendData.Length, 0)) == -1)
                        Console.WriteLine("Socket Error cannot Send Packet");
                    else
                    {
                        Console.WriteLine("No. of bytes send {0}", numBytes);
                    }
                }
                else
                    Console.WriteLine("Connection Dropped....");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Occurred : {0} ", e);
            }
        }
    }
}
