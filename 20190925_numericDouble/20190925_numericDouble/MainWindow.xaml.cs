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

namespace _20190925_numericDouble
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Button10000.Click += Button10000_Click;

            var neko =string.Format("{0:f4}", 30.5);
            double p = 3.14;
            int i = (int)p;
            var pp = p.ToString("{0:f4}");
            pp = p.ToString("0");
            double dd = double.Parse(pp);
            pp = p.ToString("00.000");
            dd = double.Parse(pp);
            pp = p.ToString("0000");
            dd = double.Parse(pp);
        }

        private void Button10000_Click(object sender, RoutedEventArgs e)
        {
            Numeric.MyValue = 10000;
        }
    }
}
