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
namespace VDebug {
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
    using Microsoft.Msagl.Routing;
    using Microsoft.Msagl.Core;
    using DLabel = Microsoft.Msagl.Drawing.Label;
    using LLabel = Microsoft.Msagl.Core.Layout.Label;
    using DStyle = Microsoft.Msagl.Drawing.Style;
   
    using EdgeLabelPlacement= Microsoft.Msagl.Core.Layout.EdgeLabelPlacement;
    using GVH = GraphViewerHelper;
    using ToggleButton = System.Windows.Controls.Primitives.ToggleButton;
    using System.Diagnostics;
    using controls;
    using Xceed.Wpf.AvalonDock.Layout;

    public partial class MainWindow : Window {

        Panel panel;//= new DockPanel();
        GraphViewer _graphViewer;
        bool _iset_ShapeStyle = false;
        //bool _iset_edgeLineStyle = false;
        //bool _iset_edgeSrcArrow = false;
        //bool _iset_edgeDstArrow = false;
        //bool _iset_edgeDecrate = false;

        public int _nodeIdCounter = 0;
        string _currentCmd = "select";
        ToggleButton _currentPressedBtn = null;
        bool _mouseEventHandled = false;
        IViewerObject _editingObj;
        string _fileName="";
        readonly ToggleButton[] toolBtns;
        ICommand[] commandList;
        EdgeAttr edge_defApplyAttr = new EdgeAttr();
        LayoutAnchorable edge_defApplyAttrPanel;
        private class Enum_List<TEnum> {
            public class Enum_Record {
                public string Enum_str { get; set; }
                public TEnum Enum_value { get; set; }
            }
            public List<Enum_Record> List { get;  set; }
            public Enum_List() {
                var _list = new List<Enum_Record>();
                var es = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
                foreach (var e in es) {
                    var v = new Enum_Record() {
                        Enum_str = Enum.GetName(typeof(TEnum), e),
                        Enum_value = e
                    };
                    _list.Add(v);
                }
                this.List = _list;
            }
        }


        public MainWindow() {
            InitializeComponent();
            panel = docPanel;
            InitControls();
            loadDataToControls();

            VDebug.D.tag = 1;
            Loaded += window_loaded;
            panel.MouseDown += (s, e) => panel.Focus();
            panel.FocusVisualStyle = null;
            toolBtns = new [] { nodeBtn, edgeBtn,selectBtn};

            
            //window_loaded(null, null);
        }
        public static GraphViewer global_graphViewer;
        public static ICollection<IViewerObject> SelectedViewerObjects() {
            if (global_graphViewer == null)
                return null;
            return global_graphViewer.LayoutEditor.dragGroup;
        }

        private void InitControls() {
            //panel.LastChildFill = true;
            GraphViewer graphViewer = new GraphViewer();

            graphViewer.BindToPanel(panel);

            panel.Background = Brushes.LightGray;
            panel.Focusable = true;
            panel.UpdateLayout();
            _graphViewer = graphViewer;

            nodeAttrGroup.Visibility = Visibility.Hidden;
            //propertyPanel.UpdateLayout();
            //edgeAttrGroup.Visibility = Visibility.Hidden;
            edgeEditor.Visibility = Visibility.Visible;
            global_graphViewer = _graphViewer;
        }

