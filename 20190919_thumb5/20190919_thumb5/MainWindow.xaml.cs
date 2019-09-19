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

//●のThumb

namespace _20190919_thumb5
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        SquareOrEllipseThumb MyThumb;

        public MainWindow()
        {
            InitializeComponent();

            MyButton1.Click += MyButton1_Click;
            MyButton2.Click += MyButton2_Click;
            MyButton3.Click += MyButton3_Click;

            MyThumb = new SquareOrEllipseThumb(Colors.Cyan) { Width = 100, Height = 100 };
            MyCanvas.Children.Add(MyThumb);
            Canvas.SetLeft(MyThumb, 100); Canvas.SetTop(MyThumb, 100);

            MyThumb.DragDelta += Thumb_DragDelta;
        }

        private void MyButton3_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MyButton2_Click(object sender, RoutedEventArgs e)
        {
            MyThumb.ToSquare();
        }

        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            //MyThumb.Width = 20;
            MyThumb.ToEllipse();
        }

        /*親Thumbの背景色、子Thumbの背景色
* 子Thumbの形を■と○切り替え
* 子Thumbの位置を親Thumbの枠の中心にしたい
* 子Thumbに枠の表示の有無、枠の色の指定
* 依存関係プロパティで処理したい

*/



        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb t = (Thumb)sender;
            Canvas.SetLeft(t, Canvas.GetLeft(t) + e.HorizontalChange);
            Canvas.SetTop(t, Canvas.GetTop(t) + e.VerticalChange);
        }
    }

  /// <summary>
  /// 形状を■と●に切り替えられるThumb
  /// ■のときはGridで表現
  /// ●のときはGridにEllipseを乗せて、Gridの背景色をnullで表現
  /// Template
  /// Grid(VisualTree)
  ///   ┗Ellipse
  /// </summary>
    public class SquareOrEllipseThumb : Thumb
    {
        private Grid BaseGrid;//TemplateのベースにするGrid
        private Color BGColor;
        private Ellipse MyEllipse;
        private bool IsSquare;

        public SquareOrEllipseThumb(Color bgColor)
        {
            SetTemplate(bgColor);
        }

        private void SetTemplate(Color bgColor)
        {
            BGColor = bgColor;
            IsSquare = true;

            //template作成
            ControlTemplate template = new ControlTemplate(typeof(Thumb));

            //Templateの見た目の作成
            //任意の名前をつけておくと、これをもとに検索して要素を取得できる
            template.VisualTree = new FrameworkElementFactory(typeof(Grid), "tempCanvas");
            this.Template = template;

            //再構築、これを実行しないと名前検索で取得できない
            this.ApplyTemplate();

            //名前検索で取得
            BaseGrid = (Grid)this.Template.FindName("tempCanvas", this);
            BaseGrid.Background = new SolidColorBrush(bgColor);

            MyEllipse = new Ellipse();
            MyEllipse.Fill = new SolidColorBrush(bgColor);

        }

        //●に変更
        public void ToEllipse()
        {
            if (IsSquare)
            {                
                BaseGrid.Children.Add(MyEllipse);
                BaseGrid.Background = null;
                IsSquare = false;
            }
        }

        //■に変更
        public void ToSquare()
        {
            if (!IsSquare)
            {
                BaseGrid.Children.Remove(MyEllipse);
                BaseGrid.Background = new SolidColorBrush(BGColor);
                IsSquare = true;
            }
        }

    }



}
