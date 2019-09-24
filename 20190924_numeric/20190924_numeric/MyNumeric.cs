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

namespace _20190924_numeric
{

    class MyNumeric : StackPanel
    {
        //private StackPanel MyStackPanel;
        private TextBox MyTextBox;
        private ScrollBar MyScrollBar;

        #region dependencyProperty
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

        public int Maximun
        {
            get => (int)GetValue(MaximunProperty);
            set => SetValue(MaximunProperty, value);
        }
        public static readonly DependencyProperty MaximunProperty =
            DependencyProperty.Register(nameof(Maximun), typeof(int), typeof(MyNumeric), new PropertyMetadata(100));

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
            DependencyProperty.Register(nameof(LargeChange), typeof(int), typeof(MyNumeric), new PropertyMetadata(1));

        public double MyWidth
        {
            get => (double)GetValue(MyWidthProperty);
            set => SetValue(MyWidthProperty, value);
        }
        public static readonly DependencyProperty MyWidthProperty =
            DependencyProperty.Register(nameof(MyWidth), typeof(double), typeof(MyNumeric), new PropertyMetadata(30.0));

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


        #endregion

        public MyNumeric()
        {
            Orientation = Orientation.Horizontal;
            HorizontalAlignment = HorizontalAlignment.Center;


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
            
            

            Binding b = new Binding
            {
                Source = this,
                Path = new PropertyPath(MyValueProperty),
                Mode = BindingMode.TwoWay
            };
            MyTextBox.SetBinding(TextBox.TextProperty, b);
            MyScrollBar.SetBinding(ScrollBar.ValueProperty, b);



            MyBinding(MaximunProperty, MyScrollBar, ScrollBar.MaximumProperty);
            MyBinding(MinimumProperty, MyScrollBar, ScrollBar.MinimumProperty);
            MyBinding(SmallChangeProperty, MyScrollBar, ScrollBar.SmallChangeProperty);
            MyBinding(LargeChangeProperty, MyScrollBar, ScrollBar.LargeChangeProperty);
            MyBinding(MyWidthProperty, MyTextBox, TextBox.WidthProperty);
            //MyBinding(MyHeightProperty, MyTextBox, TextBox.HeightProperty);
            MyBinding(MyHeightProperty, this, StackPanel.HeightProperty);
            MyBinding(MyFontSizeProperty, MyTextBox, TextBox.FontSizeProperty);

            Binding bb = new Binding()
            {
                Source = this,
                Path = new PropertyPath(MyValueProperty),
                Mode = BindingMode.TwoWay,
                Converter = new MyConvert(),
            };
            MyTextBox.SetBinding(TextBox.TextProperty, bb);

        }

        private void MyBinding(DependencyProperty source,FrameworkElement targetElement,DependencyProperty target)
        {
            Binding b = new Binding()
            {
                Source = this,
                Path = new PropertyPath(source),
                Mode = BindingMode.TwoWay,
            };
            targetElement.SetBinding(target, b);


        }


    }

    public class MyConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = (int)value;
            var str = string.Format("000", i);
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