        private void loadDataToControls() {

            #region shapeStyle_cmb initial data
            {
                shapeStyle_cmb.ItemsSource = new Enum_List<Shape>().List;
                _iset_ShapeStyle = true;
                shapeStyle_cmb.SelectedIndex = 0;
                _iset_ShapeStyle = false;
            }

            #endregion

            #region srcArrowStyle_cmb initial data
            //srcArrowStyle_cmb.ItemsSource = new Enum_List<ArrowStyle>().List;
            //_iset_edgeSrcArrow  = true;
            //srcArrowStyle_cmb.SelectedIndex = 0;
            //_iset_edgeSrcArrow = false;
            //#endregion

            //#region lineStyle_cmb initial data
            //{
            //    var el = new Enum_List<DStyle>();
            //    var conCol = new[] { DStyle.Dashed, DStyle.Dotted, DStyle.Solid };
            //    el.List = el.List.Where(x => conCol.Contains(x.Enum_value) ).ToList();
            //    lineStyle_cmb.ItemsSource = el.List;
            //    _iset_edgeLineStyle = true;
            //    lineStyle_cmb.SelectedIndex = 0;
            //    _iset_edgeLineStyle = false;
            //}
            //#endregion

            //#region dstArrowStyle_cmb initial data
            //dstArrowStyle_cmb.ItemsSource = new Enum_List<ArrowStyle>().List;
            //_iset_edgeDstArrow = true;
            //dstArrowStyle_cmb.SelectedIndex = 0;
            //_iset_edgeDstArrow = false;
            //#endregion

            //#region decSymbol_cmb initial data
            //decSymbol_cmb.ItemsSource = new List<string>() { "","center_of" };
            //_iset_edgeDecrate  = true;
            //decSymbol_cmb.SelectedIndex = 0;
            //_iset_edgeDecrate = false;
            #endregion


            #region graphViewer mouse event callback

            _graphViewer.MouseDown += graphViewer_MouseDown;
            _graphViewer.MouseMove += graphViewer_MouseMove;
            _graphViewer.MouseUp += graphViewer_MouseUp;
            _graphViewer.MouseDoubleClick += graphViewer_MouseDoubleClick;

            #endregion

            #region Command Environment initilize 
            {
                CommandEnvironment ce = new CommandEnvironment();
                ce.init(_graphViewer, this);
            }

            #endregion

            #region load all command
            commandList = new[] {new AddNodeTool() };
            #endregion
        }

        //private bool edge_rendering_delegate(Edge edge, object edgeLine_path) {
        //    Path path = edgeLine_path as Path;
        //    ICurve cv = edge.EdgeCurve;

        //    var t_middle = cv.GetParameterAtLength(cv.Length / 2);
        //    var dva = cv.Derivative(t_middle); //求中点的导数
        //    var midPoint = cv[t_middle];// Common.WpfPoint( cv[t_middle]);
        //    float x = (float)midPoint.X, y = (float)midPoint.Y;

        //    var geo2 = DataDefine.get_svg2();
        //    MatrixTransform mt = new MatrixTransform();
        //    Func<double, double> rad2Deg = v => { return v * 180 / Math.PI; };
        //    //mt.Matrix.
        //    var deg = rad2Deg(Math.Atan2(dva.Y, dva.X));
        //    var mat = Matrix.Identity;

        //    mat.TranslatePrepend(x, y);
        //    mat.RotatePrepend(deg);
        //    mat.ScalePrepend(-0.2f, 0.2f);
        //    mt.Matrix = mat;
        //    geo2.Transform = mt;

        //    var circle = new EllipseGeometry(new Point(x, y), 2, 2);
        //    PathGeometry pp = new PathGeometry();
        //    pp.FillRule = FillRule.EvenOdd;

        //    pp.AddGeometry(Common.GetICurveWpfGeometry(cv));
        //    pp.AddGeometry(circle);
        //    pp.AddGeometry(geo2);
        //    path.Data = pp;

        //    return true;
        //}

