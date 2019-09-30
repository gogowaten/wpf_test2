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


//普通のシリアライズだとbrushやcolorが保存できない

namespace _20190930_シリアライズ設定保存
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            ButtonSave.Click += ButtonSave_Click;
            ButtonColorCyan.Click += (o, e) => { MyBorder.Background = Brushes.Cyan; };

        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            MySetting setting = new MySetting();
            setting.Left = MySliderLeft.Value;
            setting.Top = MySliderTop.Value;
            //setting.Background = MyBorder.Background;
            SolidColorBrush brush =(SolidColorBrush) MyBorder.Background;
            setting.BackgounrColor = brush.Color;


            string myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullPath = System.IO.Path.Combine(myDocument, "mySetting.xml");
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MySetting));
            using (var stream = new System.IO.StreamWriter(fullPath, false, new System.Text.UTF8Encoding(false)))
            {
                serializer.Serialize(stream, setting);//エラー、brushは保存できないけど、colorはできた
            }

            //fullPath = System.IO.Path.Combine(myDocument, "mySetting.bin");
            //using (var stream = new System.IO.FileStream(fullPath,System.IO.FileMode.Create,System.IO.FileAccess.Write))
            //{
            //    var bin = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //    bin.Serialize(stream, setting);//えらー、brushやcolorは保存できない
            //}


            MessageBox.Show("保存できた");
        }
    }

    [Serializable]
    public class MySetting
    {
        public double Left;
        public double Top;
        //public Brush Background;
        [NonSerialized]//これをつけるとシリアライズ対象から外れるはずだけど、xmlでのシリアライズでは無視されて保存された
        public Color BackgounrColor;

    }
}
