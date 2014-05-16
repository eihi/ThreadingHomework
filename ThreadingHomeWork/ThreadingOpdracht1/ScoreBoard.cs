using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadingOpdracht1
{
    class ScoreBoard
    {
        private List<List<string>> top10;
        public ScoreBoard()
        {
            top10 = new List<List<string>>();
        }
        public void showTop10()
        {
            Console.WriteLine("ScoreBoard: Top 10");
            if (top10.Count != 0)
            {
                for (int i = 0; i < top10.Count; i++)
                {
                    Console.WriteLine("#" + (i + 1) + " " + top10[i][0]);
                }
            }
            else
            {
                Console.WriteLine("Top 10 is empty");
            }
        }

        public void addResult(string file, string number) 
        {
            // TODO Create critical section
            List<string> tempList = new List<string>();
            tempList.Add(file); 
            tempList.Add(number);
            top10.Add(tempList);
        }

        public void clearTop10()
        {
            top10 = null;
        }
    }
}
