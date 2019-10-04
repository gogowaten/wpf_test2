using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace _20190922_クリップボード監視2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        ClipboardWatcher clipboardWatcher = null;//クリップボード監視クラス
        BitmapSource PastBitmap;//画像の比較用、一時記録用

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





        //クリップボード内容変更イベント時の処理
        private void ClipboardWatcher_DrawClipboard(object sender, EventArgs e)
        {
            AddImage();
        }

        //クリップボードに前回と違う画像があったらlistBoxに追加        
        //画像取得でたまにエラーになるけど繰り返すと取得できるから、回数制限をつけて回している
        //それでも取得できなかったらエラーメッセージ表示
        private void AddImage()
        {
            if (Clipboard.ContainsImage())
            {
                var img = new Image();
                int count = 1;
                int limit = 5;//取得試行回数制限、1以上指定、5あれば十分、環境によるかも
                BitmapSource NowBitmap;
                do
                {
                    try
                    {
                        //画像取得
                        NowBitmap = Clipboard.GetImage();//ここでたまにエラーになる

                        //前回の画像と今回の画像が同じなら、なにもしないでreturn
                        if (IsBitmapEqual(PastBitmap, NowBitmap)) return;
                        //違う画像ならリストに追加
                        img.Source = NowBitmap;
                        MyListBox.Items.Add(img);
                        PastBitmap = NowBitmap;
                    }
                    catch (Exception ex)
                    {
                        //指定回数内に画像取得できなかったらメッセージ表示
                        if (count == limit)
                        {
                            string str = $"{ex.Message}\n" +
                                $"画像の取得に失敗\n" +
                                $"{limit}回試行";
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

        /// <summary>
        /// 2つのBitmapSourceが同じ画像(すべてのピクセルの色)なのか判定する、MD5のハッシュ値を作成して比較
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <returns></returns>
        private bool IsBitmapEqual(BitmapSource bmp1, BitmapSource bmp2)
        {
            if (bmp1 == null || bmp2 == null) return false;
            //それぞれのハッシュ値を作成
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] h1 = md5.ComputeHash(MakeBitmapByte(bmp1));
            byte[] h2 = md5.ComputeHash(MakeBitmapByte(bmp2));
            md5.Clear();
            //ハッシュ値を比較
            return IsArrayEquals(h1, h2);
        }
        //2つのハッシュ値を比較
        private bool IsArrayEquals(byte[] h1, byte[] h2)
        {
            for (int i = 0; i < h1.Length; i++)
            {
                if (h1[i] != h2[i])
                {
                    return false;
                }
            }
            return true;
        }
        //BitmapSourceをbyte配列に変換
        private byte[] MakeBitmapByte(BitmapSource bitmap)
        {
            int w = bitmap.PixelWidth;
            int h = bitmap.PixelHeight;
            int stride = w * bitmap.Format.BitsPerPixel / 8;
            byte[] pixels = new byte[h * stride];
            bitmap.CopyPixels(new Int32Rect(0, 0, w, h), pixels, stride, 0);
            return pixels;
        }




        //クリップボード監視開始
        private void MyButtonStart_Click(object sender, RoutedEventArgs e)
        {
            clipboardWatcher.Start();
            MessageBox.Show("start");
        }

        //クリップボード監視停止
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
            //更新通知が来たらイベント起動
            if (msg == WM_DRAWCLIPBOARD)//クリップボード更新がなくても、なぜか5分ごとに更新されたよって通知が来る
            {
                this.raiseDrawClipboard();//イベント起動
                handled = true;//コレがよくわからん
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

//クリップボードの更新を監視、AddClipboardFormatListener - 午後わてんのブログ
//https://gogowaten.hatenablog.com/entry/2019/09/22/143931
