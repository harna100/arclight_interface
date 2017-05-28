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

namespace Software_Interface_v2 {
	/// <summary>
	/// Interaction logic for NumericUpDown.xaml
	/// </summary>
	public partial class NumericUpDown:UserControl {

		private int _numValue;
		private int _maxValue;
		private int _minValue;

		public event EventHandler NumericValueChanged;

		public NumericUpDown() {
			InitializeComponent();
			Loaded += (sender, args) => {
				NumericSlider.Maximum = MaxValue;
				NumericSlider.Minimum = MinValue;
				NumericSlider.Value = NumValue;
				txtNum.Text = NumValue.ToString();

			};
//			SetValue(MaxValueProperty, Int32.MaxValue);
//			SetValue(MinValueProperty, Int32.MinValue);
//			_maxValue = 255;
//			_minValue = 0;
//			_numValue = _maxValue;
//			NumericSlider.Maximum = _maxValue;
//			NumericSlider.Minimum = _minValue;
//			NumericSlider.Value = _numValue;
//			txtNum.Text = _numValue.ToString();

		}


		private void cmdUp_Click(object sender, RoutedEventArgs e) {
			if (NumValue == MaxValue)
			{
				return;
			}
			NumericSlider.Value += 1;
			txtNum.Text = NumericSlider.Value.ToString();
//			SetValue(NumValueProperty, (int)GetValue(NumValueProperty)+1);
			++NumValue;
		}

		private void cmdDown_Click(object sender, RoutedEventArgs e) {
			if (NumValue == MinValue)
			{
				return;
			}
			NumericSlider.Value -= 1;
			txtNum.Text = NumericSlider.Value.ToString();
//			SetValue(NumValueProperty, (int)GetValue(NumValueProperty)- 1);
			--NumValue;
		}

		private void txtNum_TextChanged(object sender, TextChangedEventArgs e) {
			if(txtNum == null) {
				return;
			}

			int toSet;
			if (int.TryParse(txtNum.Text, out toSet))
			{
				NumValue = toSet;
				e.Handled = true;
//				txtNum.Text = NumValue.ToString();
				NumericSlider.Value = NumValue;
//				if (toSet < MinValue)
//				{
//					SetValue(NumValueProperty, MinValue);
//				}
//				else if (toSet > MaxValue)
//				{
//					SetValue(NumValueProperty, MaxValue);
//				}
//				else
//				{
//					SetValue(NumValueProperty, toSet);
//				}
//				SetValue(TextBoxValueProperty, NumValue.ToString());
			}

		}

		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
			"MaxValue", typeof(int), typeof(NumericUpDown), new PropertyMetadata(default(int)));

		public int MaxValue {
			get { return (int) GetValue(MaxValueProperty); }
			set {
				NumericSlider.Maximum = value;
//				SetValue(MaxValueProperty, value);
				_maxValue = value;
			}
		}

		public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
			"MinValue", typeof(int), typeof(NumericUpDown), new PropertyMetadata(default(int)));

		public int MinValue {
			get { return (int) GetValue(MinValueProperty); }
			set {
				NumericSlider.Minimum = value;
//				SetValue(MinValueProperty, value);
				_minValue = value;
			}
		}

		public static readonly DependencyProperty NumValueProperty = DependencyProperty.Register(
			"NumValue", typeof(int), typeof(NumericUpDown), new PropertyMetadata(default(int)));

		public int NumValue {
			get { return (int) GetValue(NumValueProperty); }
			set {
				if (value > _maxValue)
				{
//					SetValue(NumValueProperty, MaxValue);
					_numValue = _maxValue;
				}
				else if (value < _minValue)
				{
//					SetValue(NumValueProperty, MinValue);
					_numValue = _minValue;
				}
				else
				{
//					SetValue(NumValueProerty, value);
					_numValue = value;
				}
//				TextBoxValue = NumValue.ToString();
//				NumericSlider.Value = NumValue;
				txtNum.Text = _numValue.ToString();
//				SetValue(TextBoxValueProperty, NumValue.ToString());
				NumericValueChanged?.Invoke(this, EventArgs.Empty);
			}
		}


/*		public static readonly DependencyProperty TextBoxValueProperty = DependencyProperty.Register(
			"TextBoxValue", typeof(string), typeof(NumericUpDown), new PropertyMetadata(default(string)));

		public string TextBoxValue {
			get { return GetValue(TextBoxValueProperty).ToString(); }
			set {
				int toCheck = Int32.Parse(value);
				if (toCheck > MaxValue)
				{
					SetValue(TextBoxValueProperty, MaxValue.ToString());
				}
				else if (toCheck < MinValue)
				{
					SetValue(TextBoxValueProperty, MinValue.ToString());
				}
				else
				{
					SetValue(TextBoxValueProperty, toCheck.ToString());
				}
				txtNum.Text = TextBoxValue;

			}
		}*/


		private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			e.Handled = true;
			NumValue = (int) NumericSlider.Value;
			txtNum.TextChanged -= txtNum_TextChanged;
			txtNum.Text = NumericSlider.Value.ToString();
			txtNum.TextChanged += txtNum_TextChanged;
//			SetValue(NumValueProperty, (int) NumericSlider.Value);
		}

		private void OnPreviewTextInput(object sender, TextCompositionEventArgs e) {
			int temp;
			if (!Int32.TryParse(e.Text, out temp))
			{
				e.Handled = true;
			}
		}
	}
}
