using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//-----------------------------
using System.Drawing.Drawing2D;
//----------------------
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl;
using CommonDrawingUtilsForSamples;
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
        enum UserOprationMode {
            None,Select=None,AddNode,OpLast
        }
        private UserOprationMode _useOpMode;
        //void Form1_MouseUp(object sender, MsaglMouseEventArgs e) {
        //    object obj = gViewer.GetObjectAt(e.X, e.Y);
        //    Node node = null;
        //    Edge edge = null;
        //    var dnode = obj as DNode;
        //    var dedge = obj as DEdge;
        //    var dl = obj as DLabel;
        //    if (dnode != null)
        //        node = dnode.DrawingNode;
        //    else if (dedge != null)
        //        edge = dedge.DrawingEdge;
        //    else if (dl != null) {
        //        if (dl.Parent is DNode)
        //            node = (dl.Parent as DNode).DrawingNode;
        //        else if (dl.Parent is DEdge)
        //            edge = (dl.Parent as DEdge).DrawingEdge;
        //    }

        //}
        void graphViewer_ObjectUnderMouseCursorChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e) {
            //var n = e.NewObject.DrawingObject;// as DNode;
            if (e.NewObject != null)
                this.Text = "mouse hover in";
            else this.Text = "mouse hover out";
            //var node = gViewer.ObjectUnderMouseCursor as Node;// as IViewerNode;
            //if (node != null) {
            //    this.Text = node.Id;
            //}
            //else {
            //    var edge = gViewer.ObjectUnderMouseCursor as Edge;// IViewerEdge;
            //    if (edge != null)
            //        this.Text = "edge clicked";
            //}
        }
        private void setup_graphViewer() {
            Graph graph = new Graph();

            establish_graph(graph);

            GViewer gv = new GViewer();
            gv.Graph = graph;
            gv.Dock = DockStyle.Fill;
            this.Controls.Add(gv);

            ///var node = new Node("55");
            //node.Label.Text = nte.DefaultLabel;
            //node.Attr.FillColor = nte.FillColor;
            //node.Label.FontColor = nte.FontColor;
            //node.Label.FontSize = nte.FontSize;
            //node.Attr.Shape = Shape.Box;//. nte.Shape;
            //var pos=gv.ScreenToSource(0.0, 0);
            //IViewerNode dNode = gv.CreateIViewerNode(node, pos, null);
            //gv.AddNode(dNode, true);
            //var cnode=graph.FindNode("c");
            //gv.AddEdge(cnode, node, true);
            //IViewerEdge dEdge = gv.CreateEdgeWithGivenGeometry()
            //gv.AddEdge()

            gViewer = gv;
            gViewer.KeyDown += Form1_KeyDown;
            var viewer = (gViewer as IViewer);
            //viewer.ObjectUnderMouseCursorChanged += graphViewer_ObjectUnderMouseCursorChanged;
            viewer.MouseDown += gviewer_mousedown;
            viewer.MouseUp += gviewer_mouseup;
        }

        private void gviewer_mousedown(object sender, MsaglMouseEventArgs e) {
            
        }
        void gviewer_mouseup(object sender, MsaglMouseEventArgs e) {
            if(_useOpMode == UserOprationMode.Select) {
                var obj=gViewer.GetObjectAt(new Point(e.X, e.Y));
                if (obj == null)
                    return;
                var node = obj as DNode;
                if(node != null) {
                    Text = node.DrawingNode.Id;
                }else {
                    var edge = obj as DEdge;
                    Text = edge.Edge.Source;
                }
            }else if(_useOpMode == UserOprationMode.AddNode) {
                var pos = gViewer.ScreenToSource(e);
                //var node = new Node("smt");

                var gv = gViewer;
                gv.AddNode(gv.CreateIViewerNode(new Node("ssss"), pos, null),true);
                var ggg = gv.Graph;
                if (ggg.NodeCount > 100)
                    ;
                ////(gv as IViewer).a   .add(), pos, null);
                ////node.LabelText = "smt show";
                //var graph = gViewer.Graph;
                ////graph.AddNode(node);
                //graph.AddEdge("11", "222");
                ////DrawingUtilsForSamples.AddNode("smt", graph.GeometryGraph, 10, 2);
                //gViewer.Graph = graph;
                //var gv = gViewer;
                ////IViewerNode n=gViewer.CreateIViewerNode(new Node("smart"));
                ////gViewer.CreateIViewerNode(new Node("marks"), pos, null);
                ////gViewer.AddNode(n, true);
            }

            //var viewer = gViewer as IViewer;
            //viewer.
        }

        private void edge_rendering_delegate(Edge edge, object graphics)
        {
            Graphics g = graphics as Graphics;
            var curve = edge.EdgeCurve;
            var t_middle=curve.GetParameterAtLength(curve.Length / 2);
            var dva=curve.Derivative(t_middle); //求中点的导数
            //var k = del.Y / del.X;
            //this.Text = string.Format("x:{0},y:{1}", d.X, d.Y);

            var midPoint = curve[t_middle];
            float x = (float) midPoint.X, y = (float)midPoint.Y;

            var path = dataDefine.graphicsPath_data.get_svg2(g);
            Matrix m= new Matrix();
            Func<double,double> rad2Deg = v => { return v * 180/Math.PI; };

            m.Translate(x, y);
            m.Rotate((float)rad2Deg(Math.Atan2(dva.Y, dva.X)));//, new PointF(x, y));
            m.Scale(-0.2f, 0.2f);
            path.Transform(m);

            Pen pen = Pens.Black;
            g.FillEllipse(Brushes.White, x - 2, y - 2, 4, 4);
            g.DrawEllipse(pen, x - 2, y - 2, 4, 4);
            g.DrawPath(pen, path);
            //return false;
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
            e.DrawEdge_Last = edge_rendering_delegate;
            Node n = g.FindNode("c");
            oe = e;
            //e.DrawEdgeDelegate
            //e.EdgeCurve
            //e.Attr.Separation = 3;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            Keys key = e.KeyCode;
            if (key == Keys.T) {
                if (oe == null)
                    return;
                if (oe.Attr.ArrowheadAtTarget == ArrowStyle.None)
                    oe.Attr.ArrowheadAtTarget = ArrowStyle.ODiamond;
                else oe.Attr.ArrowheadAtTarget = ArrowStyle.None;
                gViewer.Invalidate();
            }else if(key == Keys.N) {
                _useOpMode = UserOprationMode.AddNode;
            }
        }
    }
}
