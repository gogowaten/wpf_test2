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

using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace _20191103_エクセルからセルのコピペ
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




        ObservableCollection<MyBitmapSource> MyBitmaps = new ObservableCollection<MyBitmapSource>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = MyBitmaps;
            MyListBox.SelectionChanged += MyListBox_SelectionChanged;
        }

        //クリップボードへコピー
        private void MyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyBitmapSource item = MyListBox.SelectedItem as MyBitmapSource;
            if (item != null && item.BitmapSource != null)
            {
                Clipboard.Clear();
                Clipboard.SetImage(item.BitmapSource);//これが一番いいけど、アルファ値が255にされてしまう

                //Clipboard.SetDataObject(item.BitmapSource);//なぜかセットされない
                //Clipboard.SetData("Bitmap", item.BitmapSource);//おk、たぶんSetImageと同じ
                //Clipboard.SetData("BitmapSource", item.BitmapSource);//セットされない
                //これも動作がおかしい、他のアプリには画像がないとされるけど、このアプリには貼り付けできるけど、それも変色している
                //Clipboard.SetData("System.Windows.Media.Imaging.BitmapSource", item.BitmapSource);
                
                
                var pixels = GetPixels(item.BitmapSource);
            }
        }
        

        //クリップボードの画像取得してリストに追加
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyBitmaps.Clear();

            var data = Clipboard.GetDataObject();
            var neko = data.GetFormats();
            byte[] pixels;
            BitmapSource bmp;

            bmp = Clipboard.GetImage();
            MyBitmaps.Add(new MyBitmapSource(bmp, "GetImage()"));
            pixels = GetPixels(bmp);

            bmp = data.GetData("Bitmap") as BitmapSource;
            MyBitmaps.Add(new MyBitmapSource(bmp, "GetData\"Bitmap\""));

            bmp = data.GetData("System.Windows.Media.Imaging.BitmapSource") as BitmapSource;
            MyBitmaps.Add(new MyBitmapSource(bmp, "GetData\"BitmapSource\""));

            if (bmp == null) return;
            bmp = new FormatConvertedBitmap(Clipboard.GetImage(), PixelFormats.Bgr32, null, 0);
            MyBitmaps.Add(new MyBitmapSource(bmp, "FormatConvertedBitmapでBgr32"));

            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(Clipboard.GetImage()));
            using (var stream = new System.IO.MemoryStream())
            {
                encoder.Save(stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                var decoder = new BmpBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                bmp = decoder.Frames[0];
            }
            MyBitmaps.Add(new MyBitmapSource(bmp, "BmpBitmapEncoderDecoder"));

            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(Clipboard.GetImage()));
            using (var stream = new System.IO.MemoryStream())
            {
                pngEncoder.Save(stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                bmp = decoder.Frames[0];
            }
            MyBitmaps.Add(new MyBitmapSource(bmp, "PngBitmapEncoderDecoder"));

            //System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap
        }

        private byte[] GetPixels(BitmapSource bmp)
        {
            if (bmp == null) return null;
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int stride = w * 4;
            byte[] pixels = new byte[h * stride];
            bmp.CopyPixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);
            return pixels;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MyBitmaps.Clear();

            var data = Clipboard.GetDataObject();
            var neko = data.GetFormats();
            Bitmap bmp;

            bmp = data.GetData("System.Drawing.Bitmap") as Bitmap;
            MyBitmaps.Add(new MyBitmapSource(BitmapToBitmapSource(bmp, System.Drawing.Imaging.ImageFormat.Bmp), "\"System.Drawing.Bitmap\" to Bmp"));
            MyBitmaps.Add(new MyBitmapSource(BitmapToBitmapSource(bmp, System.Drawing.Imaging.ImageFormat.Png), "\"System.Drawing.Bitmap\" to Png"));

            //途中でエラーになるか真っ黒
            //System.Drawing.Imaging.Metafile metafile;
            //metafile = data.GetData("System.Drawing.Imaging.Metafile") as System.Drawing.Imaging.Metafile;
            //MyBitmaps.Add(new MyBitmapSource(MetafileToBitmapSource(metafile), "\"System.Drawing.Imaging.Metafile\""));
        }

        private BitmapSource BitmapToBitmapSource(System.Drawing.Image bmp, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            BitmapSource source;
            using (var stream = new System.IO.MemoryStream())
            {
                bmp.Save(stream, imageFormat);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
            return source;
        }
        private BitmapSource MetafileToBitmapSource(System.Drawing.Imaging.Metafile metafile, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            BitmapSource source;
            using (var stream = new System.IO.MemoryStream())
            {
                metafile.Save(stream, imageFormat);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
            return source;
        }


        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MyBitmaps.Clear();

            var data = Clipboard.GetDataObject();
            var neko = data.GetFormats();
            System.Drawing.Imaging.Metafile metafile;

            //APIを使ってメタファイル取得
            metafile = GetEnhMetafileOnClipboard(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            //メタファイル→Bitmap→BitmapSourceに変換
            MyBitmaps.Add(new MyBitmapSource(MetafileToBitmapSource(metafile, System.Drawing.Imaging.ImageFormat.Bmp), "metafile to bmp"));
            MyBitmaps.Add(new MyBitmapSource(MetafileToBitmapSource(metafile, System.Drawing.Imaging.ImageFormat.Png), "metafile to png"));

            ////
            ////var image = System.Windows.Forms.Clipboard.GetImage();
            //var formsData = System.Windows.Forms.Clipboard.GetDataObject();
            //var image = formsData.GetData("System.Drawing.Bitmap") as Bitmap;
            ////System.Drawing.Image image = data.GetData("System.Drawing.Bitmap") as Bitmap;
            //MyBitmaps.Add(new MyBitmapSource(BitmapToBitmapSource(image, System.Drawing.Imaging.ImageFormat.Bmp), "bmp"));
            //MyBitmaps.Add(new MyBitmapSource(BitmapToBitmapSource(image, System.Drawing.Imaging.ImageFormat.Png), "png"));



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



    }




    public class MyBitmapSource
    {
        public BitmapSource BitmapSource { get; set; }
        public string DataFormatName { get; set; }
        public string Dpi { get; set; }

        public MyBitmapSource(BitmapSource source, string name)
        {
            BitmapSource = source;
            DataFormatName = name;
            if (source != null)
            {
                Dpi = "dpi = " + source.DpiX.ToString();
            }
            else
            {
                Dpi = "none";
            }

        }
    }

}
//WPFのClipboard.GetImage() のバグ？ (ソフトウェア ) - Simple is best - Yahoo!ブログ
//https://blogs.yahoo.co.jp/elku_simple/35320048.html
//Microsoft Officeでコピーした図をPictureBoxに表示する - .NET Tips(VB.NET, C#...)
//https://dobon.net/vb/dotnet/graphics/getclipboardmetafile.html
//クリップボードのデータの形式を取得する - .NET Tips(VB.NET, C#...)
//https://dobon.net/vb/dotnet/system/clipboardformats.html
//クリップボードの画像を表示する、クリップボードに画像をコピーする - .NET Tips(VB.NET, C#...)
//https://dobon.net/vb/dotnet/graphics/getclipboarddata.html
//画像のdpi（画像解像度）を取得／設定するには？［C#、VB］ - ＠IT
//https://www.atmarkit.co.jp/fdotnet/dotnettips/961dpiresolution/dpiresolution.html
