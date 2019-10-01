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
using System.ComponentModel;


#region メモ
//WPFでキーボードフォーカスを変更できないようにするには « Bluestick.JP 開発メモ
//http://bluestick.jp/tech/index.php/archives/131

/*キーボードのカーソルでもMyTrimThumb(切り抜き範囲)の移動やサイズを変更したいけど
 カーソルキー押すとフォーカスも移動してまう、これはよくない、期待する動作は
 MyTrimThumbからフォーカスを外すのは、サイドにある設定用の要素群のどれかをクリックしたときだけ

 フォーカスをMyTrimThumbに完全固定するには
 MyTrimThumbのPreviewLostKeyboardFocusイベントの
 KeyboardFocusChangedEventArgsプロパティにTrueを指定する、 
 Handled = true
 これでキーボードによるフォーカス移動がキャンセルされるみたい

    期待する動作のために
 フィールドに切り抜き範囲のフラグ用のIsTrimForcusedを用意しておいて
 Trueにするのは、MyTrimThumbクリックイベント時
 falseにするのは、 設定用の要素のベースパネルになっているDockPanelMainのPreviewMouseDownイベント
 DockPanelMain.PreviewMouseDown += (o, e) => { IsTrimFocused = false; };

 あとは、MyTrimThumbのPreviewLostKeyboardFocusイベント時にフラグ判定して
 if (IsTrimFocused == true) { e.Handled = true; }
 
 */


#endregion


namespace _20190924_pixtrim2
{
    public enum SaveImageType
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
        private const string CONFIG_FILE_NAME = "MyConfig.xml";
        private ClipboardWatcher ClipboardWatcher;
        private ObservableCollection<MyBitmapAndName> ListMyBitmapSource;
        private TrimThumb MyTrimThumb;//切り取り範囲
        private Window1 window1;//プレビュー用        
        private Config MyConfig;//設定
        //public bool IsClosing = false;//子ウィンドウを閉じるとき用フラグ

        public MainWindow()
        {
            InitializeComponent();


            ComboBoxConfigs.Items.Add("neko1");
            ComboBoxConfigs.Items.Add("neko2");


            ButtonTest.Click += ButtonTest_Click;
            ButtonSave.Click += ButtonSave_Click;
            ButtonPreview.Click += ButtonPreview_Click;
            ButtonSaveDirSelect.Click += ButtonSaveDirSelect_Click;
            ButtonSaveDirOpen.Click += ButtonSaveDirOpen_Click;
            ButtonSaveDirPaste.Click += ButtonSaveDirPaste_Click;
            ButtonLoadConfig.Click += ButtonLoadConfig_Click;
            ButtonSaveConfig.Click += ButtonSaveConfig_Click;

            //this.KeyDown += MainWindow_KeyDown;
            //this.PreviewKeyDown += MainWindow_KeyDown;

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            //this.Closed += MainWindow_Closed;
            CheckBox_ClipCheck.Click += CheckBox_ClipCheck_Click;
            MyListBox.SelectionChanged += MyListBox_SelectionChanged;

            //ListBitmap = new List<BitmapSource>();

            ListMyBitmapSource = new ObservableCollection<MyBitmapAndName>();

            MyButtonRemoveSelectedImtem.Click += MyButtonRemoveSelectedImtem_Click;
            MyListBox.DataContext = ListMyBitmapSource;

            //切り取り範囲Thumb初期化
            MyTrimThumb = new TrimThumb(MyCanvas, 20, (int)MyNumericX.MyValue2, 100, 100, 100);
            //MyTrimThumb.Focusable = true;//重要！！！
            //MyTrimThumb.PreviewLostKeyboardFocus += MyTrimThumb_PreviewLostKeyboardFocus;
            //MyTrimThumb.PreviewMouseDown += (o, e) => { IsTrimFocused = true;TextBoxDammy.Focus(); Keyboard.Focus(TextBoxDammy); };
            TextBoxDammy.PreviewKeyDown += MyTrimThumb_KeyDown;
            MyTrimThumb.PreviewMouseDown += (o, e) => { TextBoxDammy.Focus(); Keyboard.Focus(TextBoxDammy); };
            MyTrimThumb.MouseDown += (o, e) => { TextBoxDammy.Focus(); };
            //MyTrimThumb.KeyDown += MyTrimThumb_KeyDown;
            MyTrimThumb.SetBackGroundColor(Color.FromArgb(100, 0, 0, 0));
            MyCanvas.Children.Add(MyTrimThumb);
            //DockPanelMain.PreviewMouseDown += (o, e) => { IsTrimFocused = false; };


            //            C# で実行ファイルのフォルダを取得
            //http://var.blog.jp/archives/66978870.html

            //実行ファイルのディレクトリ取得3種
            //string str = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //str = System.IO.Directory.GetCurrentDirectory();
            //str = Environment.CurrentDirectory;

            //前回終了時の設定読み込み
            MyConfig = new Config();
            LoadConfig(System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location) +
                "\\" + CONFIG_FILE_NAME);
            MySetBinding();


