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

namespace _20190920_thumb7
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private VariableThumb MyThumb;

        public MainWindow()
        {
            InitializeComponent();

            MyThumb = new VariableThumb(MyCanvas, Colors.Pink, new Size(100, 100));
            MyCanvas.Children.Add(MyThumb);
            Canvas.SetLeft(MyThumb, 300); Canvas.SetTop(MyThumb, 200);
            MyThumb.AddThumbs8();
            MyThumb.Thumbs8SetLocate();

            MyButton1.Click += MyButton1_Click;
            MyButton2.Click += MyButton2_Click;
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            MyThumb.AddThumbs8();
        }

        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            MyThumb.RemoveThumbs8();
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

        public VariableThumb(Canvas parentCanvas, Color bgColor, Size size) : base(bgColor)
        {
            ParentCanvas = parentCanvas;
            this.Width = size.Width;
            this.Height = size.Height;

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
                var ft = new FlatThumb(Colors.Cyan) { Width = 20, Height = 20, Tag = i };
                Thumbs8[i] = ft;
                ft.DragDelta += DirectionThumb_DragDelta;
                ft.SetOutLine(Colors.Black, 1);
                ft.Cursor = Cursors.Hand;
            }

            this.DragDelta += VariableThumb_DragDelta;

        }

        private void VariableThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
            Thumbs8SetLocate();
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
            if (ParentCanvas.Children.Contains(Thumbs8[0])) return ;
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


        private void DirectionThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FlatThumb flatThumb = (FlatThumb)sender;
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
            //縦幅と位置を修正
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

            //自身の位置とサイズ変更する
            switch (flatThumb.Tag)
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

            if (w < 0) w = 1;
            if (h < 0) h = 1;

            //Thumbのサイズと位置を指定
            this.Width = w;
            this.Height = h;
            Canvas.SetLeft(this, l);
            Canvas.SetTop(this, t);

            //8つのThumbの位置変更
            Thumbs8SetLocate();

        }

    }

    //public class FlatThumb : Thumb
    //{
    //    private Grid MyGrid;
    //    private Size MySize;
    //    private Border MyBorder;
        
    //    public FlatThumb(Color bgColor, Size size)
    //    {
    //        MySize = size;
    //        SetTemplate(bgColor);
    //    }
    //    private void SetTemplate(Color bgColor)
    //    {
    //        //template作成
    //        ControlTemplate template = new ControlTemplate(typeof(Thumb));

    //        //Templateの見た目の作成、ベースにはGrid要素を使った
    //        //任意の名前をつけておくと、これをもとに検索して要素を取得できる
    //        template.VisualTree = new FrameworkElementFactory(typeof(Grid), "tempGrid");
    //        this.Template = template;

    //        this.ApplyTemplate();//再構築、これで名前検索で取得できる

    //        //名前検索で取得して背景色指定
    //        MyGrid = (Grid)this.Template.FindName("tempGrid", this);
    //        MyGrid.Background = new SolidColorBrush(bgColor);

    //        //枠表示用Border
    //        MyBorder = new Border()
    //        {
    //            Width = MySize.Width,
    //            Height = MySize.Height,
    //            Background = new SolidColorBrush(bgColor)
    //        };
    //        MyGrid.Children.Add(MyBorder);


    //    }

    //    //枠の色と太さ指定
    //    public void SetOutLine(Color color, double lineWidth)
    //    {
    //        MyBorder.BorderThickness = new Thickness(lineWidth);
    //        MyBorder.BorderBrush = new SolidColorBrush(color);
    //    }
    //}

    public class FlatThumb : Thumb
    {
        
        private Rectangle MyRectangle;
        public FlatThumb(Color bgColor)
        {            
            SetTemplate(bgColor);
        }
        private void SetTemplate(Color bgColor)
        {
            //template作成
            ControlTemplate template = new ControlTemplate(typeof(Thumb));

            //Templateの見た目の作成、ベースはRectangleに変更した
            //任意の名前をつけておくと、これをもとに検索して要素を取得できる
            template.VisualTree = new FrameworkElementFactory(typeof(Rectangle), "tempRectangle");
            this.Template = template;

            this.ApplyTemplate();//再構築、これで名前検索で取得できる

            //名前検索で取得して背景色指定
            MyRectangle = (Rectangle)this.Template.FindName("tempRectangle", this);
            MyRectangle.Fill = new SolidColorBrush(bgColor);
        }

        //破線の枠の色と太さ指定、非表示は色をTransparentにするか幅を0
        //実線にするにはdoublecollectionを0,1とかにすればいいけど
        public void SetOutLine(Color color, double lineWidth)
        {
            MyRectangle.StrokeDashArray = new DoubleCollection(new List<double> { 4, 2 });
            MyRectangle.StrokeThickness = lineWidth;
            MyRectangle.Stroke = new SolidColorBrush(color);
        }

    }





}
