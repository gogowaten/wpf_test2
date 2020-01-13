using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

[assembly: System.Diagnostics.DebuggerVisualizer(
    typeof(ClassLibrary20200112.Visualizer20200112),
    typeof(ClassLibrary20200112.MyObjectSource),
    Target = typeof(BitmapSource),
    Description = "🐭びっとまっぷそーす🐭")]
namespace ClassLibrary20200112
{
  

    public class Visualizer20200112 : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            var data = (MyBitmapSourceProxy2)objectProvider.GetObject();
            //bitmapSourceからbitmapを作成
            var bitmapSource = BitmapSource.Create(data.Width, data.Height, 96, 96, PixelFormats.Bgra32, null, data.Pixels, data.Stride);
            var bmp = new Bitmap(data.Width, data.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
            bitmapSource.CopyPixels(Int32Rect.Empty, bmpData.Scan0, bmpData.Height * bmpData.Stride, bmpData.Stride);
            bmp.UnlockBits(bmpData);

            PictureBox pictureBox = new PictureBox
            {
                Image = bmp,
                SizeMode = PictureBoxSizeMode.AutoSize
            };
            using (Form displayForm = new Form())
            {
                displayForm.Text = data.ToString();
                displayForm.Controls.Add(pictureBox);
                windowService.ShowDialog(displayForm);
            }
            pictureBox.Dispose();
            bmp.Dispose();
        }

      
        public static void TestShowVisualizer(object objectToVisualize)
        {
            //var visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(Visualizer20200112));
            //↑これだとGetDataが使用されないので↓
            var visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(Visualizer20200112), typeof(MyObjectSource));
            visualizerHost.ShowVisualizer();
        }


    }




    public class MyObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            //targetはビジュアライズする対象なので今回はbitmapSourceが入っている
            //targetを元にstreamにしているみたい、シリアライズできないものはここでエラーになるし
            //PixelFormatはそのままだとエラーにならないけど、すべてPixelFormats.Defaultに変換されてしまうので
            //対応が必要
            var source = (BitmapSource)target;
            var p = new MyBitmapSourceProxy2(source);
            base.GetData(p, outgoingData);
        }
    }





    /// <summary>
    /// BitmapSourceをシリアライズできる型に分解
    /// PixelFormatをBgra32に決め打ち方式
    /// </summary>
    [Serializable]
    public class MyBitmapSourceProxy2
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Stride { get; private set; }
        public byte[] Pixels { get; private set; }

        public MyBitmapSourceProxy2(BitmapSource source)
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



    //元のShowとTestShowVisualizer
    //public class Visualizer20200112 : DialogDebuggerVisualizer
    //{
    //    protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
    //    {
    //        if (windowService == null)
    //            throw new ArgumentNullException("windowService");
    //        if (objectProvider == null)
    //            throw new ArgumentNullException("objectProvider");

    //        // TODO: ビジュアライザーを表示する目的のオブジェクトを取得します。
    //        //       objectProvider.GetObject() の結果をキャスト
    //        //       されるオブジェクトの型にキャストします。
    //        object data = (object)objectProvider.GetObject();

    //        // TODO: オブジェクトのビューを表示します。
    //        //       displayForm をユーザー独自のカスタム フォームまたはコントロールで置き換えます。
    //        using (Form displayForm = new Form())
    //        {
    //            displayForm.Text = data.ToString();
    //            windowService.ShowDialog(displayForm);
    //        }
    //    }

    //    // TODO: ビジュアライザーをテストするために、次のコードをユーザーのコードに追加します:
    //    // 
    //    //    Visualizer20200112.TestShowVisualizer(new SomeType());
    //    // 
    //    /// <summary>
    //    /// デバッガーの外部にホストすることにより、ビジュアライザーをテストします。
    //    /// </summary>
    //    /// <param name="objectToVisualize">ビジュアライザーに表示するオブジェクトです。</param>
    //    public static void TestShowVisualizer(object objectToVisualize)
    //    {
    //        VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(Visualizer20200112));
    //        visualizerHost.ShowVisualizer();
    //    }
    //}
}
