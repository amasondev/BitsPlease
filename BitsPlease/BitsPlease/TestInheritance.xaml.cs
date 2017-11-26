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
using System.Windows.Shapes;

namespace BitsPlease
{
  /// <summary>
  /// Interaction logic for TestInheritance.xaml
  /// </summary>
  public partial class TestInheritance : VideoDropWindow
  {
    public TestInheritance()
    {
      InitializeComponent();
    }

    protected override void OnDropVideo(string filepath)
    {
      MessageBox.Show("Got a valid video!");
    }

  }
}
