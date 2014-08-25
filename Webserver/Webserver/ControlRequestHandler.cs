using System;
using System.Collections.Generic;
using System.IO;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Webserver
{
    public class ControlRequestHandler
    {
        private TcpClient client;
        private ControlSettings settings;
        private SendResponse sendResponse;
        private int authenticationlevel = 0;

        public ControlRequestHandler(TcpListener listener, ControlSettings settings)
        {
            this.settings = settings;
            sendResponse = new SendResponse();

            while (true)
            {
                this.client = listener.AcceptTcpClient();
                start();
            }
        }

        private void start()
        {
            Console.WriteLine("\nWaiting for client to connect...");
            if (client.Connected)
            {
                Console.WriteLine("ControlServer: Accept connection from client: " + client.Client.RemoteEndPoint);
                SslStream sslStream = createStream(client);

                // Receive data from client
                string request = ReceiveClientData(sslStream);
                //Console.WriteLine("received: " + request);
                

                // Determine request type
                string requestType = DetermineRequestType(request);
                switch (requestType)
                {
                    case "GET":
                        Console.WriteLine("RequestType: " + requestType + "\n" + request);
                        break;
                    case "POST":
                        handlePOST(request);
                        break;
                    default:
                        Console.WriteLine("RequestType: " + requestType);
                        break;
                }

                

                // Create response
                string response = "";
                switch (authenticationlevel)
                {
                    case CONST.SECURITY_BEHEERDER:
                        response = ControlPanelBuilder(authenticationlevel);
                        break;
                    case CONST.SECURITY_ONDERSTEUNER:
                        response = ControlPanelBuilder(authenticationlevel);
                        break;
                    default:
                        response = LoginFormBuilder();
                        break;
                }

                // Send response
                sendResponse.SendSSLResponse(sslStream, response);
                
                // Close connection
                this.client.Close();
            }
        }

        private void handlePOST(string request)
        {
            //TODO: handle POST
            List<string> formdata = new List<string>();
            Console.WriteLine("POST: " + request);
            string[] postlines = request.Split('\n');
            formdata = processInlog(postlines[postlines.Length-1]);
            // Save settings
            switch(formdata.Count)
            {
                case 2:
                    //inloggegevens
                    // Authenticate client
                    string username = formdata[0];
                    string password = formdata[1];
                    authenticationlevel = AuthenticateUser(username, password);
                    username = "";
                    password = "";
                    break;
                case 5:
                    if(authenticationlevel != 0)
                    {
                        settings.Webport = int.Parse(formdata[0]);
                        settings.Controlport = int.Parse(formdata[1]);
                        settings.Webroot = formdata[2];
                        settings.Defaultpage = formdata[3];
                        settings.Directorybrowsing = formdata[4] == "on" ? true : false;

                        settings.Save();
                    }
                    //settings
                    
                    break;
            }
            
        }
        public List<string> processInlog(string line)
        {
            List<string> formdata = new List<string>();

            string[] data = line.Split('&');
            for(int i = 0; i < data.Length; i++)
            {
                string value = data[i].Split('=')[1];
                formdata.Add(value);
            }

            return formdata;
        }

        public int AuthenticateUser(string username, string password)
        {
            string connectionstring = "Server=" + CONST.DBCONN_SERVER
                                   + ";Database=" + CONST.DBCONN_DATABASE
                                   + ";Uid=" + CONST.DBCONN_USERID
                                   + ";Pwd=" + CONST.DBCONN_PASSWORD + ";";
            MySqlConnection conn = null;
            MySqlDataReader reader = null;
            try
            {
                conn = new MySqlConnection(connectionstring);
                conn.Open();

                Console.WriteLine("Database connection state: " + conn.State.ToString());
                // Check if username and password are valid
                if (UsernameIsValid(username) && PasswordIsValid(password))
                {
                    string getSaltQuery = "SELECT salt FROM users WHERE username = @Username;";

                    MySqlCommand saltcommand = new MySqlCommand(getSaltQuery, conn);
                    saltcommand.Parameters.Add(new MySqlParameter("Username", username));
                    
                    reader = saltcommand.ExecuteReader();
                    string salt = null;
                    while (reader.Read())
                    {
                        salt = reader["salt"].ToString();
                        Console.WriteLine(salt);
                    }
                    reader.Close();
                    //TODO: encrypt password with salt
                    MD5hasher hasher = new MD5hasher();
                    string hashedPass = hasher.GetMd5Hash(password, salt);

                    //TODO: personal cookie to identify user in browser
                    string getUserQuery = "SELECT securitylevel FROM users WHERE username = @Username and password = @Password;";
                    MySqlCommand userCommand = new MySqlCommand(getUserQuery, conn);
                    userCommand.Parameters.Add(new MySqlParameter("Username", username));
                    userCommand.Parameters.Add(new MySqlParameter("Password", hashedPass));

                    reader = userCommand.ExecuteReader();
                    int count = reader.FieldCount;
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("security level from DB: " + int.Parse(reader["securitylevel"].ToString()));
                            return int.Parse(reader["securitylevel"].ToString());
                        }
                    }
                    return 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Database connection failed:" + e.ToString());
            }
            return 0;
        }

        private bool PasswordIsValid(string password)
        {
            //TODO: password validation
            return true;
        }

        private bool UsernameIsValid(string username)
        {
            //TODO: geeft false terug??
            bool match = Regex.IsMatch(username, "r\"^[A-Za-z0-9\\.\\+_-]+@[A-Za-z0-9\\._-]+\\.[a-zA-Z]*$\"");
            return true;
        }
        public void ReadLog()
        {
            //TODO: read log file and show in browser
        }

        private string ControlPanelBuilder(int authenticationlevel)
        {
            string disabledValue = authenticationlevel != CONST.SECURITY_BEHEERDER ? "disabled" : "";
            string checkedValue = settings.Directorybrowsing ? "checked" : "";
            string html = "";
            html += "<!doctype html>\n<html lang=\"en\">\n<head>\n<style>div.log {padding:10px;}\ntable {\n";
            html += "border:1px solid #000;\npadding:5px;\n}\ntd {\npadding:5px;\n";
            html += "}\n</style>\n</head>\n<body>\n<div>\n<form method=\"POST\">\n";
            html += "<table>\n<tr>\n<td><h1>SuperServer</h1></td>\n<td><h1>Control Panel</h1></td>\n</tr>\n";
            html += "<tr>\n<td>Web port: </td>\n";
            html += "<td><input type=\"text\" name=\"webport\" value=\"" + settings.Webport + "\" " + disabledValue + "></td>\n";
            html += "</tr>\n<tr>\n<td>Control port: </td>\n";
            html += "<td><input type=\"text\" name=\"controlport\" value=\"" + settings.Controlport + "\" " + disabledValue + "></td>\n";
            html += "</tr>\n<tr>\n<td>Webroot: </td>\n";
            html += "<td><input type=\"text\" name=\"webroot\" value=\"" + settings.Webroot + "\" " + disabledValue + "></td>\n";
            html += "</tr>\n<tr>\n<td>Default page: </td>\n";
            html += "<td><input type=\"text\" name=\"defaultpage\" value=\"" + settings.Defaultpage + "\" " + disabledValue + "></td>\n";
            html += "</tr>\n<tr>\n<td>Directory browsing: </td>\n";
            html += "<td><input type=\"checkbox\" name=\"dirbrow\"" + checkedValue + " " + disabledValue + "></td>\n";
            html += "</tr>\n<tr>\n<td><input type=\"button\" value=\"Show Log\"></td>\n";
            html += "<td><input type=\"submit\" value=\"OK\"" + disabledValue + "></td>\n</tr>\n";
            string log = "";
            foreach (string value in LogFile())
            {
                log += value + "<br>";
            }
            html += "</table>\n</form>\n</div>\n<div class=\"log\">"+ log +"</div>\n</body>\n</html>\n";
            return html;
        }

        private List<string> LogFile()
        {
            string line;
            List<string> logFile = new List<string>();
            // Read the file and display it line by line.
            System.IO.StreamReader file = new StreamReader(CONST.CONTROLSERVER_LOGFILE);
            while ((line = file.ReadLine()) != null)
            {
                logFile.Add(line);
            }

            file.Close();
            return logFile;
        }

        private string LoginFormBuilder()
        {
            string html = "";
            html += "<!doctype html> <html lang=\"en\">";
            html += "<head><style>table {border:1px solid #000;padding:5px;}td {padding:5px;}</style></head>";
            html += "<body><div><form method=\"POST\">";
            html += "<table><tr><td><h1>Control Panel</h1></td><td><h1>Login</h1></td></tr>";
            html += "<tr><td>Username: </td><td><input type=\"text\" name=\"uid\"></td></tr>";
            html += "<tr><td>Password: </td><td><input type=\"password\" name=\"pwd\"></td></tr>";
            html += "<tr><td></td><td><input type=\"submit\" value=\"Login\"></td></tr>";
            html += "</table></form></div></body></html>";
            return html;
        }

        private string ReceiveClientData(SslStream sslStream)
        {
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            if (sslStream != null)
            {
                do
                {

                    // Read the client's test message.
                    bytes = sslStream.Read(buffer, 0, buffer.Length);

                    // Use Decoder class to convert from bytes to UTF8 
                    // in case a character spans two buffers.
                    Decoder decoder = Encoding.UTF8.GetDecoder();
                    char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                    decoder.GetChars(buffer, 0, bytes, chars, 0);
                    messageData.Append(chars);
                    // Check for EOF or an empty message. 
                    int end = messageData.ToString().IndexOf("\r\n");
                    if (end != -1)
                    {
                        break;
                    }

                } while (bytes != 0);
            }
            return messageData.ToString();
        }

        private string ByteArrayToString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        private string DetermineRequestType(string request)
        {
            string requestType = request.Split(' ')[0];
            requestType = Regex.IsMatch(requestType, @"^[A-Z]{3,7}$") ? requestType : "INVALID_REQUESTTYPE";
            return requestType;
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
    }
}
