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
    string filelabelprefix, inputFilePath;


    public SlicerMain()
    {
      InitializeComponent();
      filelabelprefix = LBL_File.Content.ToString();
    }


    protected override void OnDropVideo(string filepath)
    {
      LBL_File.Content = filelabelprefix + filepath;

      inputFilePath = filepath;
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



























      /* if (!String.IsNullOrEmpty(start.Text) && (!String.IsNullOrEmpty(end.Text)))
      {

          ss = start.Text;
          t = end.Text;

          Process process = new Process();
          process.StartInfo.FileName = "ffmpeg.exe";
          process.StartInfo.UseShellExecute = false;
          process.StartInfo.RedirectStandardOutput = true;
          process.StartInfo.CreateNoWindow = true;
          process.StartInfo.Arguments = "-y -i \""+inputFilePath+"\" -ss "+ ss +" -t "+ t +" OutputVideoFile.mp4";
          Console.WriteLine("COMMAND: ffmpeg " + process.StartInfo.Arguments);
          process.Start();

          StreamReader reader = process.StandardOutput;
          string output = reader.ReadToEnd();
          Console.WriteLine(output);
          process.WaitForExit();
          process.Close();
      }*/
    }


  }
}
