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
using System.Windows.Controls.Primitives;

//カーソルの形を矢印に変えてサイズ変化

namespace _20190919_thumb6
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        FlatThumb MyThumb;
        public MainWindow()
        {
            InitializeComponent();

            MyThumb = new FlatThumb(new Size(100, 100), new Point(100, 100), Colors.Cyan);
            MyCanvas.Children.Add(MyThumb);
            MyThumb.DragDelta += MyThumb_DragDelta;
        }

        private void MyThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(MyThumb, Canvas.GetLeft(MyThumb) + e.HorizontalChange);
            Canvas.SetTop(MyThumb, Canvas.GetTop(MyThumb) + e.VerticalChange);

        }
    }

    public class FlatThumb : Thumb
    {
        private Grid MyBaseGrid;
        public FlatThumb(Size size,Point localte,Color bgColor)
        {
            this.Width = size.Width;
            this.Height = size.Height;
            Canvas.SetLeft(this, localte.X);
            Canvas.SetTop(this, localte.Y);
            SetTemplate(bgColor);

        }
        private void SetTemplate(Color bgColor)
        {
            var template = new ControlTemplate(typeof(Thumb));
            template.VisualTree = new FrameworkElementFactory(typeof(Grid), "templatePanel");
            this.Template = template;
            this.ApplyTemplate();
            MyBaseGrid = (Grid)template.FindName("templatePanel", this);
            MyBaseGrid.Background = new SolidColorBrush(bgColor);

        }
    }
}
