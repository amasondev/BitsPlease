using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BitsPlease
{
  public class TimelineHeader : FrameworkElement
  {
    
    public Brush Background
    {
      get { return (Brush)GetValue(BackgroundProperty);}
      set { SetValue(BackgroundProperty, value);}
    }
    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register
      ("Background",
      typeof(Brush), typeof(TimelineHeader),
      new PropertyMetadata(SystemColors.ControlBrush));

    public Brush Foreground
    {
      get { return (Brush)GetValue(ForegroundProperty);}
      set { SetValue(ForegroundProperty, value);}
    }
    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
      "Foreground",
      typeof(Brush), typeof(TimelineHeader),
      new PropertyMetadata(SystemColors.ControlTextBrush));

    public double tickPixels = 50.0;
    

    protected override void OnRender(DrawingContext drawingContext)
    {
      
      base.OnRender(drawingContext);
      
      // Draw background
      drawingContext.DrawRectangle(Background, null, new Rect(new Point(0, 0), new Size(ActualWidth, ActualHeight)));

      Pen mainPen= new Pen(Foreground, 1.0);

      // Draw vertical ticks
      for(double x = 0; x <= this.Width; x++)
      {
        if (x % 50 == 0)
        {
          drawingContext.DrawLine(mainPen, new Point(x, ActualHeight / 2), new Point (x, ActualHeight / 1.25));

        }
      }
    }

  }
}
