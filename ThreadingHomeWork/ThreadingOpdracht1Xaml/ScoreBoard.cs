using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadingOpdracht1
{
    class ScoreBoard
    {
        private Array[,] top10;
        public ScoreBoard()
        {
            top10 = new Array[10,2];
        }
        public void showTop10()
        {
            Console.WriteLine("ScoreBoard: Top 10");
            if (top10 != null)
            {
                for (int i = 0; i < top10.Length; i++)
                {
                    Console.WriteLine("#" + (i + 1) + " " + top10[i, 0] + " contains " + top10[i, 1] + " instances of 42");
                }
            }
            else
            {
                Console.WriteLine("Top 10 is empty");
            }
        }

        public void addResult(string file, string number) 
        {

        }

        public void clearTop10()
        {
            top10 = new Array[10, 2];
        }
    }
}
