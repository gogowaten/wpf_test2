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
        private ObservableCollection<MyBitmapAndName> ListMyBitmapSource;
        private TrimThumb MyTrimThumb;//切り取り範囲
        private string SaveDir;//保存フォルダ
        private Window1 window1 = new Window1();

        public MainWindow()
        {
            InitializeComponent();

            MyTest();

          


            ButtonSave.Click += ButtonSave_Click;
            ButtonPreview.Click += ButtonPreview_Click;
            ButtonSaveDirSelect.Click += ButtonSaveDirSelect_Click;
            ButtonSaveDirOpen.Click += ButtonSaveDirOpen_Click;
            ButtonSaveDirPaste.Click += ButtonSaveDirPaste_Click;

            //this.KeyDown += MainWindow_KeyDown;
            this.PreviewKeyDown += MainWindow_KeyDown;

            this.Loaded += MainWindow_Loaded;
            CheckBox_ClipCheck.Click += CheckBox_ClipCheck_Click;
            MyListBox.SelectionChanged += MyListBox_SelectionChanged;

            //ListBitmap = new List<BitmapSource>();

            ListMyBitmapSource = new ObservableCollection<MyBitmapAndName>();

            MyButtonRemoveSelectedImtem.Click += MyButtonRemoveSelectedImtem_Click;
            MyListBox.DataContext = ListMyBitmapSource;

            //切り取り範囲Thumb初期化
            MyTrimThumb = new TrimThumb(MyCanvas, 20, (int)MyNumericX.MyValue2, 100, 100, 100);
            MyTrimThumb.Focusable = true;//重要！！！
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

            //プレビューウィンドウ
            //window1 = new Window1();

            //切り抜き後の拡縮
            SliderSaveScale.ValueChanged += SliderSaveScale_ValueChanged;

            

        }

        //切り抜き範囲を画像内に収める、移動させる     
        private void TrimAreaCorrection()
        {
            var right = MyNumericW.MyValue2 + MyNumericX.MyValue2;
            var bottom = MyNumericH.MyValue2 + MyNumericY.MyValue2;
            BitmapSource bmp = (BitmapSource)MyImage.Source;
            var imgWidht = bmp.PixelWidth;
            var imgHeight = bmp.PixelHeight;
            var xDiff = right - imgWidht;//範囲内ならマイナス値
            var yDiff = bottom - imgHeight;
            if (xDiff > 0)
            {
                if (MyNumericX.MyValue - xDiff < 0)
                {
                    MyNumericW.MyValue = imgWidht;
                }
                MyNumericX.MyValue -= xDiff;
            }
            if (yDiff > 0)
            {
                if (MyNumericY.MyValue - yDiff < 0)
                {
                    MyNumericH.MyValue = imgHeight;
                }
                MyNumericY.MyValue -= yDiff;
            }
        }

        //  画像の拡縮、最近傍補間法
        private BitmapSource NearestnaverScale(BitmapSource bitmap, decimal scale)
        {
            int w = bitmap.PixelWidth;
            int h = bitmap.PixelHeight;
            int stride = w * 4;
            byte[] pixels = new byte[h * stride];
            bitmap.CopyPixels(pixels, stride, 0);


            int ww = (int)Math.Round(w * scale);
            int hh = (int)Math.Round(h * scale);
            int stride2 = ww * 4;
            byte[] pixels2 = new byte[hh * stride2];
            int p, pp;
            decimal rate = 1 / scale;
            for (int y = 0; y < hh; y++)
            {
                for (int x = 0; x < ww; x++)
                {

                    int motoX = (int)Math.Round(x * rate);
                    int motoY = (int)Math.Round(y * rate);
                    p = motoY * stride + motoX * 4;
                    pp = (y * stride2) + (x * 4);

                    pixels2[pp] = pixels[p];
                    pixels2[pp + 1] = pixels[p + 1];
                    pixels2[pp + 2] = pixels[p + 2];
                    pixels2[pp + 3] = pixels[p + 3];

                }
            }
            return BitmapSource.Create(ww, hh, 96, 96, bitmap.Format, null, pixels2, stride2);
        }

        private void SliderSaveScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string str;
            int i = (int)SliderSaveScale.Value;
            if (i < 0) { str = $"1/{-i + 1}"; }
            else { str = $"x{i + 1}"; }
            TextBoxSaveScale.Text = str;
        }



        //プレビュー
        private void ButtonPreview_Click(object sender, RoutedEventArgs e)
        {
            if (ListMyBitmapSource.Count == 0) return;
            var bs = (MyBitmapAndName)MyListBox.SelectedItem;
            BitmapSource bmp = MakeCroppedBitmap(bs.Source);

            double rate = (double)GetMagnificationScale();
            var scale = new ScaleTransform(rate, rate);

            window1.MyPreview.RenderTransform = scale;
            window1.MaxWidth = this.ActualWidth;
            window1.MaxHeight = this.ActualHeight;
            window1.MyPreview.Source = bmp;
            window1.Visibility = Visibility.Visible;
            //window1.Show();
            //window1.Owner = this;

            //切り抜いた画像を拡縮表示は別ウィンドウに表示して、それをBitmapレンダーで保存？
            //できたけど、別ウィンドウを表示した状態visivleじゃないと画像が更新されない
            //自分で拡縮したほうがいいかも？
        }
        //拡大率取得
        private decimal GetMagnificationScale()
        {
            //スライダーの値から生成
            //0 = 1, 1 = 2, 2 = 3,...
            //-1 = 1 / 2, -2 = 1 / 3,...
            int i = (int)SliderSaveScale.Value;
            decimal rate = (i < 0) ? 1.0m / (-i + 1) : i + 1;
            return rate;
        }


        #region キーボードで操作
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //ctrl+Shift、サイズ変更、ラージ
            if (e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                switch (e.Key)
                {
                    case Key.Left:
                        MyNumericW.MyValue -= MyNumericW.MyLargeChange;
                        break;
                    case Key.Up:
                        MyNumericH.MyValue -= MyNumericH.MyLargeChange;
                        break;
                    case Key.Right:
                        MyNumericW.MyValue += MyNumericW.MyLargeChange;
                        break;
                    case Key.Down:
                        MyNumericH.MyValue += MyNumericH.MyLargeChange;
                        break;
                    default:
                        break;
                }
            }
            //ctrl+、移動、ラージ
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.Left:
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
            //shift+、サイズ変更、スモール
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        MyNumericW.MyValue -= MyNumericW.MySmallChange;
                        break;
                    case Key.Up:
                        MyNumericH.MyValue -= MyNumericH.MySmallChange;
                        break;
                    case Key.Right:
                        MyNumericW.MyValue += MyNumericW.MySmallChange;
                        break;
                    case Key.Down:
                        MyNumericH.MyValue += MyNumericH.MySmallChange;
                        break;
                    default:
                        break;
                }
            }
            //移動、スモール
            else
            {
                switch (e.Key)
                {
                    case Key.Left:
                        MyNumericX.MyValue -= MyNumericY.MySmallChange;
                        break;
                    case Key.Up:
                        MyNumericY.MyValue -= MyNumericY.MySmallChange;
                        break;
                    case Key.Right:
                        MyNumericX.MyValue += MyNumericY.MySmallChange;
                        break;
                    case Key.Down:
                        MyNumericY.MyValue += MyNumericY.MySmallChange;
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

        //画像の保存
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (ListMyBitmapSource.Count == 0) return;
            //切り抜き範囲チェック
            if (CheckCropRect() == false) { return; }

            //リストの画像全部を保存
            for (int i = 0; i < ListMyBitmapSource.Count; i++)
            {
                SaveImage(ListMyBitmapSource[i]);
            }
        }


        //画像保存処理
        private void SaveImage(MyBitmapAndName data)
        {
            //
            BitmapSource bmp = MakeCroppedBitmap(data.Source);
            decimal scale = GetMagnificationScale();
            if (scale == 1)
            {
                SaveImage(bmp, data.Name);
            }
            else
            {
                SaveImage(NearestnaverScale(bmp, scale), data.Name);
            }
            //CroppedBitmapで切り抜いた画像で保存
            //SaveImage(MakeCroppedBitmap(data.Source), data.Name);
        }

        //Bitmapをファイルに保存
        private void SaveImage(BitmapSource bitmap, string fileName)
        {
            //CroppedBitmapで切り抜いた画像でBitmapFrame作成して保存
            BitmapEncoder encoder = GetEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var fs = new System.IO.FileStream(
                MakeFullPath(fileName), System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                encoder.Save(fs);
            }
        }

        //ファイル名の重複を回避、拡張子の前に"_"を付け足す
        private string MakeFullPath(string fileName)
        {
            var dir = System.IO.Path.Combine(SaveDir, fileName);
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


        //CroppedBitmapを使って切り抜いた画像を作成
        private BitmapSource MakeCroppedBitmap(BitmapSource bitmap)
        {
            //切り抜き範囲取得
            var rect = MakeCropRect();
            return new CroppedBitmap(bitmap, rect);
        }
        private Int32Rect MakeCropRect()
        {
            return new Int32Rect((int)MyNumericX.MyValue2,
                (int)MyNumericY.MyValue2, (int)MyNumericW.MyValue2, (int)MyNumericH.MyValue2);
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

        //切り抜き範囲が画像範囲内にあるかどうかのチェック
        private bool CheckCropRect()
        {
            //切り抜き範囲チェック
            BitmapSource bitmap;
            Int32Rect intRect = MakeCropRect();
            Rect crop = new Rect(intRect.X, intRect.Y, intRect.Width, intRect.Height);
            for (int i = 0; i < ListMyBitmapSource.Count; i++)
            {
                bitmap = ListMyBitmapSource[i].Source;
                if (CheckCropRect(bitmap, crop) == false)
                {
                    string str = $"切り抜き範囲が画像範囲外なので保存できない\n" +
                        $"{ListMyBitmapSource[i].Name.ToString()}";
                    MyListBox.SelectedItem = MyListBox.Items[i];
                    MyListBox.ScrollIntoView(MyListBox.Items[i]);
                    MessageBox.Show(str);
                    return false;
                }
            }
            return true;
        }
        private bool CheckCropRect(BitmapSource bitmap)
        {
            Int32Rect intRect = MakeCropRect();
            Rect crop = new Rect(intRect.X, intRect.Y, intRect.Width, intRect.Height);
            return CheckCropRect(bitmap, crop);
        }
        private bool CheckCropRect(BitmapSource bitmap, Rect crop)
        {
            Rect r2 = new Rect(crop.X, crop.Y, crop.Width, crop.Height);
            Rect r1 = new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            return r1.Contains(r2);
        }

        #endregion


        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var x = Canvas.GetLeft(MyTrimThumb);
            var vx = MyNumericX.MyValue;
            var ty = ComboBoxSaveImageType.SelectedItem;
            var tv = ComboBoxSaveImageType.SelectedValue;
            var type = GetEncoder();
            var aw = window1.MyPreview.ActualWidth;

            //var bmp = MakeCroppedBitmap(ListMyBitmapSource[0].Source);
            //window1.MyPreview.Source = bmp;
            //window1.Show();
            //var im = window1.MyPreview;
            //var rb = new RenderTargetBitmap(200, 200, 96, 96, PixelFormats.Default);
            //rb.Render(im);
            //SaveImage2(rb, GetFileName(ListMyBitmapSource[0]));

            var bmp = MakeCroppedBitmap(ListMyBitmapSource[0].Source);
            //NearestnaverScaleDown(bmp, 2);
            bmp = NearestnaverScale(bmp, GetMagnificationScale());
            SaveImage(bmp, ListMyBitmapSource[0].Name);

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
            var cc = new ObservableCollection<MyBitmapAndName>();
            foreach (MyBitmapAndName item in MyListBox.SelectedItems)
            {
                cc.Add(item);
            }
            foreach (MyBitmapAndName item in cc)
            {
                ListMyBitmapSource.Remove(item);
            }

        }


        private void MyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyBitmapAndName myBitmap = (MyBitmapAndName)MyListBox.SelectedItem;
            if (myBitmap == null)
            {
                MyImage.Source = null;
            }
            else
            {

                MyImage.Source = myBitmap.Source;
            }
        }

        //クリップボード更新時に画像取得してリストに追加、名前もつける
        //画像取得時に失敗することがあるので指定回数連続トライしている
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
                        MyCanvas.Width = bitmap.PixelWidth;
                        MyCanvas.Height = bitmap.PixelHeight;
                        //画像と名前をリストに追加
                        var source = new MyBitmapAndName(bitmap, TextBox_FileName.Text + GetStringNowTime());
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

        //        【C#入門】現在時刻を取得する方法(DateTime.Now/UtcNow) | 侍エンジニア塾ブログ（Samurai Blog） - プログラミング入門者向けサイト
        //https://www.sejuku.net/blog/51208

        //今の日時をStringで作成
        private string GetStringNowTime()
        {
            DateTime dt = DateTime.Now;
            string str = dt.ToString("yyyyMMdd" + "_" + "HHmmss" + "_" + dt.Millisecond.ToString("000"));
            return str;
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

    public class MyBitmapAndName
    {
        public BitmapSource Source { get; }
        public string Name { get; }
        public MyBitmapAndName(BitmapSource source, string name)
        {
            Source = source;
            Name = name;
        }
    }
}
