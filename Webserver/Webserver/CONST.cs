using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver
{
    public static class CONST
    {
        public const int LOGLENGTH = 20;

        // Controlserver config
        public const string CONTROLSERVER_ROOT          = "..\\..\\Controlserver\\";
        public const string CONTROLSERVER_SETTINGS      = CONTROLSERVER_ROOT + "controlserver.xml";
        public const string CONTROLSERVER_LOGFILE       = CONTROLSERVER_ROOT + "logfile.txt";

        // Security levels
        public const int SECURITY_BEHEERDER             = 1;
        public const int SECURITY_ONDERSTEUNER          = 2;

        // SSL config
        public const string SSL_ROOT                    = "..\\..\\Certificaat\\";
        public const string SSL_CERTIFICATE             = SSL_ROOT + "servercertificaat.pfx";
        public const string SSL_PASSWORD                = "ab12345";

        // Databaseconnection config
        public const string DBCONN_SERVER               = "databases.aii.avans.nl";
        public const string DBCONN_DATABASE             = "sjpoel_db";
        public const string DBCONN_USERID               = "sjpoel";
        public const string DBCONN_PASSWORD             = "Ab12345";

        // Test account
        public const string TEST_ACCOUNT                = "stefanieavansnl";
        public const string TEST_PASSWORD               = "wachtwoord";
    }
}
