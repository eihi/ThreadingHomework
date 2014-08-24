using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
//using MySql.Data.MySqlClient;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Webserver
{
    public class ControlRequestHandler
    {
        private TcpClient client;
        private ControlSettings settings;
        private SendResponse sendResponse;

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
            Console.WriteLine("Waiting for client to connect...");
            if (client.Connected)
            {
                Console.WriteLine("ControlServer: Accept connection from client: " + client.Client.RemoteEndPoint);
                SslStream sslStream = createStream(client);

                // Receive data from client
                Byte[] receivedBytes = ReceiveClientData();

                // Determine request type
                string requestType = DetermineRequestType(receivedBytes);
                switch (requestType)
                {
                    case "GET":
                        // Handle GET request
                        break;
                    case "POST":
                        handlePOST();
                        break;
                    default:
                        Console.WriteLine("RequestType: is not valid");
                        break;
                }

                // Authenticate client
                string username = CONST.TEST_ACCOUNT;
                string password = CONST.TEST_PASSWORD;
                int authenticationlevel = AuthenticateUser(username, password);
                authenticationlevel = 1; //TODO: remove when done with testing

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

        private void handlePOST()
        {
            //TODO: handle POST

            // Save settings
            //settings.Save();
        }

        public int AuthenticateUser(string username, string password)
        {
            /*string connectionstring = "Server=" + CONST.DBCONN_SERVER
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
                    //TODO: personal cookie to identify user in browser
                    string getUserQuery = "SELECT securitylevel FROM users WHERE username = @Username and password = @Password;";
                    MySqlCommand userCommand = new MySqlCommand(getUserQuery, conn);
                    userCommand.Parameters.Add(new MySqlParameter("Username", username));
                    userCommand.Parameters.Add(new MySqlParameter("Password", password));

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
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Database connection failed:" + e.ToString());
            }*/
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
            html += "<!doctype html>\n<html lang=\"en\">\n<head>\n<style>\ntable {\n";
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
            html += "</table>\n</form>\n</div>\n</body>\n</html>\n";
            return html;
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

        private byte[] ReceiveClientData()
        {
            Console.WriteLine("ControlServer: Accept connection from client: " + client.Client.RemoteEndPoint);
            Byte[] receivedBytes = new Byte[1024];
            int i = client.Client.Receive(receivedBytes, receivedBytes.Length, 0);
            return receivedBytes;
        }

        private string DetermineRequestType(byte[] receivedData)
        {
            string requestType = "";
            string request = Encoding.ASCII.GetString(receivedData);
            Console.WriteLine("Incoming request: " + request);
            return requestType = request.Split(' ')[0];
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
