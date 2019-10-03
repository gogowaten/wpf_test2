using System;
using System.Windows;
using System.Windows.Media.Imaging;

/// <summary>
/// 2つのBitmapSourceが同じ画像(のすべてのピクセルの色)なのか判定する、MD5のハッシュ値を作成して比較
/// 
/// それぞれのBitmapSourceのピクセルをbyte配列に変換して
///     (BitmapSourceクラスのCopyPixels)
/// それぞれのbyte配列のMD5ハッシュ値を作成して
///     (System.Security.Cryptography.MD5CryptoServiceProvider())の
///     ComputeHash関数
/// 比較
/// </summary>
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
            //同じ画像を比較
            fileName = @"D:\ブログ用\チェック用2\WP_20190714_22_18_03_Pro_2019_07_14_午後わてん.jpg";
            Uri uri = new Uri(fileName);
            BitmapSource bmp10 = new BitmapImage(uri);
            BitmapSource bmp11 = new BitmapImage(uri);

            bool neko = bmp10 == bmp11;            //false            
            bool inu = bmp10.Equals(bmp11);        //false
            //MD5ハッシュ値で比較
            bool uma = IsBitmapEqual(bmp10, bmp11);//true

            fileName = @"D:\ブログ用\チェック用2\WP_20190714_22_18_03_Pro_2019_07_14_午後わてん - コピー.jpg";
            BitmapSource bmp20 = new BitmapImage(new Uri(fileName));
            bool tori = IsBitmapEqual(bmp10, bmp20);//true

            //違う画像と比較
            fileName = @"D:\ブログ用\テスト用画像\8x8_100と2002色.png";
            BitmapSource bmp30 = new BitmapImage(new Uri(fileName));
            //MD5ハッシュ値で比較
            bool saru = IsBitmapEqual(bmp10, bmp30);//false
        }

        /// <summary>
        /// 2つのBitmapSourceが同じ画像(のすべてのピクセルの色)なのか判定する、MD5のハッシュ値を作成して比較
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


