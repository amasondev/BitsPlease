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
        }

        string SelectedOutput; // This should get checked for validity if the URL changes.

        private void DownloadVideoURL(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(urlInput.Text))
            {
                string PathToDownloadTo = "C:\\Users\\Alex\\Desktop";
                ProcessStartInfo info = GetDownloaderStartInfo(urlInput.Text);
                Process.Start(info);
            }
        }

        private ProcessStartInfo GetDownloaderStartInfo(string Arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("youtube-dl.exe")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                Arguments = Arguments
            };

            return startInfo;
        }

        private List<string> GetVideoQualityList()
        {
            //string Query = "-F" + urlInput.Text;
            string Query = "-F https://www.youtube.com/watch?v=F04iu5IR3CM";
            ProcessStartInfo info = GetDownloaderStartInfo(Query);
            List<string> output = new LineReader(info).GetOutput();
            return output;
        }

        private void SetAudioOnly(object sender, RoutedEventArgs e) 
        {
            bool IsAudioOnly = AudioOnlyBox.IsChecked ?? false;
            if (IsAudioOnly)
            {
                DisableVideoQuality();
                EnableAudioQuality();
                string message = String.Join(", ", GetVideoQualityList().ToArray()); // temp
                MessageBox.Show(message);
            } 
            else 
            {
                EnableVideoQuality();
                DisableAudioQuality();
            }
            UpdateSelectedOutput();
        }

        private void DisableVideoQuality()
        {
            // Video quality window gets blanked out.
            VideoOutputs.Cursor = Cursors.No;
        }

        private void EnableVideoQuality()
        {
            // Set the video quality appearance and interactions to default.
            // Your SelectedOutput becomes whichever one of these has been selected.
            VideoOutputs.Cursor = Cursors.Hand;
        }

        private void DisableAudioQuality()
        {
            AudioFormat.Visibility = Visibility.Hidden;
        }

        private void EnableAudioQuality()
        {
            AudioFormat.Visibility = Visibility.Visible;
            //ComboboxItem SelectedAudio = (ComboboxItem)AudioFormatSelector.SelectedItem;
            // Your SelectedOutput becomes the last option that has been selected.
        }

        private void UpdateSelectedOutput()
        {
            // Change the SelectedOutput text to the selected audio/video quality
            SelectedOutput = "";
            SelectedOutputLabel.Text = "Output:";
        }    
    }

    public class LineReader
    {
        // TODO filter video versus audio formats.
        Process process;
        List<string> output = new List<string>();

        public LineReader(ProcessStartInfo info)
        {
            process = new Process();
            process.OutputDataReceived += new DataReceivedEventHandler(AppendData);
            process.StartInfo = info;
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }

        public List<string> GetOutput()
        {
            return output;
        }

        private void AppendData(object sendingProcess, DataReceivedEventArgs line)
        {
            MessageBox.Show(line.Data);
            output.Add(line.Data);
        }
    }
}
