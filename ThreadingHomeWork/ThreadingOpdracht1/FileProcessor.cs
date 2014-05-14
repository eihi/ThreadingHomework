using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadingOpdracht1
{
    class FileProcessor
    {
        public List<String> files { get; set; }

        public FileProcessor()
        {
            files = new List<string>();
        }

        public List<string> process(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        files.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("Er ging iets mis.");
                Console.WriteLine(e.Message);
            }
            return files;
            //if (File.Exists(path))
            //{
            //    // This path is a file
            //    ProcessFile(path);
            //}
            //else if (Directory.Exists(path))
            //{
            //    // This path is a directory
            //    ProcessDirectory(path);
            //}
            //else
            //{
            //    Console.WriteLine(path + " is not a valid file or directory.");
            //}
            //return files;
        }

        // Process all files in the directory passed in, recurse on any directories  
        // that are found, and process the files they contain. 
        private void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory. 
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory. 
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        // Insert logic for processing found files here. 
        private void ProcessFile(string path)
        {
            Console.WriteLine("Processed file '{0}'.", path);
            files.Add(path);
        }
    }
}
