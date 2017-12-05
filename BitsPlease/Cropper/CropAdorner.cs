using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Cropper
{
    public class CropAdorner : Adorner
    {
        private Thumb topLeft, top, topRight, right, bottomRight, bottom, bottomLeft, left;
        private VisualCollection visualChildren;

        public CropAdorner(UIElement adornedElement) : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            Rect adornedElementRect = new Rect(AdornedElement.RenderSize);

            Pen borderPen = new Pen(new SolidColorBrush(Colors.Black), 3.0);

            drawingContext.DrawRectangle(new SolidColorBrush(Colors.Transparent), borderPen, adornedElementRect);
        }

        private void BuildAdornerHandles()
        {
            // Top left
            topLeft = new Thumb();
            topLeft.Cursor = Cursors.SizeNWSE;
            topLeft.Width = topLeft.Height = 10.0;
            topLeft.Background = new SolidColorBrush(Colors.DarkGray);
            topLeft.BorderBrush = new SolidColorBrush(Colors.Gray);
            topLeft.BorderThickness = new Thickness(1.0);
            topLeft.DragDelta += TopLeftDrag;
            visualChildren.Add(topLeft);
        }

        private void TopLeftDrag(object sender, DragDeltaEventArgs e)
        {

        }

        protected override int VisualChildrenCount {get { return visualChildren.Count;}}
        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
        }
    }
}
