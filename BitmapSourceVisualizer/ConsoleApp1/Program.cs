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
            var image = new BitmapImage(new Uri(@"D:\ブログ用\消費期限とか\WP_20200111_09_38_14_Pro_2020_01_11_午後わてん.jpg"));
            BitmapSourceVisualizer.MyDialog.TestShowVisualizer(image);
        }
    }
}
