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
        private string baseUrl;
        private string directory = Directory.GetCurrentDirectory()+"\\Website";
        public static Semaphore limit { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            limit = new Semaphore(2, 10);
            Console.WriteLine(directory);
        }
        private void Downloader(string url)
        {

                
                // thread start
                Thread thread = new Thread(new ParameterizedThreadStart(HtmlParser));
                Console.WriteLine("thread has started");
                thread.Start(url);
           
        }

        public void HtmlParser(object url)
        {

            limit.WaitOne();
            int nummer = 0;
            int progress = 0;
            this.Dispatcher.Invoke((Action)(() =>
            {
                nummer = getFreeProgressbar();
                startProgress(nummer, url.ToString(), progress);
            }));
            
            WebClient client = new WebClient();
            //download html from url
            string html = client.DownloadString("http://" + url);

            //laag aan de hand van // maken
            //count /
            string[] folders = url.ToString().Split('/');
            string currentDirectory = directory;
            string writeTo = "";
            for (int i = 0; i < folders.Length - 1; i++ )
            {
                if (folders[i] == "")
                {
                    writeTo = folders[i - 1];
                }
                else
                {
                    writeTo = folders[i];
                }
                currentDirectory += "\\" + folders[i];
                Directory.CreateDirectory(currentDirectory);
            }
            
            //write html to file
            File.WriteAllText(currentDirectory + "\\" + writeTo + ".html", html);
            // Scan links on this page
            HtmlTag tag;
            HtmlParser parse = new HtmlParser(html);
            this.Dispatcher.Invoke((Action)(() =>
            {
                progress = 10;
                startProgress(nummer, url.ToString(), progress);
            }));
            while (parse.ParseNext("a", out tag))
            {
                //check if there is a link
                string value;
                if (tag.Attributes.TryGetValue("href", out value))
                {
                    //check if link is valid
                    if(value[0] == '/' && value.Length > 2 && value[1] != '/')
                    {
                        progress += 5;
                        Console.WriteLine("url gevonden:" + value);
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            startProgress(nummer, url.ToString(), progress);
                        }));
                        //check if link is not in list
                        if(!links.Contains(baseUrl+value))
                        {
                            links.Add(baseUrl + value);
                            Downloader(baseUrl + value);

                        }
                    }
                }
            }
            limit.Release(1);
            this.Dispatcher.Invoke((Action)(() =>
            {
                stopProgress(nummer);
            }));
            Console.WriteLine("thread stopped");
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
            baseUrl = url;
            // Test
            Console.WriteLine(links.Contains(url));

            Downloader(url);
            //call downloader with url

        }
        private void startProgress(int nummer, string url, int progress)
        {
            switch(nummer)
            {
                case 1:
                    progressStatus1.Content = "Running";
                    progressUrl1.Content = url;
                    progressBar1.Value = progress;
                    break;
                case 2:
                    progressStatus2.Content = "Running";
                    progressUrl2.Content = url;
                    progressBar2.Value = progress;
                    break;
                case 3:
                    progressStatus3.Content = "Running";
                    progressUrl3.Content = url;
                    progressBar3.Value = progress;
                    break;
                case 4:
                    progressStatus4.Content = "Running";
                    progressUrl4.Content = url;
                    progressBar4.Value = progress;
                    break;
                case 5:
                    progressStatus5.Content = "Running";
                    progressUrl5.Content = url;
                    progressBar5.Value = progress;
                    break;
                case 6:
                    progressStatus6.Content = "Running";
                    progressUrl6.Content = url;
                    progressBar6.Value = progress;
                    break;
                case 7:
                    progressStatus7.Content = "Running";
                    progressUrl7.Content = url;
                    progressBar7.Value = progress;
                    break;
                case 8:
                    progressStatus8.Content = "Running";
                    progressUrl8.Content = url;
                    progressBar8.Value = progress;
                    break;
                case 9:
                    progressStatus9.Content = "Running";
                    progressUrl9.Content = url;
                    progressBar9.Value = progress;
                    break;
                case 10:
                    progressStatus10.Content = "Running";
                    progressUrl10.Content = url;
                    progressBar10.Value = progress;
                    break;
            }
        }
        private void stopProgress(int nummer)
        {
            switch (nummer)
            {
                case 1:
                    progressStatus1.Content = "Finished";
                    progressBar1.Value = 100;
                    break;
                case 2:
                    progressStatus2.Content = "Finished";
                    progressBar2.Value = 100;
                    break;
                case 3:
                    progressStatus3.Content = "Finished";
                    progressBar3.Value = 100;
                    break;
                case 4:
                    progressStatus4.Content = "Finished";
                    progressBar4.Value = 100;
                    break;
                case 5:
                    progressStatus5.Content = "Finished";
                    progressBar5.Value = 100;
                    break;
                case 6:
                    progressStatus6.Content = "Finished";
                    progressBar6.Value = 100;
                    break;
                case 7:
                    progressStatus7.Content = "Finished";
                    progressBar7.Value = 100;
                    break;
                case 8:
                    progressStatus8.Content = "Finished";
                    progressBar8.Value = 100;
                    break;
                case 9:
                    progressStatus9.Content = "Finished";
                    progressBar9.Value = 100;
                    break;
                case 10:
                    progressStatus10.Content = "Finished";
                    progressBar10.Value = 100;
                    break;
            }
        }
        private int getFreeProgressbar()
        {
            if(progressStatus1.Content != "Running")
            {
                return 1;
            }
            if (progressStatus2.Content != "Running")
            {
                return 2;
            }
            if (progressStatus3.Content != "Running")
            {
                return 3;
            }
            if (progressStatus4.Content != "Running")
            {
                return 4;
            }
            if (progressStatus5.Content != "Running")
            {
                return 5;
            }
            if (progressStatus6.Content != "Running")
            {
                return 6;
            }
            if (progressStatus7.Content != "Running")
            {
                return 7;
            }
            if (progressStatus8.Content != "Running")
            {
                return 8;
            }
            if (progressStatus9.Content != "Running")
            {
                return 9;
            }
            if (progressStatus10.Content != "Running")
            {
                return 10;
            }
            return 0;
        }

        
    }
}
