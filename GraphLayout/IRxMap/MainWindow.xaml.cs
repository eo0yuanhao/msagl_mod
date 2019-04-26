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
    public partial class MainWindow : Window {
        DockPanel panel = new DockPanel();
        public MainWindow() {
            InitializeComponent();
            Content = panel;

            Loaded += window_loaded;

        }


        public void window_loaded(object sender, RoutedEventArgs e) 
        {

            GraphViewer graphViewer = new GraphViewer();
            graphViewer.BindToPanel(panel);
            Graph graph = new Graph();
            graph.Attr.LayerDirection = LayerDirection.LR;


            graph.AddEdge("A", "B");
            Edge edge;
            graph.FindNode("B").Attr.Shape = Shape.House;
            edge=graph.AddEdge("B", "c");
            edge.Attr.ArrowheadAtTarget = ArrowStyle.Diamond;
            edge=graph.AddEdge("d", "e");
            edge.Attr.ArrowheadAtTarget = ArrowStyle.Generalization;
            edge = graph.AddEdge("e", "f");
            edge.Attr.ArrowheadAtTarget = ArrowStyle.ODiamond;

            edge = graph.AddEdge("g", "h");
            edge.Attr.ArrowheadAtTarget = ArrowStyle.Tee;

            graphViewer.Graph = graph; // throws exception
            //graphViewer.Invalidate();
        }
    }
}
