using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace _20191029_画像比較
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapSource MyBitmapSource1;
        private BitmapSource MyBitmapSource2;

        public MainWindow()
        {
            InitializeComponent();

            MyStackPanel1.Drop += MyStackPanel1_Drop;
            MyStackPanel2.Drop += MyStackPanel2_Drop;
        }

        //ファイルがドロップされたとき画像なら追加
        private void MyStackPanel2_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) == false) { return; }
            string[] filePath = (string[])e.Data.GetData(DataFormats.FileDrop);

            //Bgra32に変換して読み込む
            MyBitmapSource2 = GetBitmapSourceWithCangePixelFormat2(filePath[0], PixelFormats.Bgra32, 96, 96);
            if (MyBitmapSource2 == null)
            {
                MessageBox.Show("not Image");
            }
            else
            {
                MyImage2.Source = MyBitmapSource2;
                MyDir2.Text = System.IO.Path.GetFullPath(filePath[0]);//ファイルのフルパス表示
            }
        }

        //ファイルがドロップされたとき画像なら追加
        private void MyStackPanel1_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) == false) { return; }
            string[] filePath = (string[])e.Data.GetData(DataFormats.FileDrop);
            //Bgra32に変換して読み込む
            MyBitmapSource1 = GetBitmapSourceWithCangePixelFormat2(filePath[0], PixelFormats.Bgra32, 96, 96);
            if (MyBitmapSource1 == null)
            {
                MessageBox.Show("not Image");
            }
            else
            {
                MyImage1.Source = MyBitmapSource1;
                MyDir1.Text = System.IO.Path.GetFullPath(filePath[0]);//ファイルのフルパス表示
            }
        }

        //判定ボタンクリック時
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsBitmapEqual(MyBitmapSource1, MyBitmapSource2))
            {
                MessageBox.Show("同じ");
            }
            else
            {
                MessageBox.Show("違う");
            }
        }

        //クリップボードから画像取得して表示
        private void Button_Click_1(object sender, RoutedEventArgs e)
        { 
            BitmapSource source = Clipboard.GetImage();
            if (source == null) return;
            MyBitmapSource1 = source;
            MyImage1.Source = MyBitmapSource1;
            MyDir1.Text = "FromClipboard";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            BitmapSource bitmap = Clipboard.GetImage();
            if (bitmap == null) return;
            MyBitmapSource2 = bitmap;
            MyImage2.Source = MyBitmapSource2;
            MyDir2.Text = "FromClipboard";
        }

        //画像のストレッチ表示法変更
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            MyImage1.Stretch = Stretch.Uniform;//ウィンドウに合わせて表示
        }

        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            MyImage1.Stretch = Stretch.None;//そのまま等倍で表示
        }

        private void RadioButton_Click_2(object sender, RoutedEventArgs e)
        {
            MyImage2.Stretch = Stretch.Uniform;
        }

        private void RadioButton_Click_3(object sender, RoutedEventArgs e)
        {
            MyImage2.Stretch = Stretch.None;
        }



        /// <summary>
        /// 2つのBitmapSourceが同じ画像(のすべてのピクセルの色)なのか判定する、MD5のハッシュ値を作成して比較
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <returns></returns>
        private bool IsBitmapEqual(BitmapSource bmp1, BitmapSource bmp2)
        {
            if (bmp1 == null || bmp2 == null) return false;
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
            if (bitmap == null) return null;
            int w = bitmap.PixelWidth;
            int h = bitmap.PixelHeight;
            int stride = ((w * bitmap.Format.BitsPerPixel) + 7) / 8;
            byte[] pixels = new byte[h * stride];
            bitmap.CopyPixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);
            return pixels;
        }

      


        /// <summary>
        ///  ファイルパスとPixelFormatを指定してBitmapSourceを取得、dpiの変更は任意
        /// </summary>
        /// <param name="filePath">画像ファイルのフルパス</param>
        /// <param name="pixelFormat">PixelFormatsの中からどれかを指定</param>
        /// <param name="dpiX">無指定なら画像ファイルで指定されているdpiになる</param>
        /// <param name="dpiY">無指定なら画像ファイルで指定されているdpiになる</param>
        /// <returns></returns>
        private BitmapSource GetBitmapSourceWithCangePixelFormat2(
                    string filePath, PixelFormat pixelFormat, double dpiX = 0, double dpiY = 0)
        {
            BitmapSource source = null;
            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    var bf = BitmapFrame.Create(fs);
                    var convertedBitmap = new FormatConvertedBitmap(bf, pixelFormat, null, 0);

                    int w = convertedBitmap.PixelWidth;
                    int h = convertedBitmap.PixelHeight;
                    int stride = ((w * pixelFormat.BitsPerPixel) + 7) / 8;
                    byte[] pixels = new byte[h * stride];

                    convertedBitmap.CopyPixels(pixels, stride, 0);
                    //dpi指定がなければ元の画像と同じdpiにする
                    if (dpiX == 0) { dpiX = bf.DpiX; }
                    if (dpiY == 0) { dpiY = bf.DpiY; }
                    //dpiを指定してBitmapSource作成
                    source = BitmapSource.Create(
                        w, h, dpiX, dpiY,
                        convertedBitmap.Format,
                        convertedBitmap.Palette, pixels, stride);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return source;
        }



    }
}
