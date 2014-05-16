using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine(directory);
        }
        private List<string> Downloader(string url)
        {
            // thread start

            // html parser
            return null;
        }

        private void HtmlParser(string url)
        {

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
        }
    }
}
