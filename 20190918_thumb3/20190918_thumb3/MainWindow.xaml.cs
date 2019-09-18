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
using System.Windows.Controls.Primitives;


namespace _20190918_thumb3
{

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thumb MyThumb;
        private My8Point My8;
        //private MySizeChangePointThumb[] mySizeChangePointThumbs;
        //private Thumb Trim1;
        //private List<Thumb> MySizeChangePoints;
        //private readonly string[] MY_POINT_NAME = new string[] { "Bottom", "BottomL", "L", "TopL", "Top", "TopR", "R", "BottomR" };

        public MainWindow()
        {
            InitializeComponent();

            VariableThumb variableThumb = new VariableThumb(MyCanvas, Colors.Pink, 100, 100);
            MyCanvas.Children.Add(variableThumb);
            Canvas.SetLeft(variableThumb, 300); Canvas.SetTop(variableThumb, 200);
            variableThumb.AddThumbs8();
            variableThumb.Thumbs8SetLocate();


            //Thumb作成してCanvasに追加
            MyThumb = new Thumb { Width = 150, Height = 100, };
            Canvas.SetLeft(MyThumb, 100);
            Canvas.SetTop(MyThumb, 100);
            MyCanvas.Children.Add(MyThumb);
            MyThumb.DragDelta += MyThumb_DragDelta;

            //Thumbの見た目変更
            FlatThumb(MyThumb, Colors.Cyan);

            //MyPoint8 myPoint8=new MyPoint8(MyThumb,20,Colors.Red,loca)
            //Trim1 = new MySizeChangePointThumb(MyThumb, 20, Colors.Red);

            //Trim1 = new Thumb { Width = 20, Height = 20 };
            //Canvas.SetLeft(Trim1, 100);
            //Canvas.SetTop(Trim1, 100);
            //MyCanvas.Children.Add(Trim1);
            //Trim1.DragDelta += Trim1_DragDelta;
            //FlatThumb(Trim1, Colors.Red);


            //MyThumb.Cursor = Cursors.SizeWE;
            this.Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            My8 = new My8Point(MyThumb, 20, Colors.Red, MyCanvas);
            //Set8Point(MyThumb, 20, Colors.Red);

        }
        //public void Set8Point(Thumb parentThumb, double size, Color bgColor)
        //{
        //    mySizeChangePointThumbs = new MySizeChangePointThumb[8];

        //    for (int i = 0; i < 8; i++)
        //    {
        //        var pThumb = new MySizeChangePointThumb(parentThumb, size, bgColor,
        //            (LocateDirection)Enum.GetValues(typeof(LocateDirection)).GetValue(i));
        //        mySizeChangePointThumbs[i] = pThumb;
        //        MyCanvas.Children.Add(pThumb);
        //    }
        //}


        /// <summary>
        /// Thumbの見た目変更、ControlTemplateを使って、フラットなThumbにする
        /// </summary>
        /// <param name="thumb"></param>
        /// <param name="bgColor">背景色</param>
        private void FlatThumb(Thumb thumb, Color bgColor)
        {
            //template作成
            ControlTemplate template = new ControlTemplate(typeof(Thumb));

            //Templateの見た目の作成、ベースにはGrid要素を使った
            //任意の名前をつけておくと、これをもとに検索して要素を取得できる
            template.VisualTree = new FrameworkElementFactory(typeof(Grid), "tempGrid");
            thumb.Template = template;

            //再構築、これを実行しないと名前検索で取得できない
            thumb.ApplyTemplate();

            //名前検索で取得して背景色に水色指定
            Grid myGrid = (Grid)thumb.Template.FindName("tempGrid", thumb);
            myGrid.Background = new SolidColorBrush(bgColor);
        }

        //DragDeltaイベントで移動
        private void MyThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var xMove = e.HorizontalChange;
            var yMove = e.VerticalChange;
            //移動(位置指定)
            //現在地にマウスの移動量を足したものが移動先
            Canvas.SetLeft(MyThumb, Canvas.GetLeft(MyThumb) + xMove);
            Canvas.SetTop(MyThumb, Canvas.GetTop(MyThumb) + yMove);

