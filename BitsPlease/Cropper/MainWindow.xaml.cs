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

namespace Cropper
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : VideoDropWindow
  {
    string filelabelprefix;

    string inputFilePath;

    public MainWindow()
    {
      InitializeComponent();

      filelabelprefix = LBL_File.Content.ToString();
    }

    protected override void OnDropVideo(string filepath)
    {
      LBL_File.Content = filelabelprefix + filepath;

      inputFilePath = filepath;
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
  }

}
