using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;


//ControlTemplateを使ってフラットなThumb作成

namespace _20191918_thumb2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thumb MyThumb;
        public MainWindow()
        {
            InitializeComponent();

            //Thumb作成してCanvasに追加
            MyThumb = new Thumb
            {
                Width = 150,
                Height = 100,
            };
            Canvas.SetLeft(MyThumb, 100);
            Canvas.SetTop(MyThumb, 100);
            MyCanvas.Children.Add(MyThumb);
            MyThumb.DragDelta += MyThumb_DragDelta;

            //Thumbの見た目変更
            FlatThumb(MyThumb);
        }

        /// <summary>
        /// Thumbの見た目変更、ControlTemplateを使って、水色のフラットなThumbにする
        /// </summary>
        /// <param name="thumb"></param>
        private void FlatThumb(Thumb thumb)
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
            myGrid.Background = new SolidColorBrush(Colors.Cyan);
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
