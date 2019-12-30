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


namespace _20191230_pdf
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Windows.Data.Pdf.PdfDocument PdfDocument;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {   
            string filePath;
            filePath = @"D:\ブログ用\1708_04.pdf";            
            var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(filePath);

            try
            {
                PdfDocument = await Windows.Data.Pdf.PdfDocument.LoadFromFileAsync(file);
            }
            catch (Exception)
            {

            }

            if (PdfDocument != null)
            {

                using (Windows.Data.Pdf.PdfPage page = PdfDocument.GetPage(0))
                {
                    BitmapImage image = new BitmapImage();
                    using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
                    {
                        await page.RenderToStreamAsync(stream);

                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream.AsStream();
                        image.EndInit();
                    }
                    ImagePDF.Source = image;
                }
            }
        }

        private async void ButtonOpenScale_Click(object sender, RoutedEventArgs e)
        {
            var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(@"D:\ブログ用\1708_04.pdf");
            try
            {
                PdfDocument = await Windows.Data.Pdf.PdfDocument.LoadFromFileAsync(file);
            }
            catch (Exception)
            {

            }

            if (PdfDocument != null)
            {
                using (Windows.Data.Pdf.PdfPage page = PdfDocument.GetPage(0))
                {
                    BitmapImage image = new BitmapImage();
                    using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
                    {
                        var options = new Windows.Data.Pdf.PdfPageRenderOptions();
                        options.DestinationHeight = 1000;
                        await page.RenderToStreamAsync(stream, options);

                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream.AsStream();
                        image.EndInit();
                    }
                    ImagePDF.Source = image;
                }
            }
        }
    }
}
