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
using System.Runtime.InteropServices;
using System.Windows.Interop;

//AddClipboardFormatListenerを使ったクリップボード監視

//クリップボードが更新されたとき、中に画像があればリストに追加する
//なぜかクリップボードから画像を取り出すときにClipboard openエラーになることがあるので
//取り出せるまで２～３回試行している
namespace _20190922_クリップボード監視
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool AddClipboardFormatListener(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

        private const int WM_DRAWCLIPBOARD = 0x031D;


        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RemoveClipboardFormatListener(new WindowInteropHelper(this).Handle);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AddClipboardFormatListener(new WindowInteropHelper(this).Handle);
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc2));
        }


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DRAWCLIPBOARD)
            {
                try
                {
                    //if (Clipboard.ContainsText()) MyListBox.Items.Add(Clipboard.GetText());

                    if (Clipboard.ContainsImage())
                    {
                        var img = new Image();
                        img.Source = Clipboard.GetImage();
                        MyListBox.Items.Add(img);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    //MessageBox.Show(e.ToString());
                }
                finally
                {
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }


        private IntPtr WndProc2(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DRAWCLIPBOARD)
            {
                if (Clipboard.ContainsImage())
                {
                    BitmapSource source;
                    Image img = new Image();
                    bool isEmpty = true;//データ取得確認用
                    int count = 0;
                    int limit = 2;//最大試行回数を指定、1以上を指定、3あればほぼ問題ない、4以上はいらないかなあ

                    do
                    {
                        try
                        {
                            //画像を取得してlistBoxに追加
                            source = Clipboard.GetImage();//ここでエラーになる
                            isEmpty = false;
                            img.Source = source;
                            MyListBox.Items.Add(img);
                        }
                        catch (Exception e)
                        {
                            if (count == limit - 1)
                            {
                                string str = e.Message + "\r\n" + "クリップボードからの画像取得に失敗" + "\r\n" + limit.ToString();
                                MessageBox.Show(str);
                            }
                        }
                        finally
                        {
                            count++;
                            handled = true;
                        }
                    } while (isEmpty && limit > count);

                }

            }
            return IntPtr.Zero;
        }

    }
}
