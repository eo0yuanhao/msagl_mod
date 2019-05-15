using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
namespace DebugV {
    public class D {
        public static int tag = 0;
        public static GraphViewer gv;
    }
}
namespace IRxMap {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    using Shape = Microsoft.Msagl.Drawing.Shape;
    using DPoint = Microsoft.Msagl.Core.Geometry.Point;
    using Microsoft.Msagl.Layout;
    using Microsoft.Msagl.Core.Geometry.Curves;
    public partial class MainWindow : Window {
        
        DockPanel panel = new DockPanel();
        GraphViewer _graphViewer;
        public MainWindow() {
            InitializeComponent();
            Content = panel;

            Loaded += window_loaded;
            //window_loaded(null, null);
        }

        void addCordinate(GraphViewer gv) {
            Line a1 = new Line();
            a1.X1 = -100;
            a1.Y1 = 0;
            a1.X2 = 100;
            a1.Y2 = 0;
            a1.Stroke = Brushes.Black;
            gv.GraphCanvas.Children.Add(a1);

            Line a2 = new Line();
            a2.X1 = 00;
            a2.Y1 = 100;
            a2.X2 = 0;
            a2.Y2 = -100;
            a2.Stroke = Brushes.Black;
            gv.GraphCanvas.Children.Add(a2);
        }

        IViewerNode addNode(GraphViewer gv,string id,double x,double y) {
            //var node = new Node(id);
            var graph = gv.Graph;
            var node = graph.AddNode(id);
            var lnode = GeometryGraphCreator.CreateGeometryNode(graph, graph.GeometryGraph, node, ConnectionToGraph.Disconnected);
            lnode.BoundaryCurve = NodeBoundaryCurves.GetNodeBoundaryCurve(node, 15, 12);
            node.GeometryNode = lnode;
            var vnode=gv.CreateIViewerNode(node, new DPoint(x, y), null);
            return vnode;
            //gv.AddNode(vnode, true);
            //gv.CreateVNode(node);
        }
        void addLineEdge(GraphViewer gv,string src, string tar) {
            var graph = gv.Graph;
            var edge = graph.AddEdge(src,null, tar);
            edge.SourceNode.AddOutEdge(edge);
            edge.TargetNode.AddInEdge(edge);
            
            var ledge = GeometryGraphCreator.CreateGeometryEdgeFromDrawingEdge(edge);
            //ledge.Source = edge.SourceNode.GeometryNode;
            //ledge.Target = edge.TargetNode.GeometryNode;
            ledge.Source.AddOutEdge(ledge);
            ledge.Target.AddInEdge(ledge);
            edge.GeometryEdge = ledge;
            ledge.Curve = new LineSegment(ledge.Source.Center, ledge.Target.Center);
            //gv.AddEdge(gv.CreateEdgeWithGivenGeometry(edge), true);
            Microsoft.Msagl.Layout.LargeGraphLayout.LgLayoutSettings lgsets = null;

            Microsoft.Msagl.Routing.StraightLineEdges.RouteEdge(ledge, 0);
            var oute=gv.CreateEdgeWithGivenGeometry(edge);
            gv.LayoutEditor.AttachLayoutChangeEvent(oute);
            gv.Invalidate();
            var sss = oute.Target;
            //gv.RouteEdge(edge);
        }
        IViewerEdge addLineEdge2(GraphViewer gv, string src, string tar) {
            var graph = gv.Graph;
            var edge = graph.AddEdge(src, null, tar);
            edge.SourceNode.AddOutEdge(edge);
            edge.TargetNode.AddInEdge(edge);            

            var ledge = GeometryGraphCreator.CreateGeometryEdgeFromDrawingEdge(edge);
            ledge.Source.AddOutEdge(ledge);
            ledge.Target.AddInEdge(ledge);
            ledge.GeometryParent = graph.GeometryGraph;
            graph.GeometryGraph.Edges.Add(ledge);
            edge.GeometryEdge = ledge;
            //ledge.Curve = new LineSegment(ledge.Source.Center, ledge.Target.Center);
            //gv.AddEdge(gv.CreateEdgeWithGivenGeometry(edge), true);
            Microsoft.Msagl.Routing.StraightLineEdges.RouteEdge(ledge, 0);
            var oute = gv.CreateEdgeWithGivenGeometry(edge);
            gv.LayoutEditor.AttachLayoutChangeEvent(oute);
            return oute;
        }

