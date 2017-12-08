using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BitsPlease;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace Slicer
{
    /// <summary>
    /// Interaction logic for SlicerMain.xaml
    /// </summary>
    public partial class SlicerMain : VideoDropWindow
    {
        //public string FileLocation {get; set;}
        const string titlePrefix = "Slicer";
        string inputFilePath;
        bool wasPlaying = false;

        public SlicerMain()
        {
            Unosquare.FFME.MediaElement.FFmpegDirectory = ".";
            InitializeComponent();
            Title = titlePrefix;
        }

        protected override void OnDropVideo(string filepath)
        {
            // Open the media in video preview
            try
            {
                VideoPreview.Source = new Uri(filepath);
                inputFilePath = filepath;
                this.Title = titlePrefix + ": " + Path.GetFileName(filepath);
                GetStartedLabel.Visibility = Visibility.Hidden;
                Console.WriteLine("Set media source to: " + VideoPreview.Source);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open media: " + ex.Message);
                inputFilePath = "";
                this.Title = titlePrefix;
            }
        }

        private void VideoPreview_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (VideoPreview.HasVideo)
            {
                TB_Start.Text = GetTimecode(Timeline.LowerValue, VideoPreview.NaturalDuration.TimeSpan);
                TB_End.Text = GetTimecode(Timeline.UpperValue, VideoPreview.NaturalDuration.TimeSpan);
                UpdateDuration();
            }
        }

        private void OnMessageLogged(object sender, Unosquare.FFME.MediaLogMessagEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private void Trim(object sender, RoutedEventArgs e)
        {

            string ext = Path.GetExtension(inputFilePath);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Video file (*" + ext + ")|*" + ext;

            Console.WriteLine("Input file: " + inputFilePath);

            if (saveFileDialog.ShowDialog() == true
              && !string.IsNullOrEmpty(inputFilePath)
              && !string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                string start = TB_Start.Text;
                string duration = GetTimecode(Timeline.UpperValue - Timeline.LowerValue, VideoPreview.NaturalDuration.TimeSpan);

                Console.WriteLine("Output file: " + saveFileDialog.FileName);
                VideoOperations.PerformTrim(
                    this,
                    inputFilePath,
                    saveFileDialog.FileName,
                    start, duration);
            }
        }

        private void BTN_Play_Click(object sender, RoutedEventArgs e)
        {
            wasPlaying = true;
            PlayVideo();
        }

        private void BTN_Pause_Click(object sender, RoutedEventArgs e)
        {
            wasPlaying = false;
            PauseVideo();
            PausingIconColour();
        }

        private void PlayVideo()
        {
            if (VideoPreview.HasVideo)
            {
                PlayingIconColour();
                VideoPreview.Play();
                Console.WriteLine("Playing video.");
            }
        }

        private void PlayingIconColour()
        {
            PlayIcon.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF17D94E"));
        }

        private void PausingIconColour()
        {
            PlayIcon.Fill = new SolidColorBrush(Color.FromRgb(0,0,0));
        }

        private void PauseVideo()
        {
            if (VideoPreview.HasVideo && VideoPreview.IsPlaying)
            {
                VideoPreview.Pause();
                Console.WriteLine("Pausing video.");
            }
        }

        private void SlicerTimeline_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (VideoPreview.HasVideo)
            {
                TB_Start.Text = GetTimecode(Timeline.LowerValue, VideoPreview.NaturalDuration.TimeSpan);
                TB_End.Text = GetTimecode(Timeline.UpperValue, VideoPreview.NaturalDuration.TimeSpan);
                UpdateDuration();
            }
        }

        private string GetTimecode(double timeValue, TimeSpan timeSpan)
        {
            string ret = "00:00:00";
            if (timeSpan != null)
            {
                Duration adjusted = new Duration(new TimeSpan((long)(timeSpan.Ticks * timeValue)));
                ret = adjusted.TimeSpan.Hours.ToString("D2");
                ret += ":" + adjusted.TimeSpan.Minutes.ToString("D2");
                ret += ":" + adjusted.TimeSpan.Seconds.ToString();
                ret += "." + adjusted.TimeSpan.Milliseconds.ToString("D");

            }

            return ret;
        }

        private double GetTimeValue(TimeSpan current, TimeSpan total)
        {
            return (double)current.Ticks / (double)total.Ticks;
        }

        private void VideoPreview_RenderingVideo(object sender, Unosquare.FFME.RenderingVideoEventArgs e)
        {
            // TODO: Better math on this? Longs to double
            double playpoint = (double)VideoPreview.Position.Ticks / (double)VideoPreview.NaturalDuration.TimeSpan.Ticks;
            Timeline.Playhead = playpoint;
        }

        private void Timeline_PlayheadDragging(object sender, RoutedEventArgs e)
        {
            if (VideoPreview.HasVideo && VideoPreview.IsPlaying)
            {
                TempPauseLabel.Visibility = Visibility.Visible;
                PauseVideo();
            }
        }

        private void Timeline_PlayheadMoved(object sender, RoutedEventArgs e)
        {
            if (VideoPreview.HasVideo)
            {
                // TODO: Better math on this? Doubles to long
                double tick = VideoPreview.NaturalDuration.TimeSpan.Ticks * Timeline.Playhead;
                TimeSpan timeSpan = new TimeSpan((long)tick);
                VideoPreview.Position = timeSpan;
            }
            if (wasPlaying)
            {
                TempPauseLabel.Visibility = Visibility.Hidden;
                PlayVideo();
            }
        }

        private void UpdateSeekTime(object sender, RoutedEventArgs e)
        {
            if (VideoPreview.HasVideo)
            {
                double tick = VideoPreview.NaturalDuration.TimeSpan.Ticks * Timeline.Playhead;
                TimeSpan timeSpan = new TimeSpan((long)tick);
                SeekTime.Text = timeSpan.ToString();
            }
        }

        private void UpdateDuration()
        {
            double timeDifference = Timeline.UpperValue - Timeline.LowerValue;
            string timeCode = GetTimecode(timeDifference, VideoPreview.NaturalDuration.TimeSpan);
            Duration.Text = timeCode;
        }

        private void ParseTimecodeTB(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            Console.WriteLine("Parsing timecode input: " + tb.Text);
            
            string[] input = tb.Text.Split(':');
            Array.Reverse(input);

            int milliseconds = 0;
            int minutes = 0;
            int hours = 0;

            if (input.Length > 0)
            {
                double parsedSeconds;
                if (double.TryParse(input[0], out parsedSeconds))
                {
                    milliseconds = (int)(1000 * parsedSeconds);
                }
            }
            if (input.Length > 1)
            {
                double parsedMinutes;
                if (double.TryParse(input[1], out parsedMinutes))
                    minutes = (int)parsedMinutes;
            }
            if (input.Length > 2)
            {
                double parsedHours;
                if (double.TryParse(input[2], out parsedHours))
                    hours = (int)parsedHours;
            }

            TimeSpan timeSpan = new TimeSpan(0, hours, minutes, 0, milliseconds);
            
            tb.Text = GetTimecode(1.0, timeSpan);

            // Set slider
            if (VideoPreview.HasVideo)
            {
                if (tb == TB_Start) Timeline.LowerValue = GetTimeValue(timeSpan, VideoPreview.NaturalDuration.TimeSpan);
                if (tb == TB_End) Timeline.UpperValue = GetTimeValue(timeSpan, VideoPreview.NaturalDuration.TimeSpan);
            }
        }

        private void ApproveTimecodeInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex approvedChars = new Regex("[0-9:.]+");
            e.Handled = !approvedChars.IsMatch(e.Text);
        }
        
    }
}