        int selfcounter = 0;
        public void MouseWheelEventHandler(object sender, MouseWheelEventArgs e) {
            this.Title = $"canvas receive {selfcounter++}";
        }
        bool LoadConfigData() {
            //System.Xml.XmlReader xr = new System.Xml.XmlReader();
            string configFilePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase
                                + "config.xml";
            if (!System.IO.File.Exists(configFilePath))
                return false;
            var xf = System.Xml.Linq.XElement.Load(configFilePath);
            try {
                var x1 = xf.Element("msagl_file");
                var msagl_file = x1.Value.ToString();
                _fileName = msagl_file;
            }catch (Exception ex) {
                //MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }
        public void window_loaded_prev1(object sender, RoutedEventArgs e) {
            GraphViewer graphViewer = new GraphViewer();

            graphViewer.BindToPanel(panel);
            //panel.LastChildFill = true;
            panel.Background = Brushes.LightGray;
            Graph graph = new Graph();
            //graph.Attr.LayerDirection = LayerDirection.LR;

            var edge = graph.AddEdge("A", "B");

            //edge.DrawEdgeDelegate = edge_rendering_delegate;
            edge.UserData = "center_of";
            graphViewer.Graph = graph;
            this.MouseWheel += MouseWheelEventHandler;
            //panel.MouseWheel += MouseWheelEventHandler;
            //graphViewer.GraphCanvas.MouseWheel += MouseWheelEventHandler;
            var gv = graphViewer;
            _graphViewer = gv;

            GVH.addNode(gv, "55", 0, 0);
            GVH.addLineEdge2(gv, "B", "55");
            graph.GeometryGraph.UpdateBoundingBox();
            //DebugV.D.tag = 1;
            gv.SetInitialTransform();
            //gv.GraphCanvas.Background = Brushes.Red;
            var ov = gv.GraphCanvas.Focus();
        }


        public void viewerObjectSelected(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {
            var ob = e.NewObject;
            if (ob == null)
                clear_ObjectAttrViewer();
            else
                getValueFromViewerObject(e.NewObject);
        }

        private void clear_ObjectAttrViewer() {
            id_box.Text = "";
            label_box.Text = "";
            nodeAttrGroup.Visibility = Visibility.Hidden;
            //edgeAttrGroup.Visibility = Visibility.Hidden;
            edgeEditor.Visibility = Visibility.Hidden;
        }

        private void getValueFromViewerObject(IViewerObject newObject) {
            var ob = newObject.DrawingObject;
            _editingObj = newObject;
            var node = ob as Node;
            if(node != null) {
                
                var att = node.Attr;
                id_box.Text = att.Id;
                label_box.Text = node.LabelText;
                _iset_ShapeStyle = true;
                //SelectionChanged will not fire when hidden
                shapeStyle_cmb.SelectedValue = att.Shape;
                _iset_ShapeStyle = false;
                nodeAttrGroup.Visibility = Visibility.Visible;
                
            }else {
                var edge = ob as Edge;
                if (edge == null)
                    return;
                
                var att = edge.Attr;
                id_box.Text = att.Id;
                label_box.Text = edge.LabelText;

                edgeEditor.SetAttr(att);
                edgeEditor.Visibility = Visibility.Visible;
                ////_iset_edgeDecrate = true;

                //_iset_edgeLineStyle = true;
                //lineStyle_cmb.SelectedValue = att.FirstStyle;
                //_iset_edgeLineStyle = false;

                //_iset_edgeDecrate = true;
                //decSymbol_cmb.SelectedValue = edge.UserData==null? "": edge.UserData ;
                //_iset_edgeDecrate = false;
                
                //_iset_edgeSrcArrow = true;
                //srcArrowStyle_cmb.SelectedValue = att.ArrowheadAtSource;
                //_iset_edgeSrcArrow = false;

                //_iset_edgeDstArrow = true;
                //dstArrowStyle_cmb.SelectedValue = att.ArrowheadAtTarget;
                //_iset_edgeDstArrow = false;
                
                //edgeAttrGroup.Visibility = Visibility.Visible;
            }

        }
        public void window_loaded(object sender, RoutedEventArgs e) {
            Graph graph = null ;
            if (LoadConfigData()) {
                if (System.IO.File.Exists(_fileName)) {
                    loadFile(_fileName);
                    graph = _graphViewer.Graph;
                }
            }
            var gv = _graphViewer;
            if (graph == null) {
                _graphViewer.Graph = new Graph();
                //gv.SetInitialTransform();
                gv.Transform = gv.Transform;
            }
            _graphViewer.LayoutEditor.ObjectEditingStatusChanged += viewerObjectSelected;
        }

        public void window_loaded_prev2(object sender, RoutedEventArgs e) {


                

            Graph graph = new Graph();
            //graph.Attr.LayerDirection = LayerDirection.LR;
            //graph.AddNode("aaa");

            //graphViewer.Graph.CreateGeometryGraph();

            //var edge = graph.AddEdge("A", "B");
            //edge.DrawEdgeDelegate = edge_rendering_delegate;
            var edgeg = graph.AddEdge("ma", "mi");
            edgeg.LabelText = "has edge";
            var gv = _graphViewer;
            gv.Graph = graph;
            
            this.MouseWheel += MouseWheelEventHandler;
            
            //_graphViewer = gv;

            GVH.addNode(gv, "A", 0, 30);
            GVH.addNode(gv, "B", 20,15);
            var dnode = graph.FindNode("A");
            var vnode=gv.GetIViewerObject(dnode) as VNode;
            //dnode.GeometryNode.BoundaryCurve = null;
            //VDebug.D.tag = 2;
            dnode.Attr.Shape = Shape.Diamond;

            dnode.GeometryNode.BoundaryCurve =  NodeBoundaryCurves.GetNodeBoundaryCurve(dnode, 15, 12);
            
           
            //vnode.Invalidate();
            
            var edge=GVH.addLineEdge2(gv, "A", "B");
            //edge.Edge.DrawEdgeDelegate = edge_rendering_delegate;
            edge.Edge.UserData = "center_of";
            GVH.addNode(gv, "55", 0, 0);
            GVH.addLineEdge2(gv, "B", "55");
            graph.GeometryGraph.UpdateBoundingBox();
            //DebugV.D.tag = 1;

            gv.SetInitialTransform();
            gv.Transform = gv.Transform;

            gv.LayoutEditor.ObjectEditingStatusChanged += viewerObjectSelected;

            //timer1 = new System.Windows.Threading.DispatcherTimer();
            //timer1.Tick += TimeTick;
            //timer1.Interval = TimeSpan.FromMilliseconds(500);
            //timer1.Start();
            panel.Focusable = true;
        }
        void TimeTick(object sender, EventArgs e) {
            Title = $"selfctt:{selfcounter++}";
        }
        System.Windows.Threading.DispatcherTimer timer1;
        public NodeAttr _defaultNodeAttr= new NodeAttr();

        private void Id_box_LostFocus(object sender, RoutedEventArgs e) {
            var editingObj = _graphViewer.LayoutEditor.EditingObject;
            if (editingObj == null)
                return;
            var obj = editingObj.DrawingObject as DrawingObject2;
            if (obj == null)
                return;

            var att = obj.AttrBase;
            if (id_box.Text == att.Id)
                return;
            att.Id = id_box.Text;
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                panel.Focus();
            }
        }

        private void Label_box_LostFocus(object sender, RoutedEventArgs e) {
            var editingObj = _graphViewer.LayoutEditor.EditingObject;
            if (editingObj == null)
                return;
            var obj = editingObj.DrawingObject as DrawingObject2;
            if (obj == null)
                return;

            if (obj.Label == null) {
                if (label_box.Text == "")
                    return;
            }  else if (obj.Label.Text == label_box.Text) {
                return;
            }

            var node = obj as Node;
            if(node != null) {
                _graphViewer.updateVNodeLabelText(editingObj as VNode, label_box.Text);
                //var sz = Common.MeasureLabel(label);
                //node.GeometryNode.BoundaryCurve = NodeBoundaryCurves.GetNodeBoundaryCurve(node,sz.Width,sz.Height );
                //(editingObj as VNode).Invalidate();
            } else {
                var edge = obj as Edge;
                if (edge == null)
                    return;
                var vedge = editingObj as VEdge;
                var label = edge.Label;
                if (label == null) {
                    label=new DLabel(label_box.Text);
                    edge.Label = label;
                    label.Owner = edge;
                    //var sz = Common.MeasureLabel(label);
                    //edge.GeometryEdge.Label = new Microsoft.Msagl.Core.Layout.Label(sz.Width, sz.Height, edge.GeometryEdge);
                    var llabel = new LLabel();
                    edge.GeometryEdge.Label = llabel;
                    llabel.GeometryParent = edge.GeometryEdge;
                    label.GeometryLabel = llabel;

                    //edge.GeometryEdge.Label.UserData = edge.Label;
                    //var fe = _graphViewer.CreateDefaultFrameworkElementForDrawingObject(edge);
                    //var vlabel = new VLabel(edge, fe);
                    //fe.Tag = vlabel;
                    //(editingObj as VEdge).VLabel = vlabel;
                    
                    _graphViewer.SetVEdgeLabel(edge, vedge, _graphViewer.ZIndexOfEdge(edge));
                    _graphViewer.LayoutEditor.AttachLayoutChangeEvent(vedge.VLabel);
                }
                else {
                    label.Text = label_box.Text;
                    var textBlock = _graphViewer.drawingObjectsToFrameworkElements[edge] as TextBlock;
                    textBlock.Text = label.Text;
                    //textBlock.InvalidateMeasure();
                    textBlock.Width = Double.NaN;
                    textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    textBlock.Width = textBlock.DesiredSize.Width;
                    //textBlock.Height = textBlock.DesiredSize.Height;
                    //label.GeometryLabel.Width
                }
                //var vedge = _graphViewer.GetIViewerObject(edge) as VEdge;
                //_graphViewer.SetVEdgeLabel(edge, vedge, _graphViewer.ZIndexOfEdge(edge));
                _graphViewer.AssignLabelWidthHeight(edge.GeometryEdge, edge);
                //_graphViewer.LayoutEditor.AttachLayoutChangeEvent(vedge.VLabel);

                //var labelCol = new List<LLabel>();
                //labelCol.Add(label.GeometryLabel);
                //EdgeLabelPlacement lp = new EdgeLabelPlacement(_graphViewer.Graph.GeometryGraph,labelCol);//,label.GeometryLabel)
                //var lp=new EdgeLabelPlacement(_graphViewer.Graph.GeometryGraph, new List<LLabel> { label.GeometryLabel});
                var lp=new EdgeLabelPlacement(_graphViewer.Graph.GeometryGraph, new []{ label.GeometryLabel});
                lp.Run();
                vedge.Invalidate();
                //edge.LabelText = label_box.Text;
                //var vedge = _graphViewer.GetIViewerObject(edge) as VEdge;
                //_graphViewer.SetVEdgeLabel(edge, vedge, _graphViewer.ZIndexOfEdge(edge));
                //vedge.Invalidate();
            }

             
            //if (label.Text == label_box.Text)
            //    return;
            //label.Text = label_box.Text;
            //obj.Label = label;


        }

        private void ShapeStyle_cmb_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //if (_graphViewer == null)
            //    return;
            if (_iset_ShapeStyle) {
                _iset_ShapeStyle = false;
                return;
            }

            var editingObj = _graphViewer.LayoutEditor.EditingObject;
            if (editingObj == null)
                return;
            var vnode = editingObj as VNode;
            if (vnode == null)
                return;
            var sh = (Shape)shapeStyle_cmb.SelectedValue;
            if (vnode.Node.Attr.Shape == sh)
                return;
            var node = vnode.Node;
            node.Attr.Shape = sh;
            var sz=Common.MeasureLabel(node.Label);
            var pos = node.GeometryNode.Center;
            node.GeometryNode.BoundaryCurve = NodeBoundaryCurves.GetNodeBoundaryCurve(vnode.Node, sz.Width, sz.Height);
            node.GeometryNode.Center = pos;
            foreach (var dEdge in node.Edges) {
                StraightLineEdges.CreateSimpleEdgeCurveWithUnderlyingPolyline(dEdge.GeometryEdge);
                (_graphViewer.GetIViewerObject(dEdge) as VEdge).Invalidate();
            }
            vnode.Invalidate();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs evt) {
            if(_currentCmd == "node editing") {
                return;
            }
            switch (evt.Key) {
                case Key.V:
                    selectBtn.IsChecked = true;
                    pressingBtn(selectBtn);
                    _currentCmd = "select";  
                    break;
                case Key.E:
                    edgeBtn.IsChecked = true;
                    pressingBtn(edgeBtn);
                    _currentCmd = "add edge";
                    break;
                case Key.N:
                    nodeBtn.IsChecked = true;
                    pressingBtn(nodeBtn);
                    _currentCmd = "add node";
                    break;
                case Key.Space:
                    _graphViewer.Panning = true;
                    var sresInfo = Application.GetResourceStream(new Uri("cur_scroll.cur", UriKind.Relative));
                    var cur = new System.Windows.Input.Cursor(sresInfo.Stream);
                    this.Cursor = cur;
                    break;
                case Key.T:
                    VDebug.D.tag++;
                    this.Title = $"trigged{selfcounter++}";
                    break;
                case Key.Delete:
                    if(_currentCmd == "select") {
                        foreach( var obj in _graphViewer.LayoutEditor.dragGroup) {
                            var vnode = obj as VNode;
                            if(vnode != null) {
                                _graphViewer.RemoveNode(vnode, true);
                                continue;
                            }
                            var vedge = obj as VEdge;
                            if(vedge != null) {
                                _graphViewer.RemoveEdge(vedge, true);
                                continue;
                            }
                            var vlabel = obj as VLabel;
                            if(vlabel != null) {
                                _graphViewer.RemoveLabel(vlabel);
                            }
                        }
                        _graphViewer.LayoutEditor.Clear();
                    }
                    break;
            }

        }
        private void Grid_KeyUp(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Space:
                    _graphViewer.Panning = false;
                    //var sresInfo = Application.GetResourceStream(new Uri("cur_scroll.cur",UriKind.Relative));
                    //var cur = new System.Windows.Input.Cursor(sresInfo.Stream);
                    this.Cursor = System.Windows.Input.Cursors.Arrow;
                    break;
                default:
                    break;
            }
        }
        private void NodeBtn_CheckChanged(object sender, RoutedEventArgs e) {
            bool b = nodeBtn.IsChecked.GetValueOrDefault();
            //popup all other toggle command button
            //if (b && edgeBtn.IsChecked.GetValueOrDefault()) {
            //    edgeBtn.IsChecked = false;

            //}
            pressingBtn(nodeBtn);

            if (b) {
                _currentCmd = "add node";
            } else {
                _currentCmd = "select";
            }

        }

