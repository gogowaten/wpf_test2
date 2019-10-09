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

namespace _20191009_TreeView
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //TreeView━TreeViewItem
            //              ┗TreeViewItem
            //                  ┗TreeViewItem
            TreeViewItem item = MakeTreeViewItem("test1");
            MyTreeView.Items.Add(item);

            TreeViewItem item2 = MakeTreeViewItem("test1-1");
            item.Items.Add(item2);

            TreeViewItem item3 = MakeTreeViewItem("test1-1-1");
            item2.Items.Add(item3);

            item.Expanded += (s, e) => { };

            //これでもツリー状になるけど、項目が選択できない、選択状態にならない
            //TreeViewItem
            //  ┗TreeViewItem            
            //      ┗TreeViewItem
            //          ┗TreeViewItem
            MyTreeViewItem.Header = "rootTreeViweItem";

            TreeViewItem item4 = MakeTreeViewItem("test4");
            MyTreeViewItem.Items.Add(item4);

            TreeViewItem item5 = MakeTreeViewItem("test5");
            item4.Items.Add(item5);

            TreeViewItem item6 = MakeTreeViewItem("test6");
            item5.Items.Add(item6);

            item4.Collapsed += (s, e) => { };//閉じられたとき、test5のときも発生する
            item4.Expanded += (s, e) => { };//展開されたとき、test5のときも発生する
            item4.Selected += (s, e) => { };//選択されたときだけど、選択できないから発生しなかった
        }

        public TreeViewItem MakeTreeViewItem(string header)
        {
            var item = new TreeViewItem();
            item.Header = header;
            return item;
        }
    }
}
