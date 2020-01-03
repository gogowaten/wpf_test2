using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using System.IO;

//Windows 8.1の新機能、PDFを表示するには？［Windows 8.1ストア・アプリ開発］：WinRT／Metro TIPS - ＠IT
//https://www.atmarkit.co.jp/ait/articles/1310/24/news070.html


//下の2つを参照に追加する必要がある
//"C:\Program Files (x86)\Windows Kits\8.1\References\CommonConfiguration\Neutral\Annotated\Windows.winmd"
//"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll"


namespace _20200102_atmarkitより_PDF表示
{
    /// <summary>
    /// PdfView.xaml の相互作用ロジック
    /// </summary>
    public partial class PdfView : UserControl
    {
        Windows.Data.Pdf.PdfDocument _pdfDoc;
        public uint PageCount { get { return _pdfDoc.PageCount; } }


        public PdfView()
        {
            InitializeComponent();
        }


        public async Task<uint> LoadPdfDocumentAsync(Windows.Storage.StorageFile pdfFile)
        {
            _pdfDoc = await Windows.Data.Pdf.PdfDocument.LoadFromFileAsync(pdfFile);
            return this.PageCount;
        }

        uint _currentPageIndex;
        private async Task<Windows.Storage.Streams.InMemoryRandomAccessStream> RenderPageBitmapAsync()
        {
            using (Windows.Data.Pdf.PdfPage pdfPage = _pdfDoc.GetPage(_currentPageIndex))
            {
                var memStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
                await pdfPage.RenderToStreamAsync(memStream);
                return memStream;
            }
        }

        private void ShowImageAsync(Windows.Storage.Streams.InMemoryRandomAccessStream bitmapStream)
        {
            //var bitmap = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
            //await bitmap.SetSourceAsync(bitmapStream);

            //image1.Width = bitmap.PixelWidth;
            //image1.Height = bitmap.PixelHeight;
            //image1.Source = bitmap;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = bitmapStream.AsStream();
            bitmap.EndInit();
            image1.Source = bitmap;
        }

        public uint RenderPage(uint pageIndex)
        {
            if (_currentPageIndex == pageIndex) return _currentPageIndex;
            if (_pdfDoc.PageCount <= pageIndex) return _currentPageIndex;
            _currentPageIndex = pageIndex;
            RenderPageAsync();
            return _currentPageIndex;
        }

        private async void RenderPageAsync()
        {
            if (_pdfDoc == null || _pdfDoc.PageCount == 0) return;
            using (Windows.Storage.Streams.InMemoryRandomAccessStream bitmapStream=await RenderPageBitmapAsync())
            {
                ShowImageAsync(bitmapStream);
            }
        }
    }
}
