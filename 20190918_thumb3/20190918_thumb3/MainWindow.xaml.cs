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
        private Thumb Trim1;

        public MainWindow()
        {
            InitializeComponent();

            //Thumb作成してCanvasに追加
            MyThumb = new Thumb { Width = 150, Height = 100, };
            Canvas.SetLeft(MyThumb, 100);
            Canvas.SetTop(MyThumb, 100);
            MyCanvas.Children.Add(MyThumb);
            MyThumb.DragDelta += MyThumb_DragDelta;

            //Thumbの見た目変更
            FlatThumb(MyThumb, Colors.Cyan);



            Trim1 = new Thumb { Width = 20, Height = 20 };
            Canvas.SetLeft(Trim1, 100);
            Canvas.SetTop(Trim1, 100);
            MyCanvas.Children.Add(Trim1);
            Trim1.DragDelta += Trim1_DragDelta;
            FlatThumb(Trim1, Colors.Red);


            MyThumb.Cursor = Cursors.SizeWE;

        }

        private void Trim1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var mt = Canvas.GetTop(MyThumb);
            var mb = mt + MyThumb.Height;
            var ml = Canvas.GetLeft(MyThumb);
            var mr = ml + MyThumb.Width;

            Thumb t = (Thumb)sender;
            var x = Canvas.GetLeft(t) + e.HorizontalChange;
            var y = Canvas.GetTop(t) + e.VerticalChange;
            if (x > mr) { x = mr; }
            if (x < ml) { x = ml; }
            if (y > mb) { y = mb; }
            if (y < mt) { y = mt; }

            Canvas.SetLeft(t, x);
            Canvas.SetTop(t, y);
        }

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
            //移動(位置指定)
            //現在地にマウスの移動量を足したものが移動先
            Canvas.SetLeft(MyThumb, Canvas.GetLeft(MyThumb) + e.HorizontalChange);
            Canvas.SetTop(MyThumb, Canvas.GetTop(MyThumb) + e.VerticalChange);
        }
    }
}
