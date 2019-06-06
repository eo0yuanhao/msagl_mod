using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
//using System.Windows.Input;

namespace IRxMap {
    using GVH = GraphViewerHelper;
    using CE = CommandEnvironment;
    interface ICommand {
        void Initilize(object o);
        void Execute(object o);
    }
    interface ITool : ICommand {
        void Preparation();
        void Finish();
    }
    public class CommandEnvironment{
        static GraphViewer gv;
        static MainWindow mw;
        public void init(GraphViewer g, MainWindow m) {
            gv = g;
            mw = m;
        }
        public static GraphViewer GraphViewer {
            get => gv;
        }
        public static MainWindow MainWindow {
            get => mw;
        }
    }

    public class AddNodeTool : ITool {
        GraphViewer gv;
        public void Execute(object o) {
            var e = o as MsaglMouseEventArgs;
            var id = (++CE.MainWindow._nodeIdCounter).ToString();
            var n = gv.Graph.AddNode(id);
            n.Attr =  CE.MainWindow._defaultNodeAttr.Clone();
            n.Attr.Id = id;
            GVH.addVnodeByDnode(gv, n, gv.ScreenToSource(e));
            //_mouseEventHandled = true;
        }

        public void Finish() {
        }

        public void Initilize(object o) {
        }

        public void Preparation() {
        }
    }

}
