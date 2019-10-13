﻿using System;
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
using System.Collections.ObjectModel;


namespace _20191010_フォルダダイアログ
{
    /// <summary>
    /// FolderDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderDialog : Window
    {
        public FolderDialog(string folderPath, Window owner)
        {
            InitializeComponent();
            this.Owner = owner;//確実に閉じるため、複数のdialogが作られると1個しか閉じられない
            this.KeyDown += FolderDialog_KeyDown;

            //ドライブ全部を表示
            string[] drives = Environment.GetLogicalDrives();
            for (int i = 0; i < drives.Length; i++)
            {
                AddNode(new DirectoryInfo(drives[i]));
            }

            //指定フォルダが在るときは、そこまで作成して展開
            if (folderPath != null)
            {
                if (Directory.Exists(folderPath))
                {
                    ExpandAll(new DirectoryInfo(folderPath));
                }
            }

        }
        public FolderDialog(Window owner) : this(null, owner)
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
            catch (Exception)
            {
                return false;
            }
        }

        #region 指定フォルダまで作成して展開
        //指定フォルダまですべて展開
        private void ExpandAll(DirectoryInfo info)
        {
            //ルートドライブ群取得
            ObservableCollection<DirectoryTreeItem> subTrees = GetDrives();

            //フォルダをさかのぼってすべてのフォルダ名取得
            List<DirectoryInfo> dirInfos = GetAllDirectoryInfo(info);
            //上から順番に展開していく
            DirectoryTreeItem subTree = null;
            for (int i = dirInfos.Count - 1; i >= 0; i--)
            {
                for (int ii = 0; ii < subTrees.Count; ii++)
                {
                    //名前一致で、そのツリーを展開
                    if (subTrees[ii].DirectoryInfo.Name == dirInfos[i].Name)
                    {
                        subTree = subTrees[ii];
                        subTree.IsExpanded = true;//ツリー展開、ここでサブフォルダが追加される
                        subTree.IsSelected = true;
                        subTree.BringIntoView();//見えるところまでスクロール
                        subTree.Focus();
                        subTrees = subTree.SubDirectorys;//次のサブフォルダ群
                        break;
                    }
                }
            }
            //指定フォルダのサブフォルダは展開しない(どっちでもいい)
            if (subTree != null) { subTree.IsExpanded = false; }
        }

        //ルート直下のTreeItemを取得
        private ObservableCollection<DirectoryTreeItem> GetDrives()
        {
            var subTrees = new ObservableCollection<DirectoryTreeItem>();
            for (int i = 0; i < Root.Items.Count; i++)
            {
                var item = (DirectoryTreeItem)Root.Items[i];
                subTrees.Add(item);
            }
            return subTrees;
        }

        //指定フォルダをさかのぼってルートドライブ群までのDirectoryInfo取得
        private List<DirectoryInfo> GetAllDirectoryInfo(DirectoryInfo info)
        {
            DirectoryInfo temp = info;
            var dir = new List<DirectoryInfo>();
            dir.Add(info);

            while (temp.Parent != null)
            {
                dir.Add(temp.Parent);
                temp = temp.Parent;
            }
            return dir;
        }
        public string GetFullPath()
        {
            return Root.SelectedItem.ToString();
        }
        private string[] GetDir(DirectoryInfo info)
        {
            return info.FullName.Split(Char.Parse("\\"));
        }

        #endregion


        #region イベント
        //エンターキーでResult
        private void FolderDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Root.SelectedItem != null)
            {
                DialogResult = true;
            }
        }
        //キャンセルボタン
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        //okボタン
        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (Root.SelectedItem == null)
            {
                this.DialogResult = false;
            }
            else
            {
                this.DialogResult = true;
            }
        }


        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {

        }


        #endregion
      



        public class DirectoryTreeItem : TreeViewItem
        {
            public readonly System.IO.DirectoryInfo DirectoryInfo;
            private bool IsAdd;//サブフォルダを作成済みかどうか
            private TreeViewItem Dummy;//ダミーアイテム
            public ObservableCollection<DirectoryTreeItem> SubDirectorys;//サブフォルダ用

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
                                      //SubDirectoryInfos = new ObservableCollection<DirectoryInfo>();
                    SubDirectorys = new ObservableCollection<DirectoryTreeItem>();
                    AddSubDirectory();
                };


                ////フォルダ選択でサブフォルダ開閉
                //this.PreviewMouseLeftButtonDown += (s, e) =>
                //{
                //    var source = (DirectoryTreeItem)e.Source;
                //    if (source != s) return;
                //    //開閉
                //    source.IsExpanded = !IsExpanded;
                //    e.Handled = true;
                //};

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
                    var fileAttributes = directories[i].Attributes;
                    if ((fileAttributes & System.IO.FileAttributes.Hidden) == System.IO.FileAttributes.Hidden ||
                            (fileAttributes & System.IO.FileAttributes.System) == System.IO.FileAttributes.System)
                    {
                        continue;
                    }

                    //サブフォルダにもアクセスできるフォルダのみItem追加
                    try
                    {
                        //これが通れば
                        directories[i].GetDirectories();
                        //追加
                        var item = new DirectoryTreeItem(directories[i]);
                        Items.Add(item);
                        SubDirectorys.Add(item);
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
}
//    WPF TreeView – 展開されたブランチが見えるようにスクロールする方法 - コードログ
//https://codeday.me/jp/qa/20190128/169432.html