        private void EdgeBtn_CheckChanged(object sender, RoutedEventArgs e) {
            var b = edgeBtn.IsChecked.GetValueOrDefault();

            //popup all other toggle command button
            pressingBtn(edgeBtn);
            //if (b && nodeBtn.IsChecked.GetValueOrDefault()) {
            //    nodeBtn.IsChecked = false;
            //}

            var gv = _graphViewer;                
            gv.InsertingEdge = b;
            if (b) {
                _currentCmd = "add edge";
                gv.LayoutEditor.PrepareForEdgeDragging();

            }    else {
                gv.LayoutEditor.ForgetEdgeDragging();
                _currentCmd = "select";
            }
        }
        void popupOtherBtn_prev(ToggleButton btn) {
            if (btn.IsChecked.GetValueOrDefault()) {
                foreach (var bn in toolBtns) {
                    if (bn == btn)
                        continue;
                    if (bn.IsChecked.Value) {
                        bn.IsChecked = false;
                        return;
                    }
                }
            }

        }
        void pressingBtn(ToggleButton btn) {
            var preBtn = _currentPressedBtn;

            if (preBtn == null) {
                _currentPressedBtn = btn;
                return;
            }
            if(preBtn == btn) {
                var b = btn.IsChecked.Value;
                if(!b)
                    _currentPressedBtn = null;
                return;
            }
            preBtn.IsChecked = false;
            //btn.IsChecked = true;
            _currentPressedBtn = btn;
        }

