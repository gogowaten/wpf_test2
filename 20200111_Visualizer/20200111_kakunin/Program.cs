using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace _20200111_kakunin
{
    class Program
    {
        static void Main(string[] args)
        {
            //BitmapImage image = new BitmapImage(new Uri(@"D:\ブログ用\20200107_pdftojpeg_08.png"));
            BitmapImage image = new BitmapImage(new Uri(@"D:\ブログ用\テスト用画像\テスト結果用\NEC_8041_2017_05_09_午後わてん_96dpi_Indexed2.png"));


            BitmapSource source = image;            
            ClassLibrary1.Visualizer1.TestShowVisualizer(source);
            //_20200111_Visualizer.Class1.MyTest(source);
        }
    }
}
