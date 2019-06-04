using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Point = Microsoft.Msagl.Core.Geometry.Point;
using Panel = System.Windows.Controls.Panel;

namespace Microsoft.Msagl.WpfGraphControl {
    using ICurve =Microsoft.Msagl.Core.Geometry.Curves.ICurve;
    using WPoint = System.Windows.Point;
    /// <summary>
    /// WPF public helper functions
    /// </summary>
    public class Common {
        
        public static System.Windows.Point WpfPoint(Point p) {
            return new System.Windows.Point(p.X, p.Y);
        }

        internal static Point MsaglPoint(System.Windows.Point p) {
            return new Point(p.X, p.Y);
        }


        public static Brush BrushFromMsaglColor(Microsoft.Msagl.Drawing.Color color) {
            Color avalonColor = new Color {A = color.A, B = color.B, G = color.G, R = color.R};
            return new SolidColorBrush(avalonColor);
        }

        public static Brush BrushFromMsaglColor(byte colorA, byte colorR, byte colorG, byte colorB) {
            Color avalonColor = new Color {A = colorA, R = colorR, G = colorG, B = colorB};
            return new SolidColorBrush(avalonColor);
        }




        internal static void PositionFrameworkElement(FrameworkElement frameworkElement, Point center, double scale) {
            PositionFrameworkElement(frameworkElement, center.X, center.Y, scale);
        }

        static void PositionFrameworkElement(FrameworkElement frameworkElement, double x, double y, double scale) {
            if (frameworkElement == null)
                return;
            frameworkElement.RenderTransform =
                new MatrixTransform(new Matrix(scale, 0, 0, -scale, x - scale*frameworkElement.Width/2,
                    y + scale*frameworkElement.Height/2));
        }

        public static Geometry GetICurveWpfGeometry(ICurve curve) {
            var streamGeometry = new StreamGeometry();
            using (StreamGeometryContext context = streamGeometry.Open()) {
                FillStreamGeometryContext(context, curve);
                return streamGeometry;
            }
        }
        static void FillStreamGeometryContext(StreamGeometryContext context, ICurve curve) {
            if (curve == null)
                return;
            VEdge.FillContextForICurve(context, curve);
        }
        /// <summary>
        ///  direct set pos to element center point
        /// </summary>
        /// <param name="fe"></param>
        /// <param name="pos"></param>
        public static void SetFrameworkElementCenter(FrameworkElement fe,WPoint pos) {
            var w = fe.Width;
            var h = fe.Height;
            // _graphCanvas has a cordinate, which y axis is reverted
            Canvas.SetLeft(fe, pos.X-w/2);
            Canvas.SetTop(fe, pos.Y + h/2);
        }

        public static Size MeasureText(string text, FontFamily family, double size) {
            FormattedText formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(family, new System.Windows.FontStyle(), FontWeights.Regular, FontStretches.Normal),
                size,
                Brushes.Black,
                null);

            return new Size(formattedText.Width, formattedText.Height);
        }
        public static Size MeasureLabel(Drawing.Label label) {
            return MeasureText(label.Text, new FontFamily(label.FontName), label.FontSize);
        }
    }
}