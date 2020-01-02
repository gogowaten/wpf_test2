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

//下の2つを参照に追加する必要がある
//"C:\Program Files (x86)\Windows Kits\8.1\References\CommonConfiguration\Neutral\Annotated\Windows.winmd"
//"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll"

//参照したところ
//.NET Core デスクトップアプリケーションで PDF 帳票を画面表示する - rksoftware
//https://rksoftware.hatenablog.com/entry/2019/01/31/032913
//【UWP】UWPアプリでインターネット上のPDFファイルを表示する - Qiita
//https://qiita.com/ryo-ta/items/81edc024fa70c2e5ed99
//PdfDocument.LoadFromStreamAsync - Google 検索
//https://www.google.com/search?q=PdfDocument.LoadFromStreamAsync



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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyPdfDocument = null;
            MyImage.Source = null;
        }
    }
}
