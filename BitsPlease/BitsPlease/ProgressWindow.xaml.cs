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
        public IProgress<double> progress;
        public string TaskTitle;

        public ProgressWindow(string taskTitle)
        {
            InitializeComponent();
            TaskTitle = taskTitle;
            this.Title = taskTitle;

            //this.Loaded += ProgressWindow_Loaded;
            this.Closing += ProgressWindow_Closing;

            Progress<double> progressHandler = new Progress<double>(percent =>
            {
                PROGBAR_Job.Value = percent;

                this.Title = TaskTitle + "... " + percent.ToString() + "%";
            });
            progress = progressHandler as IProgress<double>;
        }
        private void ProgressWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        public void Complete()
        {
            this.Closing -= ProgressWindow_Closing;
            this.Close();
        }
    }
}
