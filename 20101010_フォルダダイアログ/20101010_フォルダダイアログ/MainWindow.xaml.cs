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
            string folderPath;
            folderPath = @"E:\";
            folderPath = @"C:\Users\waten\Source\Repos\wpf_test2\20101010_フォルダダイアログ";
            //folderPath = @"E:\オレ\携帯\2018\";

            FolderDialog dialog = new FolderDialog(folderPath, this);
            //dialog = new FolderDialog(this);

            dialog.ShowDialog();
            if (dialog.DialogResult == true)
            {
                TextBlockFullName.Text = dialog.GetFullPath();
            }
            //else { TextBlockFullName.Text = ""; }
        }
    }
}
