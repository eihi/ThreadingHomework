using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Webserver
{
    public class ControlSettings
    {
        public int Webport
        {
            get { return webport; }
            set { webport = value; }
        }
        private int webport;

        public int Controlport
        {
            get { return controlport; }
            set { controlport = value; }
        }
        private int controlport;

        public string Webroot
        {
            get { return webroot; }
            set { webroot = value; }
        }
        private string webroot;

        public string Defaultpage
        {
            get { return defaultpage; }
            set { defaultpage = value; }
        }
        private string defaultpage;

        public bool Directorybrowsing
        {
            get { return directorybrowsing; }
            set { directorybrowsing = value; }
        }
        private bool directorybrowsing;

        public ControlSettings Load()
        {
            XmlSerializer reader = new XmlSerializer(typeof(ControlSettings));
            StreamReader file = new StreamReader(CONST.CONTROLSERVER_SETTINGS);
            return (ControlSettings)reader.Deserialize(file);
        }
        public void Save()
        {
            XmlSerializer writer = new XmlSerializer(typeof(ControlSettings));
            StreamWriter file = new StreamWriter(CONST.CONTROLSERVER_SETTINGS);
            if (file != null)
            {
                writer.Serialize(file, this);
            }
            file.Close();
        }
    }
}
