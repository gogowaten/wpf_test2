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

using System.IO;


namespace _20200102_PDFファイルをロックしないで開く
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        Windows.Data.Pdf.PdfDocument MyPdfDocument;
        string MyPdfFilePath;


        public MainWindow()
        {
            InitializeComponent();

            MyPdfFilePath = @"D:\ブログ用\1708_04.pdf";
        }

        //ロックじゃない開き方
        private async void ButtonFree_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(MyPdfFilePath);            
            //ここから
            using (Windows.Storage.Streams.IRandomAccessStream RAStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                MyPdfDocument = await Windows.Data.Pdf.PdfDocument.LoadFromStreamAsync(RAStream);
                //ここまで
                using (Windows.Data.Pdf.PdfPage page = MyPdfDocument.GetPage(0))
                {
                    using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
                    {
                        await page.RenderToStreamAsync(stream);
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream.AsStream();
                        image.EndInit();
                        MyImage.Source = image;
                    }
                }
            }            
        }

        //ロックな開き方
        private async void ButtonLock_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(MyPdfFilePath);
            //ここから
            MyPdfDocument = await Windows.Data.Pdf.PdfDocument.LoadFromFileAsync(file);
            //ここまで
            using (Windows.Data.Pdf.PdfPage page = MyPdfDocument.GetPage(0))
            {
                using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(stream);
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream.AsStream();
                    image.EndInit();
                    MyImage.Source = image;                    
                }
            }
        }

    }
}
