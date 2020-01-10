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



namespace _20200109_デバッガービジュアライザーテスト
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string imgPath = @"D:\ブログ用\20200107_pdftojpeg_08.png";
            BitmapImage source = new BitmapImage(new Uri(imgPath));
            BitmapSource bmp = source;
            _20200109_デバッガービジュアライザー.Visualizer1.TestShowVisualizer(source);

            //_20200109_デバッガービジュアライザー.MyBitmapSourceProxy proxy = new _20200109_デバッガービジュアライザー.MyBitmapSourceProxy(source);
            //_20200109_デバッガービジュアライザー.Visualizer1.TestShowVisualizer(proxy);

        }
    }
}
