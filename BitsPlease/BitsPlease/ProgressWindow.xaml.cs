using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace BitsPlease
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public IProgress<int> progress;
        Window parent;

        public ProgressWindow(Window parent)
        {
            this.parent = parent;
            InitializeComponent();

            //this.Loaded += ProgressWindow_Loaded;
            //this.Closing += ProgressWindow_Closing;

            Progress<int> progressHandler = new Progress<int>(percent =>
            {
                PROGBAR_Job.Value = (double)percent;
            });
            progress = progressHandler as IProgress<int>;
        }

    /*
        private async void ProgressWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Parse process output and update progress bar

            parent.IsEnabled = false;
            
            await Task.Run(() =>
            {
                string line;
                int p = 0;
                while ((line = process.StandardError.ReadLine()) != null)
                {
                    // TODO: Parse output and update percent
                    p++;
                    if (progress != null)
                        progress.Report(p);

                    Console.WriteLine("FFMPEG: " + line);
                }
            });

            // Complete
            parent.IsEnabled = true;
            this.Closing -= ProgressWindow_Closing;
            process.Close();
            Console.WriteLine("Task Complete.");
            Close();
        }
        */
        private void ProgressWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
