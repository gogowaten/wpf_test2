using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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


//依存プロパティはシリアライズできるけど、特定のものをシリアライズから除外することはできない
//[NonSerialized]をつけたプロパティはシリアライズ対象外になるけど、フィールドにあるプロパティだけにしかつけられない
//で、依存プロパティはフィールドにあるとみなされないみたい
//ってことは通知プロパティを使うしかない？

namespace _20190930_シリアライズ設定保存2
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
            
            ButtonColorCyan.Click += (o, e) => { MyBorder.Background = Brushes.Cyan; };

            MySetting = new MySetting();
            MySetting.BackgoundColor = Colors.Red;

            Binding b = MakeBinding(MySetting.LeftProperty);
            MyBorder.SetBinding(LeftProperty, b);
            MySliderLeft.SetBinding(Slider.ValueProperty, b);
            b = MakeBinding(MySetting.TopProperty);
            MyBorder.SetBinding(TopProperty, b);
            MySliderTop.SetBinding(Slider.ValueProperty, b);
            MyBorder.SetBinding(Border.BackgroundProperty, MakeBinding(MySetting.BackgrounBrushProperty));


            //this.DataContext = MySetting;
        }

        private Binding MakeBinding(DependencyProperty dependency)
        {
            return new Binding() { Source=MySetting,Path=new PropertyPath(dependency),Mode=BindingMode.TwoWay };
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            //MySetting setting = new MySetting();
            //setting.Left = MySliderLeft.Value;
            //setting.Top = MySliderTop.Value;
            ////setting.Background = MyBorder.Background;
            //SolidColorBrush brush = (SolidColorBrush)MyBorder.Background;
            //setting.BackgounrColor = brush.Color;


            string myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullPath = System.IO.Path.Combine(myDocument, "mySetting.xml");
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MySetting));
            using (var stream = new System.IO.StreamWriter(fullPath, false, new System.Text.UTF8Encoding(false)))
            {
                serializer.Serialize(stream, MySetting);//エラー、brushは保存できないけど、colorはできた
            }

            //fullPath = System.IO.Path.Combine(myDocument, "mySetting.bin");
            //using (var stream = new System.IO.FileStream(fullPath,System.IO.FileMode.Create,System.IO.FileAccess.Write))
            //{
            //    var bin = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //    bin.Serialize(stream, setting);//えらー、brushやcolorは保存できない
            //}


            MessageBox.Show("保存できた");
        }

        private void ButtonKakunin_Click(object sender, RoutedEventArgs e)
        {
            var neko = MySetting;
        }
    }

    [Serializable]
    public class MySetting :DependencyObject// System.ComponentModel.INotifyPropertyChanged
    {
        public double Left
        {
            get => (double)GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(nameof(Left), typeof(double), typeof(MySetting));

        public double Top
        {
            get => (double)GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }
        public static readonly DependencyProperty TopProperty = DependencyProperty.Register(nameof(Top), typeof(double), typeof(MySetting));

        [NonSerialized]
        public SolidColorBrush BackgrounBrush
        {
            get => (SolidColorBrush)GetValue(BackgrounBrushProperty);
            set => SetValue(BackgrounBrushProperty, value);
        }
        public static readonly DependencyProperty BackgrounBrushProperty = DependencyProperty.Register(nameof(BackgrounBrush), typeof(SolidColorBrush), typeof(MySetting));
        public Color BackgoundColor
        {
            get => (Color)GetValue(BackgoundColorProperty);
            set => SetValue(BackgoundColorProperty, value);
        }
        public static readonly DependencyProperty BackgoundColorProperty = DependencyProperty.Register(nameof(BackgoundColor), typeof(Color), typeof(MySetting));
  


        //public Brush Background;
        //[NonSerialized]//これをつけるとシリアライズ対象から外れるはずだけど、xmlでのシリアライズでは無視されて保存された
        //public Color BackgounrColor;

        //public event PropertyChangedEventHandler PropertyChanged;
    }

    public class MyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
