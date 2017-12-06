using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cropper
{
    /// <summary>
    /// Interaction logic for CropControls.xaml
    /// </summary>
    public partial class CropControls : UserControl
    {
        public delegate void CropModifiedEventHandler();
        public event CropModifiedEventHandler CropModified;

        public CropControls()
        {
            InitializeComponent();

        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // XAML designer bug without trycatch block
            try
            {
                // On resize, create a clip mask for the backdrop grid
                UpdateBackdropMask();
            } catch (Exception ex)
            {
                return;
            }
        }

        public Rect GetVideoCropDimensions(int videoWidth, int videoHeight)
        {
            double left = (GRID_Crop.Margin.Left / ActualWidth) * (double)videoWidth;
            double top = (GRID_Crop.Margin.Top / ActualHeight) * (double)videoHeight;
            double width = (GRID_Crop.ActualWidth / ActualWidth) * (double)videoWidth;
            double height = (GRID_Crop.ActualHeight / ActualHeight) * (double)videoHeight;

            return new Rect(left, top, width, height);
        }

        public void SetVideoCropDimensions(int videoWidth, int videoHeight, Rect rect)
        {

        }

        private void UpdateBackdropMask()
        {
            if (GRID_Backdrop == null || GRID_Crop == null) return;

            Size size = new Size(ActualWidth, ActualHeight);

            // Rectangle for top, left, bottom, right
            // Top
            RectangleGeometry top = new RectangleGeometry(new Rect(
                0, 0, size.Width, GRID_Crop.Margin.Top));
            // Left
            RectangleGeometry left = new RectangleGeometry(new Rect(
                0, 0, GRID_Crop.Margin.Left, size.Height));
            // Bottom
            RectangleGeometry bottom = new RectangleGeometry(new Rect(
                0, size.Height - GRID_Crop.Margin.Bottom, size.Width, GRID_Crop.Margin.Bottom));
            // Right
            RectangleGeometry right = new RectangleGeometry(new Rect(
                size.Width - GRID_Crop.Margin.Right, 0, GRID_Crop.Margin.Right, size.Height));


            Geometry backdropMask = Geometry.Combine(top, left, GeometryCombineMode.Union, null);
            backdropMask = Geometry.Combine(backdropMask, bottom, GeometryCombineMode.Union, null);
            backdropMask = Geometry.Combine(backdropMask, right, GeometryCombineMode.Union, null);

            GRID_Backdrop.Clip = backdropMask;

            CropModified();
        }

        private void DragMove(object sender, DragDeltaEventArgs e)
        {
            double newLeft = GRID_Crop.Margin.Left;
            double newRight = GRID_Crop.Margin.Right;
            double newTop = GRID_Crop.Margin.Top;
            double newBottom = GRID_Crop.Margin.Bottom;

            double desiredLeft = newLeft + e.HorizontalChange;
            double desiredRight = newRight - e.HorizontalChange;
            double desiredTop = newTop + e.VerticalChange;
            double desiredBottom = newBottom - e.VerticalChange;

            if (desiredLeft >= 0 && desiredRight >= 0)
            {
                newLeft = desiredLeft;
                newRight = desiredRight;
            }

            if (desiredTop >= 0 && desiredBottom >= 0)
            {
                newTop += e.VerticalChange;
                newBottom -= e.VerticalChange;
            }

            GRID_Crop.Margin = new Thickness(newLeft, newTop, newRight, newBottom);

            UpdateBackdropMask();
        }

        private void THUMB_topleft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newLeft = GRID_Crop.Margin.Left;
            double newRight = GRID_Crop.Margin.Right;
            double newTop = GRID_Crop.Margin.Top;
            double newBottom = GRID_Crop.Margin.Bottom;

            double desiredLeft = newLeft + e.HorizontalChange;
            double desiredTop = newTop + e.VerticalChange;

            if (desiredLeft >= 0) newLeft = desiredLeft;
            if (desiredTop >= 0) newTop = desiredTop;

            GRID_Crop.Margin = new Thickness(newLeft, newTop, newRight, newBottom);

            UpdateBackdropMask();
        }

        private void Left_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newLeft = GRID_Crop.Margin.Left;
            double newRight = GRID_Crop.Margin.Right;
            double newTop = GRID_Crop.Margin.Top;
            double newBottom = GRID_Crop.Margin.Bottom;

            double desiredLeft = newLeft + e.HorizontalChange;

            if (desiredLeft >= 0) newLeft = desiredLeft;

            GRID_Crop.Margin = new Thickness(newLeft, newTop, newRight, newBottom);

            UpdateBackdropMask();
        }

        private void THUMB_bottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newLeft = GRID_Crop.Margin.Left;
            double newRight = GRID_Crop.Margin.Right;
            double newTop = GRID_Crop.Margin.Top;
            double newBottom = GRID_Crop.Margin.Bottom;

            double desiredLeft = newLeft + e.HorizontalChange;
            double desiredBottom = newBottom - e.VerticalChange;

            if (desiredLeft >= 0) newLeft = desiredLeft;
            if (desiredBottom >= 0) newBottom = desiredBottom;

            GRID_Crop.Margin = new Thickness(newLeft, newTop, newRight, newBottom);

            UpdateBackdropMask();
        }

        private void Bottom_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newLeft = GRID_Crop.Margin.Left;
            double newRight = GRID_Crop.Margin.Right;
            double newTop = GRID_Crop.Margin.Top;
            double newBottom = GRID_Crop.Margin.Bottom;
            
            double desiredBottom = newBottom - e.VerticalChange;
            
            if (desiredBottom >= 0) newBottom = desiredBottom;

            GRID_Crop.Margin = new Thickness(newLeft, newTop, newRight, newBottom);

            UpdateBackdropMask();
        }

        private void THUMB_bottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newLeft = GRID_Crop.Margin.Left;
            double newRight = GRID_Crop.Margin.Right;
            double newTop = GRID_Crop.Margin.Top;
            double newBottom = GRID_Crop.Margin.Bottom;

            double desiredRight = newRight - e.HorizontalChange;
            double desiredBottom = newBottom - e.VerticalChange;

            if (desiredRight >= 0) newRight = desiredRight;
            if (desiredBottom >= 0) newBottom = desiredBottom;

            GRID_Crop.Margin = new Thickness(newLeft, newTop, newRight, newBottom);

            UpdateBackdropMask();
        }

        private void Right_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newLeft = GRID_Crop.Margin.Left;
            double newRight = GRID_Crop.Margin.Right;
            double newTop = GRID_Crop.Margin.Top;
            double newBottom = GRID_Crop.Margin.Bottom;

            double desiredRight = newRight - e.HorizontalChange;

            if (desiredRight >= 0) newRight = desiredRight;

            GRID_Crop.Margin = new Thickness(newLeft, newTop, newRight, newBottom);

            UpdateBackdropMask();
        }

        private void THUMB_topRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newLeft = GRID_Crop.Margin.Left;
            double newRight = GRID_Crop.Margin.Right;
            double newTop = GRID_Crop.Margin.Top;
            double newBottom = GRID_Crop.Margin.Bottom;

            double desiredRight = newRight - e.HorizontalChange;
            double desiredTop = newTop + e.VerticalChange;

            if (desiredRight >= 0) newRight = desiredRight;
            if (desiredTop >= 0) newTop = desiredTop;

            GRID_Crop.Margin = new Thickness(newLeft, newTop, newRight, newBottom);

            UpdateBackdropMask();
        }

        private void Top_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newLeft = GRID_Crop.Margin.Left;
            double newRight = GRID_Crop.Margin.Right;
            double newTop = GRID_Crop.Margin.Top;
            double newBottom = GRID_Crop.Margin.Bottom;
            
            double desiredTop = newTop + e.VerticalChange;
            
            if (desiredTop >= 0) newTop = desiredTop;

            GRID_Crop.Margin = new Thickness(newLeft, newTop, newRight, newBottom);

            UpdateBackdropMask();
        }
    }
}
