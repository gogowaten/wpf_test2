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