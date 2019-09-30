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

using System.ComponentModel;
using System.Globalization;

//通知プロパティでbrushはARGBのbyteに分解してみた
//BindingはマルチバインディングでコンバーターでARGBとbrushを双方向変換
//xmlでのシリアライズはできた
//けど、バイナリ型だとエラーになる

namespace _20190930_シリアライズ設定保存4
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        MySetting MySetting;
        string SettingFileName = "mySetting.xml";
        public MainWindow()
        {
            InitializeComponent();
            ButtonSave.Click += ButtonSave_Click;

            ButtonColorCyan.Click += (o, e) => { MyBorder.Background = Brushes.Cyan; };
            ButtonColorMediumPurple.Click += (object sender, RoutedEventArgs e) => MyBorder.Background = Brushes.MediumPurple;

            MySetting = new MySetting();


            MyBinding();


        }
        private void MyBinding()
        {
            //DataContextを使うと単純なBindingはプロパティ名を指定するだけで済む
            this.DataContext = MySetting;
            MyBorder.SetBinding(LeftProperty, new Binding(nameof(MySetting.Left)));
            MySliderLeft.SetBinding(Slider.ValueProperty, new Binding(nameof(MySetting.Left)));
            MyBorder.SetBinding(TopProperty, new Binding(nameof(MySetting.Top)));
            MySliderTop.SetBinding(Slider.ValueProperty, new Binding(nameof(MySetting.Top)));



            //canvertが必要なBindingは手間がかかる
            MultiBinding mb = new MultiBinding();
            mb.Converter = new MyConverter();
            mb.Mode = BindingMode.TwoWay;
            mb.Bindings.Add(MakeBinding(nameof(MySetting.A)));
            mb.Bindings.Add(MakeBinding(nameof(MySetting.R)));
            mb.Bindings.Add(MakeBinding(nameof(MySetting.G)));
            mb.Bindings.Add(MakeBinding(nameof(MySetting.B)));
            MyBorder.SetBinding(Border.BackgroundProperty, mb);

            Binding MakeBinding(string str)
            {
                return new Binding() { Source = MySetting, Path = new PropertyPath(str), Mode = BindingMode.TwoWay };
            }
        }

        //シリアライズ
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {

            string myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullPath = System.IO.Path.Combine(myDocument, SettingFileName);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MySetting));
            using (var stream = new System.IO.StreamWriter(fullPath, false, new UTF8Encoding(false)))
            {
                serializer.Serialize(stream, MySetting);//エラー、brushは保存できないけど、colorはできた
            }

            //fullPath = System.IO.Path.Combine(myDocument, "mySetting.bin");
            //using (var stream = new System.IO.FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            //{
            //    var bin = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //    bin.Serialize(stream, MySetting);//えらー、PropertyChangedをシリアライズしようとしてエラーになる
            //    //無視する設定にしたいけどできない
            //}


            MessageBox.Show("保存できた");
        }

        private void ButtonKakunin_Click(object sender, RoutedEventArgs e)
        {
            var neko = MySetting;
        }



        //デシリアライズ
        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            string myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullPath = System.IO.Path.Combine(myDocument, SettingFileName);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MySetting));
            using (var stream = new System.IO.StreamReader(fullPath, new UTF8Encoding(false)))
            {
                MySetting = (MySetting)serializer.Deserialize(stream);
            }
            MyBinding();
        }
    }


    //using System.ComponentModel;

    //    INotifyPropertyChangedプロパティ実装方法まとめ C#3からC#7、Fodyも - Qiita
    //https://qiita.com/soi/items/d0c83a0cc3a4b23237ef
    //より追加したコードスニペットを使うときのショートカット、propn
    [Serializable]
    public class MySetting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private double MyLeft;
        public double Left
        {
            get => MyLeft;
            set
            {
                if (value == MyLeft) return;
                MyLeft = value;
                RaisePropertyChanged();
            }
        }

        private double _Top;
        public double Top
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

        //private Color _BackgroundColor;
        //public Color BackgroundColor
        //{
        //    get => _BackgroundColor;
        //    set
        //    {
        //        if (_BackgroundColor == value)
        //            return;
        //        _BackgroundColor = value;
        //        RaisePropertyChanged();
        //    }
        //}

        private byte _A;
        //[NonSerialized]//シリアライズから除外はできない
        public byte A
        {
            get => _A;
            set
            {
                if (_A == value)
                    return;
                _A = value;
                RaisePropertyChanged();
            }
        }

        private byte _R;
        public byte R
        {
            get => _R;
            set
            {
                if (_R == value)
                    return;
                _R = value;
                RaisePropertyChanged();
            }
        }

        private byte _G;
        public byte G
        {
            get => _G;
            set
            {
                if (_G == value)
                    return;
                _G = value;
                RaisePropertyChanged();
            }
        }

        private byte _B;
        public byte B
        {
            get => _B;
            set
            {
                if (_B == value)
                    return;
                _B = value;
                RaisePropertyChanged();
            }
        }


        //public Brush Background;
        //[NonSerialized]//これをつけるとシリアライズ対象から外れるはずだけど、xmlでのシリアライズでは無視されて保存された
        //public Color BackgounrColor;

        //public event PropertyChangedEventHandler PropertyChanged;
    }

    public class MyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //argbをブラシに変換
            byte A = (byte)values[0];
            byte R = (byte)values[1];
            byte G = (byte)values[2];
            byte B = (byte)values[3];
            return new SolidColorBrush(Color.FromArgb(A, R, G, B));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            //ブラシをargbに変換
            SolidColorBrush brush = (SolidColorBrush)value;
            Color c = brush.Color;
            return new object[] { c.A, c.R, c.G, c.B };
        }
    }
}
