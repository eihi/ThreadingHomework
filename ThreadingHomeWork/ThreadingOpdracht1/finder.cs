using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadingOpdracht1
{
    public class finder
    {
        public int found { get; set; }
        public void now(object fileName)
        {
            this.found = 0;
            try
            {
                using (StreamReader sr = new StreamReader((string)fileName))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("42"))
                        {
                            this.found++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Er ging iets mis.");
                Console.WriteLine(e.Message);
            }
        }
    }
}
