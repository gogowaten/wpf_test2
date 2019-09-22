using System;
using System.Windows;
using System.Windows.Controls;
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

        //アプリ起動直後
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Clipboardwatcher作成
            //ウィンドウハンドルを渡す
            clipboardWatcher = new ClipboardWatcher(new WindowInteropHelper(this).Handle);
            //クリップボード内容変更イベントに関連付け
            clipboardWatcher.DrawClipboard += ClipboardWatcher_DrawClipboard;
        }

        //private void ClipboardWatcher_DrawClipboard(object sender, EventArgs e)
        //{
        //    if (Clipboard.ContainsImage())
        //    {
        //        var img = new Image();
        //        img.Source = Clipboard.GetImage();
        //        MyListBox.Items.Add(img);
        //    }
        //}

        //クリップボード内容変更イベント時の処理
        private void ClipboardWatcher_DrawClipboard(object sender, EventArgs e)
        {
            AddImage();
        }

        //画像があったらlistBoxに追加        
        //たまにエラーになるけど繰り返すと取得できるから
        //回数制限をつけて回している
        //それでも取得できなかったらエラーメッセージ表示
        private void AddImage()
        {
            if (Clipboard.ContainsImage())
            {
                var img = new Image();
                int count = 1;
                int limit = 3;//取得試行回数制限
                do
                {
                    try
                    {
                        img.Source = Clipboard.GetImage();//たまにエラーになる
                        MyListBox.Items.Add(img);
                    }
                    catch (Exception ex)
                    {
                        if (count == limit)
                        {
                            string str = $"{ex.Message}\n" +
                                $"画像の取得に失敗\n" +
                                $"{count}回試行";
                            MessageBox.Show(str);
                        }
                    }
                    finally
                    {
                        count++;
                    }
                } while (limit >= count && img.Source == null);
            }
        }

        //クリップボード監視開始
        private void MyButtonStart_Click(object sender, RoutedEventArgs e)
        {
            clipboardWatcher.Start();
            MessageBox.Show("start");
        }

        //クリップボード監視中止
        private void MyButtonStop_Click(object sender, RoutedEventArgs e)
        {
            clipboardWatcher.Stop();
            MessageBox.Show("stop");
        }
    }


    /// <summary>
    /// AddClipboardFormatListenerを使ったクリップボード監視
    /// クリップボード更新されたらDrawClipboardイベント起動
    /// </summary>
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
        //イベント起動
        private void raiseDrawClipboard()
        {
            DrawClipboard?.Invoke(this, EventArgs.Empty);
        }
        //↑は↓と同じ意味
        //private void raiseDrawClipboard()
        //{
        //    if (DrawClipboard != null)
        //    {
        //        DrawClipboard(this, EventArgs.Empty);
        //    }
        //}


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DRAWCLIPBOARD)
            {
                this.raiseDrawClipboard();
                handled = true;
            }
            return IntPtr.Zero;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle">System.Windows.Interop.WindowInteropHelper(this).Handleとかで取得</param>
        public ClipboardWatcher(IntPtr handle)
        {
            hwndSource = HwndSource.FromHwnd(handle);
            hwndSource.AddHook(WndProc);
            this.handle = handle;
            AddClipboardFormatListener(handle);
        }

        //クリップボード監視開始
        public void Start()
        {
            AddClipboardFormatListener(handle);
        }

        //クリップボード監視停止
        public void Stop()
        {
            RemoveClipboardFormatListener(handle);
        }
    }
}
