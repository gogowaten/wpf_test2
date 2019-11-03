using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using System.Collections.ObjectModel;

namespace _20191101_エクセルからコピペ2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<MyBitmap> MyBitmaps = new ObservableCollection<MyBitmap>();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MyBitmaps;
            MyListBox.SelectionChanged += MyListBox_SelectionChanged;
        }

        private void MyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyBitmap item = MyListBox.SelectedItem as MyBitmap;
            if (item != null && item.BitmapSource != null)
            {
                Clipboard.Clear();
                Clipboard.SetImage(item.BitmapSource);
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyBitmaps.Clear();

            var data = Clipboard.GetDataObject();
            var neko = data.GetFormats();
            foreach (var item in data.GetFormats())
            {
                if (item != DataFormats.MetafilePicture)
                {
                    //var obj = data.GetData(item);
                    var ms = data.GetData(item) as System.IO.MemoryStream;
                    var bmp = data.GetData(item) as BitmapSource;
                    if (ms != null)
                    {
                        try
                        {
                            var bf = BitmapFrame.Create(ms);
                            MyBitmaps.Add(new MyBitmap(bf, item));
                            

                            var cat = MakePixles(bf);
                        }
                        catch (Exception)
                        {
                            MyBitmaps.Add(new MyBitmap(null, item));
                        }
                    }
                    if (bmp != null)
                    {
                        try
                        {
                            MyBitmaps.Add(new MyBitmap(bmp, item));
                            var cat = MakePixles(bmp);
                        }
                        catch (Exception)
                        {
                            MyBitmaps.Add(new MyBitmap(null, item));
                        }
                    }
                    if (ms == null && bmp == null)
                    {
                        MyBitmaps.Add(new MyBitmap(null, item));
                    }
                }
            }
        }

        private byte[] MakePixles(BitmapSource source)
        {
            int w = source.PixelWidth;
            int h = source.PixelHeight;
            int stride = (w * source.Format.BitsPerPixel + 7) / 8;
            byte[] pixels = new byte[w * stride];
            source.CopyPixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);
            return pixels;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MyBitmaps.Clear();

            var data = Clipboard.GetDataObject();
            var neko = data.GetFormats();
            foreach (var item in data.GetFormats())
            {
                if (item != DataFormats.MetafilePicture)
                {
                    //var obj = data.GetData(item);
                    var ms = data.GetData(item) as System.IO.MemoryStream;
                    var bmp = data.GetData(item) as BitmapSource;
                    if (ms != null)
                    {
                        try
                        {
                            MyBitmaps.Add(new MyBitmap(BitmapFrame.Create(ms), item));
                        }
                        catch (Exception)
                        {
                            MyBitmaps.Add(new MyBitmap(null, item));
                        }
                    }
                    if (bmp != null)
                    {
                        try
                        {
                            //MyBitmaps.Add(new MyBitmap(new FormatConvertedBitmap(bmp, System.Windows.Media.PixelFormats.Bgra32, null, 0), item));
                            MyBitmaps.Add(new MyBitmap(new FormatConvertedBitmap(Clipboard.GetImage(), System.Windows.Media.PixelFormats.Bgra32, null, 0), item));

                        }
                        catch (Exception)
                        {
                            MyBitmaps.Add(new MyBitmap(null, item));
                        }
                    }
                    if (ms == null && bmp == null)
                    {
                        MyBitmaps.Add(new MyBitmap(null, item));
                    }
                }
            }
        }
    }




    public class MyBitmap
    {
        public BitmapSource BitmapSource { get; set; }
        public string DataFormatName { get; set; }
        public string Dpi { get; set; }

        public MyBitmap(BitmapSource source, string name)
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
