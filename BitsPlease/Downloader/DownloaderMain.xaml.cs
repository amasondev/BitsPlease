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
        OutputOption selectedOutput;
        string formatCode;

        public DownloaderMain()
        {
            urlInputTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(URLINPUT_WAIT_TIME) };
            urlInputTimer.Tick += OnURLInputTimerComplete;
            InitializeComponent();
            BUSYdownload.Visibility = Visibility.Hidden;
        }

        private OutputOption GetOutputOption()
        {
            if (isAudioOnly)
            {
                return AudioFormatSelector.SelectedValue as OutputOption;
            }
            else
            {
                return VideoOutputs.SelectedValue as OutputOption;
            }
        }

        private string GetFileFilter(OutputOption selected)
        {
            return "Video file (*." + selected.Extension + ")|*." + selected.Extension;
        }

        private bool CanStartProcess(SaveFileDialog saveFileDialog)
        {
            return saveFileDialog.ShowDialog() ?? false
               && !String.IsNullOrEmpty(urlInput.Text)
               && !String.IsNullOrEmpty(saveFileDialog.FileName);
        }

        private async void DownloadVideoURL(object sender, RoutedEventArgs e)
        {
            if (!IsValidOutput())
            {
                MessageBox.Show("Please select a quality option.");
                return;
            }
            // TODO: Get file extension properly
            OutputOption selected = GetOutputOption();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = GetFileFilter(selected);

            if (!CanStartProcess(saveFileDialog)) return;
            string arguments = "-f " + selected.FormatCode + " " + urlInput.Text +
                               " -o \"" + saveFileDialog.FileName + "\"";
            ProcessStartInfo info = GetDownloaderStartInfo(arguments);
            Process process = new Process();
            process.StartInfo = info;
            DisableUI();

            if (process.Start())
            {
                string fileName = saveFileDialog.SafeFileName;
                ProgressWindow progressWindow = CreateProgressWindow(fileName);

                // Parse process output and update progress
                await Task.Run(() =>
                {
                    new Downloader(progressWindow, process).RunDownload();
                });
            }
            else
            { 
                MessageBox.Show("There was an error starting youtube-dl.exe");
                return;
            }

            EnableUI();
        }

        private ProgressWindow CreateProgressWindow(string fileName)
        {
            string downloadLabel = "Downloading " + fileName;
            ProgressWindow progressWindow = new ProgressWindow(downloadLabel);
            progressWindow.Show();
            return progressWindow;
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
                BUSYdownload.Visibility = Visibility.Visible;
                PopulateOptions();
                BUSYdownload.Visibility = Visibility.Hidden;
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
                VideoOutputs.Items.Clear();
                AudioFormatSelector.Items.Clear();
                AddVideoOptions(processFilter);
                AddAudioOptions(processFilter);
            }
        }

        private void AddVideoOptions(ProcessFilter processFilter)
        {
            List<string[]> videoQualityList = processFilter.GetVideoOutputs();
            foreach (string[] qualityOption in videoQualityList)
            {
                VideoOutputs.Items.Add(new OutputOption(qualityOption));
            }
        }

        private void AddAudioOptions(ProcessFilter processFilter)
        {
            List<string[]> audioQualityList = processFilter.GetAudioOutputs();
            foreach (string[] qualityOption in audioQualityList)
            {
                AudioFormatSelector.Items.Add(new OutputOption(qualityOption));
            }
            AudioFormatSelector.SelectedIndex = 0;
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
            string label;
            if (hasVideoOptions)
            {
                OutputOption selectedItem = (OutputOption)VideoOutputs.SelectedItem;

                if (selectedItem != null)
                {
                    label = "Video - " + selectedItem.Extension + ", " + selectedItem.Resolution;
                    UpdateSelectedOutput(selectedItem, label);
                }
                else
                {
                    label = "";
                }
            }
        }

        private void UpdateAudioSelection()
        {
            bool hasAudioOptions = AudioFormatSelector.Items.Count > 0;
            if (hasAudioOptions)
            {
                OutputOption selectedItem = (OutputOption)AudioFormatSelector.SelectedItem;
                string label = "Audio only - " + selectedItem.Bitrate;
                UpdateSelectedOutput(selectedItem, label);
            }
        }

        private void UpdateSelectedOutput(OutputOption selectedItem, string label)
        {
            selectedOutput = selectedItem;
            SelectedOutputLabel.Text = "Output: " + label;
        }

        private bool IsValidOutput()
        {
            return selectedOutput != null;
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
    }

    public class Downloader
    {
        ProgressWindow progressWindow;
        Process process;

        public Downloader(ProgressWindow progressWindow, Process process)
        {
            this.progressWindow = progressWindow;
            this.process = process;
        }

        public void RunDownload()
        {
            string line;
            bool gotCancel = false;

            // Set up progress window cancel
            progressWindow.OnGetCancel += (s, ee) =>
            {
                gotCancel = true;
                process.Kill();
            };

            double p = 0;
            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                if (gotCancel) break;

                // TODO: Bad parsing but it works for now
                string[] segments = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 2 && segments[1].Contains('%'))
                {
                    string percentstr = segments[1].TrimEnd('%');
                    double percentdbl;
                    if (double.TryParse(percentstr
                      , out percentdbl))
                        p = percentdbl;

                }
                if (progressWindow.progress != null)
                    progressWindow.progress.Report(p);

                Console.WriteLine("YOUTUBE-DL: " + line);
            }

            while ((line = process.StandardError.ReadLine()) != null)
            {
                Console.WriteLine("YOUTUBE-DL: " + line);
            }

            process.WaitForExit();
            process.Close();
            progressWindow.Complete();
        }
    }

    public class OutputOption
    {
        // Binder for XAML
        public string FormatCode { get; set; }
        public string Extension { get; set; }
        public string Resolution { get; set; }
        public string Bitrate { get; set; }

        public OutputOption(string[] qualityOption)
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

        public List<string[]> GetAudioOutputs()
        {
            List<string[]> filtered = new List<string[]>();
            foreach (string outputLine in output)
            {
                if (IsValidAudio(outputLine))
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
