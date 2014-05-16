using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadingOpdracht1
{
    public class MeaningOfLife
    {
        private int found { get; set; }
        private List<string> filelist;

        public MeaningOfLife()
        {
            filelist = new List<string>();
        }

        public MeaningOfLife(List<string> list)
        {
            filelist = list;
            ListProcessor(filelist);
        }

        private void ListProcessor(List<string> files)
        {
            ScoreBoard scoreBoard = new ScoreBoard();
            for (int i = 0; i < files.Count; i++)
            {
                finder f = new finder();

                Thread thread = new Thread(new ParameterizedThreadStart(f.now));

                thread.Start(files[i]);

                scoreBoard.addResult(files[i], f.found.ToString());

                Console.WriteLine("Thread has finished");
            }
            scoreBoard.showTop10();
        }
    }
}
