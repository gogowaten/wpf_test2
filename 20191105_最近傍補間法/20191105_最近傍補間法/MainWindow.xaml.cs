using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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


        /// <summary>
        /// 最近傍補間法で画像拡大、対応ピクセルフォーマットはBgra32、他にはBgr32、Pbgra32もできるはず
        /// </summary>
        /// <param name="source"></param>
        /// <param name="scale">拡大倍率</param>
        /// <returns></returns>
        private BitmapSource NearestNeighbor(BitmapSource source, double scale)
        {
            //変換前画像のCopyPixels作成
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

            //変換後の座標から参照するべき変換前の座標を計算
            //変換前座標 = 切り捨て（変換後座標＋0.5）＊逆倍率）
            for (int y = 0; y < hh; y++)
            {
                for (int x = 0; x < ww; x++)
                {
                    int pp = y * sstride + x * 4;//変換後の座標
                    int ny = (int)((y + 0.5) * rScale);//intへのキャストで小数点以下切り捨て
                    int nx = (int)((x + 0.5) * rScale);
                    int p = (ny * stride) + (nx * 4);//変換前の座標

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

        //16倍に拡大
        private void Button16_Click(object sender, RoutedEventArgs e)
        {
            int scale = 16;
            int limit = 1000;//このピクセル数を超える画像は処理しない
            BitmapSource source = (BitmapSource)MyImage.Source;
            int w = source.PixelWidth;
            int h = source.PixelHeight;
            if (w * h > limit)
            {
                return;
            }
            int stride = w * 4;
            byte[] pixels = new byte[h * stride];
            source.CopyPixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);

            int ww = w * scale;
            int hh = h * scale;
            if (ww == 0 || hh == 0)
            {
                MyImage.Source = source;
            }

            int sstride = ww * 4;
            byte[] ppixels = new byte[hh * sstride];

            int p, pp;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    p = y * stride + x * 4;
                    var yy = y * scale;
                    var xx = x * scale;

                    for (int i = 0; i < scale; i++)
                    {
                        for (int j = 0; j < scale; j++)
                        {
                            pp = ((yy + i) * sstride) + ((xx + j) * 4);
                            ppixels[pp] = pixels[p];
                            ppixels[pp + 1] = pixels[p + 1];
                            ppixels[pp + 2] = pixels[p + 2];
                            ppixels[pp + 3] = pixels[p + 3];
                        }
                    }
                }
            }
            MyImage.Source = BitmapSource.Create(ww, hh, 96, 96, source.Format, null, ppixels, sstride);
        }
    }
}
//最近傍補間法で画像の拡大縮小試してみた、2回め - 午後わてんのブログ
//https://gogowaten.hatenablog.com/entry/2019/11/08/182906

//E:\オレ\エクセル\画像処理.xlsm_最近傍法_$A$460