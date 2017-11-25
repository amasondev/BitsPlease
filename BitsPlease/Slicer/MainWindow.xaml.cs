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

namespace Slicer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

                Process.Start("ffmpeg.exe", "-i IMG_0163.mp4"+"-"+ ss +"-"+ t +"OutputVideoFile.mp4");
            }
        }

       
    }
}
