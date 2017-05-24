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
			_maxValue = 255;
			_minValue = 0;
			_numValue = _maxValue;
			NumericSlider.Maximum = _maxValue;
			NumericSlider.Minimum = _minValue;
			NumericSlider.Value = _numValue;
			txtNum.Text = _numValue.ToString();

		}


		private void cmdUp_Click(object sender, RoutedEventArgs e) {
			if (NumValue == MaxValue)
			{
				return;
			}
			++NumValue;
		}

		private void cmdDown_Click(object sender, RoutedEventArgs e) {
			if (NumValue == MinValue)
			{
				return;
			}
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
			}

		}

		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
			"MaxValue", typeof(int), typeof(NumericUpDown), new PropertyMetadata(default(int)));

		public int MaxValue {
			get { return (int) GetValue(MaxValueProperty); }
			set {
				SetValue(MaxValueProperty, value);
				NumericSlider.Maximum = value;
			}
		}

		public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
			"MinValue", typeof(int), typeof(NumericUpDown), new PropertyMetadata(default(int)));

		public int MinValue {
			get { return (int) GetValue(MinValueProperty); }
			set {
				SetValue(MinValueProperty, value);
				NumericSlider.Minimum = value;
			}
		}

		public static readonly DependencyProperty NumValueProperty = DependencyProperty.Register(
			"NumValue", typeof(int), typeof(NumericUpDown), new PropertyMetadata(default(int)));

		public int NumValue {
			get { return (int) GetValue(NumValueProperty); }
			set {
				if (value > _maxValue)
				{
					SetValue(NumValueProperty, MaxValue);
				}
				else if (value < _minValue)
				{
					SetValue(NumValueProperty, MinValue);
				}
				else
				{
					SetValue(NumValueProperty, value);
				}
				TextBoxValue = NumValue.ToString();
				NumericSlider.Value = NumValue;
				NumericValueChanged?.Invoke(this, EventArgs.Empty);}
		}


		public static readonly DependencyProperty TextBoxValueProperty = DependencyProperty.Register(
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
		}


		private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			NumValue = (int) NumericSlider.Value;
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
