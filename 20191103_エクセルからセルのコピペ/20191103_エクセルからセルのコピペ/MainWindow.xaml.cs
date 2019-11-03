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


namespace _20191103_エクセルからセルのコピペ
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<MyBitmap> MyBitmaps = new ObservableCollection<MyBitmap>();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MyBitmaps;
            MyListBox.SelectionChanged += MyListBox_SelectionChanged;
        }

        private void MyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyBitmap item = MyListBox.SelectedItem as MyBitmap;
            if (item != null && item.BitmapSource != null)
            {
                Clipboard.Clear();
                Clipboard.SetImage(item.BitmapSource);//おk、これが一番いい
                //Clipboard.SetDataObject(item.BitmapSource);//なぜかセットされない
                //Clipboard.SetData("Bitmap", item.BitmapSource);//おk、たぶんSetImageと同じ
                //Clipboard.SetData("BitmapSource", item.BitmapSource);//セットされない

                //これも動作がおかしい、他のアプリには画像がないとされるけど、このアプリには貼り付けできるけど、それも変色している
                //Clipboard.SetData("System.Windows.Media.Imaging.BitmapSource", item.BitmapSource);
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyBitmaps.Clear();

            var data = Clipboard.GetDataObject();
            var neko = data.GetFormats();
            var bmp = data.GetData("Bitmap") as BitmapSource;
            MyBitmaps.Add(new MyBitmap(bmp, "GetData\"Bitmap\""));

            bmp = data.GetData("System.Windows.Media.Imaging.BitmapSource") as BitmapSource;
            MyBitmaps.Add(new MyBitmap(bmp, "GetData\"BitmapSource\""));

            bmp = Clipboard.GetImage();
            MyBitmaps.Add(new MyBitmap(bmp, "GetImage()"));

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }




    public class MyBitmap
    {
        public BitmapSource BitmapSource { get; set; }
        public string DataFormatName { get; set; }
        public string Dpi { get; set; }

        public MyBitmap(BitmapSource source, string name)
        {
            BitmapSource = source;
            DataFormatName = name;
            if (source != null)
            {
                Dpi = "dpi = " + source.DpiX.ToString();
            }
            else
            {
                Dpi = "none";
            }

        }
    }
}
