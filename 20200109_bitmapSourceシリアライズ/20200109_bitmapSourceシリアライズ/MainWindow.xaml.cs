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

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace _20200109_bitmapSourceシリアライズ
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string savePath = @"D:\ブログ用\シリアライズテスト20200109.bin";

            string imgPath;
            imgPath = @"D:\ブログ用\20200107_pdftojpeg_05.png";
            var img = new BitmapImage(new Uri(imgPath));
            var data = new MyData(img);

            var deImg = new BitmapImage();


            using (var stream = new System.IO.FileStream(savePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(stream, data);
            }

            MyData deDate;
            using (var stream = new System.IO.FileStream(savePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                var bf = new BinaryFormatter();
                deDate = (MyData)bf.Deserialize(stream);
                var inu = deDate.PixelFormat;
            }

            var uma = deDate.pixels;
            

        }
    }

    [Serializable]
    public class MyData
    {
        public int stride;
        public int width;
        public int height;
        public byte[] pixels;
        public PixelFormat PixelFormat;

        public MyData(BitmapSource source)
        {
            width = source.PixelWidth;
            height = source.PixelHeight;
            PixelFormat = source.Format;
            stride = (width * PixelFormat.BitsPerPixel + 7) / 8;
            pixels = new byte[height * stride];
            source.CopyPixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
        }
    }
}