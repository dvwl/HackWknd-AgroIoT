using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HackWkndIoTKeynoteDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int START_LED = 4;
        private const int LED_1 = 13;
        private const int LED_2 = 6;
        private const int LED_3 = 5;
        private static string query = "https://graph.facebook.com/v2.12/bawa.cane?fields=fan_count&access_token=[INSERT_YOUR_FACEBOOK_GRAPH_API_TOKEN_HERE]";
        private int likes = 198;
        private int currentLikes = 198;
        private GpioPin pin0, pin1, pin2, pin3;
        private DispatcherTimer ledUpdateTimer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;

            ledUpdateTimer.Interval = TimeSpan.FromSeconds(5);
            ledUpdateTimer.Start();
            ledUpdateTimer.Tick += LedUpdateTimer_Tick;
        }

        private async void LedUpdateTimer_Tick(object sender, object e)
        {
            var currentTimeStamp = DateTime.Now;

            likes = await GetFBLikes();

            if (likes - currentLikes >= 20)
            {
                pin1.Write(GpioPinValue.Low);
                pin2.Write(GpioPinValue.Low);
                pin3.Write(GpioPinValue.Low);
                await Task.Delay(100);
                pin1.Write(GpioPinValue.High);
                await Task.Delay(100);
                pin2.Write(GpioPinValue.High);
                await Task.Delay(100);
                pin3.Write(GpioPinValue.High);
            }
            else if (likes - currentLikes >= 15)
            {
                pin3.Write(GpioPinValue.High);
            }
            else if (likes - currentLikes >= 10)
            {
                pin2.Write(GpioPinValue.High);
            }
            else if (likes - currentLikes >= 5)
            {
                pin1.Write(GpioPinValue.High);
            }
        }

        private void MainPage_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            pin0.Dispose();
            pin1.Dispose();
            pin2.Dispose();
            pin3.Dispose();
        }

        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            GpioController gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                Debug.WriteLine("This device does not have GPIO controller.");
                return;
            }
            
            Task.Run(async () => 
            {
                currentLikes = await GetFBLikes();
                likes = currentLikes;
            });
            
            pin0 = gpio.OpenPin(START_LED);
            pin0.Write(GpioPinValue.High);
            pin0.SetDriveMode(GpioPinDriveMode.Output);

            pin1 = gpio.OpenPin(LED_1);
            pin1.Write(GpioPinValue.Low);
            pin1.SetDriveMode(GpioPinDriveMode.Output);

            pin2 = gpio.OpenPin(LED_2);
            pin2.Write(GpioPinValue.Low);
            pin2.SetDriveMode(GpioPinDriveMode.Output);

            pin3 = gpio.OpenPin(LED_3);
            pin3.Write(GpioPinValue.Low);
            pin3.SetDriveMode(GpioPinDriveMode.Output);

            Debug.WriteLine("Gpio pin initialized correctly.");
        }

        public static async Task<int> GetFBLikes()
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(query);

                FbFanCount jsonstring = JsonConvert.DeserializeObject<FbFanCount>(response);
                return jsonstring.fan_count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error occured: " + ex);
            }

            return 0;
        }
    }
}
