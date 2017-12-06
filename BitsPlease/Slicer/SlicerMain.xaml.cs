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
                Console.WriteLine("Output file: " + saveFileDialog.FileName);
                VideoOperations.PerformTrim(
                    this,
                    inputFilePath,
                    saveFileDialog.FileName,
                    start.Text, end.Text);
            }


        }

        private void BTN_Play_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPreview.HasVideo)
            {
                VideoPreview.Play();
                Console.WriteLine("Playing video.");
            }
        }

        private void BTN_Pause_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPreview.HasVideo && VideoPreview.IsPlaying)
            {
                VideoPreview.Pause();
                Console.WriteLine("Pausing video.");
            }
        }
    }
}
