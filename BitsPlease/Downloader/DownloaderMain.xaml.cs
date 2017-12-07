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
using Microsoft.Win32;
using BitsPlease;

namespace Downloader
{
    /// <summary>
    /// Interaction logic for DownloaderMain.xaml
    /// </summary>
    public partial class DownloaderMain : Window
    {
        private const double URLINPUT_WAIT_TIME = 1000.0f;
        DispatcherTimer urlInputTimer;
        bool isAudioOnly = false;
        VideoOutput selectedVideo;
        String[] audioOptions = { "ogg", "wav", "mp3", "aac", "m4a" };

        public DownloaderMain()
        {
            InitializeUrlTimer();
            InitializeComponent();
            DisableBusyIndicator();
            AddAudioOptions();
        }

        private void InitializeUrlTimer()
        {
            urlInputTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(URLINPUT_WAIT_TIME) };
            urlInputTimer.Tick += OnURLInputTimerComplete;
        }

        private void DownloadVideoURL(object sender, RoutedEventArgs e)
        {
            if (IsValidOutput())
            {
                DisableUI();
                StartDownloadProcess();
                EnableUI();
            }
            else
            {
                MessageBox.Show("Please select a quality option.");
            }
        }

        private void StartDownloadProcess()
        {
            SaveFileDialog saveFileDialog = CreateSaveFileDialog();
            if (!CanStartProcess(saveFileDialog)) return;

            string safeName = saveFileDialog.SafeFileName;
            string fileName = saveFileDialog.FileName;
            string query = GenerateQuery(fileName);
            Process process = GetProcess(query);
            if (process.Start())
            {
                new DownloadLauncher(process, safeName).Run();
            }
            else
            {
                MessageBox.Show("There was an error starting youtube-dl.exe");
            }
        }

