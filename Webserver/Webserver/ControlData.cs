using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver
{
    public class ControlData
    {
        private int webport;
        public int Webport
        {
            get { return webport; }
            set { webport = value; }
        }
        private int controlport;

        public int Controlport
        {
            get { return controlport; }
            set { controlport = value; }
        }
        private string webroot;

        public string Webroot
        {
            get { return webroot; }
            set { webroot = value; }
        }
        private string defaultpage;

        public string Defaultpage
        {
            get { return defaultpage; }
            set { defaultpage = value; }
        }
        private bool directorybrowsing;

        public bool Directorybrowsing
        {
            get { return directorybrowsing; }
            set { directorybrowsing = value; }
        }
    }
}
