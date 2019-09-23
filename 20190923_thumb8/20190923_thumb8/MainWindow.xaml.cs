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

namespace _20190923_thumb8
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private FlatThumb MyThumb;
        private double PastHorizontalChange;
        private double PastVerticalChange;

        public MainWindow()
        {
            InitializeComponent();
            MyThumb = new FlatThumb(Colors.Red);
            MyCanvas.Children.Add(MyThumb);
            MyThumb.Width = 100; MyThumb.Height = 200;
            Canvas.SetLeft(MyThumb, 100);
            Canvas.SetTop(MyThumb, 100);

            MyThumb.DragDelta += MyThumb_DragDelta;
            MyThumb.DragCompleted += MyThumb_DragCompleted;
            MyThumb.MouseMove += MyThumb_MouseMove;

        }

        private void MyThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            PastVerticalChange = 0;
            PastHorizontalChange = 0;
        }



        private void MyThumb_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(MyCanvas);
            double left = Canvas.GetLeft(MyThumb);
            double top = Canvas.GetTop(MyThumb);
            if (point.X > left + MyThumb.Width - 20 && point.Y > top + MyThumb.Height - 20)
            {
                MyThumb.Cursor = Cursors.SizeNWSE;
            }
            else if (point.X > left + MyThumb.Width - 20)
            {
                MyThumb.Cursor = Cursors.SizeWE;
            }
            else if (point.Y > top + MyThumb.Height - 20)
            {
                MyThumb.Cursor = Cursors.SizeNS;
            }
            else { MyThumb.Cursor = Cursors.Arrow; }
        }

        //ドラッグイベント中
        //カーソルの形でサイズ変更と移動を分けている
        //サイズ変更はe.HorizontalChangeやe.VerticalChangeをWidthに加算すると
        //思った以上に伸縮するので、直前の変更量を引き算したのを加算している
        private void MyThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double horizontal = e.HorizontalChange;
            double verchical = e.VerticalChange;

            //横幅変更
            void ChangeWidthSize()
            {
                MyThumb.Width += horizontal - PastHorizontalChange;
                PastHorizontalChange = horizontal;//変更量を記録
            }
            //縦幅変更
            void ChangeHeightSize()
            {
                MyThumb.Height += verchical - PastVerticalChange;
                PastVerticalChange = verchical;
            }

            //カーソルの形で判定
            if (MyThumb.Cursor == Cursors.SizeWE)//左右
            {
                ChangeWidthSize();
            }
            else if (MyThumb.Cursor == Cursors.SizeNS)//上下
            {
                ChangeHeightSize();
            }
            else if (MyThumb.Cursor == Cursors.SizeNWSE)//斜め
            {
                ChangeHeightSize();
                ChangeWidthSize();
            }
            else if (MyThumb.Cursor == Cursors.Arrow)//通常の矢印
            {
                //ドラッグ移動
                Canvas.SetLeft(MyThumb, Canvas.GetLeft(MyThumb) + horizontal);
                Canvas.SetTop(MyThumb, Canvas.GetTop(MyThumb) + verchical);
            }


        }
    }


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