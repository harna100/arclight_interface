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
				serialPort = new SerialPort();
				WriteToDebug("Could not auto detect port");
//				ListBoxComPorts.ItemsSource = GetAvailablePorts();
//				DialogBox.ShowDialog(null);
			}
			serialPort.DataReceived += (sender2, args) => {
				string post = serialPort.ReadExisting();
				WriteToDebug(post);

			};
		}
		

		private void SetColorClicked(object sender, RoutedEventArgs e) {
			SendValueToArduino();
		}

		private void WriteToDebug(string valToWrite) {
			TextBoxOutput.Text += DateTime.Now.ToLongTimeString() + ": " + valToWrite + "\n";
		}

		private void SendValueToArduino() {
			if (!serialPort.IsOpen)
			{
				WriteToDebug("Serial port not open. Unable to send.");
				return;
			}
			byte[][] valuesToSend = GetValues();
			try
			{
				string toSend = "$";
				toSend += JoinArray(valuesToSend[0]);
				toSend += JoinArray(valuesToSend[1]) + Width.NumValue + "*";
				WriteToDebug("About to write.");
				serialPort.Write(toSend);
				WriteToDebug("Wrote to Serial.");
			}
			catch (Exception e)
			{
				WriteToDebug("Failed to send. " + e.Message);
			}
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
			UpdatePrimaryRGBColor();
		}
		
		//updates the color on the RGBA slider page
		public void UpdatePrimaryRGBColor() {
			byte[] rgba = GetColorPrimaryRGBSlider();
			UpdateImage(PrimaryRGBImage, rgba);
		}

		public void UpdateSecondaryRGBColor() {
			byte[] rgba = GetColorSecondaryRGBSlider();
			UpdateImage(SecondaryRGBImage, rgba);
		}

		// gets a RGBA from the sliders on the Primary RGBA slider page
		public byte[] GetColorPrimaryRGBSlider() {
			byte red = (byte) PrimaryRed.NumValue;
			byte green = (byte) PrimaryGreen.NumValue;
			byte blue = (byte) PrimaryBlue.NumValue;
			byte bright = (byte) PrimaryAlpha.NumValue;
			return new[] {red, green, blue, bright};
		}
		
		// gets a RGBA from the sliders on the Secondary RGBA slider page
		public byte[] GetColorSecondaryRGBSlider() {
			byte red = (byte) SecondaryRed.NumValue;
			byte green = (byte) SecondaryGreen.NumValue;
			byte blue = (byte) SecondaryBlue.NumValue;
			byte bright = (byte) SecondaryAlpha.NumValue;
			return new[] {red, green, blue, bright};
		}
		
		
		
		
		private void PrimaryWhiteChanged(object sender, EventArgs e) {
			UpdatePrimaryWhite();
		}

		private void SecondaryWhiteChanged(object sender, EventArgs e) {
			UpdateSecondaryWhite();
		}

		private void UpdatePrimaryWhite() {
			byte[] rgba = GetColorWhiteTemp(PrimaryWhite);
			UpdateImage(PrimaryWhiteImage, rgba);
		}

		private void UpdateSecondaryWhite() {
			byte[] rgba = GetColorWhiteTemp(SecondaryWhite);
			UpdateImage(SecondaryWhiteImage, rgba);
		}

		private byte[] GetColorPrimaryWhiteTemp() {
			return GetColorWhiteTemp(PrimaryWhite);
		}

		private byte[] GetColorSecondaryWhiteTemp() {
			return GetColorWhiteTemp(SecondaryWhite);
		}

		// get a RGBA from the white temp page
		// algorithm from http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/
		private byte[] GetColorWhiteTemp(NumControl toUse) {
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
		
		
		
		
		
		private void PrimaryModChanged(object sender, EventArgs e) {
			UpdatePrimaryModColor();
		}

		private void SecondaryModChanged(object sender, EventArgs e) {
			UpdateSecondaryModColor();
		}

		private void UpdatePrimaryModColor() {
			byte[] rgba = GetColorPrimaryModWheel();
			UpdateImage(PrimaryModImage, rgba);
		}

		private void UpdateSecondaryModColor() {
			byte[] rgba = GetColorSecondaryModWheel();
			UpdateImage(SecondaryModImage, rgba);
		}
		
		
		private byte[] GetColorPrimaryModWheel() {
			return GetModWheel(PrimaryModRed, PrimaryModGreen, PrimaryModBlue, PrimaryModMod);
		}

		private byte[] GetColorSecondaryModWheel() {
			return GetModWheel(SecondaryModRed, SecondaryModGreen, SecondaryModBlue, SecondaryModMod);
		}


		private byte[] GetModWheel(NumControl red, NumControl green, NumControl blue, NumControl mod) {
			decimal modRedVal = new decimal(red.NumValue/100.0);
			decimal modGreenVal = new decimal(green.NumValue/100.0);
			decimal modBlueVal = new decimal(blue.NumValue/100.0);
			int modVal = mod.NumValue;

			byte valR = (byte)MaxOrLess((int)(modVal * modRedVal), 255);
			byte valG = (byte)MaxOrLess((int)(modVal * modGreenVal), 255);
			byte valB = (byte)MaxOrLess((int)(modVal * modBlueVal), 255);

			return new byte[] {valR, valG, valB, 255};
		}

		private void UpdateImage(Canvas toUpdate, byte[] rgba) {
			Color c = Color.FromArgb(rgba[3], rgba[0], rgba[1], rgba[2]);
			toUpdate.Background = new SolidColorBrush(c);
		}





		public string PadZereos(byte toPad) {
			string toReturn = toPad.ToString().PadLeft(3, '0');
			return toReturn;
		}

		public string JoinArray(byte[] arr) {
			string toReturn = "";
			foreach (byte i in arr)
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
