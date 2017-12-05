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
using System.Windows.Documents;

namespace Cropper
{
    /// <summary>
    /// Interaction logic for CropperMain.xaml
    /// </summary>
    public partial class CropperMain : VideoDropWindow
    {
        string filelabelprefix;

        string inputFilePath;

        public CropperMain()
        {
            InitializeComponent();

            Unosquare.FFME.MediaElement.FFmpegDirectory = ".";

            filelabelprefix = LBL_File.Content.ToString();


        }

        protected override void OnDropVideo(string filepath)
        {
            LBL_File.Content = filelabelprefix + filepath;

            inputFilePath = filepath;

            // Open the media in video preview
            try
            {
                VideoPreview.Source = new Uri(filepath);

                Console.WriteLine("Set media source to: " + VideoPreview.Source);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open media: " + ex.Message);
                inputFilePath = "";
                LBL_File.Content = filelabelprefix;
            }
        }

        private void Crop_OnClick(object sender, RoutedEventArgs e)
        {
            // Make sure user used valid dimensions
            // TODO: Use regex on textbox event
            int x;
            int y;
            int width;
            int height;
            if (!int.TryParse(TB_X.Text, out x)) return;
            if (!int.TryParse(TB_Y.Text, out y)) return;
            if (!int.TryParse(TB_Width.Text, out width)) return;
            if (!int.TryParse(TB_Height.Text, out height)) return;


            string ext = Path.GetExtension(inputFilePath);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Video file (*" + ext + ")|*" + ext;

            Console.WriteLine("Input file: " + inputFilePath);

            if (saveFileDialog.ShowDialog() == true
              && !string.IsNullOrEmpty(inputFilePath)
              && !string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                Console.WriteLine("Output file: " + saveFileDialog.FileName);
                VideoOperations.PerformCrop(
                  this,
                  inputFilePath,
                  saveFileDialog.FileName,
                  (uint)x, (uint)y, (uint)width, (uint)height);

            }

        }

        private void OnPlayClicked(object sender, RoutedEventArgs e)
        {
            if (VideoPreview.HasVideo)
            {
                VideoPreview.Play();
                Console.WriteLine("Playing video.");
            }
        }

        private void OnPauseClicked(object sender, RoutedEventArgs e)
        {
            if (VideoPreview.IsPlaying)
            {
                VideoPreview.Pause();
                Console.WriteLine("Pausing video.");
            }
        }

        private void OnMessageLogged(object sender, Unosquare.FFME.MediaLogMessagEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private void VideoDropWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Resize crop controls to fit video aspect
            ResizeCropControls();
        }

        private void ResizeCropControls()
        {
            if (!VideoPreview.HasVideo) return;
            double vidRatio = (double)VideoPreview.NaturalVideoHeight / (double)VideoPreview.NaturalVideoWidth;
            double prevRatio = VideoPreview.ActualHeight / VideoPreview.ActualWidth;


            if (prevRatio > vidRatio)
            {
                double newHeight = VideoPreview.ActualHeight * (vidRatio / prevRatio);
                double margin = (VideoPreview.ActualHeight - newHeight) / 2.0;
                CROPCONTROL.Margin = new Thickness(0, margin, 0, margin);
            }
            else
            {
                double newWidth = VideoPreview.ActualWidth / (vidRatio / prevRatio);
                double margin = (VideoPreview.ActualWidth - newWidth) / 2.0;
                CROPCONTROL.Margin = new Thickness(margin, 0, margin, 0);
            }

        }

        private void VideoPreview_MediaOpened(object sender, RoutedEventArgs e)
        {
            ResizeCropControls();
        }

        private void CROPCONTROL_Loaded(object sender, RoutedEventArgs e)
        {
            //AdornerLayer cropLayer = AdornerLayer.GetAdornerLayer(CROPCONTROL);
            //cropLayer.Add(new CropAdorner(CROPCONTROL));
        }
    }

}
