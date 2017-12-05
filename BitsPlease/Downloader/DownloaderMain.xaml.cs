using System;
using System.Collections.Generic;
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
using System.Diagnostics;

namespace Downloader
{
    /// <summary>
    /// Interaction logic for DownloaderMain.xaml
    /// </summary>
    public partial class DownloaderMain : Window
    {

        public DownloaderMain()
        {
            InitializeComponent();
            BUSYdownload.Visibility = Visibility.Hidden;
        }

        private void DownloadVideoURL(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(urlInput.Text))
            {
                string PathToDownloadTo = "C:\\Users\\Alex\\Desktop";

                ProcessStartInfo startInfo = new ProcessStartInfo("youtube-dl.exe");
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.Arguments = urlInput.Text;

                Process p = Process.Start(startInfo);
                
            }
        }
    }
}
