using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


//Thumbの右端や下側をドラッグ移動でサイズ変更
namespace _20190923_thumbサイズ変更
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private double PastHorizontalChange;//直前の横幅変更量記録用
        private double PastVerticalChange;//縦幅用
        private double MyPadding = 20;//Thumb上でマウスカーソルの形を矢印にする範囲
        public MainWindow()
        {
            InitializeComponent();

            MyThumb.DragDelta += MyThumb_DragDelta;
            MyThumb.DragCompleted += MyThumb_DragCompleted;
            MyThumb.MouseMove += MyThumb_MouseMove;
        }

        //マウス移動イベント時
        //マウスの位置によって形状を変える
        private void MyThumb_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(MyCanvas);
            double left = Canvas.GetLeft(MyThumb);
            double top = Canvas.GetTop(MyThumb);
            double w = MyThumb.Width;
            double h = MyThumb.Height;

            if (p.X > left + w - MyPadding && p.Y > top + h - MyPadding)
            {
                MyThumb.Cursor = Cursors.SizeNWSE;//右下のときは斜め＼
            }
            else if (p.X > left + w - MyPadding)
            {
                MyThumb.Cursor = Cursors.SizeWE;//右側なら左右矢印↔
            }
            else if (p.Y > top + h - MyPadding)
            {
                MyThumb.Cursor = Cursors.SizeNS;//下側なら上下矢印↕
            }
            else
            {
                MyThumb.Cursor = Cursors.Arrow;
            }

        }

        //ドラッグイベント終了時
        //直前の変更量をリセット
        private void MyThumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            PastHorizontalChange = 0;
            PastVerticalChange = 0;
        }

        //ドラッグイベント中
        //カーソルの形でサイズ変更と移動を分けている
        //サイズ変更はe.HorizontalChangeやe.VerticalChangeをWidthに加算すると
        //思った以上に伸縮するので、直前の変更量を引き算したのを加算している
        private void MyThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {            
            //横幅変更
            void ChangeWidthSize()
            {
                //変更後の幅が小さくなりすぎないように調整
                //もし、小さすぎなら変更しない
                double w = MyThumb.Width + e.HorizontalChange - PastHorizontalChange;
                if (MyPadding > w)
                {
                    PastHorizontalChange = 0;
                }
                else
                {
                    MyThumb.Width = w;//変更
                    PastHorizontalChange = e.HorizontalChange;//変更量を記録
                }
            }
            //縦幅変更
            void ChangeHeightSize()
            {
                double h = MyThumb.Height + e.VerticalChange - PastVerticalChange;
                if (MyPadding > h)
                {
                    PastVerticalChange = 0;
                }
                else
                {
                    MyThumb.Height = h;
                    PastVerticalChange = e.VerticalChange;
                }
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
                Canvas.SetLeft(MyThumb, Canvas.GetLeft(MyThumb) + e.HorizontalChange);
                Canvas.SetTop(MyThumb, Canvas.GetTop(MyThumb) + e.VerticalChange);
            }
        }
    }
}
