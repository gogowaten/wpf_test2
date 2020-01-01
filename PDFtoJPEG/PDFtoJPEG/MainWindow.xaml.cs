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

using System.IO;//必須
using Windows.Data.Pdf;
using MyNumericUpDownInteger;

//下の2つを参照に追加する必要がある
//"C:\Program Files (x86)\Windows Kits\8.1\References\CommonConfiguration\Neutral\Annotated\Windows.winmd"
//"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll"

//参照したところ
//WPFアプリにPDFを表示する — 某エンジニアのお仕事以外のメモ（分冊）
//https://water2litter.net/rum/post/cs_pdf_wpf/

//    [UWP][PDF] PDFファイルを表示する | HIROs.NET Blog
//http://blog.hiros-dot.net/?p=7346

//C# Taskの待ちかた集 - Qiita
//https://qiita.com/takutoy/items/d45aa736ced25a8158b3


//農林水産省 トマトの種類と見分け方
//http://www.maff.go.jp/j/pr/aff/1708/pdf/1708_04.pdf


namespace PDFtoJPEG
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private PdfDocument MyPdfDocument;//PDFファイルを読み込んだもの
        //private string MyPdfPath;//読み込んだPDFファイルのフルパス
        private string MyPdfDirectory;//読み込んだPDFファイルのフォルダ
        private string MyPdfName;//読み込んだPDFファイル名
        private double MyDpi;//PDFを画像に変換する時のDPI

        public MainWindow()
        {
            InitializeComponent();

            this.Title = "PDFtoJPEG";
            this.AllowDrop = true;
            this.Drop += MainWindow_Drop;


            MyScrollViewer.PreviewMouseLeftButtonDown += (s, e) => { Panel.SetZIndex(MyImage, 1); };
            MyScrollViewer.PreviewMouseLeftButtonUp += (s, e) => { Panel.SetZIndex(MyImage, -1); };

        }

        //private void MyScrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    Panel.SetZIndex(MyImage, -1);
        //}

        //private void MyScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{            
        //    Panel.SetZIndex(MyImage, 1);
        //}



        //ファイルがドロップされたとき
        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) == false) { return; }

            string[] filePath = (string[])e.Data.GetData(DataFormats.FileDrop);
            LoadPdf(filePath[0]);
        }

        //PDFファイルを読み込んで最初のページを表示
        private async void LoadPdf(string filePath)
        {
            var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(filePath);
            try
            {
                MyPdfDocument = await PdfDocument.LoadFromFileAsync(file);
                MyPdfDirectory = System.IO.Path.GetDirectoryName(filePath);
                MyPdfName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                MyDpi = 96;
                DisplayImage(0, 96);//表示
                NumePageIndex.Value = 1;

                var pageCount = MyPdfDocument.PageCount;
                tbPageCount.Text = $"{pageCount.ToString()} ページ";
                NumePageIndex.Max = (int)pageCount;
            }
            catch (Exception)
            { }
        }

        //PDFファイルを画像に変換して表示
        private async void DisplayImage(int pageIndex, double dpi)
        {
            if (MyPdfDocument == null) { return; }
            int c = (int)MyPdfDocument.PageCount - 1;
            if (pageIndex > c)
            {
                pageIndex = c;
            }
            MyDpi = dpi;
            using (PdfPage page = MyPdfDocument.GetPage((uint)pageIndex))
            {
                double h = page.Size.Height;
                var options = new PdfPageRenderOptions();
                options.DestinationHeight = (uint)Math.Round(page.Size.Height * (dpi / 96.0), MidpointRounding.AwayFromZero);
                tbDpi.Text = $"{dpi.ToString()} dpi";
                tbHeight.Text = $"縦{options.DestinationHeight.ToString()} px";

                using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(stream, options);

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream.AsStream();//using System.IOがないとエラーになる
                    image.EndInit();
                    MyImage.Source = image;
                    MyImage.Width = image.PixelWidth;
                    MyImage.Height = image.PixelHeight;
                    //プレビュー用のjpeg画像表示
                    MyImagePreviwer.Source = MakeJpegPreviewImage(image, NumeJpegQuality.Value);
                }
            }
        }

        //プレビュー用のjpeg画像作成
        private BitmapSource MakeJpegPreviewImage(BitmapSource source, int quality)
        {
            if (source == null) { return null; }
            var encoder = new JpegBitmapEncoder();
            JpegBitmapDecoder decoder;
            encoder.QualityLevel = quality;
            encoder.Frames.Add(BitmapFrame.Create(source));
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                decoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                var fileSize = stream.Length / 1000;
                if (fileSize > 1000)
                {
                    tbFileSize.Text = $"{(stream.Length / 1000 / 1000.0).ToString(".0")} MB";
                }
                else
                {
                    tbFileSize.Text = $"{stream.Length / 1000} KB";
                }
            }
            return decoder.Frames[0];
        }


        //DPI指定ボタンクリック時
        private void ButtonDpi96_Click(object sender, RoutedEventArgs e)
        {
            DisplayImage(NumePageIndex.Value - 1, 96);
        }

        private void ButtonDpi150_Click(object sender, RoutedEventArgs e)
        {
            DisplayImage(NumePageIndex.Value - 1, 150);
        }

        private void ButtonDpi300_Click(object sender, RoutedEventArgs e)
        {
            DisplayImage(NumePageIndex.Value - 1, 300);
        }

        private void ButtonDpi600_Click(object sender, RoutedEventArgs e)
        {
            DisplayImage(NumePageIndex.Value - 1, 600);
        }

        //プレビュー画像の更新
        private void ButtonPreviweRenew_Click(object sender, RoutedEventArgs e)
        {
            if (MyImage.Source == null) { return; }
            DisplayImage(NumePageIndex.Value - 1, MyDpi);
            //MyImagePreviwer.Source = MakeJpegPreviewImage((BitmapSource)MyImage.Source, NumeJpegQuality.Value);
        }






        /// <summary>
        /// jpeg画像で保存
        /// </summary>
        /// <param name="pdfDocument">読み込んだPDF、Windows.Data.Pdf.PdfDocument</param>
        /// <param name="dpi">PDFファイルを読み込む時のDPI</param>
        /// <param name="directory">保存フォルダ</param>
        /// <param name="fileName">保存名</param>
        /// <param name="pageIndex">保存するPDFのページ</param>
        /// <param name="quality">jpegの品質min0、max100</param>
        /// <param name="keta">保存名につける連番0埋めの桁数</param>
        /// <returns></returns>
        private async Task SaveSub2(PdfDocument pdfDocument, double dpi, string directory, string fileName, int pageIndex, int quality, int keta)
        {
            using (PdfPage page = pdfDocument.GetPage((uint)pageIndex))
            {
                //指定されたdpiを元に画像サイズ指定、四捨五入                
                var options = new PdfPageRenderOptions();
                options.DestinationHeight = (uint)Math.Round(page.Size.Height * (dpi / 96.0), MidpointRounding.AwayFromZero);

                using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(stream, options);

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream.AsStream();
                    image.EndInit();

                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.QualityLevel = quality;
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    pageIndex++;
                    string renban = pageIndex.ToString("d" + keta);
                    using (var fileStream = new FileStream(
                        System.IO.Path.Combine(directory, fileName) + "_" + renban + ".jpg", FileMode.Create, FileAccess.Write))
                    {
                        encoder.Save(fileStream);
                    }
                }
            }

        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (MyPdfDocument == null) { return; }

            this.IsEnabled = false;
            try
            {
                int keta = MyPdfDocument.PageCount.ToString().Length;//0埋め連番の桁数

                //各ページの保存処理のリスト作成
                var MyTasks = new List<Task>();
                for (int i = 0; i < MyPdfDocument.PageCount; i++)
                {
                    MyTasks.Add(SaveSub2(MyPdfDocument, MyDpi, MyPdfDirectory, MyPdfName, i, NumeJpegQuality.Value, keta));
                }

                //各タスク実行
                for (int i = 0; i < MyTasks.Count; i++)
                {
                    await MyTasks[i];
                }

                MessageBox.Show("処理完了");
            }
            catch (Exception ex) { MessageBox.Show($"なんかエラー出たわ \n {ex.Message} \n {ex.ToString()}"); }

            finally { this.IsEnabled = true; }

        }


    }

}
