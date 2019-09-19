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


/*親Thumb
    ControlTemplate
        Canvas
            子Thumb ←これは動かせる？
動かせる、けどそのままだと上の親Thumbも同時に動いてしまうので
バブルイベントをキャンセルする必要がある*/
namespace _20190919_thumb4in
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        Thumb MyThumb;
        public MainWindow()
        {
            InitializeComponent();
            MyThumb = new Thumb() { Width = 100, Height = 100 };
            Canvas.SetLeft(MyThumb, 100);Canvas.SetTop(MyThumb, 100);
            FlatThumbCanvasBase(MyThumb, Colors.Cyan);
            MyCanvas.Children.Add(MyThumb);

            MyThumb.DragDelta += MyThumb_DragDelta;
        }

        private void MyThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(MyThumb, Canvas.GetLeft(MyThumb) + e.HorizontalChange);
            Canvas.SetTop(MyThumb, Canvas.GetTop(MyThumb) + e.VerticalChange);
        }

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

            var inside = new Thumb() { Width = 20, Height = 20, Background = Brushes.Red };
            Canvas.SetLeft(inside, 0); Canvas.SetTop(inside, 0);
            myGrid.Children.Add(inside);
            inside.DragDelta += Inside_DragDelta;
        }

        private void FlatThumbCanvasBase(Thumb thumb, Color bgColor)
        {
            //template作成
            ControlTemplate template = new ControlTemplate(typeof(Thumb));

            //Templateの見た目の作成、ベースにはGridではなくCanvas要素を使ったのは
            //子Thumbの移動にはCanvas.Setを使うから
            //任意の名前をつけておくと、これをもとに検索して要素を取得できる
            template.VisualTree = new FrameworkElementFactory(typeof(Canvas), "tempCanvas");
            thumb.Template = template;

            //再構築、これを実行しないと名前検索で取得できない
            thumb.ApplyTemplate();

            //名前検索で取得して背景色に水色指定
            Canvas bCanvas = (Canvas)thumb.Template.FindName("tempCanvas", thumb);
            bCanvas.Background = new SolidColorBrush(bgColor);

            //子Thumbを追加
            var inside = new Thumb() { Width = 20, Height = 20, Background = Brushes.Red };
            Canvas.SetLeft(inside, 0); Canvas.SetTop(inside, 0);
            bCanvas.Children.Add(inside);
            inside.DragDelta += Inside_DragDelta;
        }

//        WPF4.5入門 その46 「WPFのイベントシステム」 - かずきのBlog @hatena
//https://blog.okazuki.jp/entry/2014/08/22/211021

        private void Inside_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb t = (Thumb)sender;
            Canvas.SetLeft(t, Canvas.GetLeft(t) + e.HorizontalChange);
            Canvas.SetTop(t, Canvas.GetTop(t) + e.VerticalChange);
            e.Handled = true;//後続のバブルイベントをキャンセル
        }
    }
}
