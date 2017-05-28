using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
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
using MaterialDesignThemes.Wpf;
using System.Management;

namespace Software_Interface_v2 {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow:Window {
		private SerialPort serialPort;


		public MainWindow() {
			InitializeComponent();
			this.Closing += (sender, args) => {
				try
				{
					serialPort.Close();
				}
				catch (Exception e)
				{

				}

			};

			string portName = AutodetectArduinoPort();
			if (portName != null)
			{
				serialPort = new SerialPort(portName, 115200);
				try
				{
					serialPort.Open();
					WriteToDebug("Found arduino on " + serialPort.PortName);
				}
				catch (Exception e)
				{
					WriteToDebug(e.ToString());
				}

			}
			else
			{
				WriteToDebug("Could not auto detect port");
//				ListBoxComPorts.ItemsSource = GetAvailablePorts();
//				DialogBox.ShowDialog(null);
			}
//			PrimaryAlpha.NumValue = 255;
//			SecondaryAlpha.NumValue = 255;
//			Width.MaxValue = 60;
//			Width.NumValue = 0;
//			SecondaryWhite.MaxValue = 27000;
//			SecondaryWhite.NumValue = 6500;
//			SecondaryWhite.MinValue = 1700;
//			PrimaryWhite.MaxValue = 27000;
//			PrimaryWhite.NumValue = 6500;
//			PrimaryWhite.MinValue = 1700;
		}

		private void SetColorClicked(object sender, RoutedEventArgs e) {
			SendValueToArduino();
		}

		private void WriteToDebug(string valToWrite) {
			TextBoxOutput.Text += DateTime.Now.ToLongTimeString() + ": " + valToWrite + "\n";
		}

		private void SendValueToArduino() {
			byte[][] valuesToSend = GetValues();
		}

		private byte[][] GetValues() {
			byte[][] toReturn = new byte[2][];
			toReturn[0] = GetValueFromPrimary();
			toReturn[1] = GetValueFromSecondary();

			return toReturn;
		}

		private byte[] GetValueFromPrimary() {
			byte[] toReturn = null;
			switch (TabcontrolPrimary.SelectedIndex)
			{
					case 0:
						toReturn = GetColorPrimaryRGBSlider();
						break;
					case 1:
						toReturn = GetColorPrimaryWhiteTemp();
						break;
					case 2:
						toReturn = GetColorPrimaryColorPicker();
						break;
					case 3:
						toReturn = GetColorPrimaryModWheel();
						break;
			}

			return toReturn;
		}

		private byte[] GetValueFromSecondary() {
			byte[] toReturn = null;
			switch (TabcontrolSecondary.SelectedIndex)
			{
				case 0:
					toReturn = GetColorSecondaryRGBSlider();
					break;
				case 1:
					toReturn = GetColorSecondaryWhiteTemp();
					break;
				case 2:
					toReturn = GetColorSecondaryColorPicker();
					break;
				case 3:
					toReturn = GetColorSecondaryModWheel();
					break;
			}

			return toReturn;
		}


		private void SecondaryNumericChanged(object sender, EventArgs e) {
			UpdateSecondaryRGBColor();
		}

		private void PrimaryNumericChanged(object sender, EventArgs e) {
//			MessageBox.Show("sfgsdfg");
			UpdateRGBColor();
		}

		//updates the color on the RGBA slider page
		public void UpdateRGBColor() {
			byte[] rgba = GetColorPrimaryRGBSlider();
			Color c = Color.FromArgb(rgba[3], rgba[0], rgba[1], rgba[2]);
			PrimaryRGBImage.Background = new SolidColorBrush(c);

		}

		// gets a RGBA from the sliders on the RGBA slider page
		public byte[] GetColorPrimaryRGBSlider() {
			byte red = (byte) PrimaryRed.NumValue;
			byte green = (byte) PrimaryGreen.NumValue;
			byte blue = (byte) PrimaryBlue.NumValue;
			byte bright = (byte) PrimaryAlpha.NumValue;
			return new[] {red, green, blue, bright};
		}

		public void UpdateSecondaryRGBColor() {
			byte[] rgba = GetColorSecondaryRGBSlider();
			Color c = Color.FromArgb(rgba[3], rgba[0], rgba[1], rgba[2]);
			SecondaryRGBImage.Background = new SolidColorBrush(c);
		}

		// gets a RGBA from the sliders on the RGBA slider page
		public byte[] GetColorSecondaryRGBSlider() {
			byte red = (byte) SecondaryRed.NumValue;
			byte green = (byte) SecondaryGreen.NumValue;
			byte blue = (byte) SecondaryBlue.NumValue;
			byte bright = (byte) SecondaryAlpha.NumValue;
			return new[] {red, green, blue, bright};
		}


		private byte[] GetColorPrimaryWhiteTemp() {
			return GetColorWhiteTemp(PrimaryWhite);
		}

		private byte[] GetColorSecondaryWhiteTemp() {
			return GetColorWhiteTemp(SecondaryWhite);
		}


