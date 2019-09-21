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

namespace _20190921_contextmenu
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ContextMenu contextMenu = new ContextMenu();
            this.ContextMenu = contextMenu;


            var l = new Label() { Content = "label1" };            
            contextMenu.Items.Add(l);

            MenuItem menuItem = new MenuItem() { Header = "menuItem1" };
            contextMenu.Items.Add(menuItem);
            menuItem.Click += (s, e) => { MessageBox.Show(""); };


        }
    }
}
