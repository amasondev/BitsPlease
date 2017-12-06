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

namespace Slicer
{
    /// <summary>
    /// Interaction logic for SlicerTimeline.xaml
    /// </summary>
    public partial class SlicerTimeline : UserControl
    {
        #region DependencyProperties
        public static readonly DependencyProperty MinimumProperty
        = DependencyProperty.Register("Minimum", typeof(double), typeof(SlicerTimeline), new UIPropertyMetadata(0d));
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty LowerValueProperty
        = DependencyProperty.Register("LowerValue", typeof(double), typeof(SlicerTimeline), new UIPropertyMetadata(0d));
        public double LowerValue
        {
            get { return (double)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }

        public static readonly DependencyProperty UpperValueProperty
        = DependencyProperty.Register("UpperValue", typeof(double), typeof(SlicerTimeline), new UIPropertyMetadata(1d));
        public double UpperValue
        {
            get { return (double)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty
        = DependencyProperty.Register("Maximum", typeof(double), typeof(SlicerTimeline), new UIPropertyMetadata(1d));
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public event RoutedEventHandler ValueChanged;
        public event RoutedEventHandler PlayheadMoved;


        public static readonly DependencyProperty PlayheadProperty
        = DependencyProperty.Register("Playhead", typeof(double), typeof(SlicerTimeline), new UIPropertyMetadata(0d));
        public double Playhead
        {
            get { return (double)GetValue(PlayheadProperty); }
            set { SetValue(PlayheadProperty, value); }
        }
        #endregion

        public SlicerTimeline()
        {
            InitializeComponent();
            UpdateRangeHighlight();
        }

        private void LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpperSlider.Value = Math.Max(UpperSlider.Value, LowerSlider.Value);
            UpdateRangeHighlight();
            if (ValueChanged != null)
                ValueChanged(sender, e);
        }

        private void UpperSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LowerSlider.Value = Math.Min(UpperSlider.Value, LowerSlider.Value);
            UpdateRangeHighlight();
            if (ValueChanged != null)
                ValueChanged(sender, e);
        }

        private void UpdateRangeHighlight()
        {
            Thickness margins = RangeHighlight.Margin;
            margins.Left = (ActualWidth * LowerValue) - (LowerValue * LowerSlider.Margin.Right);
            margins.Right = (ActualWidth * (Maximum - UpperValue)) - ((Maximum - UpperValue) * UpperSlider.Margin.Left);
            RangeHighlight.Margin = margins;
        }

        private void root_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateRangeHighlight();
        }

        private void PlayheadSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PlayheadMoved != null)
                PlayheadMoved(sender, e);
        }
    }
}
