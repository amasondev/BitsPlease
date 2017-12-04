using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Threading;

namespace Downloader
{
    /// <summary>
    /// Interaction logic for DownloaderMain.xaml
    /// </summary>
    public partial class DownloaderMain : Window
    {
        private const double URLINPUT_WAIT_TIME = 1000.0f;
        DispatcherTimer urlInputTimer;

        StringDictionary Outputs = new StringDictionary();
        string SelectedOutput; // This should get checked for validity if the URL changes.
        string PathToDownloadTo = "C:\\Users\\Alex\\Desktop";

        public DownloaderMain()
        {
            urlInputTimer = new DispatcherTimer{Interval = TimeSpan.FromMilliseconds(URLINPUT_WAIT_TIME)};
            urlInputTimer.Tick += OnURLInputTimerComplete;
            InitializeComponent();
        }

        private void DownloadVideoURL(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(urlInput.Text))
            {
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

        private List<string[]> GetVideoQualityList()
        {
            string query = "-F " + urlInput.Text;
            ProcessStartInfo info = GetDownloaderStartInfo(query);
            List<string[]> output = new ProcessFilter(info).GetVideoOutputs();
            return output;
        }

        private void SetAudioOnly(object sender, RoutedEventArgs e) 
        {
            bool IsAudioOnly = AudioOnlyBox.IsChecked ?? false;
            if (IsAudioOnly)
            {
                DisableVideoQuality();
                EnableAudioQuality();
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

        private void OnURLInputTimerComplete(object sender, EventArgs e)
        {
            urlInputTimer.Stop();
            if (string.IsNullOrEmpty(urlInput.Text)) return;

            List<string[]> videoQualityList = GetVideoQualityList();
            foreach (string[] qualityOption in videoQualityList)
            {
                string extension = qualityOption[1];
                VideoOutputs.Items.Add(extension);
            }
        }

        private void On_URLTextInput(object sender, TextChangedEventArgs e)
        {
            urlInputTimer.Stop();
            urlInputTimer.Start();
        }
  }

  public class ProcessFilter
    {
        Process process;
        List<string> output = new List<string>();

        public ProcessFilter(ProcessStartInfo info)
        {
            process = new Process();
            process.OutputDataReceived += new DataReceivedEventHandler(AppendData);
            process.StartInfo = info;
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }

        public List<string[]> GetVideoOutputs()
        {
            List<string[]> filtered = GetFilteredVideo();
            return filtered;
        }

        public List<string[]> GetAudioOutputs()
        {
            List<string[]> filtered = GetFilteredAudio();
            return filtered;
        }

        private List<string[]> GetFilteredVideo()
        {
            List<string[]> filtered = new List<string[]>();
            foreach (string outputLine in output)
            {
                if (IsValidVideo(outputLine))
                {
                    string[] formattedLine = FilterVideo(outputLine);
                    filtered.Add(formattedLine);
                }
            }
            return filtered;
        }

        private string[] FilterVideo(string outputLine)
        {
            /* yt-dl outputs a line such as: 
             * 43           webm       640x360    medium , vp8.0, vorbis@128k
             * The goal is to get the format code, extension, and resolution 
             * (File size is not offered in muxed video/audio option)
             */

            string[] dividedLine = outputLine.Split(','); 
            string firstEntry = dividedLine[0]; // 43           webm       640x360    medium 
            string[] info = firstEntry.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries); // [43, webm, 640x360, medium]
            return info;
        }

        private List<string[]> GetFilteredAudio()
        {
            List<string[]> filtered = new List<string[]>();

            foreach (string outputLine in output)
            {
                if (IsValidAudio(outputLine))
                {
                    string[] dividedLine = outputLine.Split(',');
                    filtered.Add(dividedLine);
                }
            }
            return filtered;
        }

        private bool IsValidVideo(string outputLine)
        {
            if (String.IsNullOrEmpty(outputLine)) return false;
            bool notVideoOnly = !outputLine.Contains("video only");
            bool notAudioOnly = !outputLine.Contains("audio only");
            char firstCharacter = outputLine.ToCharArray().ElementAt(0);
            bool hasFormatCode = char.IsNumber(firstCharacter);
            return notVideoOnly && notAudioOnly && hasFormatCode;
        }

        private bool IsValidAudio(string outputLine)
        {
            if (String.IsNullOrEmpty(outputLine)) return false;
            return outputLine.Contains("audio only");
        }

        private void AppendData(object sendingProcess, DataReceivedEventArgs line)
        {
            output.Add(line.Data);
        }
    }
}
