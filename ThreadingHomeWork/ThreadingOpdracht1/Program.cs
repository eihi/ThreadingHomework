using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadingOpdracht1
{
    class Program
    {
        private static FileProcessor fileProcessor;
        private static List<string> filelist;
        private static MeaningOfLife meaningofLife;
        static void Main(string[] args)
        {
            // Initialize fields
            fileProcessor = new FileProcessor();
            filelist = new List<string>();

            // Ask for file or folder to read
            Console.Write("Enter path: ");
            string path = Console.ReadLine();
            if (path == null)
            {
                path = "C:\\Users\\st\\Documents\\GitHub\\ThreadingHomework\\Bestanden Opdr1\\Bestanden.txt";
            }
            path = "C:\\Users\\st\\Documents\\GitHub\\ThreadingHomework\\Bestanden Opdr1\\Bestanden.txt";
            // Process files into list
            filelist = fileProcessor.process(path);

            // Find meaning of life
            meaningofLife = new MeaningOfLife(filelist);

            Console.ReadLine();
        }
    }
}
