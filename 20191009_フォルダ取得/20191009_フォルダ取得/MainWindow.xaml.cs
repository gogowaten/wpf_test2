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
//TreeViewのテスト

namespace _20191009_フォルダ取得
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MyTreeView.ItemsSource = new List<Person>()
            {
                new Person("neko2")
                {
                    Children = new List<Person>()
                    {
                        new Person("neko2-1"),
                        new Person("neko2-2")
                        {
                            Children=new List<Person>()
                            {
                                new Person("neko2-2-1")
                            }
                        }
                    }
                },
                new Person("neko3")
                {
                    Children =new List<Person>()
                    {
                        new Person("neko3-1"),
                        new Person("neko3-2")
                    }
                }
            };


    }




    public sealed class Person
    {
        public string Name { get; set; }
        public List<Person> Children { get; set; } = new List<Person>();
        public Person(string name)
        {
            Name = name;
        }
    }

}
//WPF4.5入門 その26 「TreeViewコントロール その2」 - かずきのBlog @hatena
//https://blog.okazuki.jp/entry/20130409/1365479109
