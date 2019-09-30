using System;
using System.Collections.Generic;
using System.ComponentModel;
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

//INotifyPropertyChanged(通知プロパティ)を使ったBindingとシリアライズ
//相変わらずbrushはシリアライズできない
//依存プロパティと違うのは、xmlで保存だと、シリアライズできないプロパティは自動で無視して
//できたものだけ保存してくれる

namespace _20190930_シリアライズ設定保存3
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
            //MySetting.BackgoundColor = Colors.Red;

            this.DataContext = MySetting;
            MyBorder.SetBinding(LeftProperty, new Binding(nameof(MySetting.Left)));
            MySliderLeft.SetBinding(Slider.ValueProperty, new Binding(nameof(MySetting.Left)));
            MyBorder.SetBinding(Border.BackgroundProperty, new Binding(nameof(MySetting.BackgroundSolidBrush)));


            //this.DataContext = MySetting;
        }

        private Binding MakeBinding(DependencyProperty dependency)
        {
            return new Binding() { Source = MySetting, Path = new PropertyPath(dependency), Mode = BindingMode.TwoWay };
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
           
            string myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullPath = System.IO.Path.Combine(myDocument, "mySetting.xml");
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MySetting));
            using (var stream = new System.IO.StreamWriter(fullPath, false, new System.Text.UTF8Encoding(false)))
            {
                serializer.Serialize(stream, MySetting);//エラー、brushは保存できないけど、colorはできた
            }

            //fullPath = System.IO.Path.Combine(myDocument, "mySetting.bin");
            //using (var stream = new System.IO.FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            //{
            //    var bin = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //    bin.Serialize(stream, MySetting);//えらー、brushやcolorは保存できない
            //}


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
    [Serializable]
    public class MySetting : System.ComponentModel.INotifyPropertyChanged
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

        private Color _BackgroundColor;
        public Color BackgroundColor
        {
            get => _BackgroundColor;
            set
            {
                if (_BackgroundColor == value)
                    return;
                _BackgroundColor = value;
                RaisePropertyChanged();
            }
        }

        private SolidColorBrush _BackgroundSolidBrush;
        public SolidColorBrush BackgroundSolidBrush
        {
            get => _BackgroundSolidBrush;
            set
            {
                if (_BackgroundSolidBrush == value)
                    return;
                _BackgroundSolidBrush = value;
                RaisePropertyChanged();
            }
        }


        //public Brush Background;
        //[NonSerialized]//これをつけるとシリアライズ対象から外れるはずだけど、xmlでのシリアライズでは無視されて保存された
        //public Color BackgounrColor;

        //public event PropertyChangedEventHandler PropertyChanged;
    }

}
