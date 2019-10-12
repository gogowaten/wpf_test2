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

namespace _20101010_フォルダダイアログ
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonOpenFolderDialog_Click(object sender, RoutedEventArgs e)
        {
            FolderDialog dialog;
            //FolderDialog dialog = new FolderDialog(new System.IO.DirectoryInfo(@"C:\"));
            dialog = new FolderDialog(new System.IO.DirectoryInfo(@"E:\アプリ\動画キャプチャ\AGデスクトップレコーダー\AGDRec_131F"));
            //dialog = new FolderDialog(new System.IO.DirectoryInfo(@"E:\オレ\携帯\2018\douga"));

            dialog = new FolderDialog();
            dialog.ShowDialog();
            if (dialog.DialogResult == true)
            {
                TextBlockFullName.Text = dialog.GetFullPath();
            }
        }
    }
}
