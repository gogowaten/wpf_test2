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

using System.Drawing;
using System.Runtime.InteropServices;


namespace _20191031_クリップボードからエクセルの画像取得
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int CF_ENHMETAFILE = 14;
        [DllImport("user32.dll")]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll")]
        private static extern int IsClipboardFormatAvailable(int wFormat);
        [DllImport("user32.dll")]
        private static extern IntPtr GetClipboardData(int wFormat);
        [DllImport("user32.dll")]
        private static extern int CloseClipboard();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            MyImage.Source = null;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BitmapSource image = Clipboard.GetImage();
            if (image == null) return;
            MyImage.Source = new FormatConvertedBitmap(image, PixelFormats.Bgra32, null, 0);
            MyCanvas.Width = image.PixelWidth;
            MyCanvas.Height = image.PixelHeight;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource image = Clipboard.GetImage();
            if (image == null) return;
            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var stream = new System.IO.MemoryStream())
            {
                encoder.Save(stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                var decoder = new BmpBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                image = decoder.Frames[0];
            }
            MyImage.Source = image;
            MyCanvas.Width = image.PixelWidth;
            MyCanvas.Height = image.PixelHeight;
        }



        private void Button_Click_Png(object sender, RoutedEventArgs e)
        {
            BitmapSource image = Clipboard.GetImage();
            if (image == null) return;
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var stream = new System.IO.MemoryStream())
            {
                encoder.Save(stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                image = decoder.Frames[0];
            }
            MyImage.Source = image;
            MyCanvas.Width = image.PixelWidth;
            MyCanvas.Height = image.PixelHeight;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            BitmapSource image = Clipboard.GetImage();
            if (image == null) return;
            var data = Clipboard.GetDataObject();

            System.IO.MemoryStream stream = (System.IO.MemoryStream)data.GetData("PNG");
            var frame = BitmapFrame.Create(stream);
            MyImage.Source = frame;
            MyCanvas.Width = frame.PixelWidth;
            MyCanvas.Height = frame.PixelHeight;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            BitmapSource image = Clipboard.GetImage();
            if (image == null) return;
            //ウィンドウハンドルの取得その1
            var source = System.Windows.Interop.HwndSource.FromVisual(this) as System.Windows.Interop.HwndSource;
            IntPtr handle = source.Handle;
            
            //ウィンドウハンドルの取得その2
            var helper = new System.Windows.Interop.WindowInteropHelper(this);

            System.Drawing.Imaging.Metafile metafile = GetEnhMetafileOnClipboard(helper.Handle);
            //var data = Clipboard.GetDataObject();
            //data.GetData(DataFormats.EnhancedMetafile);
            image = ToBitmapSource(metafile);
            
            
            MyImage.Source = image;
            MyCanvas.Width = image.PixelWidth;
            MyCanvas.Height = image.PixelHeight;
        }
        public static System.Drawing.Imaging.Metafile GetEnhMetafileOnClipboard(IntPtr hWnd)
        {
            System.Drawing.Imaging.Metafile meta = null;
            if (OpenClipboard(hWnd))
            {
                try
                {
                    if (IsClipboardFormatAvailable(CF_ENHMETAFILE) != 0)
                    {
                        IntPtr hmeta = GetClipboardData(CF_ENHMETAFILE);
                        meta = new System.Drawing.Imaging.Metafile(hmeta, true);                        
                    }
                }
                finally
                {
                    CloseClipboard();
                }
            }

            return meta;
        }
        private BitmapSource ToBitmapSource(System.Drawing.Imaging.Metafile metafile)
        {
            BitmapSource bitmap;
            var img = (System.Drawing.Image)metafile;
            var bmp = new System.Drawing.Bitmap(img);
            //image
            //  ┣bitmap
            //  ┗enhancedMetafile
            using (var stream  = new System.IO.MemoryStream())
            {
                //img.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                //metafile.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                bitmap = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
            return bitmap;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var data = Clipboard.GetDataObject();
            List<string> vs = new List<string>();
            foreach (var item in data.GetFormats())
            {
                vs.Add(item);
            }
            var meta = data.GetData("EnhancedMetafile");//imaging.metafile
            meta = data.GetData("System.Drawing.Imaging.Metafile");//imaging.metafile
            meta = data.GetData("PNG");//memoryStream
            meta = data.GetData("JFIF");//memoryStream
            meta = data.GetData("GIF");//memoryStream
            meta = data.GetData("Bitmap");//interop.interropBitmap
            meta = data.GetData("System.Drawing.Bitmap");//drawing.bitmap
            meta = data.GetData("System.Windows.Media.Imaging.BitmapSource");//interop.interropBitmap
            meta = data.GetData("EnhancedMetafile");//imaging.metafile

            meta = data.GetData("System.Drawing.Imaging.Metafile");//imaging.metafile
            meta = data.GetData("PNG+Office Art");//
            meta = data.GetData("JFIF+Office Art");//
            meta = data.GetData("GIF+Office Art");//
            meta = data.GetData("Preferred DropEffect");//memoryStream
            meta = data.GetData("Art::GVML ClipFormat");//memoryStream
            meta = data.GetData("Text");//string
            meta = data.GetData("HTML");//
            //meta = data.GetData("MetaFilePict");//エラー

            var stream = (System.IO.MemoryStream)data.GetData("PNG+Office Art");
            var frame = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            MyImage.Source = frame;
            MyCanvas.Width = frame.PixelWidth;
            MyCanvas.Height = frame.PixelHeight;
        }
    }
}
//WPF でウィンドウハンドルを取得する方法 - present
//https://tnakamura.hatenablog.com/entry/20100616/wpf_window_handle
