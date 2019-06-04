using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Microsoft.Msagl.WpfGraphControl.ExtendEdgeDraw {
    class DataDefine {
        static Geometry _of_s_geometry;
        public static Geometry get_svg2() {
            if (_of_s_geometry != null) {
                return _of_s_geometry.Clone();
            }

            string _of_s_data = "m 0,0 c 0,-8.6 0.1,-17.3 1.8,-25.8   0.6,-2.5 1.2,-5.4 3.2,-7.3   1,-1 3,0 3.9,1.0   2.5,3.2 3.1,7.5 3.3,12.5";
            string _of_s_data2 = "M 0,0 c 0,8.6 -0.1,17.3 -1.8,25.8   -0.6,2.5 -1.2,5.4 -3.2,7.3   -1,1 -3,-0 -3.9,-1.0   -2.5,-3.2 -3.1,-7.5 -3.3,-12.5";
            _of_s_geometry = Geometry.Parse(_of_s_data + _of_s_data2);
            return _of_s_geometry.Clone();
        }
    }
}
