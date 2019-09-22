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


namespace _20190922_クリップボード監視2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        ClipboardWatcher clipboardWatcher = null;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;

        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            clipboardWatcher.Stop();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Clipboardwatcher作成して、自身を登録、クリップボード監視開始
            clipboardWatcher = new ClipboardWatcher(new WindowInteropHelper(this).Handle);
            //クリップボード内容変更イベントに関連付け
            clipboardWatcher.DrawClipboard += ClipboardWatcher_DrawClipboard;
        }

        //イベント発生時の処理
        // クリップボード内容が変更されたとき
        //画像があったらlistBoxに追加        
        private void ClipboardWatcher_DrawClipboard(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                var img = new Image();
                img.Source = Clipboard.GetImage();
                MyListBox.Items.Add(img);
            }
        }

        private void MyButtonStart_Click(object sender, RoutedEventArgs e)
        {
            clipboardWatcher.Start();//クリップボード監視開始
            MessageBox.Show("start");
        }

        private void MyButtonStop_Click(object sender, RoutedEventArgs e)
        {
            clipboardWatcher.Stop();//クリップボード監視終了
            MessageBox.Show("stop");
        }
    }



    public class ClipboardWatcher
    {
        [DllImport("user32.dll")]
        private static extern bool AddClipboardFormatListener(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

        private const int WM_DRAWCLIPBOARD = 0x031D;

        IntPtr handle;
        HwndSource hwndSource = null;

        public event EventHandler DrawClipboard;
        private void raiseDrawClipboard()
        {
            if (DrawClipboard != null)
            {
                DrawClipboard(this, EventArgs.Empty);
            }
        }


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DRAWCLIPBOARD)
            {
                this.raiseDrawClipboard();
                handled = true;
            }
            return IntPtr.Zero;
        }

        public ClipboardWatcher(IntPtr handle)
        {
            hwndSource = HwndSource.FromHwnd(handle);
            hwndSource.AddHook(WndProc);
            this.handle = handle;
            AddClipboardFormatListener(handle);
        }

        public void Start()
        {
            AddClipboardFormatListener(handle);

        }
        public void Stop()
        {
            RemoveClipboardFormatListener(handle);
        }
    }
}
