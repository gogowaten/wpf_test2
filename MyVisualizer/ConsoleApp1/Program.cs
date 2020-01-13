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
            var image = new BitmapImage(new Uri(@"D:\ブログ用\テスト用画像\NEC_8041_2017_05_09_午後わてん_96dpi.jpg"));
            MyVisualizer.MyDialog.TestShowVisualizer(image);
        }
    }
}