        private bool edge_rendering_delegate(Edge edge, object edgeLine_path) 
        {            
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
            mat.RotatePrepend(deg );            
            mat.ScalePrepend(-0.2f, 0.2f);
            mt.Matrix = mat;
            geo2.Transform = mt;

            var circle = new EllipseGeometry(new Point(x, y), 2, 2);
            PathGeometry pp = new PathGeometry();
            pp.FillRule = FillRule.EvenOdd;
            
            pp.AddGeometry(Common.GetICurveWpfGeometry(cv));
            pp.AddGeometry(circle);
            pp.AddGeometry(geo2);
            path.Data = pp;

            return true;
        }

        int selfcounter = 0;
        public void MouseWheelEventHandler(object sender, MouseWheelEventArgs e) {
            this.Title = $"canvas receive {selfcounter++}";
        }
        public void window_loaded_prev(object sender, RoutedEventArgs e) {
            GraphViewer graphViewer = new GraphViewer();

            graphViewer.BindToPanel(mainGrid);
            mainGrid.LastChildFill = true;
            mainGrid.Background = Brushes.LightGray;
            Graph graph = new Graph();
            //graph.Attr.LayerDirection = LayerDirection.LR;

            var edge = graph.AddEdge("A", "B");

            edge.DrawEdgeDelegate = edge_rendering_delegate;
            graphViewer.Graph = graph;
            this.MouseWheel += MouseWheelEventHandler;
            //panel.MouseWheel += MouseWheelEventHandler;
            //graphViewer.GraphCanvas.MouseWheel += MouseWheelEventHandler;
            var gv = graphViewer;
            _graphViewer = gv;

            addNode(gv, "55",0,0);
            addLineEdge2(gv, "B", "55");
            graph.GeometryGraph.UpdateBoundingBox();
            DebugV.D.tag = 1;
            gv.SetInitialTransform();
            //gv.GraphCanvas.Background = Brushes.Red;
            var ov = gv.GraphCanvas.Focus();           
        }
        public void window_loaded(object sender, RoutedEventArgs e) {
            panel.LastChildFill = true;
            GraphViewer graphViewer = new GraphViewer();

            graphViewer.BindToPanel(panel);

            panel.Background = Brushes.LightGray;
            panel.UpdateLayout();
            Graph graph = new Graph();
            //graph.Attr.LayerDirection = LayerDirection.LR;
            //graph.AddNode("aaa");
            graphViewer.Graph = graph;
            //graphViewer.Graph.CreateGeometryGraph();

            //var edge = graph.AddEdge("A", "B");
            //edge.DrawEdgeDelegate = edge_rendering_delegate;
            this.MouseWheel += MouseWheelEventHandler;
            var gv = graphViewer;
            _graphViewer = gv;

            addNode(gv, "A", 0, 30);
            addNode(gv, "B", 20,15);
            var edge=addLineEdge2(gv, "A", "B");
            edge.Edge.DrawEdgeDelegate = edge_rendering_delegate;
            addNode(gv, "55", 0, 0);
            addLineEdge2(gv, "B", "55");
            graph.GeometryGraph.UpdateBoundingBox();
            //DebugV.D.tag = 1;
            DebugV.D.gv = gv;
            gv.SetInitialTransform();
            gv.Transform = gv.Transform;

            //timer1 = new System.Windows.Threading.DispatcherTimer();
            //timer1.Tick += TimeTick;
            //timer1.Interval = TimeSpan.FromMilliseconds(500);
            //timer1.Start();
        }
        void TimeTick(object sender, EventArgs e) {
            Title = $"selfctt:{selfcounter++}";
        }
        System.Windows.Threading.DispatcherTimer timer1;
    }
}
