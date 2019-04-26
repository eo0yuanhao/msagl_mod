using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//----------------------
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl;
using System.Drawing.Drawing2D;
//---------------------------

namespace irmap_form {
    using Curves=Microsoft.Msagl.Core.Geometry.Curves;
    public partial class Form1 : Form {
 
        public Form1() {
            InitializeComponent();
            setup_graphViewer();
            //this.key
            //this.OnKeyDown += form_keydown;
        }
        Edge oe = null;
        GViewer gViewer = null;
        private void form_keydown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.T) {
                if (oe == null)
                    return;
                if (oe.Attr.ArrowheadAtTarget == ArrowStyle.None)
                    oe.Attr.ArrowheadAtTarget = ArrowStyle.ODiamond;
                else oe.Attr.ArrowheadAtTarget = ArrowStyle.None;
                gViewer.Invalidate();
            }
        }
        private void setup_graphViewer() {
            Graph graph = new Graph();

            establish_graph(graph);

            GViewer gv = new GViewer();
            gv.Graph = graph;
            gv.Dock = DockStyle.Fill;
            this.Controls.Add(gv);
            gViewer = gv;
            gViewer.KeyDown += Form1_KeyDown;
        }
        private bool edge_rendering_delegate(Edge edge, object graphics)
        {
            Graphics g = graphics as Graphics;
            var curve = edge.EdgeCurve;

            //var patha = Draw.CreateGraphicsPath(curve);
            //g.DrawPath(Pens.Black, patha);
            //return false;

            var t=curve.GetParameterAtLength(curve.Length / 2);
            var del=curve.Derivative(t);
            var k = del.Y / del.X;
            //this.Text = string.Format("x:{0},y:{1}", d.X, d.Y);

            //Curves.CubicBezierSegment
            //CubicBezierSegment bs = curve as CubicBezierSegment
            var a = curve[t];
            float x = (float) a.X, y = (float)a.Y;
            Pen pen = Pens.Black;
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            //path.AddCurve(new[] { new PointF(1, 4), new PointF(2, 6), new PointF(3, 5) });// ,new PointF() });
            //path.AddLine(-2, -2, 2, 2);

            //string _of_s_data = "m 0,0 c 0,-8.6 0.1,-17.3 1.8,-25.8   0.6,-2.5 1.2,-5.4 3.2,-7.3   1,-1 3,0 3.9,1.0   2.5,3.2 3.1,7.5 3.3,12.5";
            //string _of_s_data2 = "M 0,0 c 0,8.6 -0.1,17.3 -1.8,25.8   -0.6,2.5 -1.2,5.4 -3.2,7.3   -1,1 -3,-0 -3.9,-1.0   -2.5,-3.2 -3.1,-7.5 -3.3,-12.5";

            //Svg.SvgPath sp = new Svg.SvgPath();
            //sp.PathData = Svg.SvgPathBuilder.Parse(_of_s_data + _of_s_data2 );
            //Svg.ISvgRenderer render = Svg.SvgRenderer.FromNull();// .FromGraphics(g);

            ////var converter = TypeDescriptor.GetConverter(typeof(Svg.Pathing.SvgPathSegmentList));
            ////GraphicsPath ttt = sp.Path(render);
            //var _of_s_path = sp.Path(render);

            var outpath = dataDefine.graphicsPath_data.get_svg2(g);
            path = outpath;
            Matrix m= new Matrix();
            Func<double,double> rad2Deg = v => { return v * 180/Math.PI; };

            m.Translate(x, y);
            m.Rotate((float)rad2Deg(Math.Atan2(del.Y, del.X)));//, new PointF(x, y));
            m.Scale(-0.2f, 0.2f);
            path.Transform(m);

            //g.DrawPath(pen, path);
            //var arrowLinePath = Draw.CreateGraphicsPath(curve);
            //g.DrawPath(pen,arrowLinePath);
            g.DrawEllipse(Pens.Black, x - 2, y - 2, 4, 4);
            g.DrawPath(pen, path);
            path = null;
            return false;
        }
        private void establish_graph(Graph graph) {
            //graph.AddEdge("a", "b");
            var g = graph;
            Edge e = g.AddEdge("c", "Dvi");
            e.Attr.ArrowheadAtTarget = ArrowStyle.Vee;
            e.Attr.ArrowheadAtSource = ArrowStyle.Vee;
            //e.Label = new Microsoft.Msagl.Drawing.Label();
            //e.LabelText = "aa";
            //e.GeometryEdge.Label.
            e.DrawEdgeDelegate = edge_rendering_delegate;
            oe = e;
            //e.DrawEdgeDelegate
            //e.EdgeCurve
            //e.Attr.Separation = 3;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.T) {
                if (oe == null)
                    return;
                if (oe.Attr.ArrowheadAtTarget == ArrowStyle.None)
                    oe.Attr.ArrowheadAtTarget = ArrowStyle.ODiamond;
                else oe.Attr.ArrowheadAtTarget = ArrowStyle.None;
                gViewer.Invalidate();
            }
        }
    }
}
