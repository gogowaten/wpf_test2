using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Text.RegularExpressions;

/// <summary>
/// int型のNumericUpDownのようなもの
/// 指定できる項目は
/// プロパティ名              内容
/// Myvalue         数値
/// Minimum         最小値
/// Maximum         最大値
/// SmallChange     小変化値
/// LargeChange     大変化値
/// MyWidth         TextBoxの横幅、初期値はdouble.NaNで自動調節
/// MyHeight        全体(StackPanel)の高さ、初期値はdouble.NaNで自動調節
/// MyFontSize      初期値はdouble.NaNで普通サイズ？
/// MyNumberDigits  表示桁数、初期値は4、10 は 0010 と表示する
/// 
/// 追加操作
/// TextBoxの上でマウスホイール回転でSmallChange
/// ScrollBarの上でマウスホイール回転でLargeChange
/// </summary>
namespace _20190924_numeric
{

    class MyNumeric : StackPanel
    {
        private TextBox MyTextBox;
        private ScrollBar MyScrollBar;

        #region dependencyProperty依存関係プロパティ
        public int MyValue
        {
            get => (int)GetValue(MyValueProperty);
            set => SetValue(MyValueProperty, value);
        }
        public static readonly DependencyProperty MyValueProperty =
            DependencyProperty.Register(nameof(MyValue), typeof(int), typeof(MyNumeric), new PropertyMetadata(1));


        public int Minimum
        {
            get => (int)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(MyNumeric), new PropertyMetadata(-10));

        public int Maximum
        {
            get => (int)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(MyNumeric), new PropertyMetadata(100));

        public int SmallChange
        {
            get => (int)GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(nameof(SmallChange), typeof(int), typeof(MyNumeric), new PropertyMetadata(1));
        public int LargeChange
        {
            get => (int)GetValue(LargeChangeProperty);
            set => SetValue(LargeChangeProperty, value);
        }
        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register(nameof(LargeChange), typeof(int), typeof(MyNumeric), new PropertyMetadata(10));

        public double MyWidth
        {
            get => (double)GetValue(MyWidthProperty);
            set => SetValue(MyWidthProperty, value);
        }
        public static readonly DependencyProperty MyWidthProperty =
            DependencyProperty.Register(nameof(MyWidth), typeof(double), typeof(MyNumeric), new PropertyMetadata(double.NaN));

        public double MyHeight
        {
            get => (double)GetValue(MyHeightProperty);
            set => SetValue(MyHeightProperty, value);
        }
        public static readonly DependencyProperty MyHeightProperty =
            DependencyProperty.Register(nameof(MyHeight), typeof(double), typeof(MyNumeric), new PropertyMetadata(double.NaN));

        public double MyFontSize
        {
            get => (double)GetValue(MyFontSizeProperty);
            set => SetValue(MyFontSizeProperty, value);
        }
        public static readonly DependencyProperty MyFontSizeProperty =
            DependencyProperty.Register(nameof(MyFontSize), typeof(double), typeof(MyNumeric), new PropertyMetadata(double.NaN));

        public int MyNumberDigits
        {
            get => (int)GetValue(MyNumberDigitsProperty);
            set => SetValue(MyNumberDigitsProperty, value);
        }
        public static readonly DependencyProperty MyNumberDigitsProperty =
            DependencyProperty.Register(nameof(MyNumberDigits), typeof(int), typeof(MyNumeric), new PropertyMetadata(-4));


        #endregion

        public MyNumeric()
        {
            Orientation = Orientation.Horizontal;
            HorizontalAlignment = HorizontalAlignment.Center;

            this.Loaded += MyNumeric_Loaded;

            MyTextBox = new TextBox
            {
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
            };

            MyScrollBar = new ScrollBar
            {
                RenderTransform = new RotateTransform(180),
                RenderTransformOrigin = new Point(0.5, 0.5),
            };

            this.Children.Add(MyTextBox);
            this.Children.Add(MyScrollBar);



            //Binding設定
            void MyBinding(DependencyProperty source, FrameworkElement targetElement, DependencyProperty target)
            {
                Binding bb = new Binding()
                {
                    Source = this,
                    Path = new PropertyPath(source),
                    Mode = BindingMode.TwoWay,
                };
                targetElement.SetBinding(target, bb);
            }
            MyBinding(MyValueProperty, MyScrollBar, ScrollBar.ValueProperty);
            MyBinding(MaximumProperty, MyScrollBar, ScrollBar.MaximumProperty);
            MyBinding(MinimumProperty, MyScrollBar, ScrollBar.MinimumProperty);
            MyBinding(SmallChangeProperty, MyScrollBar, ScrollBar.SmallChangeProperty);
            MyBinding(LargeChangeProperty, MyScrollBar, ScrollBar.LargeChangeProperty);
            MyBinding(MyWidthProperty, MyTextBox, TextBox.WidthProperty);
            MyBinding(MyHeightProperty, this, StackPanel.HeightProperty);
            MyBinding(MyFontSizeProperty, MyTextBox, TextBox.FontSizeProperty);

            //textboxのBindingはConveterを使うので別に設定
            Binding b = new Binding
            {
                Source = this,
                Path = new PropertyPath(MyValueProperty),
                Mode = BindingMode.TwoWay,
                Converter = new MyConvert(),
                ConverterParameter = this,
            };
            MyTextBox.SetBinding(TextBox.TextProperty, b);

        }

        #region イベント関連
        private void MyNumeric_Loaded(object sender, RoutedEventArgs e)
        {
            MyTextBox.GotFocus += MyTextBox_GotFocus;
            MyTextBox.TextChanged += MyTextBox_TextChanged;
            MyTextBox.MouseWheel += MyTextBox_MouseWheel;
            MyScrollBar.MouseWheel += MyScrollBar_MouseWheel;
        }

        //Scrollbarの上でマウスホイール回転で数値をLargeChange
        private void MyScrollBar_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollBar sb = (ScrollBar)sender;
            if (e.Delta > 0)
            {
                MyScrollBar.Value += this.LargeChange;
            }
            else
            {
                MyScrollBar.Value -= this.LargeChange;
            }
        }

