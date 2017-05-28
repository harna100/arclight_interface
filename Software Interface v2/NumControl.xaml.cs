using System.Windows;
using System.Windows.Controls;

namespace Software_Interface_v2 {
    public partial class NumControl : UserControl {
        
        
        
        
        public NumControl() {
            MaxValue = 20;
            MinValue = -10;
            NumValue = 3;
            InitializeComponent();
            Loaded += (sender, args) => {
                NumericSlider.Minimum = MinValue;
                NumericSlider.Maximum = MaxValue;
                NumericSlider.Value = NumValue;
            };
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
        }

        private void ButtUpClick(object sender, RoutedEventArgs e) {
        }

        private void ButtDownClick(object sender, RoutedEventArgs e) {
        }


        public static readonly DependencyProperty NumValueProperty = DependencyProperty.Register(
            "NumValue", typeof(int), typeof(NumControl), new PropertyMetadata(default(int)));

        public int NumValue {
            get { return (int) GetValue(NumValueProperty); }
            set {
                if (value > MaxValue)
                {
                    SetValue(NumValueProperty, MaxValue);
                }
                else if (value < MinValue)
                {
                    SetValue(NumValueProperty, MinValue);
                }
                else
                {
                    SetValue(NumValueProperty, value);
                }
            }
        }


        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(int), typeof(NumControl), new PropertyMetadata(default(int)));

        public int MaxValue {
            get { return (int) GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof(int), typeof(NumControl), new PropertyMetadata(default(int)));

        public int MinValue {
            get { return (int) GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        
        
    }
}
