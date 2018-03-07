using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MoodRPi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static string openweathermapapikey = "6c2710a83700dc8b2be058a4156a5099";
        private static string url = $"http://api.openweathermap.org/data/2.5/weather?q=Kuching&appid=" + openweathermapapikey;
        private const int RED_LED_PIN = 18;
        private const int GREEN_LED_PIN = 27;
        private const int BLUE_LED_PIN = 22;
        private GpioPin redLedPin, greenLedPin, blueLedPin;
        private DispatcherTimer timer;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // Dispose pins to free memory
            redLedPin.Dispose();
            greenLedPin.Dispose();
            blueLedPin.Dispose();
            timer.Stop();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Check presence of GPIO Controller
            // Since this is UWP, this application runs on desktop, mobile, as well as embedded devices
            // best to confirm we are running on an embedded device like R Pi
            GpioController gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                Debug.WriteLine("This device does not have GPIO Controller.");
                return;
            }

            // Opens a connection to the specified general-purpose I/O (GPIO) pin in exclusive mode
            redLedPin = gpio.OpenPin(RED_LED_PIN);
            greenLedPin = gpio.OpenPin(GREEN_LED_PIN);
            blueLedPin = gpio.OpenPin(BLUE_LED_PIN);

            // Sets the drive mode of the general-purpose I/O (GPIO) pin. 
            // The drive mode specifies whether the pin is configured as an input or an output, 
            // and determines how values are driven onto the pin.
            redLedPin.SetDriveMode(GpioPinDriveMode.Output);
            greenLedPin.SetDriveMode(GpioPinDriveMode.Output);
            blueLedPin.SetDriveMode(GpioPinDriveMode.Output);

            // Set LED to white to know it is ON
            // Other pins are set to Low
            redLedPin.Write(GpioPinValue.High);
            greenLedPin.Write(GpioPinValue.High);
            blueLedPin.Write(GpioPinValue.High);

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();

            Debug.WriteLine("GPIO pin initialized correctly.");
        }

        private void Timer_Tick(object sender, object e)
        {
            WeatherDTO weather = null;

            Task.Run(async () => 
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        // setting a timeout for the web request
                        // if we couldn't get anything back in 2 minutes,
                        // we'll ignore it for now
                        client.Timeout = TimeSpan.FromMinutes(2);

                        // HTTP GET
                        var response = await client.GetStringAsync(url);

                        // Converting the json string to our weather data transfer object
                        weather = JsonConvert.DeserializeObject<WeatherDTO>(response);                      

                        // Call dispose to free up memory
                        client.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error encountered: " + ex);
                }

                if (weather != null)
                {
                    switch (weather.weather[0].main.ToLowerInvariant())
                    {
                        case "tunderstorm":
                            // yellow
                            redLedPin.Write(GpioPinValue.High);
                            greenLedPin.Write(GpioPinValue.High);
                            blueLedPin.Write(GpioPinValue.Low);
                            break;

                        case "drizzle":
                            // cyan
                            redLedPin.Write(GpioPinValue.Low);
                            greenLedPin.Write(GpioPinValue.High);
                            blueLedPin.Write(GpioPinValue.High);
                            break;

                        case "rain":
                            // blue
                            redLedPin.Write(GpioPinValue.Low);
                            greenLedPin.Write(GpioPinValue.Low);
                            blueLedPin.Write(GpioPinValue.High);
                            break;

                        case "clouds":
                            // green
                            redLedPin.Write(GpioPinValue.Low);
                            greenLedPin.Write(GpioPinValue.High);
                            blueLedPin.Write(GpioPinValue.Low);
                            break;

                        case "clear":
                            // green
                            redLedPin.Write(GpioPinValue.Low);
                            greenLedPin.Write(GpioPinValue.High);
                            blueLedPin.Write(GpioPinValue.Low);
                            break;

                        case "atmosphere":
                            // green
                            redLedPin.Write(GpioPinValue.Low);
                            greenLedPin.Write(GpioPinValue.High);
                            blueLedPin.Write(GpioPinValue.Low);
                            break;

                        case "extreme":
                            // magenta
                            redLedPin.Write(GpioPinValue.High);
                            greenLedPin.Write(GpioPinValue.Low);
                            blueLedPin.Write(GpioPinValue.High);
                            break;

                        case "additional":
                            // red
                            redLedPin.Write(GpioPinValue.High);
                            greenLedPin.Write(GpioPinValue.Low);
                            blueLedPin.Write(GpioPinValue.Low);
                            break;

                        default:
                            // white
                            redLedPin.Write(GpioPinValue.High);
                            greenLedPin.Write(GpioPinValue.High);
                            blueLedPin.Write(GpioPinValue.High);
                            break;
                    }
                }
            });   
        }
    }
}
