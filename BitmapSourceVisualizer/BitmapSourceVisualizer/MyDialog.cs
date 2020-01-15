using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


[assembly: System.Diagnostics.DebuggerVisualizer(
    typeof(BitmapSourceVisualizer.MyDialog),
    typeof(BitmapSourceVisualizer.MySource),
    Target = typeof(BitmapSource),
    Description = "BitmapSourceVisualizer")]
namespace BitmapSourceVisualizer
{
    // TODO: SomeType のインスタンスをデバッグするときに、このビジュアライザーを表示するために SomeType の定義に次のコードを追加します:
    // 
    //  [DebuggerVisualizer(typeof(MyDialog))]
    //  [Serializable]
    //  public class SomeType
    //  {
    //   ...
    //  }
    // 
    /// <summary>
    /// SomeType のビジュアライザーです。  
    /// </summary>
    public class MyDialog : DialogDebuggerVisualizer
    {
        private Form MyForm;
        private Bitmap OriginBitmap;
        private BitmapSource OriginBitmapSource;//
        private PictureBox MyPictureBox;

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException("windowService");
            if (objectProvider == null)
                throw new ArgumentNullException("objectProvider");

            //BitmapSourceをBitmapへ変換
            var data = (MyProxy)objectProvider.GetObject();
            OriginBitmapSource = BitmapSource.Create(data.Width, data.Height, 96, 96, PixelFormats.Bgra32, null, data.Pixels, data.Stride);
            OriginBitmap = new Bitmap(data.Width, data.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bmpData = OriginBitmap.LockBits(new Rectangle(0, 0, OriginBitmap.Width, OriginBitmap.Height), ImageLockMode.ReadOnly, OriginBitmap.PixelFormat);
            OriginBitmapSource.CopyPixels(Int32Rect.Empty, bmpData.Scan0, bmpData.Height * bmpData.Stride, bmpData.Stride);
            OriginBitmap.UnlockBits(bmpData);

            //this.OriginBitmap = (Bitmap)objectProvider.GetObject();

            //Form作成表示
            using (MyForm = new Form())
            {
                //FormにボタンとかPictureBox追加
                AddToolStrip();
                MyForm.Text = "BitmapSourceVisualizer";
                MyForm.BackColor = System.Drawing.Color.White;
                MyForm.Width = Screen.PrimaryScreen.Bounds.Width / 2;
                MyForm.Height = Screen.PrimaryScreen.Bounds.Height / 2;
                windowService.ShowDialog(MyForm);
            }
            this.OriginBitmap.Dispose();
        }

        // TODO: ビジュアライザーをテストするために、次のコードをユーザーのコードに追加します:
        // 
        //    MyDialog.TestShowVisualizer(new SomeType());
        // 
        /// <summary>
        /// デバッガーの外部にホストすることにより、ビジュアライザーをテストします。
        /// </summary>
        /// <param name="objectToVisualize">ビジュアライザーに表示するオブジェクトです。</param>
        public static void TestShowVisualizer(object objectToVisualize)
        {
            VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(MyDialog), typeof(MySource));
            visualizerHost.ShowVisualizer();
        }


