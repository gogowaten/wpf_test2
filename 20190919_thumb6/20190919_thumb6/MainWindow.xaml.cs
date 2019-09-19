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

//カーソルの形を矢印に変えてサイズ変化
//できなかった
//右か下要素があるドラッグすると、グイーンって伸びすぎ
//これが解消できない
//どうやらdragdeltaイベントのマウスの移動量は
//要素のサイズ変更されるとずれてしまうみたい
//やっぱり8隅にそれぞれThumbを表示するのが良さそう
//これを透明にすればそれっぽく見えるかも

namespace _20190919_thumb6
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        FlatThumb MyThumb;

        public MainWindow()
        {
            InitializeComponent();

            MyThumb = new FlatThumb(new Size(100, 100), new Point(100, 100), Colors.Cyan);
            MyCanvas.Children.Add(MyThumb);

        }

    }

    public class FlatThumb : Thumb
    {
        private Grid MyBaseGrid;
        private const int cMargin = 20;
        private CursorDirection MyCursorDirection;

        enum CursorDirection
        {
            None = 0,
            Noth,
            NothEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NothWest
        }
        public FlatThumb(Size size, Point localte, Color bgColor)
        {
            
            this.Width = size.Width;
            this.Height = size.Height;
            Canvas.SetLeft(this, localte.X);
            Canvas.SetTop(this, localte.Y);
            SetTemplate(bgColor);

            
            this.MouseMove += FlatThumb_MouseMove;
            this.DragDelta += FlatThumb_DragDelta;
        }

     
        //サイズ変更
        private void FlatThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {

            if (MyCursorDirection == CursorDirection.None)
            {
                Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
                Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
            }
            else
            {
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
                switch (MyCursorDirection)
                {
                    case CursorDirection.None:
                        break;
                    case CursorDirection.Noth:
                        tate();
                        break;
                    case CursorDirection.NothEast:
                        w += xMove;
                        tate();
                        break;
                    case CursorDirection.East:
                        w += xMove;
                        break;
                    case CursorDirection.SouthEast:
                        w += xMove;
                        h += yMove;
                        break;
                    case CursorDirection.South:
                        h += yMove;
                        break;
                    case CursorDirection.SouthWest:
                        yoko();
                        h += yMove;
                        break;
                    case CursorDirection.West:
                        yoko();
                        break;
                    case CursorDirection.NothWest:
                        yoko();
                        tate();
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
            }
        }

        //カーソルの見た目変更
        private void FlatThumb_MouseMove(object sender, MouseEventArgs e)
        {
            
            Point p = e.GetPosition(this);
            bool isNoth = false, isSouth = false, isEast = false, isWest = false;
            if (p.X < cMargin) isWest = true;
            if (p.X > Width - cMargin) isEast = true;
            if (p.Y < cMargin) isNoth = true;
            if (p.Y > Height - cMargin) isSouth = true;

            if (isNoth)//北
            {
                Cursor = Cursors.SizeNS;
                MyCursorDirection = CursorDirection.Noth;
                if (isWest)//北西
                {
                    Cursor = Cursors.SizeNWSE;
                    MyCursorDirection = CursorDirection.NothWest;
                }
                else if (isEast)//北東
                {
                    Cursor = Cursors.SizeNESW;
                    MyCursorDirection = CursorDirection.NothEast;
                }
            }
            else if (isSouth)//南
            {
                Cursor = Cursors.SizeNS;
                MyCursorDirection = CursorDirection.South;
                if (isWest)//南西
                {
                    Cursor = Cursors.SizeNESW;
                    MyCursorDirection = CursorDirection.SouthWest;
                }
                else if (isEast)//南東
                {
                    Cursor = Cursors.SizeNWSE;
                    MyCursorDirection = CursorDirection.SouthEast;
                }
            }
            else if (isWest)//西
            {
                Cursor = Cursors.SizeWE;
                MyCursorDirection = CursorDirection.West;
            }
            else if (isEast)//東
            {
                Cursor = Cursors.SizeWE;
                MyCursorDirection = CursorDirection.East;
            }
            else//その他
            {
                Cursor = Cursors.Arrow;
                MyCursorDirection = CursorDirection.None;
            }
        }

        private void SetTemplate(Color bgColor)
        {
            var template = new ControlTemplate(typeof(Thumb));
            template.VisualTree = new FrameworkElementFactory(typeof(Grid), "templatePanel");
            this.Template = template;
            this.ApplyTemplate();
            MyBaseGrid = (Grid)template.FindName("templatePanel", this);
            MyBaseGrid.Background = new SolidColorBrush(bgColor);
        }


    }
}
