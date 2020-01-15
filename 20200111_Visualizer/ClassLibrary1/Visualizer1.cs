using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
//using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

[assembly: System.Diagnostics.DebuggerVisualizer(
    typeof(ClassLibrary1.Visualizer1),
    typeof(ClassLibrary1.MyObjectSource),
    Target = typeof(BitmapSource),
    Description = "yukkuri2")]
namespace ClassLibrary1
{
    public class Visualizer1 : DialogDebuggerVisualizer
    {
        //protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        //{
        //    var data = (MyBitmapSourceProxy)objectProvider.GetObject();
        //    //↑GetDataが実行されている？

        //    //PixelFormatを再構成
        //    Guid guid = Guid.Parse(data.Format);
        //    PixelFormat pxFormat = (PixelFormat)Activator.CreateInstance(typeof(PixelFormat),
        //        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.CreateInstance,
        //        null, new object[] { guid }, null);
        //    //paletteを再構成
        //    BitmapPalette palette = MakePalette(data.Palette);

        //    //bitmapSourceを再構成
        //    var bitmapSource = BitmapSource.Create(data.Width, data.Height, 96, 96, pxFormat, palette, data.Pixels, data.Stride);

        //    //pngエンコーダーでストリームに保存、ストリームからbitmap作成
        //    var encoder = new PngBitmapEncoder();
        //    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
        //    Bitmap bmp = null;
        //    using (var stream = new MemoryStream())
        //    {
        //        encoder.Save(stream);
        //        stream.Seek(0, SeekOrigin.Begin);
        //        //おまじない
        //        using (Bitmap temp = new Bitmap(stream))
        //        {
        //            bmp = new Bitmap(temp);
        //        }
        //    }


        //    PictureBox pictureBox = new PictureBox
        //    {
        //        Image = bmp,
        //        SizeMode = PictureBoxSizeMode.AutoSize
        //    };

        //    //Formに画像表示
        //    using (Form displayForm = new Form())
        //    {
        //        displayForm.Text = data.ToString();
        //        displayForm.Controls.Add(pictureBox);
        //        windowService.ShowDialog(displayForm);
        //    }
        //    pictureBox.Dispose();
        //    bmp.Dispose();
        //}


        private BitmapPalette MakePalette(List<byte[]> colors)
        {
            if (colors == null) return null;
            var cl = new List<System.Windows.Media.Color>();

            for (int i = 0; i < colors.Count; i++)
            {
                var c = new System.Windows.Media.Color();
                c.A = colors[i][0];
                c.R = colors[i][1];
                c.G = colors[i][2];
                c.B = colors[i][3];
                cl.Add(c);
            }
            return new BitmapPalette(cl);
        }

        ////sono2
        //protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        //{
        //    var data = (MyBitmapSourceProxy)objectProvider.GetObject();
        //    //bitmapSourceからbitmapを作成
        //    var bitmapSource = BitmapSource.Create(data.Width, data.Height, 96, 96, PixelFormats.Bgra32, null, data.Pixels, data.Stride);
        //    var bmp = new Bitmap(data.Width, data.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        //    var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
        //    bitmapSource.CopyPixels(Int32Rect.Empty, bmpData.Scan0, bmpData.Height * bmpData.Stride, bmpData.Stride);
        //    bmp.UnlockBits(bmpData);

        //    PictureBox pictureBox = new PictureBox
        //    {
        //        Image = bmp,
        //        SizeMode = PictureBoxSizeMode.AutoSize
        //    };
        //    using (Form displayForm = new Form())
        //    {
        //        displayForm.Text = data.ToString();
        //        displayForm.Controls.Add(pictureBox);
        //        windowService.ShowDialog(displayForm);
        //    }
        //    bmp.Dispose();
        //    pictureBox.Dispose();
        //}

        //sono3、失敗、本当はこれがいい、bitmapSourceをそのまま表示できる
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            var data = (MyBitmapSourceProxy)objectProvider.GetObject();
            //↑GetDataが実行されている？

            //PixelFormatを再構成
            Guid guid = Guid.Parse(data.Format);
            PixelFormat pxFormat = (PixelFormat)Activator.CreateInstance(typeof(PixelFormat),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.CreateInstance,
                null, new object[] { guid }, null);
            //paletteを再構成
            BitmapPalette palette = MakePalette(data.Palette);

            //bitmapSourceを再構成
            var bitmapSource = BitmapSource.Create(data.Width, data.Height, 96, 96, pxFormat, palette, data.Pixels, data.Stride);

            //ここでエラー" 呼び出しスレッドは、多数の ui コンポーネントが必要としているため、sta である必要があります"
            //var wpf = new System.Windows.Forms.Integration.ElementHost
            //{
            //    Child = new UserControl1(bitmapSource)

