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


namespace _20190920_sizeChange
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        VariableThumb MyThumb;

        public MainWindow()
        {
            InitializeComponent();

            MyButton1.Click += MyButton1_Click;
            MyButton2.Click += MyButton2_Click;

            MyThumb = new VariableThumb(MyCanvas) { Width = 100, Height = 100 };
            MyThumb.SetFillColor(Color.FromArgb(40, 0, 0, 0));
            Canvas.SetLeft(MyThumb, 100); Canvas.SetTop(MyThumb, 100);
            MyCanvas.Children.Add(MyThumb);


            MyThumb.AddMarker();
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            MyThumb.MarkerSize(10, 10);
        }

        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            MyThumb.Width += 10;
        }
    }


    /// <summary>
    /// フラットなThumb、Canvasパネル以外には追加できない
    /// マウスドラッグ移動とサイズ可変
    /// 8つの■マーカーをマウスドラッグ移動でサイズ変更
    /// マーカー表示は、AddMarker()を実行
    /// 
    /// </summary>
    public class VariableThumb : FlatThumb
    {

        private Canvas ParentCanvas;
        private FlatThumb[] Marker8;

        public VariableThumb(Canvas parentCanvas, double width = 100, double height = 100)
        {
            ParentCanvas = parentCanvas;
            this.Width = width;
            this.Height = height;

            //8つのマーカー■作成
            //配列の位置で配置する位置に対応            
            //┏━┓ 3,4,5
            //┃  ┃ 2   6
            //┗━┛ 1,0,7
            //順番は下から時計回りで、下、左下、左、左上、上、右上、右、右下
            //Tagに目印のindex番号を入れる
            Marker8 = new FlatThumb[8];
            for (int i = 0; i < 8; i++)
            {
                var ft = new FlatThumb() { Width = 20, Height = 20, Tag = i };
                ft.SetFillColor(Color.FromArgb(30, 0, 255, 255));
                Marker8[i] = ft;
                ft.DragDelta += MarkerThumb_DragDelta;
                //ft.SetOutLine(Colors.Black, 1);//実線の枠
                //破線の枠
                ft.SetOutLine(Colors.Black, 1, new DoubleCollection(new List<double> { 2, 4 }));
                ft.Cursor = Cursors.Hand;
            }

            this.DragDelta += VariableThumb_DragDelta;
            this.SizeChanged += VariableThumb_SizeChanged;

            //AddThumbs8();
            //Thumbs8SetLocate();
        }

        private void VariableThumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //8つのマーカーThumbの位置変更
            MarkerSetLocate();
        }

        private void VariableThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //ドラッグ移動
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
            MarkerSetLocate();
        }

        //Thumbのサイズと位置からマーカーの位置を求める
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

        //マーカーの表示位置の更新
        public void MarkerSetLocate()
        {
            Point[] points = MakeThumbs8Locate();
            for (int i = 0; i < 8; i++)
            {
                CanvasSetPoint(Marker8[i], points[i]);
            }
        }

        private void CanvasSetPoint(Thumb thumb, Point point)
        {
            Canvas.SetLeft(thumb, point.X);
            Canvas.SetTop(thumb, point.Y);
        }

        //マーカーをThumbに表示
        public void AddMarker()
        {
            if (ParentCanvas.Children.Contains(Marker8[0])) return;
            for (int i = 0; i < 8; i++)
            {
                ParentCanvas.Children.Add(Marker8[i]);
            }
        }

        //マーカーを非表示というか除去
        public void RemoveMarker()
        {
            for (int i = 0; i < 8; i++)
            {
                ParentCanvas.Children.Remove(Marker8[i]);
            }
        }

        //マーカーのサイズ変更
        public void MarkerSize(double width, double height)
        {
            foreach (var item in Marker8)
            {
                item.Width = width;
                item.Height = height;
            }
        }

        //マーカーのドラッグ移動時
        //Thumbのサイズと位置を更新する
        private void MarkerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FlatThumb MarkerThumb = (FlatThumb)sender;
            double w = this.Width;
            double h = this.Height;
            double l = Canvas.GetLeft(this);
            double t = Canvas.GetTop(this);
            double xMove = e.HorizontalChange;
            double yMove = e.VerticalChange;

            //Thumbの横幅と横位置を計算
            void yoko()
            {
                if (w - xMove < 1)//幅1ピクセル未満になる場合
                {
                    l = l + w - 1;//幅1ピクセルの位置で固定
                    w = 1;//幅1ピクセルを指定
                }
                else
                {
                    l += xMove;
                    w -= xMove;
                }
            }
            //縦幅と位置を計算、1ピクセル未満にならないよう修正
            void tate()
            {
                if (h - yMove < 1)
                {
                    t = t + h - 1;
                    h = 1;
                }
                else
                {
                    t += yMove;
                    h -= yMove;
                }
            }

            //Thumbの位置とサイズ変更する
            switch (MarkerThumb.Tag)
            {
                case 0://下
                    h += yMove;
                    break;
                case 1:
                    yoko();
                    h += yMove;
                    break;
                case 2://左
                    yoko();
                    break;
                case 3://左上
                    yoko();
                    tate();
                    break;
                case 4://上
                    tate();
                    break;
                case 5:
                    w += xMove;
                    tate();
                    break;
                case 6://右
                    w += xMove;
                    break;
                case 7:
                    w += xMove;
                    h += yMove;
                    break;
                default:
                    break;
            }
            //1ピクセル未満にならないよう修正
            if (w < 0) w = 1;
            if (h < 0) h = 1;

            //Thumbのサイズと位置を指定
            this.Width = w;
            this.Height = h;
            Canvas.SetLeft(this, l);
            Canvas.SetTop(this, t);

        }

    }




    /// <summary>
    /// フラットなThumb、枠の表示ができる
    /// ControlTemplateにRectangleを使っている
    /// </summary>
    public class FlatThumb : Thumb
    {
        private Rectangle MyRectangle;
        public FlatThumb()
        {
            //template作成
            ControlTemplate template = new ControlTemplate(typeof(Thumb));

            //Templateの見た目の作成、ベースはRectangleに変更した
            //任意の名前をつけておくと、これをもとに検索して要素を取得できる
            template.VisualTree = new FrameworkElementFactory(typeof(Rectangle), "tempRectangle");
            this.Template = template;

            this.ApplyTemplate();//再構築、これで名前検索で取得できる

            //名前検索で取得して塗りつぶしの色を指定
            MyRectangle = (Rectangle)this.Template.FindName("tempRectangle", this);
            MyRectangle.Fill = new SolidColorBrush(SystemColors.ControlColor);// Color.FromArgb(100, 0, 0, 0));
        }

        /// <summary>
        /// 枠線の表示、破線の枠の色と太さ指定、非表示は色をTransparentにするか幅を0
        /// </summary>
        /// <param name="color">枠の色</param>
        /// <param name="lineWidth">枠線の太さ</param>
        /// <param name="dashArray">枠線の設定、nullで実線、破線はnew DoubleCollection(new List＜double＞ { 2, 4 })とか</param>
        public void SetOutLine(Color color, double lineWidth, DoubleCollection dashArray = null)
        {
            MyRectangle.StrokeDashArray = dashArray;
            MyRectangle.StrokeThickness = lineWidth;
            MyRectangle.Stroke = new SolidColorBrush(color);
        }

        public void SetFillColor(Color color)//塗りつぶしの色
        {
            MyRectangle.Fill = new SolidColorBrush(color);
        }
    }
}
