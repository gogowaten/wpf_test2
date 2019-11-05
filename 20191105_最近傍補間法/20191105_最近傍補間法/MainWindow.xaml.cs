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

namespace _20191105_最近傍補間法
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapSource Source;
        public MainWindow()
        {
            InitializeComponent();
        }

        private BitmapSource NearestNeighbor(BitmapSource source, decimal scale)
        {
            int w = source.PixelWidth;
            int h = source.PixelHeight;
            int stride = w * 4;
            byte[] pixels = new byte[h * stride];
            source.CopyPixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);

            //変換後ピクセル数は四捨五入
            int ww = (int)Math.Round(w * scale, MidpointRounding.AwayFromZero);
            int hh = (int)Math.Round(h * scale, MidpointRounding.AwayFromZero);
            if (ww == 0 || hh == 0)
            {
                return source;
            }

            int sstride = ww * 4;
            byte[] ppixels = new byte[hh * sstride];

            decimal rScale = w / (decimal)ww;//逆倍率
            int p, pp;
            for (int y = 0; y < hh; y++)
            {
                for (int x = 0; x < ww; x++)
                {
                    pp = y * sstride + x * 4;
                    int ny = ((int)((decimal)(y + 0.5) * rScale)) * stride;
                    int nx = (int)((decimal)(x + 0.5) * rScale) * 4;
                    p = ny + nx;
                    //p = ((int)(y * rScale) * stride) + ((int)(x * rScale) * 4);//旧式
                    ppixels[pp] = pixels[p];
                    ppixels[pp + 1] = pixels[p + 1];
                    ppixels[pp + 2] = pixels[p + 2];
                    ppixels[pp + 3] = pixels[p + 3];
                }
            }
            return BitmapSource.Create(ww, hh, 96, 96, source.Format, null, ppixels, sstride);
        }

        private BitmapSource NearestNeighbor(BitmapSource source, double scale)
        {
            int w = source.PixelWidth;
            int h = source.PixelHeight;
            int stride = w * 4;
            byte[] pixels = new byte[h * stride];
            source.CopyPixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);

            //変換後ピクセル数は四捨五入
            int ww = (int)Math.Round(w * scale, MidpointRounding.AwayFromZero);
            int hh = (int)Math.Round(h * scale, MidpointRounding.AwayFromZero);
            if (ww == 0 || hh == 0)
            {
                return source;
            }

            int sstride = ww * 4;
            byte[] ppixels = new byte[hh * sstride];

            double rScale = (double)w / ww;//逆倍率
            int p, pp;
            for (int y = 0; y < hh; y++)
            {
                for (int x = 0; x < ww; x++)
                {
                    pp = y * sstride + x * 4;
                    int ny = ((int)((y + 0.5) * rScale)) * stride;
                    int nx = (int)((x + 0.5) * rScale) * 4;
                    p = ny + nx;
                    //p = ((int)(y * rScale) * stride) + ((int)(x * rScale) * 4);//旧式
                    ppixels[pp] = pixels[p];
                    ppixels[pp + 1] = pixels[p + 1];
                    ppixels[pp + 2] = pixels[p + 2];
                    ppixels[pp + 3] = pixels[p + 3];
                }
            }
            return BitmapSource.Create(ww, hh, 96, 96, source.Format, null, ppixels, sstride);
        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            var source = (BitmapSource)MyImage.Source;
            if (source == null) return;
            var s = (Button)sender;
            var scale = double.Parse(s.Tag.ToString());
            MyImage.Source = NearestNeighbor(source, scale);
        }

        private void ButtonDown_Click_1(object sender, RoutedEventArgs e)
        {
            var source = (BitmapSource)MyImage.Source;
            if (source == null) return;
            var s = (Button)sender;
            var scale = 1 / double.Parse(s.Tag.ToString());
            MyImage.Source = NearestNeighbor(source, scale);
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            MyImage.Source = Source;
            MyImage.RenderTransform = new ScaleTransform(1, 1);
        }

        private void ButtonPaste_Click(object sender, RoutedEventArgs e)
        {
            var bmp = Clipboard.GetImage();
            if (bmp == null) return;
            Source = new FormatConvertedBitmap(bmp, PixelFormats.Bgra32, null, 0);
            MyImage.Source = Source;
        }

        private void ButtonDownScaleTransform_Click(object sender, RoutedEventArgs e)
        {
            var s = (Button)sender;
            var scale = 1 / double.Parse(s.Tag.ToString());
            MyImage.RenderTransform = new ScaleTransform(scale, scale);
            RenderOptions.SetBitmapScalingMode(MyImage, BitmapScalingMode.NearestNeighbor);
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            var source = MyImage.Source as BitmapSource;
            if (source == null) return;
            Clipboard.SetImage(source);
        }
    }
}
