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
        public void SendSSLResponse(SslStream sslStream, string html)
        {
            if (sslStream != null)
            {
                byte[] sendBytes = new byte[2048];
                sendBytes = Encoding.ASCII.GetBytes(html);

                //FileStream filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);

                //BinaryReader reader = new BinaryReader(filestream);
                //Byte[] sendBytes = new Byte[filestream.Length];
                //int n;
                //while ((n = reader.Read(sendBytes, 0, sendBytes.Length)) != 0)
                //{
                //    response += Encoding.ASCII.GetString(sendBytes, 0, n);
                //}
                //reader.Close();
                //filestream.Close();

                sslStream.Write(sendBytes);
                sslStream.Flush();
            }
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
        public void SendToBrowser(String message, ref Socket mySocket)
        {
            SendToBrowser(Encoding.ASCII.GetBytes(message), ref mySocket);
        }
        public void SendToBrowser(Byte[] bytemessage, ref Socket mySocket)
        {
            int numBytes = 0;
            try
            {
                if (mySocket.Connected)
                {
                    if ((numBytes = mySocket.Send(bytemessage, bytemessage.Length, 0)) == -1)
                        Console.WriteLine("Socket error cannot send packet");
                    else
                    {
                        Console.WriteLine("No. of bytes send " + numBytes);
                    }
                }
                else
                {
                    Console.WriteLine("Socket not connected anymore");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error sending to browser: " + e);
            }
        }
    }
}