            //サイズ変更の■も移動
            My8.SetLocate();
            //foreach (var item in mySizeChangePointThumbs)
            //{
            //    Canvas.SetLeft(item, Canvas.GetLeft(item) + xMove);
            //    Canvas.SetTop(item, Canvas.GetTop(item) + yMove);
            //}
        }

    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////
    /// </summary>
    public class VariableThumb : FlatThumb
    {
        //サイズ可変のThumb、8つのポイントを持つ
        //FlatThumb MyThumb;
        private Canvas ParentCanvas;
        private FlatThumb[] Thumbs8;

        public VariableThumb(Canvas parentCanvas, Color bgColor, double width, double height) : base(bgColor)
        {
            ParentCanvas = parentCanvas;
            this.Width = width;
            this.Height = height;

            //8つの■作成
            //配列の位置で配置する位置に対応            
            //┏━┓ 3,4,5
            //┃  ┃ 2   6
            //┗━┛ 1,0,7
            //順番は下から時計回りで、下、左下、左、左上、上、右上、右、右下
            //Tagに目印のindex番号を入れる
            Thumbs8 = new FlatThumb[8];
            for (int i = 0; i < 8; i++)
            {
                var ft = new FlatThumb(Colors.DarkOrange) { Width = 20, Height = 20, Tag = i };
                Thumbs8[i] = ft;
                ft.DragDelta += DirectionThumb_DragDelta;
            }

        }

        public Point[] MakeThumbs8Locate()
        {
            double left = Canvas.GetLeft(this);
            double top = Canvas.GetTop(this);
            double w = this.Width;
            double h = this.Height;

            Point[] points = new Point[] {
                new Point(left + (w / 2.0), top + h),
                new Point(left, top + h),
                new Point(left, top + (h / 2.0)),
                new Point(left, top),
                new Point(left + (w / 2.0), top),
                new Point(left + w, top),
                new Point(left + w, top + (h / 2.0)),
                new Point(left + w, top + h) };

            return points;

        }
        private void CanvasSetPoint(Thumb thumb, Point point)
        {
            Canvas.SetLeft(thumb, point.X);
            Canvas.SetTop(thumb, point.Y);
        }
        public void AddThumbs8()
        {
            for (int i = 0; i < 8; i++)
            {
                ParentCanvas.Children.Add(Thumbs8[i]);
            }
        }
        public void Thumbs8SetLocate()
        {
            Point[] points = MakeThumbs8Locate();
            for (int i = 0; i < 8; i++)
            {
                CanvasSetPoint(Thumbs8[i], points[i]);
            }
        }
        public void RemoveThumbs8()
        {
            for (int i = 0; i < 8; i++)
            {
                ParentCanvas.Children.Remove(Thumbs8[i]);
            }
        }
        public void VisibleThumbs8()
        {
            foreach (var item in Thumbs8)
            {
                item.Visibility = Visibility.Visible;
            }
        }
        public void HiddenThumbs8()
        {
            foreach (var item in Thumbs8)
            {
                item.Visibility = Visibility.Hidden;
            }
        }

        private void DirectionThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FlatThumb flatThumb = (FlatThumb)sender;
            double w = this.Width;
            double h = this.Height;
            double l = Canvas.GetLeft(this);
            double t = Canvas.GetTop(this);
            double xMove = e.HorizontalChange;
            double yMove = e.VerticalChange;

            //自身の位置とサイズ変更する
            switch (flatThumb.Tag)
            {
                case 0:

                    break;
                case 1:

                    break;
                case 2:

                    break;
                case 3://左上
                    w -= xMove;
                    h -= yMove;
                    l += xMove;
                    t += yMove;
                    break;
                case 4:

                    break;
                case 5:

                    break;
                case 6:

                    break;
                case 7:

                    break;
                default:
                    break;
            }
            Canvas.SetLeft(this, l);
            Canvas.SetTop(this, t);
            this.Width = w;
            this.Height = h;
            Thumbs8SetLocate();

        }

    }

    public class FlatThumb : Thumb
    {
        public FlatThumb(Color bgColor)
        {
            SetTemplate(bgColor);
        }
        private void SetTemplate(Color bgColor)
        {
            //template作成
            ControlTemplate template = new ControlTemplate(typeof(Thumb));

            //Templateの見た目の作成、ベースにはGrid要素を使った
            //任意の名前をつけておくと、これをもとに検索して要素を取得できる
            template.VisualTree = new FrameworkElementFactory(typeof(Grid), "tempGrid");
            this.Template = template;

            //再構築、これを実行しないと名前検索で取得できない
            this.ApplyTemplate();

            //名前検索で取得して背景色に水色指定
            Grid myGrid = (Grid)this.Template.FindName("tempGrid", this);
            myGrid.Background = new SolidColorBrush(bgColor);
            //FlatDirectionThumb flatDirectionThumb = new FlatDirectionThumb(Colors.Red);

        }
    }
    //public class FlatDirectionThumb : FlatThumb
    //{
    //    //public enum LocateDirection
    //    //{
    //    //    Bottom,
    //    //    BottomLeft,
    //    //    Left,
    //    //    TopLeft,
    //    //    Top,
    //    //    TopRight,
    //    //    Right,
    //    //    BottomRight
    //    //}
    //    LocateDirection MyDirection;

    //    public FlatDirectionThumb(Color bgColor, LocateDirection locateDirection) : base(bgColor)
    //    {
    //        this.MyDirection = locateDirection;
    //        this.Width = 20; this.Height = 20;
    //    }
    //}








    public class MySizeChangePointThumb : Thumb
    {
        public enum LocateDirection
        {
            Bottom,
            BottomLeft,
            Left,
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight
        }

        LocateDirection MyDirection;//位置、方角
        Canvas ParentCanvas;//サイズ変更ThumbのParentのCanvas
        Thumb MyThumb;//サイズ変更の対象になる要素
        Thumb[] My8;
        //Canvas
        //  ┣MyThumb
        //  ┗My8(Thumb x 8個)

        public MySizeChangePointThumb(Thumb parentElement, double size, Color bgColor, LocateDirection direction, Canvas myCanvas)
        {
            ParentCanvas = myCanvas;
            MyThumb = parentElement;
            this.Width = size;
            this.Height = size;
            this.MyDirection = direction;

            SetLocate();//初期座標
            SetTemplate(bgColor);//Template作成

            this.DragDelta += MySizeChangePointThumb_DragDelta;
        }

        public void SetLocate()
        {
            var top = Canvas.GetTop(MyThumb);
            var bottom = top + MyThumb.Height;
            var left = Canvas.GetLeft(MyThumb);
            var right = left + MyThumb.Width;
            var w = MyThumb.Width;
            var h = MyThumb.Height;

            switch (MyDirection)
            {
                case LocateDirection.Bottom:
                    SetTopLeft(left + (w / 2.0), bottom);
                    break;
                case LocateDirection.BottomLeft:
                    SetTopLeft(left, bottom);
                    break;
                case LocateDirection.Left:
                    SetTopLeft(left, top + (h / 2.0));
                    break;
                case LocateDirection.TopLeft:
                    SetTopLeft(left, top);
                    break;
                case LocateDirection.Top:
                    SetTopLeft(left + (w / 2.0), top);
                    break;
                case LocateDirection.TopRight:
                    SetTopLeft(right, top);
                    break;
                case LocateDirection.Right:
                    SetTopLeft(right, top + (h / 2.0));
                    break;
                case LocateDirection.BottomRight:
                    SetTopLeft(right, bottom);
                    break;
                default:
                    break;
            }
        }
        private void SetTopLeft(double x, double y)
        {
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }
        private void MySizeChangePointThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //親Thumbの位置とサイズを変更して、それに合わせて各■も移動させる

            var mt = Canvas.GetTop(MyThumb);
            var mb = mt + MyThumb.Height;
            var ml = Canvas.GetLeft(MyThumb);
            var mr = ml + MyThumb.Width;


            //左上のThumb
            //上に移動ならMyThumbを上に広げるので、実際は上に移動＋縦サイズ増大
            //下、下移動＋縦減少
            //右、右移動＋幅減少
            //左、左移動＋幅増大

            var vc = e.VerticalChange;
            var hc = e.HorizontalChange;
            double width = 0, height = 0;
            //double left = 0, top = 0;

            switch (MyDirection)
            {
                case LocateDirection.Bottom:
                    break;
                case LocateDirection.BottomLeft:
                    break;
                case LocateDirection.Left:
                    break;
                case LocateDirection.TopLeft:
                    width = -hc; height = -vc;
                    //left = hc;top = vc;
                    break;
                case LocateDirection.Top:
                    break;
                case LocateDirection.TopRight:
                    break;
                case LocateDirection.Right:
                    break;
                case LocateDirection.BottomRight:
                    break;
                default:
                    break;
            }
            MyThumb.Width += width;
            MyThumb.Height += height;
            //Canvas.SetLeft(MyThumb, ml + left);
            //Canvas.SetTop(MyThumb, mt + top);

            //my8Point.SetLocate();

        }

        private void SetTemplate(Color bgColor)
        {
            //template作成
            ControlTemplate template = new ControlTemplate(typeof(Thumb));

            //Templateの見た目の作成、ベースにはGrid要素を使った
            //任意の名前をつけておくと、これをもとに検索して要素を取得できる
            template.VisualTree = new FrameworkElementFactory(typeof(Grid), "tempGrid");
            this.Template = template;

            //再構築、これを実行しないと名前検索で取得できない
            this.ApplyTemplate();

            //名前検索で取得して背景色に水色指定
            Grid myGrid = (Grid)this.Template.FindName("tempGrid", this);
            myGrid.Background = new SolidColorBrush(bgColor);

        }
    }

    public class My8Point
    {
        private MySizeChangePointThumb[] My8;
        Thumb MyThumb;//サイズ変更の対象になる要素


        public My8Point(Thumb parentThumb, double size, Color bgColor, Canvas myCanvas)
        {
            My8 = new MySizeChangePointThumb[8];
            for (int i = 0; i < 8; i++)
            {
                //My8[i] = new MySizeChangePointThumb(parentThumb, size, bgColor,
                //    (LocateDirection)Enum.GetValues(typeof(LocateDirection)).GetValue(i), this);
                //myCanvas.Children.Add(My8[i]);
            }
        }

        public void SetLocate()
        {

            foreach (var item in My8)
            {
                item.SetLocate();
            }
        }

    }
}
