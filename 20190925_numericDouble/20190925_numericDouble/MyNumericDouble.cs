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
/// double型のNumericUpDownのようなもの
/// 指定できる項目は
/// プロパティ名              内容
/// Myvalue         数値(double型)
/// Minimum         最小値
/// Maximum         最大値
/// SmallChange     小変化値
/// LargeChange     大変化値
/// MyWidth         TextBoxの横幅、初期値はdouble.NaNで自動調節
/// MyHeight        全体(StackPanel)の高さ、初期値はdouble.NaNで自動調節
/// MyFontSize      初期値はdouble.NaNで普通サイズ？
/// MyDigitsInteger  整数表示桁数、初期値は4
/// MyDigitsDecimal  小数点以下表示桁数、初期値は0
/// 
/// 値が 12.34 のとき
/// MyDigitsInteger = 4, MyDigitsDecimal = 0    0012
/// MyDigitsInteger = 4, MyDigitsDecimal = 2    0012.34
/// MyDigitsInteger = 1, MyDigitsDecimal = 2    12.34
/// MyDigitsInteger = 1, MyDigitsDecimal = 4    12.3400
/// MyDigitsInteger = 4, MyDigitsDecimal = 0    0012
/// MyDigitsInteger = 4, MyDigitsDecimal = 0    0012
/// 
/// 桁数    int型    double型
/// 1       12       12.3
/// 2       12       12.34
/// 3       012      12.345
/// 4       0012     12.3456
/// 
/// 追加操作
/// TextBoxの上でマウスホイール回転でSmallChange
/// ScrollBarの上でマウスホイール回転でLargeChange
/// </summary>

namespace _20190925_numericDouble
{
    class MyNumericDouble : StackPanel
    {

        private TextBox MyTextBox;
        private ScrollBar MyScrollBar;

        #region dependencyProperty依存関係プロパティ
        public double MyValue
        {
            get => (double)GetValue(MyValueProperty);
            set => SetValue(MyValueProperty, value);
        }
        public static readonly DependencyProperty MyValueProperty =
            DependencyProperty.Register(nameof(MyValue), typeof(double), typeof(MyNumericDouble), new PropertyMetadata(1.0));


        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(MyNumericDouble), new PropertyMetadata(-10.0));

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(MyNumericDouble), new PropertyMetadata(100.0));

        public double SmallChange
        {
            get => (double)GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(nameof(SmallChange), typeof(double), typeof(MyNumericDouble), new PropertyMetadata(1.0));
        public double LargeChange
        {
            get => (double)GetValue(LargeChangeProperty);
            set => SetValue(LargeChangeProperty, value);
        }
        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register(nameof(LargeChange), typeof(double), typeof(MyNumericDouble), new PropertyMetadata(10.0));

        public double MyWidth
        {
            get => (double)GetValue(MyWidthProperty);
            set => SetValue(MyWidthProperty, value);
        }
        public static readonly DependencyProperty MyWidthProperty =
            DependencyProperty.Register(nameof(MyWidth), typeof(double), typeof(MyNumericDouble), new PropertyMetadata(double.NaN));

        public double MyHeight
        {
            get => (double)GetValue(MyHeightProperty);
            set => SetValue(MyHeightProperty, value);
        }
        public static readonly DependencyProperty MyHeightProperty =
            DependencyProperty.Register(nameof(MyHeight), typeof(double), typeof(MyNumericDouble), new PropertyMetadata(double.NaN));

        public double MyFontSize
        {
            get => (double)GetValue(MyFontSizeProperty);
            set => SetValue(MyFontSizeProperty, value);
        }
        public static readonly DependencyProperty MyFontSizeProperty =
            DependencyProperty.Register(nameof(MyFontSize), typeof(double), typeof(MyNumericDouble), new PropertyMetadata(double.NaN));

        //整数部分の0埋め桁数
        public int MyDigitsInteger
        {
            get => (int)GetValue(MyDigitsIntegerProperty);
            set => SetValue(MyDigitsIntegerProperty, value);
        }
        public static readonly DependencyProperty MyDigitsIntegerProperty =
            DependencyProperty.Register(nameof(MyDigitsInteger), typeof(int), typeof(MyNumericDouble),
                new PropertyMetadata(4, MyNumberTypePropertyChanged));

        //小数点以下部分の表示(0埋め)桁数
        public int MyDigitsDecimal
        {
            get => (int)GetValue(MyDigitsDecimalProperty);
            set => SetValue(MyDigitsDecimalProperty, value);
        }
        public static readonly DependencyProperty MyDigitsDecimalProperty =
            DependencyProperty.Register(nameof(MyDigitsDecimal), typeof(int), typeof(MyNumericDouble),
                new PropertyMetadata(0, MyNumberTypePropertyChanged));

        //表示桁数変更時に表示を更新
        private static void MyNumberTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nu = (MyNumericDouble)d;

            string str = "0";
            for (int i = 1; i < nu.MyDigitsInteger; i++)
            {
                str += "0";
            }
            str += ".";
            for (int i = 0; i < nu.MyDigitsDecimal; i++)
            {
                str += "0";
            }
            
            nu.MyTextBox.Text = nu.MyValue.ToString(str);
        }

        #endregion

        public MyNumericDouble()
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
                double i = double.Parse(tb.Text) + this.SmallChange;
                if (Maximum < i) i = Maximum;
                MyScrollBar.Value = i;
            }
            else
            {
                double i = double.Parse(tb.Text) - this.SmallChange;
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
        /// doubleのvalueをstringに変換、指定桁数分を0で埋めた書式で返す
        /// parameterにMyNumericDoubleを入れる
        /// ToStringの書式作成
        /// 整数部分と小数点以下部分を指定された桁ぶんを表示、溢れた部分は0埋めか切り捨てになる
        /// 整数桁3、少数桁4なら書式は"000.0000"で
        /// 3.14の表示は 003.1400 になる
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">MyNumericDouble</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = (MyNumericDouble)parameter;
            string str="0";//書式作成
            for (int i = 1; i < param.MyDigitsInteger; i++)
            {
                str += "0";
            }
            str += ".";
            for (int i = 0; i < param.MyDigitsDecimal; i++)
            {
                str += "0";
            }
            double v = (double)value;
            if (v > param.Maximum) v = param.Maximum;
            else if (v < param.Minimum) v = param.Minimum;
            str = v.ToString(str);
            return str;
        }

        /// <summary>
        /// stringをdoubleに変換して返す
        /// doubleに変換したあとは最大値、最小値を超えていないかチェック
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">MyNumericDouble</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MyNumericDouble param = (MyNumericDouble)parameter;
            string str = (string)value;
            double v = double.Parse(str);
            if (v > param.Maximum) v = param.Maximum;
            else if (v < param.Minimum) v = param.Minimum;
            return v;
        }
    }
}
