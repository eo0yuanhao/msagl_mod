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
        public MainWindow() {
            InitializeComponent();
            Content = panel;

            Loaded += window_loaded;

        }

        void addNode(GraphViewer gv,string id) {
            var node = new Node(id);
            var graph = gv.Graph;
            var lnode = GeometryGraphCreator.CreateGeometryNode(graph, graph.GeometryGraph, node, ConnectionToGraph.Disconnected);
            lnode.BoundaryCurve = NodeBoundaryCurves.GetNodeBoundaryCurve(node, 15, 12);
            node.GeometryNode = lnode;
            gv.CreateIViewerNode(node, new DPoint(-510, 60), null);
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
        void addLineEdge2(GraphViewer gv, string src, string tar) {
            var graph = gv.Graph;
            var edge = graph.AddEdge(src, null, tar);
            edge.SourceNode.AddOutEdge(edge);
            edge.TargetNode.AddInEdge(edge);

            var ledge = GeometryGraphCreator.CreateGeometryEdgeFromDrawingEdge(edge);
            ledge.Source.AddOutEdge(ledge);
            ledge.Target.AddInEdge(ledge);
            edge.GeometryEdge = ledge;
            //ledge.Curve = new LineSegment(ledge.Source.Center, ledge.Target.Center);
            //gv.AddEdge(gv.CreateEdgeWithGivenGeometry(edge), true);
            Microsoft.Msagl.Routing.StraightLineEdges.RouteEdge(ledge, 0);
            var oute = gv.CreateEdgeWithGivenGeometry(edge);
            gv.LayoutEditor.AttachLayoutChangeEvent(oute);

        }
        public void window_loaded(object sender, RoutedEventArgs e) 
        {

            GraphViewer graphViewer = new GraphViewer();
            graphViewer.BindToPanel(panel);
            Graph graph = new Graph();
            graph.Attr.LayerDirection = LayerDirection.LR;


            graph.AddEdge("A", "B");
            graphViewer.Graph = graph;
            var gv = graphViewer;
            //gv.LayoutEditor
            //gv.ViewerGraph

            addNode(gv, "55");

            addLineEdge2(gv, "B", "55");
            gv.Graph = gv.Graph;
            ////gv.RunLayoutInUIThread();



            var vv = graph.GeometryGraph.Nodes.Count;
            //var node = new Node("3");
            //var lnode = GeometryGraphCreator.CreateGeometryNode(graph, graph.GeometryGraph, node, ConnectionToGraph.Disconnected);
            //lnode.BoundaryCurve = NodeBoundaryCurves.GetNodeBoundaryCurve(node, 15, 12);
            //node.GeometryNode = lnode;
            //gv.CreateIViewerNode(node , new DPoint(0, 0), null);
            ////gv.CreateVNode(node);

            //var edge = graph.AddEdge("B", "label", "3");

            //var ledge = GeometryGraphCreator.CreateGeometryEdgeFromDrawingEdge(edge);
            //edge.GeometryEdge = ledge;
            //ledge.Curve = new LineSegment(ledge.Source.Center, ledge.Target.Center);
            //gv.AddEdge(gv.CreateEdgeWithGivenGeometry(edge), true);
            ////gv.RouteEdge(edge);


            //gv.ViewerGraph.
            //gv.RouteEdge()

            //Edge edge;
            //graph.FindNode("B").Attr.Shape = Shape.House;
            //edge = graph.AddEdge("B", "c");
            //edge.Attr.ArrowheadAtTarget = ArrowStyle.Diamond;
            //edge = graph.AddEdge("d", "e");
            //edge.Attr.ArrowheadAtTarget = ArrowStyle.Generalization;
            //edge = graph.AddEdge("e", "f");
            //edge.Attr.ArrowheadAtTarget = ArrowStyle.ODiamond;

            //edge = graph.AddEdge("g", "h");
            //edge.Attr.ArrowheadAtTarget = ArrowStyle.Tee;

            //graphViewer.Graph = graph; // throws exception
            ////var gv = graphViewer;
            ////gv.AddNode(new Node("ss"));
            //var inode = gv.CreateIViewerNode(new Node("marks"), new DPoint(0, 0), null);
            ////IViewerEdge egg= edge.
            //var egg = graph.AddEdge("B", "", "marks");
            ////var egg = new Edge("h", "", "marks");
            ////egg.
            //var layegg = GeometryGraphCreator.CreateGeometryEdgeFromDrawingEdge(egg);

            //egg.GeometryEdge = layegg;
            //layegg.Curve = new LineSegment(layegg.Source.Center, layegg.Target.Center);
            //egg.GeometryEdge.LineWidth = 2;
            //gv.AddEdge(gv.CreateEdgeWithGivenGeometry(egg), true);
            //gv.AddNode(inode, true);
            //graphViewer.Invalidate();
        }
    }
}