		// get a RGBA from the white temp page
		// algorithm from http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/
		private byte[] GetColorWhiteTemp(NumericUpDown toUse) {
			int temp = toUse.NumValue/ 100;
			double red, green, blue;
			// calc red
			if(temp <= 66) {
				red = 255;
			}
			else {
				red = temp - 60;
				red = 329.698727446 * Math.Pow(red, -0.1332047592);
				if(red < 0)
					red = 0;
				if(red > 255)
					red = 255;
			}

			//calc green
			if(temp <= 66) {
				green = temp;
				green = 99.4708025861 * Math.Log(green) - 161.1195681661;
				if(green < 0)
					green = 0;
				if(green > 255)
					green = 255;
			}
			else {
				green = temp - 60;
				green = 288.1221695283 * Math.Pow(green, -0.0755148492);
				if(green < 0)
					green = 0;
				if(green > 255)
					green = 255;
			}
			//calc blue
			if (temp >= 66)
			{
				blue = 255;
			}
			else {
				if (temp <= 19)
				{
					blue = 0;
				}
				else
				{
					blue = temp - 10;
					blue = 138.5177312231 * Math.Log(blue) - 305.0447927307;
					if(blue < 0)
						blue = 0;
					if(blue > 255)
						blue = 255;
				}
			}
			return new byte[] {(byte) red, (byte) green, (byte) blue, 255};
		}

		private byte[] GetColorPrimaryColorPicker() {
			return GetColorPicker(PrimaryColorPicker);
		}

		private byte[] GetColorSecondaryColorPicker() {
			return GetColorPicker(SecondaryColorPicker);
		}


		private byte[] GetColorPicker(ColorBox.ColorBox toUse) {
			Color picked = toUse.Color;
			byte r = picked.R;
			byte g = picked.G;
			byte b = picked.B;
			byte a = picked.A;
			return new[] {r, g, b, a};
		}

		private byte[] GetColorPrimaryModWheel() {
			return null;
		}

		private byte[] GetColorSecondaryModWheel() {
			return null;
		}






		public string PadZereos(int toPad) {
			string toReturn = toPad.ToString().PadLeft(3, '0');
			return toReturn;
		}

		public string JoinArray(int[] arr) {
			string toReturn = "";
			foreach (int i in arr)
			{
				toReturn += PadZereos(i);
				toReturn += ",";
			}
			return toReturn;
		}




		public string AutodetectArduinoPort() {
			ManagementScope connectionScope = new ManagementScope();
			SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

			try {
				foreach(ManagementObject item in searcher.Get()) {
					string desc = item["Description"].ToString();
					string deviceId = item["DeviceID"].ToString();

					if(desc.Contains("Arduino")) {
						return deviceId;
					}
				}
			}
			catch(ManagementException e) {
				/* Do Nothing */
			}

			return null;
		}



		private List<string> GetAvailablePorts() {
			ManagementScope connectionScope = new ManagementScope();
			SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);
			List<string> toReturn = new List<string>();
			try {
				foreach(ManagementObject item in searcher.Get()) {
					string desc = item["Description"].ToString();
					string deviceId = item["DeviceID"].ToString();
					toReturn.Add(deviceId);

				}
			}
			catch(ManagementException e) {
				/* Do Nothing */
			}

			return toReturn;

		}



		public T MaxOrLess<T>(T toCheck, T max) where T:IComparable{
			return toCheck.CompareTo(max) < 0 ? toCheck : max;
		}

		/*
		private void ListBoxComPorts_OnMouseUp(object sender, MouseButtonEventArgs e) {
			string selectedPort = ListBoxComPorts.SelectedItem as string;

			try
			{
				serialPort = new SerialPort(selectedPort, 115200);
				serialPort.Open();
			}
			catch (Exception e2)
			{
				WriteToDebug(e2.ToString());
			}
		}*/

		
		
		
		
		
		
		/*


		public int[] GetColorModWheel() {
			decimal modRed = numModifierRed.Value;
			decimal modGreen = numModifierGreen.Value;
			decimal modBlue = numModifierBlue.Value;
			decimal a = numModifier.Value;

			int valR = MaxOrLess((int)(a * modRed), 255);
			int valG = MaxOrLess((int)(a * modGreen), 255);
			int valB = MaxOrLess((int)(a * modBlue), 255);

			return new[] {valR, valG, valB, 255};
		}


		// Arduino Stuff
		public void WriteToArduino() {
			int[] rgba = GetColor();
			int[] ambWidth;
			if (tabControl1.SelectedIndex == 3)
				ambWidth = GetAmbCurve();
			else
				ambWidth = GetAmbWidth();
			if (!serialPort1.IsOpen)
			{
				try
				{
//					string toSend = "$" + string.Join(",",rgba);
					string toSend = "$";
					toSend += JoinArray(rgba);
//					toSend += JoinArray(ambWidth).Substring(0,JoinArray(ambWidth).Length-1)+ "*";
					toSend += JoinArray(ambWidth) + numWidth.Value + "*";
					if (true)
					{
						WriteToDebugWindow("About to Open port");
						serialPort1.Open();
						WriteToDebugWindow("Port opened. About to write.");
						serialPort1.Write(toSend);
						WriteToDebugWindow("Wrote to Serial. About to close.");
						serialPort1.Close();
						WriteToDebugWindow("Port closed");
					}
					else
					{
						WriteToDebugWindow(toSend);
					}
				}
				catch (Exception e)
				{
					WriteToDebugWindow("Failed to send info to arduino. Show this to maintainer: " + e.ToString());
				}
				serialPort1.Close();
			}
		}*/

	}

	public struct ArcConstants {
		public static string RGB_SLIDER = "rgbSlider";
		public static string WHITE_TEMP = "whiteTemp";
		public static string WHITE_TEMP_VALUE = "whiteTempValue";
		public static string COLOR_PICKER = "colorPicker";
		public static string COLOR_PICK_CUSTOMS = "ColorPickCustoms";
		public static string MOD_WHEEL = "modWheel";
		public static string MOD_WHEEL_AMB = "modWheelAmb";
		public static string MOD_WHEEL_VALUES = "modWheelValues";
		public static string MOD_WHEEL_AMBVALUES = "modWheelAmbValues";
		public static string AMB_WIDTH = "ambWidth";

	}
}
