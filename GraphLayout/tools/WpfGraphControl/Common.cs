using System.Windows;
using System.Windows.Media;
using Point = Microsoft.Msagl.Core.Geometry.Point;

namespace Microsoft.Msagl.WpfGraphControl {
    using ICurve =Microsoft.Msagl.Core.Geometry.Curves.ICurve;

    /// <summary>
    /// WPF public helper functions
    /// </summary>
    public class Common {        
        internal static System.Windows.Point WpfPoint(Point p) {
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

    }
}