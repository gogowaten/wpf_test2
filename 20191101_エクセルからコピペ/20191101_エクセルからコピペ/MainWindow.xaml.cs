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

namespace _20191101_エクセルからコピペ
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

        private void SetImageSource(BitmapSource source)
        {
            MyImage.Source = source;
            MyCanvas.Width = source.PixelWidth;
            MyCanvas.Height = source.PixelHeight;
        }

        private BitmapSource GetClipboardImage1()
        {
            return Clipboard.GetImage();
        }

        private BitmapSource GetClipboardImage2()
        {
            BitmapSource source = Clipboard.GetImage();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using (var stream = new System.IO.MemoryStream())
            {
                encoder.Save(stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                source = decoder.Frames[0];
            }
            return source;
        }

        private BitmapSource GetClipboardImage3()
        {
            BitmapSource source = Clipboard.GetImage();
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
        private BitmapSource GetClipboardImage4()
        {
            var data = Clipboard.GetDataObject();
            var stream = (System.IO.MemoryStream)data.GetData("PNG");
            //var stream = (System.IO.MemoryStream)data.GetData("PNG+Office Art");
            return BitmapFrame.Create(stream);
        }

        private BitmapSource GetClipboardImage5()
        {
            var data = Clipboard.GetDataObject();
            string[] fmt = data.GetFormats();
            List<object> obj = new List<object>();
            List<BitmapSource> listBmp = new List<BitmapSource>();
            List<string> notStreamBmp = new List<string>();
            List<string> isStreamBmp = new List<string>();
            foreach (var item in data.GetFormats())
            {
                if (item != DataFormats.MetafilePicture)
                {
                    try
                    {
                        obj.Add(data.GetData(item));
                        var ms = (System.IO.MemoryStream)data.GetData(item);
                        listBmp.Add(BitmapFrame.Create(ms));
                        isStreamBmp.Add(item);
                    }
                    catch (Exception)
                    {
                        notStreamBmp.Add(item);
                    }

                }
            }
            var stream = (System.IO.MemoryStream)data.GetData("PNG");
            //var stream = (System.IO.MemoryStream)data.GetData("PNG+Office Art");
            return BitmapFrame.Create(stream);
        }

        private BitmapSource GetClipboardImage6()
        {   
            return new FormatConvertedBitmap(Clipboard.GetImage(), PixelFormats.Bgra32, null, 0);
        }








        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.GetImage() == null) return;
            SetImageSource(GetClipboardImage1());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Clipboard.GetImage() == null) return;
            SetImageSource(GetClipboardImage2());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Clipboard.GetImage() == null) return;
            SetImageSource(GetClipboardImage3());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (Clipboard.GetImage() == null) return;
            SetImageSource(GetClipboardImage4());
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //if (Clipboard.GetImage() == null) return;
            SetImageSource(GetClipboardImage5());
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (Clipboard.GetImage() == null) return;
            SetImageSource(GetClipboardImage6());
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            MyImage.Source = null;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var data = Clipboard.GetDataObject();
            var neko = data.GetFormats();
        }
    }
}
//WPFのClipboard.GetImage() のバグ？ (ソフトウェア ) - Simple is best - Yahoo!ブログ
//https://blogs.yahoo.co.jp/elku_simple/35320048.html
