using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using System.Diagnostics;

namespace IRxMap {
    using DPoint = Microsoft.Msagl.Core.Geometry.Point;
    using LNode = Microsoft.Msagl.Core.Layout.Node;
    class GraphViewerHelper {

        public static void addCordinate(GraphViewer gv) {
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

        public static IViewerNode addNode(GraphViewer gv, string id, double x, double y) {
            //var node = new Node(id);
            var graph = gv.Graph;
            var node = graph.AddNode(id);
            var lnode = GeometryGraphCreator.CreateGeometryNode(graph, graph.GeometryGraph, node, ConnectionToGraph.Disconnected);
            var labelSize = Common.MeasureLabel(node.Label);
            lnode.BoundaryCurve = NodeBoundaryCurves.GetNodeBoundaryCurve(node, labelSize.Width, labelSize.Height);
            node.GeometryNode = lnode;
            var vnode = gv.CreateIViewerNode(node, new DPoint(x, y), null);
            return vnode;
            //gv.AddNode(vnode, true);
            //gv.CreateVNode(node);
        }

        public static IViewerNode addNodeWithStyles(GraphViewer gv, string id,DPoint pos,ICollection<Style> styles) {
            var graph = gv.Graph;
            var node = graph.AddNode(id);
            if(styles != null) {
                foreach (var s in styles)
                    node.Attr.AddStyle(s);
            }
            
            var lnode = GeometryGraphCreator.CreateGeometryNode(graph, graph.GeometryGraph, node, ConnectionToGraph.Disconnected);
            var labelSize = Common.MeasureLabel(node.Label);
            var mar = node.Attr.LabelMargin;
            lnode.BoundaryCurve = NodeBoundaryCurves.GetNodeBoundaryCurve(node, labelSize.Width+ mar*2, labelSize.Height + mar*2);
            node.GeometryNode = lnode;
            var vnode = gv.CreateIViewerNode(node, pos, null);
            return vnode;
        }
        public static IViewerNode addVnodeByDnode(GraphViewer gv,Node node, DPoint pos) {
            var graph = gv.Graph;
            Debug.Assert(node.GeometryNode == null);
            LNode lnode = GeometryGraphCreator.CreateGeometryNode(graph, graph.GeometryGraph, node, ConnectionToGraph.Disconnected);
            var labelSize = Common.MeasureLabel(node.Label);
            var mar = node.Attr.LabelMargin;
            lnode.BoundaryCurve = NodeBoundaryCurves.GetNodeBoundaryCurve(node, labelSize.Width + mar * 2, labelSize.Height + mar * 2);
            node.GeometryNode = lnode;

            var vnode = gv.CreateIViewerNode(node, pos, null);
            return vnode;
        }
        public static void addLineEdge(GraphViewer gv, string src, string tar) {
            var graph = gv.Graph;
            var edge = graph.AddEdge(src, null, tar);
            edge.SourceNode.AddOutEdge(edge);
            edge.TargetNode.AddInEdge(edge);

            var ledge = GeometryGraphCreator.CreateGeometryEdgeFromDrawingEdge(edge);
            //ledge.Source = edge.SourceNode.GeometryNode;
            //ledge.Target = edge.TargetNode.GeometryNode;
            ledge.Source.AddOutEdge(ledge);
            ledge.Target.AddInEdge(ledge);
            edge.GeometryEdge = ledge;
            ledge.Curve = new Microsoft.Msagl.Core.Geometry.Curves.LineSegment(ledge.Source.Center, ledge.Target.Center);
            //gv.AddEdge(gv.CreateEdgeWithGivenGeometry(edge), true);
            Microsoft.Msagl.Layout.LargeGraphLayout.LgLayoutSettings lgsets = null;

            Microsoft.Msagl.Routing.StraightLineEdges.RouteEdge(ledge, 0);
            var oute = gv.CreateEdgeWithGivenGeometry(edge);
            gv.LayoutEditor.AttachLayoutChangeEvent(oute);
            gv.Invalidate();
            var sss = oute.Target;
            //gv.RouteEdge(edge);
        }
        public static IViewerEdge addLineEdge2(GraphViewer gv, string src, string tar) {
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

    }
}
