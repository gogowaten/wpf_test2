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
using System.Collections.ObjectModel;

using _20190925_NumericDecimal;
using Microsoft.WindowsAPICodePack.Dialogs;//フォルダ選択用

namespace _20190924_pixtrim2
{
    enum SaveImageType
    {
        png = 0,
        jpg,
        bmp,
        gif,
        tiff
    }
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClipboardWatcher ClipboardWatcher;
        //private List<BitmapSource> ListBitmap;
        private ObservableCollection<MyBitmapSource> ListMyBitmapSource;
        private TrimThumb MyTrimThumb;//切り取り範囲
        private string SaveDir;//保存フォルダ


        public MainWindow()
        {
            InitializeComponent();

            MyTest();
            ButtonSave.Click += ButtonSave_Click;
            ButtonSaveDirSelect.Click += ButtonSaveDirSelect_Click;
            ButtonSaveDirOpen.Click += ButtonSaveDirOpen_Click;
            ButtonSaveDirPaste.Click += ButtonSaveDirPaste_Click;

            this.KeyDown += MainWindow_KeyDown;

            this.Loaded += MainWindow_Loaded;
            CheckBox_ClipCheck.Click += CheckBox_ClipCheck_Click;
            MyListBox.SelectionChanged += MyListBox_SelectionChanged;

            //ListBitmap = new List<BitmapSource>();

            ListMyBitmapSource = new ObservableCollection<MyBitmapSource>();
            ListMyBitmapSource.CollectionChanged += ListName_CollectionChanged;
            MyButtonRemoveSelectedImtem.Click += MyButtonRemoveSelectedImtem_Click;
            MyListBox.DataContext = ListMyBitmapSource;

            //切り取り範囲Thumb初期化

            MyTrimThumb = new TrimThumb(MyCanvas, 20, (int)MyNumericX.MyValue2, 100, 100, 100);
            MyTrimThumb.SetBackGroundColor(Color.FromArgb(100, 0, 0, 0));
            MyCanvas.Children.Add(MyTrimThumb);
            //NumericUDInteger nn = new NumericUDInteger();

            MyBinding(MyNumericX, MyNumeric.MyValueProperty, MyTrimThumb, Window.LeftProperty);
            MyBinding(MyNumericY, MyNumeric.MyValueProperty, MyTrimThumb, Window.TopProperty);
            MyBinding(MyNumericW, MyNumeric.MyValueProperty, MyTrimThumb, WidthProperty);
            MyBinding(MyNumericH, MyNumeric.MyValueProperty, MyTrimThumb, FrameworkElement.HeightProperty);

            //画像形式コンボボックス初期化
            ComboBoxSaveImageType.ItemsSource = Enum.GetValues(typeof(SaveImageType));
            ComboBoxSaveImageType.SelectedIndex = 0;
        }


