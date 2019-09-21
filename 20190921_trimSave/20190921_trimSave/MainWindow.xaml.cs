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

//画像をトリム、クロップ
//CroppedBitmapを使う
namespace _20190921_trimSave
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //画像ファイルを表示
            string filePath = @"E:\emu\MegaDrive\KegaFusion\Fusion364\Langrisser Hikari (J)007.bmp";
            var img = new BitmapImage(new Uri(filePath));
            MyImage.Source = img;

            MyButton1.Click += MyButton1_Click;
        }

        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            MyClip1();
        }

        private void MyClip1()
        {
            var r = new Int32Rect((int)Canvas.GetLeft(MyRectangle),(int)Canvas.GetTop(MyRectangle),(int)MyRectangle.Width,(int)MyRectangle.Height);
            var cropBmp = new CroppedBitmap((BitmapSource)MyImage.Source, r);
            var img = new Image() { Width = MyRectangle.Width, Height = MyRectangle.Height, Stretch = Stretch.None, Source = cropBmp };
            MyStakPanel.Children.Add(img);

        }
     
    }
}
