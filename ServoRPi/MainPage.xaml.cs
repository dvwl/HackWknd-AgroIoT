using Microsoft.IoT.DeviceCore.Pwm;
using Microsoft.IoT.Devices.Pwm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.Pwm;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ServoRPi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private PwmController pwmController;
        private PwmPin pwmPin;
        private const int servoPin = 18;
        private DispatcherTimer timer;
        private bool IsClockwise;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            this.Unloaded += MainPage_Unloaded;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            pwmPin.Stop();
            pwmPin.Dispose();
            timer.Stop();
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
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

            var pwmManager = new PwmProviderManager();
            pwmManager.Providers.Add(new SoftPwm());

            var pwmContollers = await pwmManager.GetControllersAsync();

            pwmController = pwmContollers[0];
            pwmController.SetDesiredFrequency(50);

            pwmPin = pwmController.OpenPin(servoPin);
            pwmPin.Start();

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(15)
            };
            timer.Tick += Timer_Tick;
            timer.Start();

            IsClockwise = false;
        }

        private async void Timer_Tick(object sender, object e)
        {
            if (IsClockwise)
            {
                for (int i = 0; i < 98; i++)
                {
                    pwmPin.SetActiveDutyCyclePercentage(i / 100.0);
                    await Task.Delay(100);
                    
                }
                IsClockwise = false;
            }
            else
            {
                for (int i = 98; i > 0; i--)
                {
                    pwmPin.SetActiveDutyCyclePercentage(i / 100.0);
                    await Task.Delay(100);
                }
                IsClockwise = true;
            }
        }
    }
}