        //TextBoxの上でマウスホイール回転でSmallChange
        private void MyTextBox_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Delta > 0)
            {
                int i = int.Parse(tb.Text) + this.SmallChange;
                if (Maximum < i) i = Maximum;
                MyScrollBar.Value = i;
            }
            else
            {
                int i = int.Parse(tb.Text) - this.SmallChange;
                if (this.Minimum > i) i = Minimum;
                MyScrollBar.Value = i;
            }
        }

        //のぶろぐ[WPF] テキストボックスに数字以外は受け付けない簡単な方法
        //http://shen7113.blog.fc2.com/blog-entry-22.html
        //正規表現で数値以外は削除using System.Text.RegularExpressions;
        private void MyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            double d;
            if (!double.TryParse(box.Text, out d))
            {
                box.Text = Regex.Replace(box.Text, "[^0-9-]", "");
            }
        }

        //[WPF] TEXTBOX でフォーカス時にマウスクリックでもテキストを全選択する v2 | rksoftware
        //https://rksoftware.wordpress.com/2016/09/06/001-48/
        //[WPF] TextBox でフォーカス時にマウスクリックでもテキストを全選択する | rksoftware
        //https://rksoftware.wordpress.com/2016/06/17/001-38/
        //TextBoxフォーカス時にテキスト全選択
        private void MyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            //box.SelectAll();
            this.Dispatcher.InvokeAsync(() =>
            {
                Task.Delay(10); box.SelectAll();
            });
        }
        #endregion



    }


    //TextBoxの文字列値と数値のBindingに必要なConverter
    //書式設定と最大値、最小値を超えていないかチェックもする
    public class MyConvert : IValueConverter
    {
        /// <summary>
        /// intのvalueをstringに変換、指定桁数分を0で埋めた書式で返す
        /// parameterにMyNumericを入れる
        /// 桁数はMyNumeric.MyDegitsにあるint、これを使ってstring.Formatの書式作成
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">MyNumeric</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //
            MyNumeric n = (MyNumeric)parameter;
            string d = Math.Abs(n.MyNumberDigits).ToString();//指定桁数を絶対値で取得して文字列に変換
            string digit = "{0:d" + d + "}";//書式作成
            int i = (int)value;
            string str = string.Format(digit, i);
            //neko = string.Format("{0:d4}", 1);//0001になる
            return str;
        }

        /// <summary>
        /// stringをintに変換して返す
        /// intに変換したあとは最大値、最小値を超えていないかチェック
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">MyNumeric</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            MyNumeric n = (MyNumeric)parameter;
            string str = (string)value;
            int i = int.Parse(str);
            if (i > n.Maximum) i = n.Maximum;
            else if (i < n.Minimum) i = n.Minimum;
            return i;
        }
    }
}
