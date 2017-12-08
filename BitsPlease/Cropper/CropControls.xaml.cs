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
        GridMarginHandler marginHandler;

        public CropControls()
        {
            InitializeComponent();
            marginHandler = new GridMarginHandler(GRID_Crop);
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
            double width = checkMinimum(size.Width);
            double height = checkMinimum(size.Height);

            // Rectangle for top, left, bottom, right
            // Top
            RectangleGeometry top = new RectangleGeometry(new Rect(
                0, 0, width, GRID_Crop.Margin.Top));
            // Left
            RectangleGeometry left = new RectangleGeometry(new Rect(
                0, 0, GRID_Crop.Margin.Left, height));
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

            if (CropModified != null)
                CropModified();
        }

        private double checkMinimum(double side)
        {
            if (side > 0)
            {
                return side;
            }
            else
            {
                return 1;
            }
        }

        private void DragMove(object sender, DragDeltaEventArgs e)
        {
            marginHandler.SetX(e.HorizontalChange);
            marginHandler.SetY(e.VerticalChange);
            marginHandler.SetMargins();
            UpdateBackdropMask();
        }

        private void THUMB_topleft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            marginHandler.SetTop(e.VerticalChange);
            marginHandler.SetLeft(e.HorizontalChange);
            UpdateBackdropMask();
        }

        private void Left_DragDelta(object sender, DragDeltaEventArgs e)
        {
            marginHandler.SetLeft(e.HorizontalChange);
            UpdateBackdropMask();
        }

        private void THUMB_bottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            marginHandler.SetBottom(e.VerticalChange);
            marginHandler.SetLeft(e.HorizontalChange);
            UpdateBackdropMask();
        }

        private void Bottom_DragDelta(object sender, DragDeltaEventArgs e)
        {
            marginHandler.SetBottom(e.VerticalChange);
            UpdateBackdropMask();
        }

        private void THUMB_bottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            marginHandler.SetBottom(e.VerticalChange);
            marginHandler.SetRight(e.HorizontalChange);
            UpdateBackdropMask();
        }

        private void Right_DragDelta(object sender, DragDeltaEventArgs e)
        {
            marginHandler.SetRight(e.HorizontalChange);
            UpdateBackdropMask();
        }

        private void THUMB_topRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            marginHandler.SetTop(e.VerticalChange);
            marginHandler.SetRight(e.HorizontalChange);
            UpdateBackdropMask();
        }

        private void Top_DragDelta(object sender, DragDeltaEventArgs e)
        {
            marginHandler.SetTop(e.VerticalChange);
            marginHandler.SetMargins();
            UpdateBackdropMask();
        }
    }

    public class GridMarginHandler
    {
        double left;
        double right;
        double top;
        double bottom;
        double minimumSize = 20;
        Grid GRID_Crop;

        public GridMarginHandler(Grid GRID_Crop)
        {
            this.GRID_Crop = GRID_Crop;
            left = GRID_Crop.Margin.Left;
            right = GRID_Crop.Margin.Right;
            top = GRID_Crop.Margin.Top;
            bottom = GRID_Crop.Margin.Bottom;
        }

        public void ResetMargins()
        {
            left = 10;
            right = 10;
            top = 10;
            bottom = 10;
            SetMargins();
        }

        private bool IsMinimumWidth()
        {
            return GRID_Crop.ActualWidth >= minimumSize;
        }

        private bool IsMinimumHeight()
        {
            return GRID_Crop.ActualHeight >= minimumSize;
        }

        public void SetLeft(double horizontalChange)
        {
            double desiredLeft = left + horizontalChange;
            if (IsMinimumWidth() || desiredLeft < left)
            {
                left = desiredLeft;
                SetMargins();
            }
        }

        public void SetTop(double verticalChange)
        {
            double desiredTop = top + verticalChange;
            if (IsMinimumHeight() || desiredTop < top)
            {
                top = desiredTop;
                SetMargins();
            }
        }

        public void SetRight(double horizontalChange)
        {
            double desiredRight = right - horizontalChange;
            if (IsMinimumWidth() || desiredRight < right)
            {
                right = desiredRight;
                SetMargins();
            }
        }

        public void SetBottom(double verticalChange)
        {
            double desiredBottom = bottom - verticalChange;
            if (IsMinimumHeight() || desiredBottom < bottom)
            {
                bottom = desiredBottom;
                SetMargins();
            }
        }

        public void SetX(double horizontalChange)
        {
            double desiredLeft = left + horizontalChange;
            double desiredRight = right - horizontalChange;
            if (desiredLeft >= 0 && desiredRight >= 0)
            {
                left = desiredLeft;
                right = desiredRight;
            }
        }

        public void SetY(double verticalChange)
        {
            double desiredTop = top + verticalChange;
            double desiredBottom = bottom - verticalChange;
            if (desiredTop >= 0 && desiredBottom >= 0)
            {
                top = desiredTop;
                bottom = desiredBottom;
            }
        }

        public void CheckMargins()
        {
            if (left < 0)
            {
                left = 0;
            }
            if (right < 0)
            {
                right = 0;
            }
            if (top < 0)
            {
                top = 0;
            }
            if (bottom < 0)
            {
                bottom = 0;
            }
        }

        public void SetMargins()
        {
            CheckMargins();
            GRID_Crop.Margin = new Thickness(left, top, right, bottom);
        }
    }
}
