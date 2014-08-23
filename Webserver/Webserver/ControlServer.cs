using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Webserver
{
    public class ControlServer 
    {
        private ControlData data;
        private SendResponse sendResponse;
        //private int port;


        public ControlData Data
        {
            get { return data; }
            set { data = value; }
        }
        public ControlServer()
        {
            //thread starten en startserver aanroepen
            data = new ControlData();
            sendResponse = new SendResponse();
            readXML();
            Thread controlThread = new Thread(startServer);
            controlThread.Start();

        }
        public SslStream createStream(TcpClient client)
        {
            X509Certificate serverCertificate = new X509Certificate2(CONST.SSL_CERTIFICATE, CONST.SSL_PASSWORD);
            var sslStream = new SslStream(client.GetStream(), false);
            sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls, true);
            return sslStream;
        }
        public void startServer()
        {
            if (data.Controlport.ToString().Length == 0)
            {
                data.Controlport = 8081;
            }
            TcpListener serverSocket = new TcpListener(data.Controlport);
            serverSocket.Start();
            Console.WriteLine("ControlServer Started - Listening on port:" + data.Controlport);

            Byte[] bytes = new Byte[256];
            string streamData = null;

            while(true)
            {
                Console.WriteLine("waiting for client to connect");
                TcpClient client = serverSocket.AcceptTcpClient();
                if (client.Connected)
                {
                    
                    Console.WriteLine("ControlServer: Accept connection from client: " + client.Client.RemoteEndPoint);
                    //TODO: authenticatie 
                    SslStream sslStream = createStream(client);
                    //int rechten = AuthenticateUser("Stefanie@hotmail.com", "wachtwoord");
                    //send login form TODO: not showing in browser??
                    
                   
                    //sendResponse.SendSSLResponse(sslStream, CONST.CONTROLSERVER_CONTROLPANEL1);

                    byte[] buffer = new byte[2048];
                    StringBuilder messageData = new StringBuilder();
                    int bytess = -1;
                    do
                    {
                        bytess = sslStream.Read(buffer, 0, buffer.Length);

                        // Use Decoder class to convert from bytes to UTF8 
                        // in case a character spans two buffers.
                        Decoder decoder = Encoding.UTF8.GetDecoder();
                        char[] chars = new char[decoder.GetCharCount(buffer, 0, bytess)];
                        decoder.GetChars(buffer, 0, bytess, chars, 0);
                        messageData.Append(chars);
                        // Check for EOF. 
                        if (messageData.ToString().IndexOf("<EOF>") != -1)
                        {
                            break;
                        }
                    } while (bytess != 0);

                    Console.WriteLine( messageData.ToString());
                   
                    //TODO: receive post and handle this
                    //TODO: xss save maken?
                    //TODO: if port changes directly change this in servers

                    /*
                    string username = "";
                    string password = "";
                    /*int rechten = AuthenticateUser(username, password);

                    //TODO: toon pagina aan de hand van rechten
                    if(rechten == 0)
                    {
                        //geen rechten tot de pagina
                    }
                    else if(rechten == CONST.SECURITY_BEHEERDER)
                    {
                        //beheerder rechten
                        sendResponse.SendSSLResponse(sslStream, CONST.CONTROLFORM);
                        //TODO: set disabled on false
                        //TODO: fill html with controlData
                    }
                    else if(rechten == CONST.SECURITY_ONDERSTEUN)
                    {
                        sendResponse.SendSSLResponse(sslStream, "..\\Debug\\controlForm.html");
                        //TODO: fill html with controlData
                    }*/
                }
            }
        }

        public int AuthenticateUser(string username, string password)
        {
            string connectionstring = "Server=" + CONST.DBCONN_SERVER + ";Database=" + CONST.DBCONN_DATABASE + ";Uid=" + CONST.DBCONN_USERID + ";Pwd=" + CONST.DBCONN_PASSWORD + ";";
            SqlConnection connection = new SqlConnection(connectionstring);
            try
            {
                connection.Open();
            }
            catch(Exception e)
            {
                Console.WriteLine("Database connection failed:" + e.ToString());
            }
            //TODO: sql injection 
            string getSaltQuery = "SELECT salt FROM users WHERE username ="+ username;

            SqlDataReader reader = null;
            SqlCommand saltCommand = new SqlCommand(getSaltQuery, connection);
            reader = saltCommand.ExecuteReader();
            string salt = null;
            while(reader.Read())
            {
                salt = reader["salt"].ToString();
                Console.WriteLine(salt);
            }
            //TODO: encrypt password with salt
            //TODO: personal cookie to identify user in browser
            string getUserQuery = "SELECT security_level FROM users WHERE username =" + username + "and password =" + password;
            SqlCommand userCommand = new SqlCommand(getSaltQuery, connection);
            reader = userCommand.ExecuteReader();
            if(reader.HasRows)
            {
                while (reader.Read())
                {
                    return int.Parse(reader["security_level"].ToString());
                }
            }

            return 0;
        }
        public void ReadLog()
        {
            //TODO: read log file and show in browser
        }
        
        public void writeXML()
        {
            XmlSerializer writer = new XmlSerializer(typeof(ControlData));
            StreamWriter file = new StreamWriter(CONST.CONTROLSERVER_SETTINGS);
            if (file != null)
            {
                writer.Serialize(file, data);
            }
            file.Close();
        }
        public void readXML()
        {
            XmlSerializer reader = new XmlSerializer(typeof(ControlData));
            StreamReader file = new StreamReader(CONST.CONTROLSERVER_SETTINGS);
            data = (ControlData)reader.Deserialize(file);
        }

    }
}
