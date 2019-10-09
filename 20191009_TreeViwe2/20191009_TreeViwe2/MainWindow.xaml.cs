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

namespace _20191009_TreeViwe2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();



            //マイドキュメントのパス
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //dir = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            dir = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";//pcのクラスID、コレだとエラー
            var neko = Environment.GetLogicalDrives();

            //System.IO.DirectoryInfo doc = new System.IO.DirectoryInfo(dir);

            ////PCを開いてみる
            //var dialog = new Microsoft.Win32.OpenFileDialog();
            //dialog.InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
            //dialog.ShowDialog();



            //D:\ブログ用
            var rootInfo = new System.IO.DirectoryInfo(@"D:\ブログ用");
            var rootItem = new DirTreeItem(rootInfo);
            //root = new DirTreeItem(new System.IO.DirectoryInfo(@"D:\ブログ用"));
            rootItem.Header = rootInfo.Name;
            MyRoot.Items.Add(rootItem);

        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var neko = (DirTreeItem)MyRoot.Items[0];
            var inu = (TreeViewItem)neko.Items[0];
        }
    }

    public class DirTreeItem : TreeViewItem
    {
        public System.IO.DirectoryInfo DirectoryInfo;
        private bool IsAdd;//サブフォルダを作成済みかどうか
        private TreeViewItem Dummy;//ダミーアイテム


        public DirTreeItem(System.IO.DirectoryInfo info)
        {
            DirectoryInfo = info;
            Header = info.Name;

            //サブフォルダが1つでもあれば
            if (info.GetDirectories().Length > 0)
            //展開できることを示す▼を表示するためにダミーのTreeViewItemを追加する
            {
                Dummy = new TreeViewItem();
                Items.Add(Dummy);
            }

            //イベント、ツリー展開時
            //サブフォルダを追加
            this.Expanded += (s, e) =>
             {
                 if (IsAdd) return;//追加済みなら何もしない
                 AddSubDirectory();
             };
        }

        //サブフォルダツリー追加
        public void AddSubDirectory()
        {
            Items.Remove(Dummy);//ダミーのTreeViewItemを削除

            //すべてのサブフォルダを追加
            System.IO.DirectoryInfo[] directories = DirectoryInfo.GetDirectories();
            for (int i = 0; i < directories.Length; i++)
            {
                //隠しフォルダ、システムフォルダは除外する
                var fileAttributes = directories[i].Attributes;
                if ((fileAttributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden ||
                        (fileAttributes & System.IO.FileAttributes.System) == System.IO.FileAttributes.System)
                {
                    continue;
                }
                //追加
                Items.Add(new DirTreeItem(directories[i]));
            }
            IsAdd = true;//サブフォルダ使い済みフラグ
        }


        public override string ToString()
        {
            return Header.ToString();
        }
    }
}
//ファイルの属性を取得、設定する - .NET Tips(VB.NET, C#...)
//https://dobon.net/vb/dotnet/file/fileattributes.html
//WPF4.5入門 その26 「TreeViewコントロール その2」 - かずきのBlog @hatena
//https://blog.okazuki.jp/entry/20130409/1365479109
