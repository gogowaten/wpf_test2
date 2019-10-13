using System.Windows;

namespace _20191013_FolderDialog
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ButtonOpenFolderDialog.Click += ButtonOpenFolderDialog_Click;
        }

        private void ButtonOpenFolderDialog_Click(object sender, RoutedEventArgs e)
        {
            //フォルダ指定あり
            string folderPath;
            folderPath = @"C:\Users\waten\Source\Repos\wpf_test2\20191010_TreeView3";
            FolderDialog dialog = new FolderDialog(folderPath, this);

            //フォルダ指定なし
            //dialog = new FolderDialog(this);

            dialog.ShowDialog();
            if (dialog.DialogResult == true)
            {
                TextBlockFullName.Text = dialog.GetFullPath();
            }
            
        }
    }
}

//このアプリのブログ記事
//WPFでフォルダ選択ダイアログできた - 午後わてんのブログ
//https://gogowaten.hatenablog.com/entry/2019/10/13/195315
