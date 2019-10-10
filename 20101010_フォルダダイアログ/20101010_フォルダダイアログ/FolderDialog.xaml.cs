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
using System.Windows.Shapes;
using System.IO;

namespace _20101010_フォルダダイアログ
{
    /// <summary>
    /// FolderDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderDialog : Window
    {
        private DirectoryInfo OriginalInfo;
        public FolderDialog(DirectoryInfo directoryInfo)
        {
            InitializeComponent();

            OriginalInfo = directoryInfo;

            //指定フォルダがなければドライブ全部を表示
            if (directoryInfo == null)
            {
                string[] drives = Environment.GetLogicalDrives();
                for (int i = 0; i < drives.Length; i++)
                {
                    AddNode(new DirectoryInfo(drives[i]));
                }
            }
            else
            {
                string[] drives = Environment.GetLogicalDrives();
                for (int i = 0; i < drives.Length; i++)
                {
                    AddNode(new DirectoryInfo(drives[i]));
                }

                //AddNode(directoryInfo);

                DirectoryInfo p = directoryInfo.Parent;
                DirectoryInfo pp = p.Parent;
                DirectoryInfo ppp = pp.Parent;
                DirectoryInfo pppp = ppp.Parent;
                DirectoryInfo ppppp = pppp.Parent;

            }


        }
        public FolderDialog() : this(null)
        {

        }

        private bool AddNode(DirectoryInfo info)
        {
            //準備ができていないドライブならfalse
            try
            {
                Root.Items.Add(new DirectoryTreeItem(info));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        public string GetFullPath()
        {
            return Root.SelectedItem.ToString();
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            //最初はルートのドライブを展開
            DirectoryInfo originRoot = OriginalInfo.Root;
            ItemCollection items = Root.Items;
            foreach (var item in items)
            {
                var dirItem = (DirectoryTreeItem)item;
                if (dirItem.ToString() == originRoot.FullName)
                {
                    dirItem.IsExpanded = true;
                    break;
                }
            }

            //サブフォルダを順番に展開
            List<string> dirs = GetAllDirectory(OriginalInfo);
            for (int i = 0; i < dirs.Count; i++)
            {
                foreach (var item in items)
                {

                }
            }

        }

        private List<string> GetAllDirectory(DirectoryInfo info)
        {
            DirectoryInfo temp = info;
            var dir = new List<string>();
            dir.Add(info.Name);

            while (temp.Parent!=null)
            {
                dir.Add(temp.Parent.Name);
                temp = temp.Parent;
            }
            return dir;
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
                ////隠しフォルダ、システムフォルダは除外する
                //var fileAttributes = directories[i].Attributes;
                //if ((fileAttributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden ||
                //        (fileAttributes & System.IO.FileAttributes.System) == System.IO.FileAttributes.System)
                //{
                //    continue;
                //}
                ////追加
                //Items.Add(new DirectoryTreeItem(directories[i]));

                //サブフォルダにもアクセスできるフォルダのみItem追加
                try
                {
                    directories[i].GetDirectories();//これが通ればok
                    //追加
                    Items.Add(new DirectoryTreeItem(directories[i]));
                }
                catch (Exception)
                {
                }
            }
            IsAdd = true;//サブフォルダ作成済みフラグ
        }


        public override string ToString()
        {
            return DirectoryInfo.FullName;
        }
    }
}
