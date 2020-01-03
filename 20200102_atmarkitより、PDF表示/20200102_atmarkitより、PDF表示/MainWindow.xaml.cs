using System;
using System.Windows;

//Windows 8.1の新機能、PDFを表示するには？［Windows 8.1ストア・アプリ開発］：WinRT／Metro TIPS - ＠IT
//https://www.atmarkit.co.jp/ait/articles/1310/24/news070.html


//下の2つを参照に追加する必要がある
//"C:\Program Files (x86)\Windows Kits\8.1\References\CommonConfiguration\Neutral\Annotated\Windows.winmd"
//"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll"

namespace _20200102_atmarkitより_PDF表示
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        const string PdfFileName = @"D:\ブログ用\1708_04.pdf";

        public MainWindow()
        {
            InitializeComponent();

            this.LoadPdfDocument();
        }

        private async void LoadPdfDocument()
        {
            Windows.Storage.StorageFile pdfFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(PdfFileName);
            await pdfView1.LoadPdfDocumentAsync(pdfFile);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            pdfView1.RenderPage(1);
        }
    }
}
