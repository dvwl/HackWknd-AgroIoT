using System;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HelloRPi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int LED_PIN = 27;
        private const int SWITCH_PIN = 22;
        private GpioPin ledPin, switchPin;
        private GpioPinValue switchPinValue;

        public MainPage()
        {
            this.InitializeComponent();

            // Application Lifecycle Management
            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // Dispose pins to free memory
            ledPin.Dispose();
            switchPin.Dispose();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Check presence of GPIO Controller
            // Since this is UWP, this application runs on desktop, mobile, as well as embedded devices
            // Best to confirm we are running on an embedded device like R Pi
            GpioController gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                Debug.WriteLine("This device does not have GPIO controller.");
                return;
            }

            // Opens a connection to the specified general-purpose I/O (GPIO) pin in exclusive mode
            switchPin = gpio.OpenPin(SWITCH_PIN);
            ledPin = gpio.OpenPin(LED_PIN);

            // Sets a debounce timeout for GPIO Pin
            // in which is an interval during which changes to the value of the pin are filter out
            // and do not generate ValueChanged event
            switchPin.DebounceTimeout = TimeSpan.FromMilliseconds(10);

            // Sets the drive mode of the GPIO pin
            // This drive mode specifies whether the pin is configured as an input or output
            // and determins how values are driven onto the pin
            switchPin.SetDriveMode(GpioPinDriveMode.Input);
            ledPin.SetDriveMode(GpioPinDriveMode.Output);

            // Drives the specified value onto the GPIO
            ledPin.Write(GpioPinValue.Low);

            // Map ValueChanged event handler
            switchPin.ValueChanged += SwitchPin_ValueChanged;

            Debug.WriteLine("GPIO pin initialized correctly.");
        }

        private void SwitchPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            // this is assuming pull up switch with a pull down resistor
            // meaning, when switch is pressed, voltage at input pin is HIGH
            // also, assuming ledPin is connected in an active high mode
            // meaning, when output pin is high, led lights up
            switchPinValue = switchPin.Read();
            if(switchPinValue == GpioPinValue.High)
            {
                ledPin.Write(switchPinValue);
                Debug.WriteLine("LED is on!");
            }
            else
            {
                ledPin.Write(switchPinValue);
                Debug.WriteLine("LED is off!");
            }
        }
    }
}
