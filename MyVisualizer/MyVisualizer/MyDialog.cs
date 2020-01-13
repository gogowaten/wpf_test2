﻿using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


[assembly:System.Diagnostics.DebuggerVisualizer(
    typeof(MyVisualizer.MyDialog),
    typeof(MyVisualizer.MySource),
    Target =typeof(BitmapSource),
    Description = "🐭びっとまっぷそぉす🐭")]
namespace MyVisualizer
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
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException("windowService");
            if (objectProvider == null)
                throw new ArgumentNullException("objectProvider");

            // TODO: ビジュアライザーを表示する目的のオブジェクトを取得します。
            //       objectProvider.GetObject() の結果をキャスト
            //       されるオブジェクトの型にキャストします。
            var data = (MyProxy)objectProvider.GetObject();

            //MyProxyからBitmapSourceを作成
            var source = BitmapSource.Create(data.Width, data.Height, 96, 96,
                PixelFormats.Bgra32, null, data.Pixels, data.Stride);

            //Bitmapを作成して、BitmapSourceのデータをコピペ
            var bmp = new Bitmap(data.Width, data.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmp.PixelFormat);
            source.CopyPixels(Int32Rect.Empty, bmpData.Scan0, bmpData.Height * bmpData.Stride, bmpData.Stride);
            bmp.UnlockBits(bmpData);

            var pictureBox = new PictureBox
            {
                Image = bmp,
                SizeMode = PictureBoxSizeMode.AutoSize
            };
            // TODO: オブジェクトのビューを表示します。
            //       displayForm をユーザー独自のカスタム フォームまたはコントロールで置き換えます。
            using (Form displayForm = new Form())
            {
                displayForm.Text = data.ToString();
                displayForm.Controls.Add(pictureBox);
                windowService.ShowDialog(displayForm);
            }
            bmp.Dispose();
            pictureBox.Dispose();
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
            //VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(MyDialog));
            VisualizerDevelopmentHost visualizerHost 
                = new VisualizerDevelopmentHost(objectToVisualize, typeof(MyDialog), typeof(MySource));
            visualizerHost.ShowVisualizer();
        }
    }

    public class MySource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            var source = (BitmapSource)target;
            var proxy = new MyProxy(source);
            base.GetData(proxy, outgoingData);
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
