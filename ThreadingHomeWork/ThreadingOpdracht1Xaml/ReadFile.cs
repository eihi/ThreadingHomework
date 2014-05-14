using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadingOpdracht1
{
    public class Counter
    {
        private int found { get; set; }

        public Counter()
        {

        }

        public Counter(string fileName)
        {
            found = this.Read(fileName);
        }

        public int Read(string fileName)
        {
            return 0;
        }
    }
}