        private void SelectBtn_CheckChanged(object sender, RoutedEventArgs e) {
            pressingBtn(selectBtn);
            _currentCmd = "select";
        }
        private void ApplyBtn_CheckChanged(object sender, RoutedEventArgs e) {
            pressingBtn(applyBtn);
            if (applyBtn.IsChecked.Value) {
                _currentCmd = "apply attr";
            }
            else {
                _currentCmd = "select";
            }
            
        }

        void graphViewer_MouseDown(object sender, MsaglMouseEventArgs evt) {
            if(_currentCmd == "add node") {
                var gv = _graphViewer;
                string id = null;
                do {
                    id = (++_nodeIdCounter).ToString();
                } while (gv.Graph.FindNode(id) != null);
                var n = gv.Graph.AddNode(id);
                n.Attr = _defaultNodeAttr.Clone();
                n.Attr.Id = id;
                GVH.addVnodeByDnode(gv, n, gv.ScreenToSource(evt));
                
                _mouseEventHandled = true;
            }
            else if(_currentCmd == "node editing") {
                exitNodeEditor();
                _currentCmd = "select";
            }
            else if(_currentCmd == "apply attr") {
                var obj = _graphViewer.ObjectUnderMouseCursor;
                if (obj == null)
                    return;
                var vedge = obj as VEdge;
                if(vedge != null) {
                    var att = vedge.Edge.Attr;
                    if(edge_defApplyAttrPanel != null) {
                        EdgeAttrApplyEditor e = edge_defApplyAttrPanel.Content as EdgeAttrApplyEditor;
                        if (e.TarArrowStyleCmb.IsEnabled) {
                            att.ArrowheadAtTarget = (ArrowStyle)e.TarArrowStyleCmb.SelectedValue;
                        }
                        if (e.SrcArrowStyleCmb.IsEnabled) {
                            att.ArrowheadAtSource = (ArrowStyle)e.SrcArrowStyleCmb.SelectedValue;
                        }
                        if (e.LineStyleCmb.IsEnabled) {
                            att.FirstStyle = (DStyle)e.LineStyleCmb.SelectedValue;
                        }
                        if (e.DecorateSymbolCmb.IsEnabled) {
                            att.DefinedDrawDelegateName = (string)e.DecorateSymbolCmb.SelectedValue;
                        }
                    }
                }
                
            }
        }

