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
namespace Converter
{
    /// <summary>
    /// Interaction logic for ConverterMain.xaml
    /// </summary>
    public partial class ConverterMain : VideoDropWindow
    {
        string filelabelprefix, inputFilePath;


        public ConverterMain()
        {
            InitializeComponent();
            filelabelprefix = LBL_File.Content.ToString();
        }

        /*
        protected override void OnDropVideo(string filepath)
        {
            LBL_File.Content = filelabelprefix + filepath;

            inputFilePath = filepath;
        }
        */

        private void Convert(object sender, RoutedEventArgs e)
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
                /*
                VideoOperations.PerformConvert(
                    this,
                    inputFilePath,
                    saveFileDialog.FileName,
                    format.Text);
                    */
            }

        }
    }
}
