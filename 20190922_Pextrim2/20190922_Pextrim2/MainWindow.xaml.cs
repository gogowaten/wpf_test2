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

namespace _20190922_Pextrim2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClipboardWatcher ClipboardWatcher;
        private List<BitmapSource> ListBitmap;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            CheckBox_ClipCheck.Click += CheckBox_ClipCheck_Click;
            
            ListBitmap = new List<BitmapSource>();

        }

        private void ClipboardWatcher_DrawClipboard(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                BitmapSource bitmap = null;
                int count = 1;
                int limit = 1;
                do
                {
                    try
                    {
                        bitmap = Clipboard.GetImage();
                        ListBitmap.Add(bitmap);
                        MyImage.Source = bitmap;
                        MyCanvas.Width = bitmap.PixelWidth;
                        MyCanvas.Height = bitmap.PixelHeight;
                        MyListBox.Items.Add(TextBox_FileName.Text + TextBox_SerialNumber.Text);
                        int sn = int.Parse(TextBox_SerialNumber.Text)+1;
                        //TextBox_SerialNumber.Text = $"{sn,0:D4}";
                        TextBox_SerialNumber.Text = $"{sn,0:0000}";
                    }
                    catch (Exception ex)
                    {
                        if (count == limit)
                        {
                            string str = $"{limit}回試したけど画像の取得に失敗\n{ex.Message}";
                        }
                    }
                    finally { count++; }
                } while (limit >= count && bitmap == null);
            }
        }

        private void CheckBox_ClipCheck_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBox_ClipCheck.IsChecked == true) { ClipboardWatcher.Start(); }
            else ClipboardWatcher.Stop();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            ClipboardWatcher = new ClipboardWatcher(
                new System.Windows.Interop.WindowInteropHelper(this).Handle);
            ClipboardWatcher.DrawClipboard += ClipboardWatcher_DrawClipboard;
        }

    }


}
