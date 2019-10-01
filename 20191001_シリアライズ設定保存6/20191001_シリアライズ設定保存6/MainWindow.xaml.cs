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

using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.ComponentModel;

//通知プロパティとdatacontract

namespace _20191001_シリアライズ設定保存6
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        MySetting MySetting;
        public MainWindow()
        {
            InitializeComponent();
            ButtonSave.Click += ButtonSave_Click;
            ButtonLoad.Click += ButtonLoad_Click;

            ButtonColorCyan.Click += (o, e) => { MyBorder.Background = Brushes.Cyan; };
            ButtonColorMediumPurple.Click += (o, e) => MyBorder.Background = Brushes.MediumPurple;
            
            MySetting = new MySetting();            
            this.DataContext = MySetting;
            MyBindign();
            //Binding、XAMLに書いてもできるけど、プロパティ名変更する可能性を考えるとこっちのコードで書いたほうがいい？
            MyBorder.SetBinding(LeftProperty, new Binding(nameof(MySetting.Left)));
            MySliderLeft.SetBinding(Slider.ValueProperty, new Binding(nameof(MySetting.Left)));
        }
        private void MyBindign()
        {
            //canvertが必要なBindingはXAMLに書いたほうがいいかも
            //XAMLに書いておけばDataContextを変更したときに自動でBindingしてくれる
            //とはいっても、こちらでも一回実行するだけだから、あんまり変わらない？
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
        

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            string myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullPath = System.IO.Path.Combine(myDocument, "mySetting.xml");
            var serializer = new DataContractSerializer(typeof(MySetting));
            using(var xr = XmlReader.Create(fullPath))
            {
                MySetting = (MySetting)serializer.ReadObject(xr);
            }
            this.DataContext = MySetting;
            MyBindign();//必要
        }


        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullPath = System.IO.Path.Combine(myDocument, "mySetting.xml");
            var serializer = new DataContractSerializer(typeof(MySetting));
            var setting = new XmlWriterSettings();
            setting.Encoding = new UTF8Encoding(false);
            using (var wr = XmlWriter.Create(fullPath,setting))
            {
                serializer.WriteObject(wr, MySetting);
            }

            MessageBox.Show("保存できた");
        }

        private void ButtonKakunin_Click(object sender, RoutedEventArgs e)
        {
            var neko = MySetting;
        }
    }

    
    //    INotifyPropertyChangedプロパティ実装方法まとめ C#3からC#7、Fodyも - Qiita
    //https://qiita.com/soi/items/d0c83a0cc3a4b23237ef
    //より追加したコードスニペットを使うときのショートカット、propn
    [DataContract]
    public class MySetting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        [DataMember]
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

        [DataMember]
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
        //[DataMember]
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

        [DataMember]
        private byte _A;
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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
