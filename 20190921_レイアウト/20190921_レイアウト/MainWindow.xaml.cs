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

namespace _20190921_レイアウト
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ContextMenu menu = new ContextMenu();
            MenuItem item = new MenuItem();
            item.Header = "非表示";
            menu.Items.Add(item);
            menu.ContextMenuOpening += Menu_ContextMenuOpening;
            menu.Opened += Menu_Opened;
            this.ContextMenu = menu;
            item.Click += Item_Click;
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            if (MySide.Width.Value == 0)
            {
                //item.Header = "表示";
                MySide.Width = new GridLength(200);
            }
            else
            {
                //item.Header = "非表示";
                MySide.Width = new GridLength(0);
            }
        }

        private void Menu_Opened(object sender, RoutedEventArgs e)
        {
            var context = (ContextMenu)sender;
            var item =(MenuItem) context.Items.GetItemAt(0);
            var grid = MyGrid.ColumnDefinitions;
            GridLength sidewidth = MySide.Width;
            if (MySide.Width.Value == 0) {
                item.Header = "表示";
                //MySide.Width = new GridLength(200);
            }
            else
            {
                item.Header = "非表示";
                //MySide.Width = new GridLength(0);
            }
        }

        private void Menu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            
        }
    }
}