        void graphViewer_MouseMove(object sender, MsaglMouseEventArgs e) {
            if (_mouseEventHandled)
                return;
        }

        void graphViewer_MouseUp(object sender, MsaglMouseEventArgs args) {
            if (_mouseEventHandled)
                return;
        }
        FrameworkElement _nodeTextEditor;
        VNode _editingNode;
        private void graphViewer_MouseDoubleClick(object sender, MsaglMouseEventArgs e) {
            if (_currentCmd != "select")
                return;

            var vnode = _graphViewer.ObjectUnderMouseCursor as VNode;
            //var vnode = clickCounter.ClickedObject as IViewerNode;
            if (vnode == null)
                return;
            Debug.Assert(_nodeTextEditor == null);

            var node = vnode.Node;
            var pos = Common.WpfPoint(node.GeometryNode.Center);
            var bound = node.GeometryNode.BoundingBox;
            var textBox = new TextBox();
            _nodeTextEditor = textBox;


            MatrixTransform tr = new MatrixTransform();
            Matrix mm = Matrix.Identity;
            mm.Scale(1, -1);
            tr.Matrix = mm;
            textBox.RenderTransform = tr;
            var text = node.Label.Text;
            textBox.Text = text;
            if (text.Length <= 3)
                textBox.Width = 30;
            else
                textBox.Width = bound.Width;
            textBox.Height = bound.Height;
            Panel.SetZIndex(textBox, 60000);
            textBox.KeyDown += editingNode_editor_keyDown;
            Common.SetFrameworkElementCenter(textBox, pos);
            _graphViewer.GraphCanvas.Children.Add(textBox);
            textBox.Focus();
            //textBox.CaretIndex = textBox.Text.Length;
            textBox.SelectAll();
            _editingNode = vnode;
            _currentCmd = "node editing";
        }
        public void editingNode_editor_keyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                TextBox tb = _nodeTextEditor as TextBox;