        private void AddToolStrip()
        {
            var toolStrip = new ToolStrip();
            //ts.SuspendLayout();

            ToolStripButton button;
            button = new ToolStripButton { Text = "保存(&S)" };
            button.Click += (s, e) => { SaveImage(OriginBitmap); };
            toolStrip.Items.Add(button);

            button = new ToolStripButton { Text = "コピー(&C)" };
            button.Click += (s, e) => { System.Windows.Forms.Clipboard.SetImage(OriginBitmap); };
            toolStrip.Items.Add(button);

            //var b3 = new ToolStripButton { Text = "x2" };
            //b3.Click += (e, x) => { Scale2(); };
            //ts.Items.Add(b3);


            button = new ToolStripButton { Text = "ウィンドウに合わせる(&Z)" };
            button.Click += (e, x) => { MyPictureBox.SizeMode = PictureBoxSizeMode.Zoom; MyPictureBox.Dock = DockStyle.Fill; };
            toolStrip.Items.Add(button);

            button = new ToolStripButton { Text = "実寸(&A)" };
            button.Click += (o, e) => { MyPictureBox.SizeMode = PictureBoxSizeMode.AutoSize; MyPictureBox.Dock = DockStyle.None; };
            toolStrip.Items.Add(button);

            button = new ToolStripButton { Text = "背景黒(&B)" };
            button.Click += (s, e) => { MyForm.BackColor = System.Drawing.Color.Black; };
            toolStrip.Items.Add(button);

            button = new ToolStripButton { Text = "背景白(&H)" };
            button.Click += (s, e) => { MyForm.BackColor = System.Drawing.Color.White; };
            toolStrip.Items.Add(button);

            button = new ToolStripButton { Text = "閉じる(&W)" };
            button.Click += (s, e) => { MyForm.Close(); };
            toolStrip.Items.Add(button);


            //ts.ResumeLayout(false);
            //ts.PerformLayout();

            MyForm.Controls.Add(toolStrip);

            MyPictureBox = new PictureBox { SizeMode = PictureBoxSizeMode.AutoSize };
            var p = new Panel { AutoScroll = true, Dock = DockStyle.Fill };
            p.Controls.Add(MyPictureBox);
            MyForm.Controls.Add(p);
            MyPictureBox.Image = OriginBitmap;
            p.BringToFront();

        }

        //画像の保存
        private void SaveImage(Bitmap bmp)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "*.png|*.png|*.bmp|*.bmp";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.FilterIndex == 1)
                {
                    bmp.Save(dialog.FileName, ImageFormat.Png);
                }
                else if (dialog.FilterIndex == 2)
                {
                    bmp.Save(dialog.FileName, ImageFormat.Bmp);
                }
            }
            dialog.Dispose();
        }

        private void Scale2()
        {
            Bitmap canvas = new Bitmap(OriginBitmap.Width * 2, OriginBitmap.Height * 2);
            Graphics g = Graphics.FromImage(canvas);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.DrawImage(OriginBitmap, 0, 0, OriginBitmap.Width * 2, OriginBitmap.Height * 2);
            g.Dispose();
            MyPictureBox.Image = canvas;
        }





    }

    public class MySource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            var source = (BitmapSource)target;
            var data = new MyProxy(source);
            base.GetData(data, outgoingData);

            ////BitmapSourceをBitmapへ変換
            //var data = new MyProxy((BitmapSource)target);
            //var source = BitmapSource.Create(data.Width, data.Height, 96, 96, PixelFormats.Bgra32, null, data.Pixels, data.Stride);
            //var bmp = new Bitmap(data.Width, data.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            //source.CopyPixels(Int32Rect.Empty, bmpData.Scan0, bmpData.Height * bmpData.Stride, bmpData.Stride);
            //bmp.UnlockBits(bmpData);
            //base.GetData(bmp, outgoingData);
        }


    }


    /// <summary>
    /// BitmapSourceをシリアライズできる型に分解
    /// PixelFormatをBgra32に決め打ち方式用
    /// </summary>
    [Serializable]
    public class MyProxy
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Stride { get; private set; }
        public byte[] Pixels { get; private set; }
        public MyProxy(BitmapSource source)
        {
            if (source.Format != PixelFormats.Bgra32)
            {
                source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
            }
            Width = source.PixelWidth;
            Height = source.PixelHeight;
            Stride = Width * 4;// (Width * source.Format.BitsPerPixel + 7) / 8;
            Pixels = new byte[Height * Stride];
            source.CopyPixels(new Int32Rect(0, 0, Width, Height), Pixels, Stride, 0);
        }
    }

}
//補間方法を指定して画像を拡大、縮小（スケーリング）表示する - .NET Tips(VB.NET, C#...)
//https://dobon.net/vb/dotnet/graphics/interpolationmode.html
