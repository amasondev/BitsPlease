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
using System.IO;
using BitsPlease;

namespace Slicer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : VideoDropWindow
    {
        public string FileLocation {get; set;}

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Trim(object sender, RoutedEventArgs e)
        {
            String i, ss, t; //i = path ss= start point t = duration

            if (!String.IsNullOrEmpty(start.Text) && (!String.IsNullOrEmpty(end.Text)))
            {
                i = "C:\\Users\\goshe\\Desktop\\IMG_0163.mp4";
                ss = start.Text;
                t = end.Text;

                Process process = new Process();
                process.StartInfo.FileName = "ffmpeg.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Arguments = "-y -i \""+FileLocation+"\" -ss "+ ss +" -t "+ t +" OutputVideoFile.mp4";
                Console.WriteLine("COMMAND: ffmpeg " + process.StartInfo.Arguments);
                process.Start();

                StreamReader reader = process.StandardOutput;
                string output = reader.ReadToEnd();
                Console.WriteLine(output);
                process.WaitForExit();
                process.Close();
            }
        }

        protected override void OnDropVideo(string filepath)
        {
              Console.WriteLine("Got video at location: " + filepath);
              FileLocation = filepath;
        }
  }
}
