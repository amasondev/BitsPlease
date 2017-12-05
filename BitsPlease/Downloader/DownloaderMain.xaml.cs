﻿using System;
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

        StringDictionary Outputs = new StringDictionary();
        string FormatCode; // This should get checked for validity if the URL changes.
        string PathToDownloadTo = "C:\\Users\\Alex\\Desktop";

        public DownloaderMain()
        {
            urlInputTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(URLINPUT_WAIT_TIME) };
            urlInputTimer.Tick += OnURLInputTimerComplete;
            InitializeComponent();
            BUSYdownload.Visibility = Visibility.Hidden;
        }

        private async void DownloadVideoURL(object sender, RoutedEventArgs e)
        {
            // TODO: Get file extension properly
            string ext = System.IO.Path.GetExtension(FormatCode);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Video file (*" + ext + ")|*" + ext;

            if (saveFileDialog.ShowDialog() ?? false
               && !String.IsNullOrEmpty(urlInput.Text)
               && !String.IsNullOrEmpty(saveFileDialog.FileName))
            {
                ProcessStartInfo info = GetDownloaderStartInfo(urlInput.Text + " -o \"" + saveFileDialog.FileName + "\"");

                // Begin downloading
                // Disable controls
                this.IsEnabled = false;
                Process process = new Process();
                process.StartInfo = info;

                // Start process
                if (!process.Start())
                {
                    MessageBox.Show("There was an error starting youtube-dl.exe");
                    return;
                }

                // Create a progress window
                ProgressWindow progressWindow = new ProgressWindow("Downloading " + saveFileDialog.SafeFileName);
                progressWindow.Show();

                // Parse process output and update progress
                await Task.Run(() =>
                {
                    string line;
                    double p = 0;
                    while ((line = process.StandardOutput.ReadLine()) != null)
                    {
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
                    }

                    Console.WriteLine("YOUTUBE-DL: " + line);
                });

                process.WaitForExit();
                process.Close();
                progressWindow.Complete();
                this.IsEnabled = true;
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

        private ProcessFilter GetProcessFilter()
        {
            string query = "-F " + urlInput.Text;
            ProcessStartInfo info = GetDownloaderStartInfo(query);
            return new ProcessFilter(info);
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
        }

        private void DisableVideoQuality()
        {
            // Video quality window gets blanked out.
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
            if (string.IsNullOrEmpty(urlInput.Text)) return;

            // Enable busy throbber
            BUSYdownload.Visibility = Visibility.Visible;
            refreshOptions();
            // Hide busy throbber
            BUSYdownload.Visibility = Visibility.Hidden;
        }

        private void refreshOptions()
        {
            ProcessFilter processFilter = GetProcessFilter();
            VideoOutputs.Items.Clear();
            AudioFormatSelector.Items.Clear();
            addVideoOptions(processFilter);
            addAudioOptions(processFilter);
        }

        private void addVideoOptions(ProcessFilter processFilter)
        {
            List<string[]> videoQualityList = processFilter.GetVideoOutputs();
            foreach (string[] qualityOption in videoQualityList)
            {
                VideoOutputs.Items.Add(new VideoOption(qualityOption));
            }
        }

        private void addAudioOptions(ProcessFilter processFilter)
        {
            List<string[]> audioQualityList = processFilter.GetAudioOutputs();
            foreach (string[] qualityOption in audioQualityList)
            {
                AudioFormatSelector.Items.Add(new AudioOption(qualityOption));
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
            bool IsAudioOnly = AudioOnlyBox.IsChecked ?? false;
            if (!IsAudioOnly)
            {
                UpdateVideoSelection();
            }
        }

        private void AudioFormatSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool IsAudioOnly = AudioOnlyBox.IsChecked ?? false;
            if (IsAudioOnly)
            {
                UpdateAudioSelection();
            }
        }

        private void UpdateVideoSelection()
        {
            VideoOption selectedItem = (VideoOption)VideoOutputs.SelectedItem;
            string selectedFormat = selectedItem.FormatCode;
            string label = selectedItem.Extension + ", " + selectedItem.Resolution;
            UpdateSelectedOutput(selectedFormat, label);
        }

        private void UpdateAudioSelection()
        {
            AudioOption selectedItem = (AudioOption)AudioFormatSelector.SelectedItem;
            string selectedFormat = selectedItem.FormatCode;
            string label = selectedItem.Bitrate;
            UpdateSelectedOutput(selectedFormat, label);
        }

        private void UpdateSelectedOutput(string formatCode, string label)
        {
            FormatCode = formatCode;
            SelectedOutputLabel.Text = "Output: " + label;
        }
    }

    public class VideoOption
    {
        // Binder for VideoOutputs
        public string FormatCode { get; set; }
        public string Extension { get; set; }
        public string Resolution { get; set; }

        public VideoOption(string[] qualityOption)
        {
            FormatCode = qualityOption[0];
            Extension = qualityOption[1];
            Resolution = qualityOption[2];
        }
    }

    public class AudioOption
    {
        // Binder for AudioFormatSelector
        public string FormatCode { get; set; }
        public string Extension { get; set; }
        public string Bitrate { get; set; }

        public AudioOption(string[] qualityOption)
        {
            FormatCode = qualityOption[0];
            Extension = qualityOption[1];
            Bitrate = qualityOption[qualityOption.Length - 2];
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
                    string[] formattedLine = FilterVideo(outputLine);
                    filtered.Add(formattedLine);
                }
            }
            return filtered;
        }

        public List<string[]> GetAudioOutputs()
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

        private string[] FilterVideo(string outputLine)
        {
            /* yt-dl outputs a line such as: 
             * 43           webm       640x360    medium , vp8.0, vorbis@128k
             * This retrieves format code, extension, and resolution 
             */

            string[] dividedLine = outputLine.Split(',');
            string firstEntry = dividedLine[0]; // 43           webm       640x360    medium 
            return firstEntry.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // [43, webm, 640x360, medium]
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
