using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            BitmapImage image = new BitmapImage(new Uri(@"D:\ブログ用\20200107_pdftojpeg_08.png"));
            //BitmapImage image = new BitmapImage(new Uri(@"D:\ブログ用\テスト用画像\テスト結果用\NEC_8041_2017_05_09_午後わてん_96dpi_Indexed2.png"));


            BitmapSource source = image;
            //ClassLibrary20200112.Visualizer20200112.TestShowVisualizer(source);
            ClassLibrary20200112.Visualizer20200112.TestShowVisualizer(source);

        }
    }
}
