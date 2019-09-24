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

namespace _20190924_numeric
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        MyNumeric numeric;
        public MainWindow()
        {
            InitializeComponent();

            var neko = String.Format("{0:000}", 1);


            numeric = new MyNumeric();
            numeric.Maximun = 15;
            MyStackPanel.Children.Add(numeric);

            MyButton10.Click += (o, e) => { numeric.MyValue = 10; };
            Kakunin.Click += Kakunin_Click;
        }

        private void Kakunin_Click(object sender, RoutedEventArgs e)
        {
            var n = numeric;

        }
    }

}
