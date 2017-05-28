using System;
using System.Windows;
using System.Windows.Controls;

namespace Software_Interface_v2 {
    public partial class NumControl : UserControl {
        private string _prevText;
        public event EventHandler NumericValueChanged;

        
        
        public NumControl() {
            MaxValue = 20;
            MinValue = -10;
            NumValue = 3;
            InitializeComponent();
            Loaded += (sender, args) => {
                NumericSlider.Minimum = MinValue;
                NumericSlider.Maximum = MaxValue;
                NumericSlider.Value = NumValue;
                TextBoxNum.Text = NumValue.ToString();
            };
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            NumValue = (int) NumericSlider.Value;
            SetTextBox(NumValue.ToString());
        }

        private void ButtUpClick(object sender, RoutedEventArgs e) {
            if (NumValue == MaxValue)
            {
                return;
            }
            NumValue += 1;
            SetTextBox(NumValue.ToString());
            NumericSlider.Value = NumValue;
        }

        private void ButtDownClick(object sender, RoutedEventArgs e) {
            if (NumValue == MinValue)
            {
                return;
            }
            NumValue -= 1;
            SetTextBox(NumValue.ToString());
            NumericSlider.Value = NumValue;
        }
        
        
        private void OnTextChanged(object sender, TextChangedEventArgs e) {
            
        }

        private void SetTextBox(string text) {
            TextBoxNum.TextChanged -= OnTextChanged;
            TextBoxNum.Text = text;
            TextBoxNum.TextChanged += OnTextChanged;
        }
        
        
        
        private void TextBoxNum_OnLostFocus(object sender, RoutedEventArgs e) {
            int toSet;
            if (Int32.TryParse(TextBoxNum.Text, out toSet))
            {
                NumValue = toSet;
                SetTextBox(NumValue.ToString());
                NumericSlider.Value = NumValue;
            }
            else
            {
                SetTextBox(_prevText);
            }
        }

        private void TextBoxNum_OnGotFocus(object sender, RoutedEventArgs e) {
            _prevText = TextBoxNum.Text;
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
                NumericValueChanged?.Invoke(this, EventArgs.Empty);
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
