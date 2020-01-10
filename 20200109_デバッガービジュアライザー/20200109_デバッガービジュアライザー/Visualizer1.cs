using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

[assembly: System.Diagnostics.DebuggerVisualizer(
typeof(_20200109_デバッガービジュアライザー.Visualizer1),
typeof(_20200109_デバッガービジュアライザー.MyObjectSource),
Target = typeof(System.Windows.Media.Imaging.BitmapSource), Description = "ゆっくりしていってね！！！")]
namespace _20200109_デバッガービジュアライザー
{
    public class Visualizer1 : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException("windowService");
            if (objectProvider == null)
                throw new ArgumentNullException("objectProvider");

            var data = objectProvider.GetData();// (MyBitmapSourceProxy)objectProvider.GetObject();
            var bf = new BinaryFormatter();
            var obj = bf.Deserialize(data) as MyBitmapSourceProxy;

            using (Form displayForm = new Form())
            {
                //displayForm.Text = data.height.ToString();
                displayForm.BackColor = System.Drawing.Color.Aqua;
                windowService.ShowDialog(displayForm);
            }
        }

        public static void TestShowVisualizer(object objectToVisualize)
        {
            //objectToVisualizeにBitmapIamgeが入っている

            BitmapSource source = (BitmapSource)objectToVisualize;
            MyBitmapSourceProxy proxy = new MyBitmapSourceProxy(source);
            VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(proxy, typeof(Visualizer1));
            visualizerHost.ShowVisualizer();

            //VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(MyObjectSource));
            //visualizerHost.ShowVisualizer();

            //元
            //VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(Visualizer1));
            //visualizerHost.ShowVisualizer();//エラーBitmapImageはシリアライズできない
        }



    }


    public class MyObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(outgoingData, new MyBitmapSourceProxy((BitmapSource)target));
            base.GetData(target, outgoingData);
        }
    }



    [Serializable]
    public class MyBitmapSourceProxy
    {
        public int stride { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }
        public byte[] pixels { get; private set; }
        public PixelFormat PixelFormat { get; private set; }

        public MyBitmapSourceProxy(BitmapSource source)
        {
            width = source.PixelWidth;
            height = source.PixelHeight;
            PixelFormat = source.Format;
            stride = (width * PixelFormat.BitsPerPixel + 7) / 8;
            pixels = new byte[height * stride];
            source.CopyPixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
        }
    }
}





















//using Microsoft.VisualStudio.DebuggerVisualizers;
//using System;
//using System.Collections.Generic;
//using System.Windows.Forms;

//namespace _20200109_デバッガービジュアライザー
//{
//    // TODO: SomeType のインスタンスをデバッグするときに、このビジュアライザーを表示するために SomeType の定義に次のコードを追加します:
//    // 
//    //  [DebuggerVisualizer(typeof(Visualizer1))]
//    //  [Serializable]
//    //  public class SomeType
//    //  {
//    //   ...
//    //  }
//    // 
//    /// <summary>
//    /// SomeType のビジュアライザーです。  
//    /// </summary>
//    public class Visualizer1 : DialogDebuggerVisualizer
//    {
//        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
//        {
//            if (windowService == null)
//                throw new ArgumentNullException("windowService");
//            if (objectProvider == null)
//                throw new ArgumentNullException("objectProvider");

//            // TODO: ビジュアライザーを表示する目的のオブジェクトを取得します。
//            //       objectProvider.GetObject() の結果をキャスト
//            //       されるオブジェクトの型にキャストします。
//            object data = (object)objectProvider.GetObject();

//            // TODO: オブジェクトのビューを表示します。
//            //       displayForm をユーザー独自のカスタム フォームまたはコントロールで置き換えます。
//            using (Form displayForm = new Form())
//            {
//                displayForm.Text = data.ToString();
//                windowService.ShowDialog(displayForm);
//            }
//        }

//        // TODO: ビジュアライザーをテストするために、次のコードをユーザーのコードに追加します:
//        // 
//        //    Visualizer1.TestShowVisualizer(new SomeType());
//        // 
//        /// <summary>
//        /// デバッガーの外部にホストすることにより、ビジュアライザーをテストします。
//        /// </summary>
//        /// <param name="objectToVisualize">ビジュアライザーに表示するオブジェクトです。</param>
//        public static void TestShowVisualizer(object objectToVisualize)
//        {
//            VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(Visualizer1));
//            visualizerHost.ShowVisualizer();
//        }
//    }
//}
