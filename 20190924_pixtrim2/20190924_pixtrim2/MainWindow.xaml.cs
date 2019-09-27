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

namespace _20190924_pixtrim2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClipboardWatcher ClipboardWatcher;
        //private List<BitmapSource> ListBitmap;
        private ObservableCollection<MyBitmapSource> ListMyBitmapSource;
        private TrimThumb MyTrimThumb;//切り取り範囲


        public MainWindow()
        {
            InitializeComponent();

            ButtonTest.Click += ButtonTest_Click;

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

        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var x = Canvas.GetLeft(MyTrimThumb);
            var vx = MyNumericX.MyValue;
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

                        //int sn = decimal.Parse(MyNumericSerial.MyValue2);
                        //string number = $"{sn,0:00000}";
                        string name = TextBox_FileName.Text + MyNumericSerial.GetText();
                        MyNumericSerial.MyValue++;
                        //sn = int.Parse(TextBox_SerialNumber.Text) + 1;
                        //TextBox_SerialNumber.Text = $"{sn,0:D5}";
                        //TextBox_SerialNumber.Text = $"{sn,0:00000}";
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
