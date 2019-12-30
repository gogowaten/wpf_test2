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


/*パラレル処理がうまくできない
 *pdfのページをレンダーする RenderToStreamAsync これがawaitなせいか
 * parallel.forを使っても、parallel.invockを使っても速くならない
*/

namespace _20191230_pdf2
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

            string path;
            path = @"D:\ブログ用\1708_04.pdf";
            path = @"E:\アプリ\ダウンロード\JDownloader\JDownloader\downloads\MSXFAN HYDLIDE3 (Feb 1988).pdf";
            path = @"M:\小説ラノベ\(pc雑誌) MSXFAN\MSXFAN198704.pdf";
            LoadPdf(path);
        }

        private async void LoadPdf(string filePath)
        {
            var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(filePath);
            try
            {
                PdfDocument = await Windows.Data.Pdf.PdfDocument.LoadFromFileAsync(file);
            }
            catch (Exception)
            { }
        }

        private void DisplayStatus()
        {
            if (PdfDocument == null) { return; }
            tbPagesCount.Text = PdfDocument.PageCount.ToString();
            Windows.Data.Pdf.PdfPage page = PdfDocument.GetPage(0);
            tbDpi.Text = page.Size.Height.ToString();
            page.Dispose();
        }

        private void MyDpi300_Click(object sender, RoutedEventArgs e)
        {
            PageLoad(0, 300);
        }

        private void ButtonDisplayStatus_Click(object sender, RoutedEventArgs e)
        {
            DisplayStatus();
        }

        private void ButtonPageLoad_Click(object sender, RoutedEventArgs e)
        {
            PageLoad(0, 96);

        }


        /// <summary>
        /// 表示
        /// </summary>
        /// <param name="pageNumber">指定ページ</param>
        /// <param name="displayDpi">指定dpi、通常なら96</param>
        private async void PageLoad(uint pageNumber, double displayDpi)
        {
            if (PdfDocument == null) { return; }
            BitmapImage image = new BitmapImage();
            var rate = displayDpi / 96.0;//表示倍率みたいなもの
            using (Windows.Data.Pdf.PdfPage page = PdfDocument.GetPage(pageNumber))
            {
                //指定されたdpiを元にレンダーサイズ指定、四捨五入
                double h = page.Size.Height;
                var options = new Windows.Data.Pdf.PdfPageRenderOptions();
                options.DestinationHeight = (uint)Math.Round(h * rate, MidpointRounding.AwayFromZero);

                using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(stream, options);

                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream.AsStream();
                    image.EndInit();
                }
            }
            MyImage.Source = image;
            MyImage.Width = image.PixelWidth;
            MyImage.Height = image.PixelHeight;
        }

        private void SaveImage(BitmapSource source, string ImageFileFullPath)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "*.png|*.png|*.bmp|*.bmp|*.tiff|*.tiff|*.jpg|*.jpg";
            saveFileDialog.AddExtension = true;
            saveFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(ImageFileFullPath) + "_";
            saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(ImageFileFullPath);
            if (saveFileDialog.ShowDialog() == true)
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                if (saveFileDialog.FilterIndex == 1)
                {
                    encoder = new PngBitmapEncoder();
                }
                else if (saveFileDialog.FilterIndex == 2)
                {
                    encoder = new BmpBitmapEncoder();
                }
                else if (saveFileDialog.FilterIndex == 3)
                {
                    encoder = new TiffBitmapEncoder();
                }
                else if (saveFileDialog.FilterIndex == 4)
                {
                    var je = new JpegBitmapEncoder();
                    je.QualityLevel = 96;
                    encoder = je;
                }

                encoder.Frames.Add(BitmapFrame.Create(source));
                using (var fs = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    encoder.Save(fs);
                }
            }
        }

        private void MySave_Click(object sender, RoutedEventArgs e)
        {
            MakeImageAndSave(96);
        }

        private async void MakeImageAndSave(double displayDpi)
        {
            if (PdfDocument == null) { return; }
            string filePath = @"D:\ブログ用\1708_04";

            var rate = displayDpi / 96.0;//表示倍率みたいなもの
            for (uint i = 0; i < PdfDocument.PageCount; i++)
            {
                using (Windows.Data.Pdf.PdfPage page = PdfDocument.GetPage(i))
                {
                    //指定されたdpiを元にレンダーサイズ指定、四捨五入
                    double h = page.Size.Height;
                    var options = new Windows.Data.Pdf.PdfPageRenderOptions();
                    options.DestinationHeight = (uint)Math.Round(h * rate, MidpointRounding.AwayFromZero);

                    using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
                    {
                        await page.RenderToStreamAsync(stream, options);

                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream.AsStream();
                        image.EndInit();

                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                        encoder.QualityLevel = 85;
                        encoder.Frames.Add(BitmapFrame.Create(image));
                        using (var fileStream = new FileStream(filePath + "_" + i + ".jpg", FileMode.Create, FileAccess.Write))
                        {
                            encoder.Save(fileStream);
                        }
                    }

                }
            }



        }

        private async void SaveSub(double rate, string filePath, uint i)
        {
            using (Windows.Data.Pdf.PdfPage page = PdfDocument.GetPage((uint)i))
            {
                //指定されたdpiを元にレンダーサイズ指定、四捨五入
                double h = page.Size.Height;
                var options = new Windows.Data.Pdf.PdfPageRenderOptions();
                options.DestinationHeight = (uint)Math.Round(h * rate, MidpointRounding.AwayFromZero);

                using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(stream, options);

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream.AsStream();
                    image.EndInit();

                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.QualityLevel = 85;
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    using (var fileStream = new FileStream(filePath + "_" + i + ".jpg", FileMode.Create, FileAccess.Write))
                    {
                        encoder.Save(fileStream);
                    }
                }
            }
        }
        private void MakeImageAndSaveParallel(double displayDpi)
        {
            if (PdfDocument == null) { return; }
            string filePath = @"D:\ブログ用\1708_04";

            var rate = displayDpi / 96.0;//表示倍率みたいなもの
            Parallel.For(0, PdfDocument.PageCount, i =>
             {
                 SaveSub(rate, filePath, (uint)i);
             });
        }

        private void MySaveParallel_Click(object sender, RoutedEventArgs e)
        {
            MakeImageAndSaveParallel(96);
        }



        private void MakeImageAndSaveParallel2(double displayDpi)
        {
            if (PdfDocument == null) { return; }
            string filePath = @"D:\ブログ用\1708_04";

            var rate = displayDpi / 96.0;//表示倍率みたいなもの
            Parallel.For(0, PdfDocument.PageCount, async i =>
            {
                await Task.Run(() => SaveSub(rate, filePath, (uint)i));
            });
        }

        private async void MakeImageAndSaveParallel3(double displayDpi)
        {
            if (PdfDocument == null) { return; }
            string filePath = @"D:\ブログ用\1708_04";

            var rate = displayDpi / 96.0;//表示倍率みたいなもの

            await Task.Run(() =>
            {
                Parallel.For(0, PdfDocument.PageCount, i =>
                {
                    SaveSub(rate, filePath, (uint)i);
                });
            });
        }

        

        private void MySaveParallel2_Click(object sender, RoutedEventArgs e)
        {
            //MakeImageAndSaveParallel2(96);
            MakeImageAndSaveParallel3(96);
            
        }


        //invokeどちらもシングルになっているみたい
        private void MakeImageAndSaveParallelInvoke(double displayDpi)
        {
            if (PdfDocument == null) { return; }
            string filePath = @"D:\ブログ用\1708_04";

            var rate = displayDpi / 96.0;//表示倍率みたいなもの
            Parallel.For(0, PdfDocument.PageCount, i =>
            {
                Parallel.Invoke(() =>
                {
                    SaveSub(rate, filePath, (uint)i);
                });
            });
            //for (int i = 0; i < PdfDocument.PageCount; i++)
            //{
            //    Parallel.Invoke(() =>
            //    {
            //        SaveSub(rate, filePath, (uint)i);
            //    });
            //}            
        }
        private void MySaveParallelInvoke_Click(object sender, RoutedEventArgs e)
        {
            MakeImageAndSaveParallelInvoke(96);
        }




        private async Task<int> SaveSubTask(double rate, string filePath, uint i)
        {
            using (Windows.Data.Pdf.PdfPage page = PdfDocument.GetPage((uint)i))
            {
                //指定されたdpiを元にレンダーサイズ指定、四捨五入
                double h = page.Size.Height;
                var options = new Windows.Data.Pdf.PdfPageRenderOptions();
                options.DestinationHeight = (uint)Math.Round(h * rate, MidpointRounding.AwayFromZero);

                using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(stream, options);

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream.AsStream();
                    image.EndInit();

                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.QualityLevel = 85;
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    using (var fileStream = new FileStream(filePath + "_" + i + ".jpg", FileMode.Create, FileAccess.Write))
                    {
                        encoder.Save(fileStream);
                    }
                }
            }
            return (int)i;
        }

    }
}