            //画像形式コンボボックス初期化
            ComboBoxSaveImageType.ItemsSource = Enum.GetValues(typeof(SaveImageType));
            ComboBoxSaveImageType.SelectedIndex = 0;

            //プレビューウィンドウ
            //window1 = new Window1();

            //切り抜き後の拡縮
            //SliderSaveScale.ValueChanged += SliderSaveScale_ValueChanged;

            //音声
            ButtonSoundSelect.Click += ButtonSoundSelect_Click;
            ButtonSoundPlay.Click += ButtonSoundPlay_Click;

            ////Binding終わったあとに初期設定
            //MyConfig.Width = 160; MyConfig.Height = 100;
            //MyConfig.SaveScale = 1;
            //MyConfig.JpegQuality = 97;
            //MyConfig.SaveScale = 1;
        }

        //private void MainWindow_Closed(object sender, EventArgs e)
        //{
        //    IsClosing = true;
        //    window1.Close();
        //    System.Windows.Application.Current.Shutdown();
        //}



        //アプリ終了時、設定保存
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            string fullpath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + CONFIG_FILE_NAME;
            SaveConfig(fullpath);
        }


        //設定を読み込んだあとはこれを実行
        private void MySetBinding()
        {
            //MyNumericX.SetBinding(MyNumeric.MyValueProperty, new Binding(nameof(MyConfig.Left)));
            //↑これだとBindingにならないので↓
            Binding b;
            b = MakeBinding(nameof(MyConfig.Left));
            MyNumericX.SetBinding(MyNumeric.MyValueProperty, b);
            MyTrimThumb.SetBinding(LeftProperty, b);
            b = MakeBinding(nameof(MyConfig.Top));
            MyNumericY.SetBinding(MyNumeric.MyValueProperty, b);
            MyTrimThumb.SetBinding(TopProperty, b);
            b = MakeBinding(nameof(MyConfig.Width));
            MyNumericW.SetBinding(MyNumeric.MyValueProperty, b);
            MyTrimThumb.SetBinding(WidthProperty, b);
            b = MakeBinding(nameof(MyConfig.Height));
            MyNumericH.SetBinding(MyNumeric.MyValueProperty, b);
            MyTrimThumb.SetBinding(HeightProperty, b);


            //MyConfig.SaveScale;//切り抜き後のサイズ変更
            MyNumericSaveScale.SetBinding(MyNumeric.MyValueProperty, MakeBinding(nameof(Config.SaveScale)));
            //MyConfig.JpegQuality;
            MyNumericJpegQuality.SetBinding(MyNumeric.MyValueProperty, MakeBinding(nameof(Config.JpegQuality)));
            //MyConfig.IsPlaySound;
            CheckBoxSoundPlay.SetBinding(CheckBox.IsCheckedProperty, MakeBinding(nameof(Config.IsPlaySound)));
            //MyConfig.SaveImageType;
            ComboBoxSaveImageType.SetBinding(ComboBox.SelectedValueProperty, MakeBinding(nameof(Config.SaveImageType)));
            //MyConfig.SoundDir;
            TextBoxSoundDir.SetBinding(TextBox.TextProperty, MakeBinding(nameof(Config.SoundDir)));
            //MyConfig.FileName;
            TextBoxFileName.SetBinding(TextBox.TextProperty, MakeBinding(nameof(Config.FileName)));
            //MyConfig.SavaDir;
            TextBoxSaveDir.SetBinding(TextBox.TextProperty, MakeBinding(nameof(Config.SavaDir)));

            //SliderSaveScale.Value = 1;

            //this.DataContext = MyConfig;//要らないみたい
        }

        private Binding MakeBinding(string config)
        {
            var b = new Binding()
            {
                Source = MyConfig,
                Path = new PropertyPath(config),
                Mode = BindingMode.TwoWay,
            };
            return b;
        }

        #region 設定の読み書き

        //設定の保存
        private void ButtonSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = ".config";
            dialog.Filter = "*.config|*.config";
            dialog.AddExtension = true;

            if (dialog.ShowDialog() == true)
            {
                if (SaveConfig(dialog.FileName)) MessageBox.Show("保存できたよ");
            }
        }
        private bool SaveConfig(string fullPath)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Config));
            try
            {
                using (var stream = new System.IO.StreamWriter(fullPath, false, new System.Text.UTF8Encoding(false)))
                {
                    serializer.Serialize(stream, MyConfig);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"なんかエラーで設定の保存できんかったわ\n" +
                    $"{ex.Message}");
                return false;
            }
        }

        //設定の読み込み
        private void ButtonLoadConfig_Click(object sender, RoutedEventArgs e)
        {
            //string fullPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "config.config");
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "*.config|*.config";
            if (dialog.ShowDialog() == true)
            {
                LoadConfig(dialog.FileName);
            }
        }
        private bool LoadConfig(string fullPath)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Config));
            try
            {
                using (var stream = new System.IO.StreamReader(fullPath, new UTF8Encoding(false)))
                {
                    MyConfig = (Config)serializer.Deserialize(stream);
                }
                MySetBinding();//再Binding
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"なんかエラーで設定の読み込みできんかったわ\n" +
                    $"{ex.Message}");
                return false;
            }
        }
        #endregion

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var neko = MyConfig;


        }


        #region 音
        private void ButtonSoundPlay_Click(object sender, RoutedEventArgs e)
        {
            PlaySoundFile();
        }
        //音声ファイル再生
        private void PlaySoundFile()
        {
            try
            {
                var so = new System.Media.SoundPlayer(MyConfig.SoundDir);
                so.Play();
            }
            catch (Exception)
            {
                //MessageBox.Show($"指定されたファイルは再生できなかったよ\n" +
                //    $"再生できる音声ファイルは、wav形式だけ");
            }
        }
        private void ButtonSoundSelect_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "(wav)|*.wav";
            if (dialog.ShowDialog() == true)
            {
                //TextBoxSoundDir.Text = dialog.FileName;//これだとBindingが解けてしまうので
                MyConfig.SoundDir = dialog.FileName;
            }
        }
        #endregion

        #region その他
        //フォルダの存在確認、なければマイドキュメントのパスを返す
        private string CheckDir(string path)
        {
            if (System.IO.Directory.Exists(path) == false)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            return path;
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
            int motoP, pp;
            decimal rate = 1 / scale;
            for (int y = 0; y < hh; y++)
            {
                for (int x = 0; x < ww; x++)
                {
                    //元の画像の座標は小数点以下切り捨てが正解、四捨五入だとあふれる
                    int motoX = (int)Math.Floor(x * rate);// (int)Math.Round(x * rate) ;
                    int motoY = (int)Math.Floor(y * rate);
                    motoP = motoY * stride + motoX * 4;

                    pp = y * stride2 + x * 4;
                    pixels2[pp] = pixels[motoP];
                    pixels2[pp + 1] = pixels[motoP + 1];
                    pixels2[pp + 2] = pixels[motoP + 2];
                    pixels2[pp + 3] = pixels[motoP + 3];
                }
            }

            return BitmapSource.Create(ww, hh, 96, 96, bitmap.Format, null, pixels2, stride2);
        }




        //プレビュー
        private void ButtonPreview_Click(object sender, RoutedEventArgs e)
        {
            if (ListMyBitmapSource.Count == 0) return;
            var bs = (MyBitmapAndName)MyListBox.SelectedItem;
            if (CheckCropRect(bs.Source))
            {
                //画像を切り抜いて拡大
                BitmapSource img = NearestnaverScale(MakeCroppedBitmap(bs.Source), MyConfig.SaveScale);
                //表示ウィンドウの設定して表示
                window1 = new Window1();
                window1.MaxWidth = this.ActualWidth;
                window1.MaxHeight = this.ActualHeight;
                window1.MyPreview.Source = img;// bmp;            
                window1.ShowDialog();
                window1.Owner = this;
                window1.SizeToContent = SizeToContent.WidthAndHeight;//ウィンドウ自動サイズ
            }
            else { MessageBox.Show("切り抜き範囲が画像に収まっていないのでプレビューできない"); };
            //切り抜いた画像を拡縮表示は別ウィンドウに表示して、それをBitmapレンダーで保存？
            //できたけど、別ウィンドウを表示した状態visivleじゃないと画像が更新されない
            //自分で拡縮したほうがいいかも？
        }

        #endregion


        #region キーボードで操作


        private void MyTrimThumb_KeyDown(object sender, KeyEventArgs e)
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
                //TextBoxSaveDir.Text = str;//これだとBindingが解けてしまうので
                MyConfig.SavaDir = str;

            }
            else
            {
                MessageBox.Show(str.ToString() + "というフォルダは見当たらない");
            }

        }


        //保存フォルダを開く
        private void ButtonSaveDirOpen_Click(object sender, RoutedEventArgs e)
        {
            string dir = MyConfig.SavaDir;
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
            string path = CheckDir(MyConfig.SavaDir);//フォルダの存在確認
            dialog.InitialDirectory = path;
            dialog.IsFolderPicker = true;//フォルダ選択あり

            if (dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
            {
                //TextBoxSaveDir.Text = dialog.FileName;//これだとBindingが解けてしまう
                MyConfig.SavaDir = dialog.FileName;
            }

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
            if (MyConfig.SaveScale == 1)
            {
                SaveImage(bmp, data.Name);
            }
            else
            {
                SaveImage(NearestnaverScale(bmp, MyConfig.SaveScale), data.Name);
            }
            //CroppedBitmapで切り抜いた画像で保存
            //SaveImage(MakeCroppedBitmap(data.Source), data.Name);
        }

        //Bitmapをファイルに保存
        private void SaveImage(BitmapSource croppedBitmap, string fileName)
        {
            //CroppedBitmapで切り抜いた画像でBitmapFrame作成して保存
            BitmapEncoder encoder = GetEncoder();
            encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));
            try
            {
                using (var fs = new System.IO.FileStream(
                    MakeFullPath(fileName), System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    encoder.Save(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ファイル保存できなかったよ\n" +
                    $"{ex.Message}");
            }
        }

        //ファイル名の重複を回避、拡張子の前に"_"を付け足す
        private string MakeFullPath(string fileName)
        {
            var dir = System.IO.Path.Combine(MyConfig.SavaDir, fileName);
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
                    jpeg.QualityLevel = (int)MyConfig.JpegQuality;// (int)MyNumericJpegQuality.MyValue2;
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

        #region リストボックス
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
        #endregion


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
                        string name = MyConfig.FileName + GetStringNowTime();
                        //画像と名前をリストに追加
                        var source = new MyBitmapAndName(bitmap, name);
                        ListMyBitmapSource.Add(source);
                        MyListBox.SelectedItem = source;
                        MyListBox.ScrollIntoView(source);//選択アイテムまでスクロール
                        //音声ファイル再生
                        if (MyConfig.IsPlaySound == true) { PlaySoundFile(); }
                        if (CheckBoxSaveAuto.IsChecked == true)
                        {
                            SaveImage(MakeCroppedBitmap(bitmap), name);
                        }
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

    [Serializable]
    public class Config : System.ComponentModel.INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private decimal _Width;
        public decimal Width
        {
            get => _Width;
            set
            {
                if (_Width == value)
                    return;
                _Width = value;
                RaisePropertyChanged();
            }
        }

        private decimal _Height;
        public decimal Height
        {
            get => _Height;
            set
            {
                if (_Height == value)
                    return;
                _Height = value;
                RaisePropertyChanged();
            }
        }

        private decimal _Left;
        public decimal Left
        {
            get => _Left;
            set
            {
                if (_Left == value)
                    return;
                _Left = value;
                RaisePropertyChanged();
            }
        }

        private decimal _Top;
        public decimal Top
        {
            get => _Top;
            set
            {
                if (_Top == value)
                    return;
                _Top = value;
                RaisePropertyChanged();
            }
        }

        private string _FileName;
        public string FileName
        {
            get => _FileName;
            set
            {
                if (_FileName == value)
                    return;
                _FileName = value;
                RaisePropertyChanged();
            }
        }

        private string _SavaDir;
        public string SavaDir
        {
            get => _SavaDir;
            set
            {
                if (_SavaDir == value)
                    return;
                _SavaDir = value;
                RaisePropertyChanged();
            }
        }


        private string _SoundDir;
        public string SoundDir
        {
            get => _SoundDir;
            set
            {
                if (_SoundDir == value)
                    return;
                _SoundDir = value;
                RaisePropertyChanged();
            }
        }

        private SaveImageType _SaveImageType;
        public SaveImageType SaveImageType
        {
            get => _SaveImageType;
            set
            {
                if (_SaveImageType == value)
                    return;
                _SaveImageType = value;
                RaisePropertyChanged();
            }
        }


        private decimal _JpegQuality;
        public decimal JpegQuality
        {
            get => _JpegQuality;
            set
            {
                if (_JpegQuality == value)
                    return;
                _JpegQuality = value;
                RaisePropertyChanged();
            }
        }

        private bool _PlaySound;
        public bool IsPlaySound
        {
            get => _PlaySound;
            set
            {
                if (_PlaySound == value)
                    return;
                _PlaySound = value;
                RaisePropertyChanged();
            }
        }

        private decimal _SaveScale;
        public decimal SaveScale
        {
            get => _SaveScale;
            set
            {
                if (_SaveScale == value)
                    return;
                _SaveScale = value;
                RaisePropertyChanged();
            }
        }


    }
}
