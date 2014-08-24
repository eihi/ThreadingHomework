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
        private ControlSettings settings;
        private SendResponse sendResponse;
        //private int port;


        public ControlSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }
        public ControlServer()
        {
            //thread starten en startserver aanroepen
            settings = new ControlSettings();
            sendResponse = new SendResponse();
            readXML();
            Thread controlThread = new Thread(startServer);
            controlThread.Start();

        }
        public SslStream createStream(TcpClient client)
        {
            try
            {
                X509Certificate serverCertificate = new X509Certificate2(CONST.SSL_CERTIFICATE, CONST.SSL_PASSWORD);
                var sslStream = new SslStream(client.GetStream(), false);
                sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls, true);
                return sslStream;
            }
            catch (Exception e)
            {
                Console.WriteLine("SSLStream creation failed:" + e.ToString());
                return null;
            }
        }
        public void startServer()
        {
            if (settings.Controlport.ToString().Length == 0)
            {
                settings.Controlport = 8081;
            }
            TcpListener serverSocket = new TcpListener(settings.Controlport);
            serverSocket.Start();
            Console.WriteLine("ControlServer Started - Listening on port:" + settings.Controlport);

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
                    //TODO: if port changes directly change this in servers, reload config whenever xml file is edited

                    // Build response based on authenticationlevel
                    string username = CONST.TEST_ACCOUNT;
                    string password = CONST.TEST_PASSWORD;
                    int authenticationlevel = AuthenticateUser(username, password);

                    if (authenticationlevel == CONST.SECURITY_BEHEERDER || authenticationlevel == CONST.SECURITY_ONDERSTEUNER)
                    {
                        sendResponse.SendSSLResponse(sslStream, ControlPanelBuilder(authenticationlevel));
                    }
                    else
                    {
                        //TODO: redirect to loginform
                    }
                }
            }
        }

        private string ControlPanelBuilder(int authenticationlevel)
        {
            string disabledValue = authenticationlevel != CONST.SECURITY_BEHEERDER ? "disabled" : "";
            string checkedValue = settings.Directorybrowsing ? "checked" : "";
            string html = "";
            html += "<!doctype html>\n<html lang=\"en\">\n<head>\n<style>\ntable {\n";
			html += "border:1px solid #000;\npadding:5px;\n}\ntd {\npadding:5px;\n";
            html += "}\n</style>\n</head>\n<body>\n<div>\n<form method=\"POST\">\n";
            html += "<table>\n<tr>\n<td><h1>SuperServer</h1></td>\n<td><h1>Control Panel</h1></td>\n</tr>\n";
            html += "<tr>\n<td>Web port: </td>\n";
            html += "<td><input type=\"text\" name=\"webport\" value=\""+settings.Webport+"\" "+disabledValue+"></td>\n";
            html += "</tr>\n<tr>\n<td>Control port: </td>\n";
            html += "<td><input type=\"text\" name=\"controlport value=\"" + settings.Controlport + "\" " + disabledValue + "></td>\n";
            html += "</tr>\n<tr>\n<td>Webroot: </td>\n";
            html += "<td><input type=\"text\" name=\"webroot value=\"" + settings.Webroot + "\" " + disabledValue + "></td>\n";
            html += "</tr>\n<tr>\n<td>Default page: </td>\n";
            html += "<td><input type=\"text\" name=\"defaultpage value=\"" + settings.Defaultpage + "\" " + disabledValue + "></td>\n";
            html += "</tr>\n<tr>\n<td>Directory browsing: </td>\n";
            html += "<td><input type=\"checkbox\" name=\"dirbrow\""+checkedValue+"></td>\n";
            html += "</tr>\n<tr>\n<td><input type=\"button\" value=\"Show Log\"></td>\n";
            html += "<td><input type=\"submit\" value=\"OK\"></td>\n</tr>\n";
            html += "</table>\n</form>\n</div>\n</body>\n</html>\n";
            return html;
        }

        public int AuthenticateUser(string username, string password)
        {
            string connectionstring = "Server=" + CONST.DBCONN_SERVER
                                   + ";Database=" + CONST.DBCONN_DATABASE
                                   + ";Uid=" + CONST.DBCONN_USERID
                                   + ";Pwd=" + CONST.DBCONN_PASSWORD + ";";
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
            XmlSerializer writer = new XmlSerializer(typeof(ControlSettings));
            StreamWriter file = new StreamWriter(CONST.CONTROLSERVER_SETTINGS);
            if (file != null)
            {
                writer.Serialize(file, settings);
            }
            file.Close();
        }
        public void readXML()
        {
            XmlSerializer reader = new XmlSerializer(typeof(ControlSettings));
            StreamReader file = new StreamReader(CONST.CONTROLSERVER_SETTINGS);
            settings = (ControlSettings)reader.Deserialize(file);
        }

    }
}
