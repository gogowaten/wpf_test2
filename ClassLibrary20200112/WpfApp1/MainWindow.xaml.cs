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

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BitmapImage image = new BitmapImage(new Uri(@"D:\ブログ用\20200107_pdftojpeg_08.png"));
            //BitmapImage image = new BitmapImage(new Uri(@"D:\ブログ用\テスト用画像\テスト結果用\NEC_8041_2017_05_09_午後わてん_96dpi_Indexed2.png"));


            BitmapSource source = image;
            ClassLibrary20200112.Visualizer20200112.TestShowVisualizer(source);
        }
    }
}
