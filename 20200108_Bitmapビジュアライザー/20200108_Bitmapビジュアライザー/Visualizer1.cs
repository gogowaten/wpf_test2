using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

[assembly: System.Diagnostics.DebuggerVisualizer(
typeof(_20200108_Bitmapビジュアライザー.Visualizer1),
typeof(VisualizerObjectSource),
Target = typeof(System.Windows.Window),
Description = "My First Visualizer")]
namespace _20200108_Bitmapビジュアライザー
{
    // TODO: SomeType のインスタンスをデバッグするときに、このビジュアライザーを表示するために SomeType の定義に次のコードを追加します:
    // 
    //  [DebuggerVisualizer(typeof(Visualizer1))]
    //  [Serializable]
    //  public class SomeType
    //  {
    //   ...
    //  }
    // 
    /// <summary>
    /// SomeType のビジュアライザーです。  
    /// </summary>

    public class Visualizer1 : DialogDebuggerVisualizer
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

            //MessageBox.Show(objectProvider.GetObject().ToString());




            //System.Windows.Media.Imaging.BitmapSource source = (System.Windows.Media.Imaging.BitmapSource)objectProvider;
            //System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            //image.Source = source;


            //object data = (object)objectProvider.GetObject();



            //hostControl
            //System.Windows.Window window = new System.Windows.Window();
            //window.Title = objectProvider.GetObject().ToString();
            //var elementHost = new System.Windows.Forms.Integration.ElementHost();
            //elementHost.SetBounds(20, 10, 100, 30);
            //elementHost.Child = window;
            var el = new System.Windows.Forms.Integration.ElementHost() { Child =(System.Windows.Window) objectProvider };
            //var c = new Button();

            var form = new Form
            {
                Text = "ok",
                Controls = { el }
            };
            windowService.ShowDialog(form);
            //form.Dispose();
            //c.Dispose();

            //using (var f = new Form { Text = "ok" })
            //{
            //    //f.Controls.Add(new Button() { Text = "butttttttttttttt" });
            //    //f.Controls.Add(new System.Windows.Forms.Integration.ElementHost());
            //    //f.Controls.Add(elementHost);
            //    windowService.ShowDialog(f);
            //}

            //// TODO: オブジェクトのビューを表示します。
            ////       displayForm をユーザー独自のカスタム フォームまたはコントロールで置き換えます。
            //using (Form displayForm = new Form())
            //{
            //    displayForm.Text = "";
            //    //displayForm.Text = data.ToString();
            //    windowService.ShowDialog(displayForm);
            //}
        }

        // TODO: ビジュアライザーをテストするために、次のコードをユーザーのコードに追加します:
        // 
        //    Visualizer1.TestShowVisualizer(new SomeType());
        // 
        /// <summary>
        /// デバッガーの外部にホストすることにより、ビジュアライザーをテストします。
        /// </summary>
        /// <param name="objectToVisualize">ビジュアライザーに表示するオブジェクトです。</param>
        public static void TestShowVisualizer(object objectToVisualize)
        {
            VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(Visualizer1));
            visualizerHost.ShowVisualizer();
        }
    }


    #region backup
    //// TODO: SomeType のインスタンスをデバッグするときに、このビジュアライザーを表示するために SomeType の定義に次のコードを追加します:
    //// 
    ////  [DebuggerVisualizer(typeof(Visualizer1))]
    ////  [Serializable]
    ////  public class SomeType
    ////  {
    ////   ...
    ////  }
    //// 
    ///// <summary>
    ///// SomeType のビジュアライザーです。  
    ///// </summary>
    //public class Visualizer1 : DialogDebuggerVisualizer
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
    //    //    Visualizer1.TestShowVisualizer(new SomeType());
    //    // 
    //    /// <summary>
    //    /// デバッガーの外部にホストすることにより、ビジュアライザーをテストします。
    //    /// </summary>
    //    /// <param name="objectToVisualize">ビジュアライザーに表示するオブジェクトです。</param>
    //    public static void TestShowVisualizer(object objectToVisualize)
    //    {
    //        VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(Visualizer1));
    //        visualizerHost.ShowVisualizer();
    //    }
    //}
    #endregion
}
