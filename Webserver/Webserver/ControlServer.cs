using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Webserver
{
    public class ControlServer 
    {
        private ControlData data;

        public ControlData Data
        {
            get { return data; }
            set { data = value; }
        }
        public ControlServer()
        {
            data = new ControlData();
            readXML();

        }
        public void startServer(int port)
        {
            TcpListener serverSocket = new TcpListener(port);
            TcpClient clientSocket = default(TcpClient);
            Console.WriteLine("ControlServer Started - Listening on port:" + port);
            serverSocket.Start();
            while(true)
            {
            
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine("ControlServer: Accept connection from client");
            }
            string html = "C:\\Users\\school\\Documents\\GitHub\\ThreadingHomework\\Webserver\\Webserver\\bin\\Debug\\index.html";


        }
        public void writeXML()
        {
            XmlSerializer writer = new XmlSerializer(typeof(ControlData));
            StreamWriter file = new StreamWriter(@"D:\school\"+CONST.CONTROLFORM);
            if (file != null)
            {
                writer.Serialize(file, data);
            }
            file.Close();
        }
        public void readXML()
        {
            XmlSerializer reader = new XmlSerializer(typeof(ControlData));
            StreamReader file = new StreamReader(@"D:\school\"+CONST.CONTROLFORM);
            data = (ControlData)reader.Deserialize(file);
        }

    }
}
