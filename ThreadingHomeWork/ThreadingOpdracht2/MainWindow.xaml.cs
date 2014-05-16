using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ThreadingOpdracht2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HashSet<string> links;
        private string directory = Directory.GetCurrentDirectory()+"\\Website";
        public static Semaphore limit { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            limit = new Semaphore(10, 10);
            Console.WriteLine(directory);
        }
        private void Downloader(string url)
        {
            // thread start
            Thread thread = new Thread( new ParameterizedThreadStart(HtmlParser));

            thread.Start(url);
            // html parser
            
        }

        public void HtmlParser(object url)
        {
            WebClient client = new WebClient();
            //download html from url
            string html = client.DownloadString("http://" + url);
            //write html to file
            File.WriteAllText(directory + "\\" + url + ".txt", html);
            // Scan links on this page
            HtmlTag tag;
            HtmlParser parse = new HtmlParser(html);
            while (parse.ParseNext("a", out tag))
            {
                //check if there is a link
                string value;
                if (tag.Attributes.TryGetValue("href", out value))
                {
                    //check if link is valid
                    if(value[0] == '/' && value.Length > 2)
                    {
                        Console.WriteLine("url gevonden:" + value);
                        //check if link is not in list
                        if(!links.Contains(url+value))
                        {
                            links.Add(url+value);
                        }
                    }
                }
            }
        }

        private void startCrawling(object sender, RoutedEventArgs e)
        {
            // Initialize hash
            links = new HashSet<string>();
            // Create directory
            Directory.CreateDirectory(directory);
            // Get Starting Url
            string url = StartingUrl.Text;
            // Add url to hash
            links.Add(url);
            // Test
            Console.WriteLine(links.Contains(url));

            HtmlParser(url);
            //call downloader with url
            
        }
    }
}
