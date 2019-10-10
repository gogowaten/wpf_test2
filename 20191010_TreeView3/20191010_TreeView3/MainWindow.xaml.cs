using System.Windows;
using System.Windows.Controls;

namespace _20191010_TreeView3
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //TreeViewにルートで表示するフォルダを追加
            var item = new DirectoryTreeItem(new System.IO.DirectoryInfo(@"D:\ブログ用"));
            MyRoot.Items.Add(item);
        }

        //確認用
        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var neko = (DirectoryTreeItem)MyRoot.Items[0];
            var inu = (TreeViewItem)neko.Items[0];
            var item = (DirectoryTreeItem)MyRoot.SelectedItem;
            if (item == null) return;
            string str = item.ToString();
            string dir = item.DirectoryInfo.FullName;
        }
    }



    public class DirectoryTreeItem : TreeViewItem
    {
        public readonly System.IO.DirectoryInfo DirectoryInfo;
        private bool IsAdd;//サブフォルダを作成済みかどうか
        private TreeViewItem Dummy;//ダミーアイテム


        public DirectoryTreeItem(System.IO.DirectoryInfo info)
        {
            DirectoryInfo = info;
            Header = info.Name;

            //サブフォルダが1つでもあれば
            if (info.GetDirectories().Length > 0)
            //展開できることを示す▷を表示するためにダミーのTreeViewItemを追加する
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
                Items.Add(new DirectoryTreeItem(directories[i]));
            }
            IsAdd = true;//サブフォルダ作成済みフラグ
        }


        public override string ToString()
        {
            return DirectoryInfo.FullName;
        }
    }
}
//ファイルの属性を取得、設定する - .NET Tips(VB.NET, C#...)
//https://dobon.net/vb/dotnet/file/fileattributes.html
//WPF4.5入門 その26 「TreeViewコントロール その2」 - かずきのBlog @hatena
//https://blog.okazuki.jp/entry/20130409/1365479109
//WPF TreeViewを使ってみた(2) - Qiita
//https://qiita.com/kuro4/items/552b780bb2832a8de5a6


//ブログ記事
//WPFでフォルダ選択のダイアログボックスみたいなの作りたい - 午後わてんのブログ
//https://gogowaten.hatenablog.com/entry/2019/10/10/122459