                _graphViewer. updateVNodeLabelText(_editingNode as VNode, tb.Text);
                exitNodeEditor();
            }
            else if (e.Key == Key.Escape) {
                exitNodeEditor();
            }
        }
        void exitNodeEditor() {
            _graphViewer.GraphCanvas.Children.Remove(_nodeTextEditor);
            _nodeTextEditor = null;
            _editingNode = null;
        }
        private void SaveBtn_Click(object sender, RoutedEventArgs e) {
            if(_fileName== "") {
                SaveasBtn_Click(sender,e);
            }
            else {
                //make sure file path is valid
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(_fileName)))
                    return;
                _graphViewer.Graph.Write(_fileName);
            }
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e) {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog {
                        RestoreDirectory = true,
                        Filter = "MSAGL Files(*.msagl)|*.msagl" };
   
            if (openFileDialog.ShowDialog().GetValueOrDefault()) {
                loadFile(openFileDialog.FileName);
            }
    
    
        }
        public void loadFile(string filename) {
            try {
                _fileName = filename;
                var gv = _graphViewer;
                gv.NeedToCalculateLayout = false;
                gv.Graph = Graph.Read(_fileName);
                //if (GraphLoadingEnded != null)
                //    GraphLoadingEnded(this, null);
                gv.Graph.GeometryGraph.UpdateBoundingBox();
                gv.SetInitialTransform();
                gv.Transform = gv.Transform;
                this.Title = System.IO.Path.GetFileNameWithoutExtension(_fileName);
            }
                catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveasBtn_Click(object sender, RoutedEventArgs e) {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog { Filter = "MSAGL Files(*.msagl)|*.msagl" };
            try {
                if (saveFileDialog.ShowDialog().GetValueOrDefault()) {
                    _fileName = saveFileDialog.FileName;
                    //if (GraphSavingStarted != null)
                    //    GraphSavingStarted(this, null);
                    _graphViewer.Graph.Write(_fileName);
                    //if (GraphSavingEnded != null)
                    //    GraphSavingEnded(this, null);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        private void EdgeEditor_SrcArrowChanged(object sender, EdgeAttrEditor .ValueChangedEventArgs<ArrowStyle> e) {
            var vedge = _editingObj as VEdge;
            var att = vedge.Edge.Attr;
            att.ArrowheadAtSource = edgeEditor.SrcArrowStyle;

        }

        private void EdgeEditor_TarArrowChanged(object sender, EdgeAttrEditor.ValueChangedEventArgs<ArrowStyle> e) {
            var vedge = _editingObj as VEdge;
            var att = vedge.Edge.Attr;
            // set arrow will auto fire VisualChanged,not need more vedge.Invalidate()
            att.ArrowheadAtTarget = edgeEditor.TarArrowStyle;

        }

        private void EdgeEditor_LineStyleChanged(object sender, EdgeAttrEditor.ValueChangedEventArgs<DStyle> e) {
            var vedge = _editingObj as VEdge;
            var at = vedge.Edge.Attr;
            at.FirstStyle = edgeEditor.LineStyle;
        }

        private void EdgeEditor_DecoratorChanged(object sender, EdgeAttrEditor.ValueChangedEventArgs<string> e) {
            string s = edgeEditor.DecorateSymbol;
            var vedge = _editingObj as VEdge;
            if(s == "") {
                vedge.Edge.UserData = null;
                vedge.Edge.DrawEdgeDelegate = null;
            }
            else {
                vedge.Edge.UserData = s;
                vedge.Edge.DrawEdgeDelegate = vedge.GetDrawDelegate_FromUserData(s);
            }
            vedge.Invalidate();
        }

        private void MenuItem_attr_Click(object sender, RoutedEventArgs e) {
            AttrPane.IsVisible = true;
        }

        private void MenuItem_defAttr_Click(object sender, RoutedEventArgs evt) {
            if(edge_defApplyAttrPanel == null) {
                var p = edge_defApplyAttrPanel = new LayoutAnchorable();
                EdgeAttrApplyEditor e  = new EdgeAttrApplyEditor();
                p.Content = e;
                p.Title = "默认属性";
                //p.Parent = leftAnchorPane;
                leftAnchorPane.Children.Add(p);
                p.CanDockAsTabbedDocument = false;
                
            }
            if (!edge_defApplyAttrPanel.IsVisible) {
                edge_defApplyAttrPanel.IsVisible = true;
            }
                
        }

        private void MenuItem_doc_Click(object sender, RoutedEventArgs e) {

        }


    }
}
