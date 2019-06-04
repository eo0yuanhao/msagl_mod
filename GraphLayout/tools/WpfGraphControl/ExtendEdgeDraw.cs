using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Drawing;
using WpfPoint = System.Windows.Point;
namespace Microsoft.Msagl.WpfGraphControl.ExtendEdgeDraw {
    class ExtendEdgeDrawDelegate {
        static public bool edge_rendering_delegate(Edge edge, object edgeLine_path) {

            Path path = edgeLine_path as Path;
            ICurve cv = edge.EdgeCurve;

            var t_middle = cv.GetParameterAtLength(cv.Length / 2);
            var dva = cv.Derivative(t_middle); //求中点的导数
            var midPoint = cv[t_middle];// Common.WpfPoint( cv[t_middle]);
            float x = (float)midPoint.X, y = (float)midPoint.Y;

            var geo2 = DataDefine.get_svg2();
            MatrixTransform mt = new MatrixTransform();
            Func<double, double> rad2Deg = v => { return v * 180 / Math.PI; };
            //mt.Matrix.
            var deg = rad2Deg(Math.Atan2(dva.Y, dva.X));
            var mat = Matrix.Identity;

            mat.TranslatePrepend(x, y);
            mat.RotatePrepend(deg);
            mat.ScalePrepend(-0.2f, 0.2f);
            mt.Matrix = mat;
            geo2.Transform = mt;

            var circle = new EllipseGeometry(new WpfPoint(x, y), 2, 2);
            PathGeometry pp = new PathGeometry();
            pp.FillRule = FillRule.EvenOdd;

            pp.AddGeometry(Common.GetICurveWpfGeometry(cv));
            pp.AddGeometry(circle);
            pp.AddGeometry(geo2);
            path.Data = pp;

            return true;
        }
    }
}
