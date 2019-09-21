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

//画像の切り抜き
//CroppedBitmapを使って画像の切り抜き
//PixelFormatも引き継がれる
namespace _20190921_trim_crop
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {        
        public MainWindow()
        {
            InitializeComponent();
            MyButton1.Click += MyButton1_Click;

            //画像ファイルを表示
            string filePath = @"D:\ブログ用\テスト用画像\NEC_1853_2018_04_08_午後わてん_.jpg";
            MyImage.Source = new BitmapImage(new Uri(filePath));
            
        }

        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {   
            //切り抜いた画像をStackPanelに追加表示
            var bmp = new CroppedBitmap((BitmapSource)MyImage.Source, new Int32Rect(30, 40, 80, 50));
            var img = new Image() { Source = bmp, Stretch = Stretch.None };
            MyStakPanel.Children.Add(img);
        }

    }
}