        #region キーボードで操作
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) { MyNumericY.MyValue -= MyNumericY.MySmallChange; }

            //if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Down)
            //{
            //    MyNumericY.MyValue += MyNumericY.MyLargeChange;
            //}
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.Up) { MyNumericY.MyValue -= MyNumericY.MyLargeChange; }
                switch (e.Key)
                {   case Key.Left:
                        MyNumericX.MyValue -= MyNumericY.MyLargeChange;
                        break;
                    case Key.Up:
                        MyNumericY.MyValue -= MyNumericY.MyLargeChange;
                        break;
                    case Key.Right:
                        MyNumericX.MyValue += MyNumericY.MyLargeChange;
                        break;
                    case Key.Down:
                        MyNumericY.MyValue += MyNumericY.MyLargeChange;
                        break;
                    case Key.P:
                        break;
                    case Key.S:
                        break;
                    default:
                        break;
                }
            }
        }


        #endregion


        #region 保存フォルダ
        //パスの貼り付け
        //        文字列から特定の文字列を取り除くには？［C#／VB］：.NET TIPS - ＠IT
        //https://www.atmarkit.co.jp/ait/articles/0711/15/news142.html

        private void ButtonSaveDirPaste_Click(object sender, RoutedEventArgs e)
        {
            //エクスプローラーのパスのコピーからだと " がついているので取り除く
            string str = Clipboard.GetText();
            string target = "\"";
            str = str.Replace(target, "");
            if (System.IO.Directory.Exists(str))
            {
                TextBoxSaveDir.Text = str;
                SaveDir = str;
            }
            else
            {
                MessageBox.Show(str.ToString() + "というフォルダは見当たらない");
            }

        }


        //保存フォルダを開く
        private void ButtonSaveDirOpen_Click(object sender, RoutedEventArgs e)
        {
            string dir = TextBoxSaveDir.Text;
            if (System.IO.Directory.Exists(dir))
            {
                System.Diagnostics.Process.Start(dir);//フォルダを開く
            }
            else
            {
                MessageBox.Show("\"" + dir.ToString() + "\"" + "というフォルダは見当たらない");
            }
        }

        //保存フォルダ選択
        private void ButtonSaveDirSelect_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog();
            dialog.InitialDirectory = SaveDir;
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
            {
                TextBoxSaveDir.Text = dialog.FileName;
                //var tt = new ToolTip();
                //tt.Content = temp.FileName;
                //TextBoxSaveDir.ToolTip = tt;

            }

        }

        //後で消す
        private void MyTest()
        {
            ButtonTest.Click += ButtonTest_Click;
            SaveDir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        }
        #endregion


        #region 画像保存
        //画像保存時、ファイル名の重複を回避、拡張子の前に"_"を付け足す
        private string GetFileName(MyBitmapSource source)
        {
            var dir = System.IO.Path.Combine(SaveDir, source.Name);
            var ex = "." + ComboBoxSaveImageType.SelectedValue.ToString();
            var fullPath = dir + ex;

            string bar = "";
            while (System.IO.File.Exists(fullPath))
            {
                bar += "_";
                fullPath = dir + bar + ex;
            }
            return fullPath;
        }
        //画像の保存
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (ListMyBitmapSource.Count == 0) return;
            //切り抜き範囲取得
            Int32Rect rect = new Int32Rect((int)MyNumericX.MyValue2,
                (int)MyNumericY.MyValue2, (int)MyNumericW.MyValue2, (int)MyNumericH.MyValue2);

            //リストの画像全部をCroppedBitmapを使って切り抜いて保存
            for (int i = 0; i < ListMyBitmapSource.Count; i++)
            {
                string filePath = GetFileName(ListMyBitmapSource[i]);
                SaveImage2(new CroppedBitmap(ListMyBitmapSource[i].Source, rect), filePath);
            }
        }

        //Bitmapをファイルに保存
        private void SaveImage2(BitmapSource bitmap, string filePath)
        {
            BitmapEncoder encoder = GetEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                encoder.Save(fs);
            }
        }

        //画像ファイル形式によるEncoder取得
        private BitmapEncoder GetEncoder()
        {
            var type = ComboBoxSaveImageType.SelectedItem;

            switch (type)
            {
                case SaveImageType.png:
                    return new PngBitmapEncoder();
                case SaveImageType.jpg:
                    var jpeg = new JpegBitmapEncoder();
                    jpeg.QualityLevel = (int)MyNumericJpegQuality.MyValue2;
                    return jpeg;
                case SaveImageType.bmp:
                    return new BmpBitmapEncoder();
                case SaveImageType.gif:
                    return new GifBitmapEncoder();
                case SaveImageType.tiff:
                    return new TiffBitmapEncoder();
                default:
                    throw new Exception();
            }
        }

        #endregion


        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var x = Canvas.GetLeft(MyTrimThumb);
            var vx = MyNumericX.MyValue;
            var ty = ComboBoxSaveImageType.SelectedItem;
            var tv = ComboBoxSaveImageType.SelectedValue;
            var type = GetEncoder();
        }

        private void MyBinding(FrameworkElement source, DependencyProperty sourceProperty,
            FrameworkElement target, DependencyProperty targetProperty)
        {
            var b = new Binding()
            {
                Source = source,
                Path = new PropertyPath(sourceProperty),
                Mode = BindingMode.TwoWay
            };
            target.SetBinding(targetProperty, b);
        }


        private void MyButtonRemoveSelectedImtem_Click(object sender, RoutedEventArgs e)
        {
            //選択アイテム削除
            var cc = new ObservableCollection<MyBitmapSource>();
            foreach (MyBitmapSource item in MyListBox.SelectedItems)
            {
                cc.Add(item);
            }
            foreach (MyBitmapSource item in cc)
            {
                ListMyBitmapSource.Remove(item);
            }

        }

        private void ListName_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private void MyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyBitmapSource myBitmap = (MyBitmapSource)MyListBox.SelectedItem;
            if (myBitmap == null)
            {
                MyImage.Source = null;
            }
            else
            {

                MyImage.Source = myBitmap.Source;
            }
        }

        private void ClipboardWatcher_DrawClipboard(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                BitmapSource bitmap = null;
                int count = 1;
                int limit = 5;
                do
                {
                    try
                    {
                        bitmap = Clipboard.GetImage();

                        //MyImage.Source = bitmap;
                        MyCanvas.Width = bitmap.PixelWidth;
                        MyCanvas.Height = bitmap.PixelHeight;

                        string name = TextBox_FileName.Text + MyNumericSerial.GetText();
                        MyNumericSerial.MyValue++;
                        //TextBox_SerialNumber.Text = $"{sn,0:D5}";                        
                        var source = new MyBitmapSource(bitmap, name);
                        ListMyBitmapSource.Add(source);
                        MyListBox.SelectedItem = source;
                        MyListBox.ScrollIntoView(source);//選択アイテムまでスクロール

                    }
                    catch (Exception ex)
                    {
                        if (count == limit)
                        {
                            string str = $"{limit}回試したけど画像の取得に失敗\n{ex.Message}";
                            MessageBox.Show(str);
                        }
                    }
                    finally { count++; }
                } while (limit >= count && bitmap == null);
            }
        }

        private void CheckBox_ClipCheck_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBox_ClipCheck.IsChecked == true) { ClipboardWatcher.Start(); }
            else ClipboardWatcher.Stop();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            ClipboardWatcher = new ClipboardWatcher(
                new System.Windows.Interop.WindowInteropHelper(this).Handle);
            ClipboardWatcher.DrawClipboard += ClipboardWatcher_DrawClipboard;
            if (CheckBox_ClipCheck.IsChecked == true) ClipboardWatcher.Start();
        }

    }

    public class MyBitmapSource
    {
        public BitmapSource Source { get; }
        public string Name { get; }
        public MyBitmapSource(BitmapSource source, string name)
        {
            Source = source;
            Name = name;
        }
    }
}