        private SaveFileDialog CreateSaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = GetFileFilter()
            };
            return saveFileDialog;
        }

        private string GenerateQuery(string fileName)
        {
            string query;
            if (isAudioOnly)
            {
                string selectedAudio = (string)AudioOutputs.SelectedValue;
                query = "--extract-audio --audio-format " + selectedAudio + " --audio-quality 0 " + urlInput.Text;
            }
            else
            {
                VideoOutput video = VideoOutputs.SelectedValue as VideoOutput;
                query = "-f " + video.FormatCode + " " + urlInput.Text +
                               " -o \"" + fileName + "\"";
            }
            return query;
        }

        private string GetFileFilter()
        {
            string fileFilter;

            if (isAudioOnly)
            {
                fileFilter = GetAudioFilter();
            }
            else
            {
                fileFilter = GetVideoFilter();
            }
            return fileFilter;
        }

        private string GetVideoFilter()
        {
            VideoOutput video = VideoOutputs.SelectedValue as VideoOutput;
            return "Video file (*." + video.Extension + ")|*." + video.Extension;
        }

        private string GetAudioFilter()
        {
            string selectedAudio = (string)AudioOutputs.SelectedValue;
            return "Audio file (*." + selectedAudio + ")|*." + selectedAudio;
        }

        private Process GetProcess(string query)
        {
            ProcessStartInfo info = GetDownloaderStartInfo(query);
            Process process = new Process();
            process.StartInfo = info;
            return process;
        }

        private ProcessStartInfo GetDownloaderStartInfo(string query)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("youtube-dl.exe")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                Arguments = query
            };

            return startInfo;
        }

        private void SetAudioOnly(object sender, RoutedEventArgs e)
        {
            isAudioOnly = AudioOnlyBox.IsChecked ?? false;
            if (isAudioOnly)
            {
                DisableVideoQuality();
                EnableAudioQuality();
            }
            else
            {
                EnableVideoQuality();
                DisableAudioQuality();
            }
        }

        private void DisableVideoQuality()
        {
            VideoOutputs.Cursor = Cursors.No;
        }

        private void EnableVideoQuality()
        {
            VideoOutputs.Cursor = Cursors.Hand;
            UpdateVideoSelection();
        }

        private void DisableAudioQuality()
        {
            AudioFormat.Visibility = Visibility.Hidden;
        }

        private void EnableAudioQuality()
        {
            AudioFormat.Visibility = Visibility.Visible;
            UpdateAudioSelection();
        }

        private void OnURLInputTimerComplete(object sender, EventArgs e)
        {
            urlInputTimer.Stop();
            if (PreScreenedUrl())
            {
                EnableBusyIndicator();
                PopulateOptions();
                DisableBusyIndicator();
            }
        }

        private async void PopulateOptions()
        {
            ProcessFilter processFilter = null;
            string query = "-F " + urlInput.Text;
            ProcessStartInfo info = GetDownloaderStartInfo(query);
            await Task.Run(() =>
            {
                processFilter = new ProcessFilter(info);
            });
            if (processFilter != null)
            {
                ClearOptions();
                AddVideoOptions(processFilter);
            }
        }

        private void ClearOptions()
        {
            VideoOutputs.Items.Clear();
            selectedVideo = null;
        }

        private void AddVideoOptions(ProcessFilter processFilter)
        {
            List<string[]> videoQualityList = processFilter.GetVideoOutputs();
            foreach (string[] qualityOption in videoQualityList)
            {
                VideoOutputs.Items.Add(new VideoOutput(qualityOption));
            }
            VideoOutputs.SelectedIndex = 0;
        }

        private void AddAudioOptions()
        {
            foreach (string option in audioOptions)
            {
                AudioOutputs.Items.Add(option);
            }
            AudioOutputs.SelectedIndex = 0;
        }

        private void On_URLTextInput(object sender, TextChangedEventArgs e)
        {
            urlInputTimer.Stop();
            urlInputTimer.Start();
        }

        private void VideoOutputs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isAudioOnly)
            {
                UpdateVideoSelection();
            }
        }

        private void AudioFormatSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isAudioOnly)
            {
                UpdateAudioSelection();
            }
        }

        private void UpdateVideoSelection()
        {
            bool hasVideoOptions = VideoOutputs.Items.Count > 0;
            if (hasVideoOptions)
            {
                selectedVideo = (VideoOutput)VideoOutputs.SelectedItem;
                UpdateSelectedOutput(GetVideoLabel());
            }
        }

        private string GetVideoLabel()
        {
            VideoOutput selectedItem = (VideoOutput)VideoOutputs.SelectedItem;
            if (selectedItem != null)
            {
                return "Video - " + selectedItem.Extension + ", " + selectedItem.Resolution;
            }
            else
            {
                return "";
            }
        }

        private void UpdateAudioSelection()
        {
            bool hasAudioOptions = AudioOutputs.Items.Count > 0;
            if (hasAudioOptions)
            {
                string label = "Audio only - " + AudioOutputs.SelectedItem;
                UpdateSelectedOutput(label);
            }
        }

        private void UpdateSelectedOutput(string label)
        {
            SelectedOutputLabel.Text = "Output: " + label;
        }

        private bool IsValidOutput()
        {
            if (isAudioOnly)
            {
                return AudioOutputs.SelectedItem != null;
            }
            else
            {
                return selectedVideo != null;
            }
        }

        private void EnableBusyIndicator()
        {
            BUSYdownload.Visibility = Visibility.Visible;
        }

        private void DisableBusyIndicator()
        {
            BUSYdownload.Visibility = Visibility.Hidden;
        }

        private void DisableUI()
        {
            this.IsEnabled = false;
        }

        private void EnableUI()
        {
            this.IsEnabled = true;
        }

        private bool PreScreenedUrl()
        {
            return !string.IsNullOrEmpty(urlInput.Text);
        }

        private bool CanStartProcess(SaveFileDialog saveFileDialog)
        {
            bool canShowDialog = saveFileDialog.ShowDialog() ?? false;
            bool urlInputHasText = !String.IsNullOrEmpty(urlInput.Text);
            bool fileNameExists = !String.IsNullOrEmpty(saveFileDialog.FileName);
            return canShowDialog && urlInputHasText && fileNameExists;
        }
    }

    public class DownloadLauncher
    {
        ProgressWindow progressWindow;
        Process process;
        bool gotCancel = false;

        public DownloadLauncher(Process process, string fileName)
        {
            InitializeProgressWindow(fileName);
            this.process = process;
        }

        public async void Run()
        {
            await Task.Run(() =>
            {
                RunDownload();
            });
            EndDownload();
        }

        private void RunDownload()
        {
            string line;
            double progressAmount = 0;
            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                if (gotCancel) break;
                string[] segments = GetSegments(line);
                if (IsPercentageIndicator(segments))
                {
                    string percentstr = ParsePercent(segments);
                    double percentdbl;
                    if (double.TryParse(percentstr, out percentdbl))
                    {
                        progressAmount = percentdbl;
                    }
                }
                ReportProgress(progressAmount);
                Console.WriteLine("YOUTUBE-DL: " + line);
            }

            while ((line = process.StandardError.ReadLine()) != null)
            {
                Console.WriteLine("YOUTUBE-DL: " + line);
            }
        }

        private void InitializeProgressWindow(string fileName)
        {
            string downloadLabel = "Downloading " + fileName;
            progressWindow = new ProgressWindow(downloadLabel);
            progressWindow.Show();
            SetProgressCancel();
        }

        private void SetProgressCancel()
        {
            progressWindow.OnGetCancel += (s, ee) =>
            {
                gotCancel = true;
                process.Kill();
            };
        }

        private void EndDownload()
        {
            process.WaitForExit();
            process.Close();
            progressWindow.Complete();
        }

        private void ReportProgress(double progressAmount)
        {
            if (progressWindow.progress != null)
            {
                progressWindow.progress.Report(progressAmount);
            }
        }

        private string[] GetSegments(string line)
        {
            return line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private bool IsPercentageIndicator(string[] segments)
        {
            return segments.Length > 2 && segments[1].Contains('%');
        }

        private string ParsePercent(string[] segments)
        {
            return segments[1].TrimEnd('%');
        }
    }

    public class VideoOutput
    {
        // Binder for XAML
        public string FormatCode { get; set; }
        public string Extension { get; set; }
        public string Resolution { get; set; }
        public string Bitrate { get; set; }

        public VideoOutput(string[] qualityOption)
        {
            FormatCode = qualityOption[0];
            Extension = qualityOption[1];
            Resolution = qualityOption[2];
            Bitrate = qualityOption[qualityOption.Length - 2]; // Not always second last
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
            process.Close();
        }

        public List<string[]> GetVideoOutputs()
        {
            List<string[]> filtered = new List<string[]>();
            foreach (string outputLine in output)
            {
                if (IsValidVideo(outputLine))
                {
                    filtered.Add(FormatDetails(outputLine));
                }
            }
            filtered.Reverse();
            return filtered;
        }

        private string[] FormatDetails(string outputLine)
        {
            /* yt-dl outputs a line such as: 
             * 43           webm       640x360    medium , vp8.0, vorbis@128k
             * This produces [43, webm, 640x360, medium, vp8.0 ... ]
             */

            string[] dividedLine = outputLine.Split(',');
            string initialDetails = dividedLine[0]; // 43           webm       640x360    medium 
            string[] formattedInitialDetails = initialDetails.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            // [43, webm, 640x360, medium]
            string[] otherDetails = dividedLine.Skip(1).ToArray(); // [vp8.0, vorbis@128k]
            return formattedInitialDetails.Union(otherDetails).ToArray();
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
