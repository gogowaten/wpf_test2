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
            ButtonGetValue.Click += ButtonGetValue_Click;
            Button1.Click += Button1_Click;

            double neko = 0.1;
            double inu = 0.0;
            decimal deci = 0m;
            for (int i = 0; i < 100; i++)
            {
                inu += neko;
                deci += (decimal)neko;
            }

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Numeric.MyValue = 12.3456;
            //Numeric.MyValue2 = 12.3456;
        }

        private void ButtonGetValue_Click(object sender, RoutedEventArgs e)
        {
            var v = Numeric.MyValue;
            var v2 = Numeric.MyValue2;
            MessageBox.Show(v.ToString()+" "+v2.ToString());
            
        }

        private void Button10000_Click(object sender, RoutedEventArgs e)
        {
            Numeric.MyValue = 10000;
        }

    
        private void ButtonAutoHeight_Click(object sender, RoutedEventArgs e)
        {
            Numeric.MyHeight = double.NaN;
        }

        private void ButtonAutoWidth_Click(object sender, RoutedEventArgs e)
        {
            Numeric.MyWidth = double.NaN;
        }

        private void ButtonAutoFontSize_Click(object sender, RoutedEventArgs e)
        {
            Numeric.MyFontSize = double.NaN;
        }
    }
}
