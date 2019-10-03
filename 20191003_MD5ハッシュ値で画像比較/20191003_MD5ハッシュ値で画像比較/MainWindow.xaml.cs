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

namespace _20191003_MD5ハッシュ値で画像比較
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string fileName;
            fileName = @"D:\ブログ用\チェック用2\WP_20190714_22_18_03_Pro_2019_07_14_午後わてん.jpg";
            Uri uri = new Uri(fileName);
            BitmapSource bmp1 = new BitmapImage(uri);
            BitmapSource bmp2 = new BitmapImage(uri);
            //同じ画像を比較
            var neko = bmp1 == bmp2;            //false
            var inu = bmp1.Equals(bmp2);        //false
            var uma = IsBitmapEqual(bmp1, bmp2);//true

            //違う画像と比較
            fileName = @"D:\ブログ用\テスト用画像\8x8_100と2002色.png";
            BitmapSource bmp3 = new BitmapImage(new Uri(fileName));
            var saru = IsBitmapEqual(bmp1, bmp3);//false
        }

        /// <summary>
        /// 2つのBitmapSourceが同じ画像なのか判定、MD5のハッシュ値を作成して比較
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <returns></returns>
        private bool IsBitmapEqual(BitmapSource bmp1, BitmapSource bmp2)
        {
            //それぞれのハッシュ値を作成
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] h1 = md5.ComputeHash(MakeBitmapByte(bmp1));
            byte[] h2 = md5.ComputeHash(MakeBitmapByte(bmp2));
            md5.Clear();
            //ハッシュ値を比較
            return IsArrayEquals(h1, h2);
        }

        //2つのハッシュ値を比較
        private bool IsArrayEquals(byte[] h1, byte[] h2)
        {
            for (int i = 0; i < h1.Length; i++)
            {
                if (h1[i] != h2[i])
                {
                    return false;
                }
            }
            return true;
        }

        //BitmapSourceをbyte配列に変換
        private byte[] MakeBitmapByte(BitmapSource bitmap)
        {
            int w = bitmap.PixelWidth;
            int h = bitmap.PixelHeight;
            int stride = w * bitmap.Format.BitsPerPixel / 8;
            byte[] pixels = new byte[h * stride];
            bitmap.CopyPixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);
            return pixels;
        }
    }
}
//MD5やSHA1などでハッシュ値を計算する - .NET Tips(VB.NET, C#...)
//https://dobon.net/vb/dotnet/string/md5.html
//C#でBitmapの比較 - blog.kur.jp
//https://blog.kur.jp/entry/2010/03/03/bitmap-compare/


