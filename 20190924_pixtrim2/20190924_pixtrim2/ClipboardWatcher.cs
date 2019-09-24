using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Windows.Interop;


namespace _20190924_pixtrim2
{
    class ClipboardWatcher
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
            //AddClipboardFormatListener(handle);
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
