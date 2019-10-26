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
//ピクセル単位で移動するとき、2倍に拡大表示した状態のときは2ピクセルごとに移動して欲しい、
//同様に3倍に拡大表示のときは3ピクセルごと、10倍なら10ピクセルごと
//これだけならCanvasのUseLayoutRounding="True"にすればいい
//だけど移動量に小数点がつくと動きがおかしくなる
//0.5づつ移動だと整数になったときだけ移動してほしいけど、実際はそうはならないのがわかった
//よって、移動をピクセルに合わせたければ整数で座標を決定する必要があることがわかった



namespace _20191018_拡大表示
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();

            
            MyCanvas.RenderTransform = new ScaleTransform(1, 1);
            MyCanvas.Width = MyBorder1.Width;
            MyCanvas.Height = MyBorder1.Height;
            //RenderOptions.SetBitmapScalingMode(MyCanvas, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(MyCanvas, EdgeMode.Aliased);
            

            var b = new Binding()
            {
                Source = TextBlockLeft,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(TextBlock.TextProperty)
            };
            BindingOperations.SetBinding(MyBorder3, LeftProperty, b);
            TextBlockLeft.Text = "45";
        }

        private void ButtonZoomIn_Click(object sender, RoutedEventArgs e)
        {
            var scale = (ScaleTransform)MyCanvas.RenderTransform;
            scale.ScaleX++;
            scale.ScaleY++;

        }

        private void ButtonZoomOut_Click(object sender, RoutedEventArgs e)
        {
            var scale = (ScaleTransform)MyCanvas.RenderTransform;
            if (scale.ScaleX > 1)
            {
                scale.ScaleY--;
                scale.ScaleX--;
            }
        }

        private void ButtonMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            //Canvas.SetLeft(MyBorder3, Canvas.GetLeft(MyBorder3) + 1);
            Canvas.SetLeft(MyBorder3, Canvas.GetLeft(MyBorder3) + 0.5);
        }
    }
}
