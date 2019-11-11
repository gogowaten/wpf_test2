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


//エクセルからのコピーはFormatConvertedBitmapでBgr32へ変換で決め打ち
//これだと半透明が失われるけど、それ以外の見た目はそっくりにコピーできる
//APIとFormへの参照を追加してメタファイルから画像取得は手間の割にはいまいちな結果が多い
//他のアプリからの画像コピペで透明画像になってしまうのは
//そのアプリがアルファを持たない画像、つまり24bitでクリップボードにコピーしていて
//WPFでそれを受け取ったときにはBgra32に変換されるんだけど、その変換時にアルファをすべて0にしてしまうからかも
//
//WPFのClipboard.GetImage()はピクセルフォーマットBgra32に変換された画像になる
//元の画像が32bitなら問題ないけど、それ以下だとアルファが0になって完全透明になる画像がある
//これは元画像のbit数を取得して32bit未満なら、ピクセルフォーマットBgr32に変換すればいい
//bit数取得はクリップボードからGetData("DeviceIndependentBitmap")で受け取ったMemoryStreamを
//ToArray()でbyte配列に変換してbyte[14]のデータがbit数なのでこれを確認


namespace _20191111_クリップボードからの画像が透明
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //private BitmapSource MySource1;
        //private BitmapSource MySource2;

        public MainWindow()
        {
            InitializeComponent();
        }
        //データフォーマット一覧表示
        private void ButtonViewFormats_Click(object sender, RoutedEventArgs e)
        {
            var formats = Clipboard.GetDataObject().GetFormats();
            string name = "";
            foreach (var item in formats)
            {
                name += Environment.NewLine + item;
            }
            MessageBox.Show(name);
        }

        private void ButtonGetImage_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource source = Clipboard.GetImage();
            SetImageSource(source);            
        }

        private void ButtonGetDataBitmap_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource source = Clipboard.GetData("Bitmap") as BitmapSource;
            SetImageSource(source);
        }

        private void ButtonGetDataBitmapSource_Click(object sender, RoutedEventArgs e)
        {
            //BitmapSource source = Clipboard.GetData("System.Windows.Media.Imaging.BitmapSource") as BitmapSource;
            var source = Clipboard.GetDataObject().GetData("System.Windows.Media.Imaging.BitmapSource") as BitmapSource;
            SetImageSource(source);
        }


        private void ButtonDeviceIndependentBitmap_Click(object sender, RoutedEventArgs e)
        {
            var stream = Clipboard.GetData("DeviceIndependentBitmap") as System.IO.MemoryStream;
            BitmapSource source = null;
            if (stream == null)
            {
                SetImageSource(source);
            }
            else
            {
                var data = stream.ToArray();
                if (data[14] < 32)
                {
                    source = new FormatConvertedBitmap(Clipboard.GetImage(), PixelFormats.Bgr32, null, 0);
                    SetImageSource(source);
                    //MyTextBlock.Text = "bpp = " + data[14].ToString();
                }
                else
                {                    
                    SetImageSource(Clipboard.GetImage());
                    //MyTextBlock.Text = "bpp = " + data[14].ToString();
                }
            }
            
        }

        private void SetImageSource(BitmapSource source)
        {
            if (source != null)
            {
                MyImage.Source = source;
                MyTextBlock.Text = "PixelFormats = "+ source.Format.ToString() + Environment.NewLine+
                    "DpiX = "+source.DpiX;
            }
            else
            {
                MyImage.Source = null;
                MyTextBlock.Text = "null";
            }
        }

        private byte[] GetPixels(BitmapSource source)
        {
            int w = source.PixelWidth;
            int h = source.PixelHeight;
            int stride = w * 4;
            byte[] pixels = new byte[h * stride];
            source.CopyPixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);
            return pixels;
        }


        private BitmapSource GetClipboardBitmap()
        {
            var data = Clipboard.GetDataObject();
            if (data == null) return null;

            var ms = data.GetData("DeviceIndependentBitmap") as System.IO.MemoryStream;
            if (ms == null) return null;

            byte[] dib = ms.ToArray();
            if (dib[14] < 32)
            {
                return new FormatConvertedBitmap(Clipboard.GetImage(), PixelFormats.Bgr32, null, 0);
            }
            else
            {
                return Clipboard.GetImage();
            }


        }


        //private BitmapSource PngAndOfficeArt()
        //{
        //    var data = Clipboard.GetDataObject();
        //    if (data == null) return null;

        //    var ms = data.GetData("PNG") as System.IO.MemoryStream;
        //    //var ms = data.GetData("PNG+Office Art") as System.IO.MemoryStream;
        //    if (ms == null) return null;
        //    var neko = ms.ToArray();

        //    var source = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        //    //return source;

        //    int w = source.PixelWidth;
        //    int h = source.PixelHeight;
        //    int stride = w * 4;
        //    byte[] pixels = new byte[h * stride];
        //    source.CopyPixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);
        //    //return BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgra32, null, pixels, stride);
        //    double dpi = (double)570 / 365 * 96;
        //    dpi = 150;
        //    BitmapSource bmp = BitmapSource.Create(w, h, dpi, dpi, PixelFormats.Bgra32, null, pixels, stride);
        //    return bmp;

        //}


        //private BitmapSource Png()
        //{
        //    var data = Clipboard.GetDataObject();
        //    if (data == null) return null;

        //    var ms = data.GetData("PNG") as System.IO.MemoryStream;
        //    if (ms == null) return null;

        //    var source = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        //    return source;
        //}



        //private BitmapSource CreateBitmapFromDIB(System.IO.MemoryStream ms)
        //{
        //    if (ms == null) return null;

        //    byte[] dibBuffer = new byte[ms.Length];
        //    ms.Read(dibBuffer, 0, dibBuffer.Length);

        //    BITMAPINFOHEADER infoHeader =
        //        BinaryStructConverter.FromByteArray<BITMAPINFOHEADER>(dibBuffer);

        //    const int BITMAPFILEHEADER_SIZE = 14;
        //    byte[] bin = ms.ToArray();
        //    int headerSize = BitConverter.ToInt32(bin, 0);
        //    int pixelSize = bin.Length - headerSize;
        //    int fileSize = BITMAPFILEHEADER_SIZE + bin.Length;

        //    var bmpStm = new System.IO.MemoryStream(fileSize);
        //    var writer = new System.IO.BinaryWriter(bmpStm);

        //    writer.Write(Encoding.ASCII.GetBytes("BM"));
        //    writer.Write(fileSize);
        //    writer.Write(0UI);v
        //}
    }
}
