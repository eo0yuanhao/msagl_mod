using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

//using Svg;
namespace irmap_form {
    namespace dataDefine {
        using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;
        using System.Drawing.Drawing2D;
        class graphicsPath_data {
            private static GraphicsPath _of_s_path = null;
            public static GraphicsPath get_svg(System.Drawing.Graphics graphics) {
                if(_of_s_path != null) {
                    return _of_s_path;
                }
                string _of_s_data = "m 0,0 c 0,-8.6 0.1,-17.3 1.8,-25.8   0.6,-2.5 1.2,-5.4 3.2,-7.3   1,-1 3,0 3.9,1.0   2.5,3.2 3.1,7.5 3.3,12.5";
                string _of_s_data2 = "m 0,0 c 0,8.6 -0.1,17.3 -1.8,25.8   -0.6,2.5 -1.2,5.4 -3.2,7.3   -1,1 -3,-0 -3.9,-1.0   -2.5,-3.2 -3.1,-7.5 -3.3,-12.5";
                string _of_s_data3 = "";// "m 9,0 a 9,9 360 true,true 9,0";
                
                System.Windows.Media.GeometryGroup gg = new System.Windows.Media.GeometryGroup();
                gg.Children.Add(System.Windows.Media.Geometry.Parse(_of_s_data));
                gg.Children.Add(System.Windows.Media.Geometry.Parse(_of_s_data2));
                gg.Children.Add(System.Windows.Media.Geometry.Parse(_of_s_data3));
                System.Windows.Shapes.Path am = new System.Windows.Shapes.Path();
                am.Data = gg;
                
                Svg.SvgPath sp = new Svg.SvgPath();
                sp.PathData = Svg.SvgPathBuilder.Parse(_of_s_data + _of_s_data2 + _of_s_data3);


                Svg.ISvgRenderer render = Svg.SvgRenderer.FromGraphics(graphics);

                //var converter = TypeDescriptor.GetConverter(typeof(Svg.Pathing.SvgPathSegmentList));
                //GraphicsPath ttt = sp.Path(render);
                _of_s_path = sp.Path(render);
                return _of_s_path;
            }
            public static GraphicsPath get_svg2(System.Drawing.Graphics graphics) {
                if (_of_s_path != null)
                    return _of_s_path.Clone() as GraphicsPath;
                string _of_s_data = "m 0,0 c 0,-8.6 0.1,-17.3 1.8,-25.8   0.6,-2.5 1.2,-5.4 3.2,-7.3   1,-1 3,0 3.9,1.0   2.5,3.2 3.1,7.5 3.3,12.5";
                string _of_s_data2 = "M 0,0 c 0,8.6 -0.1,17.3 -1.8,25.8   -0.6,2.5 -1.2,5.4 -3.2,7.3   -1,1 -3,-0 -3.9,-1.0   -2.5,-3.2 -3.1,-7.5 -3.3,-12.5";

                Svg.SvgPath sp = new Svg.SvgPath();
                sp.PathData = Svg.SvgPathBuilder.Parse(_of_s_data + _of_s_data2);
                Svg.ISvgRenderer render = Svg.SvgRenderer.FromNull();
                _of_s_path = sp.Path(render);
                return _of_s_path.Clone() as GraphicsPath;
            }
        }
        //string path_data1 =  "n"
        //    "m -0.01499892, 0 "
        //    "c -0.073384,8.64324  "
        //    "-0.1660072,17.37071  "
        //    "-1.87113598,25.8766  "
        //    "                     "
        //    "-0.646778,2.58281    "
        //    "-1.212999,5.48135    "
        //    "-3.269519,7.35041    "
        //    "                     "
        //    "-1.30411,1.11743     "
        //    "-3.110085,0.16977    "
        //    "-3.979479,-1.02113   "
        //    "                     "
        //    "-2.5448461,-3.25985  "
        //    "-3.1092851,-7.56312  "
        //    "-3.3368031,-11.57177 "
        //    "                     "
        //    "-0.01154,-0.3263     "
        //    "-0.01894,-0.65282    "
        //    "-0.01628,-0.97935    ";
    }
}