            //};
            


            System.Windows.Forms.Integration.ElementHost elementHost1;
            //WPFのButtonコントロールを作成する
            System.Windows.Controls.Button wpfButton =
                new System.Windows.Controls.Button();
            wpfButton.Content = "Push!";
            

            //ElementHostコントロールを作成する
            elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            //コントロールの位置と大きさを設定する
            elementHost1.SetBounds(20, 10, 100, 30);

            //ElementHostのChildプロパティにWPFコントロールを設定する
            elementHost1.Child = wpfButton;

            var wpfImage = new System.Windows.Controls.Image();
            wpfImage.Source = bitmapSource;
            elementHost1.Child = wpfImage;

            //ElementHostをフォームに配置する
            //this.Controls.Add(elementHost1);



            //Formに画像表示
            var f = new Form { Controls = { elementHost1 } };
            windowService.ShowDialog(f);
            f.Dispose();
            //using (Form displayForm = new Form())
            //{
            //    displayForm.Text = data.ToString();
            //    displayForm.Controls = { wpf }
            //    windowService.ShowDialog(displayForm);
            //}            
        }


        //kyoutuu
        public static void TestShowVisualizer(object objectToVisualize)
        {
            VisualizerDevelopmentHost host;

            //host = new VisualizerDevelopmentHost(objectToVisualize, typeof(Visualizer1));//これだとGetDataが無視される
            host = new VisualizerDevelopmentHost(objectToVisualize, typeof(Visualizer1), typeof(MyObjectSource));//GetDataが使用される
            host.ShowVisualizer();//Showへ続いて、また戻ってくる
        }

    }

    //kyoutuu
    public class MyObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            //targetはビジュアライズする対象なので今回はbitmapSourceが入っている
            //targetを元にstreamにしているみたい、シリアライズできないものはここでエラーになるし
            //PixelFormatはそのままだとエラーにならないけど、すべてPixelFormats.Defaultに変換されてしまうので
            //対応が必要
            var source = (BitmapSource)target;
            var p = new MyBitmapSourceProxy(source);
            base.GetData(p, outgoingData);
        }
    }


    //sono1とsono3用
    [Serializable]
    public class MyBitmapSourceProxy
    {
        public int Stride { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public byte[] Pixels { get; private set; }
        public string Format { get; private set; }
        public List<byte[]> Palette { get; private set; }

        public MyBitmapSourceProxy(BitmapSource source)
        {
            //PixelFormatのシリアライズに必要、GUIDっていうのを使う、さっぱりわからん
            Format = typeof(PixelFormat).GetProperty("Guid",
                 System.Reflection.BindingFlags.NonPublic |
                 System.Reflection.BindingFlags.Instance).GetValue(source.Format).ToString();

            //paletteをシリアライズできるように分解
            Palette = null;
            if (source.Palette != null)
            {
                Palette = new List<byte[]>();
                for (int i = 0; i < source.Palette.Colors.Count; i++)
                {
                    var c = new byte[4];
                    c[0] = source.Palette.Colors[i].A;
                    c[1] = source.Palette.Colors[i].R;
                    c[2] = source.Palette.Colors[i].G;
                    c[3] = source.Palette.Colors[i].B;
                    Palette.Add(c);
                }
            }

            Width = source.PixelWidth;
            Height = source.PixelHeight;
            Stride = (Width * source.Format.BitsPerPixel + 7) / 8;
            Pixels = new byte[Height * Stride];
            source.CopyPixels(new System.Windows.Int32Rect(0, 0, Width, Height), Pixels, Stride, 0);
        }
    }


    ////sono2
    ////PixelFormatをBgra32に決め打ち方式
    //[Serializable]
    //public class MyBitmapSourceProxy
    //{
    //    public int Stride { get; private set; }
    //    public int Width { get; private set; }
    //    public int Height { get; private set; }
    //    public byte[] Pixels { get; private set; }

    //    public MyBitmapSourceProxy(BitmapSource source)
    //    {
    //        if (source.Format != PixelFormats.Bgra32)
    //        {
    //            source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
    //        }
    //        Width = source.PixelWidth;
    //        Height = source.PixelHeight;
    //        Stride = (Width * source.Format.BitsPerPixel + 7) / 8;
    //        Pixels = new byte[Height * Stride];
    //        source.CopyPixels(new Int32Rect(0, 0, Width, Height), Pixels, Stride, 0);
    //    }
    //}




}


//C#における「ビットマップ形式の画像データを相互変換」まとめ - Qiita
//https://qiita.com/YSRKEN/items/a24bf2173f0129a5825c
