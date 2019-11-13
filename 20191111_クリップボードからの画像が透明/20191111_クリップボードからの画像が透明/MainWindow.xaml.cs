using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


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
            //var neko = GetPixels(source);
            SetImageSource(source);
        }

        private void ButtonGetDataBitmap_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource source = Clipboard.GetData("Bitmap") as BitmapSource;
            SetImageSource(source);
        }

        private void ButtonGetDataBitmapSource_Click(object sender, RoutedEventArgs e)
        {
            //var source = Clipboard.GetData("System.Windows.Media.Imaging.BitmapSource") as BitmapSource;
            var source = Clipboard.GetDataObject().GetData("System.Windows.Media.Imaging.BitmapSource") as BitmapSource;
            SetImageSource(source);
        }

        private void ButtonEncDec_Click(object sender, RoutedEventArgs e)
        {
            //Streamに一時保存方式、BmpBitmapのエンコーダーとデコーダーを使う
            SetImageSource(GetClipboardBitmapEncDec());
        }

        //Streamに一時保存方式、BmpBitmapのエンコーダーとデコーダーを使う
        private BitmapSource GetClipboardBitmapEncDec()
        {
            BitmapSource source = Clipboard.GetImage();
            if (source == null)
            {
                return null;
            }

            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using (var stream = new System.IO.MemoryStream())
            {
                encoder.Save(stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                var decoder = new BmpBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                source = decoder.Frames[0];
            }
            return source;
        }

        private void ButtonBgr32_Click(object sender, RoutedEventArgs e)
        {
            SetImageSource(GetClipboarBitmapBgr32());
        }
        private BitmapSource GetClipboarBitmapBgr32()
        {
            BitmapSource source = Clipboard.GetImage();
            if (source == null) return null;
            return new FormatConvertedBitmap(source, PixelFormats.Bgr32, null, 0);
        }


        private void ButtonDeviceIndependentBitmap_Click(object sender, RoutedEventArgs e)
        {
            //bppが32未満ならBgr32、それ以外はBgra32のまま
            SetImageSource(GetClipboardBitmapDIB());
        }

        /// <summary>
        /// 透明画像にならないようにクリップボードの画像取得、DeviceIndependentBitmapでToArrayの15番目がbpp、32未満ならBgr32へ変換
        /// </summary>
        /// <returns></returns>
        private BitmapSource GetClipboardBitmapDIB()
        {
            var data = Clipboard.GetDataObject();
            if (data == null) return null;

            var ms = data.GetData("DeviceIndependentBitmap") as System.IO.MemoryStream;
            if (ms == null) return null;

            //DeviceIndependentBitmapのbyte配列の15番目がbpp、
            //これが32未満ならBgr32へ変換、これでアルファの値が255になる
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

        //エクセル判定追加
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetImageSource(GetClipboadBitmapDIBExcel());
        }
        private BitmapSource GetClipboadBitmapDIBExcel()
        {
            var data = Clipboard.GetDataObject();
            if (data == null) return null;

            var ms = data.GetData("DeviceIndependentBitmap") as System.IO.MemoryStream;
            if (ms == null) return null;

            //DeviceIndependentBitmapのbyte配列の15番目がbpp、
            //これが32未満ならBgr32へ変換、これでアルファの値が255になる
            //エクセルからのコピーなのかも判定、そうならBgr32へ変換
            byte[] dib = ms.ToArray();
            if (dib[14] < 32 || IsExcel())
            {
                return new FormatConvertedBitmap(Clipboard.GetImage(), PixelFormats.Bgr32, null, 0);
            }
            else
            {
                return Clipboard.GetImage();
            }
        }

        //エクセルからのコピーなのかを判定、フォーマット形式にEnhancedMetafileがあればエクセル判定
        private bool IsExcel()
        {
            string[] formats = Clipboard.GetDataObject().GetFormats();
            foreach (var item in formats)
            {
                if (item == "EnhancedMetafile")
                {
                    return true;
                }
            }
            return false;
        }
        //画像を表示と画像のピクセルフォーマットとdpi表示するだけ
        private void SetImageSource(BitmapSource source)
        {
            if (source != null)
            {
                MyImage.Source = source;
                MyTextBlock.Text = "PixelFormats = " + source.Format.ToString() + Environment.NewLine +
                    "DpiX = " + source.DpiX;
            }
            else
            {
                MyImage.Source = null;
                MyTextBlock.Text = "null";
            }
        }

        //bitmapSourceのCopyPixels取得
        private byte[] GetPixels(BitmapSource source)
        {
            int w = source.PixelWidth;
            int h = source.PixelHeight;
            int stride = w * 4;
            byte[] pixels = new byte[h * stride];
            source.CopyPixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);
            return pixels;
        }


        /// <summary>
        /// すべてのピクセルのアルファ値を見て、完全透明な画像ならtrueを返す、ピクセルフォーマットBgra32専用
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private bool IsTransparent(BitmapSource source)
        {
            var pixels = GetPixels(source);

            if (pixels[3] != 0)
            {
                return false;
            }
            ulong alpha = 0;
            for (int i = 3; i < pixels.Length; i += 4)
            {
                alpha += pixels[i];
            }
            if (alpha == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SetImageSource(ClipboardGetImageFix());
        }
        private BitmapSource ClipboardGetImageFix()
        {
            var data = Clipboard.GetDataObject();
            if (data == null) return null;

            BitmapSource source = Clipboard.GetImage();
            if (source == null) return null;

            if (IsTransparent(source))
            {
                return new FormatConvertedBitmap(source, PixelFormats.Bgr32, null, 0);
            }
            else
            {
                return source;
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
//クリップボードの中にある画像をWPFで取得してみた、Clipboard.GetImage() だけだと透明になる - 午後わてんのブログ
// https://gogowaten.hatenablog.com/entry/2019/11/12/201852
