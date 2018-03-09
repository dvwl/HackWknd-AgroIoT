# HackWknd-AgroIoT
A compilation of demo projects showcased during HackWknd: AgroIoT

I would like to express my gratitude to TEGAS and MaGIC for jointly organizing HackWknd: Agro-IoT and for extending their invitation to me to deliver this workshop.

These demo runs on WIndows 10 IoT Core Build 16299 on a Raspberry Pi 3.
These demo assumes you know the basic setup (running Windows 10 IoT Core on Raspberry Pi 3 and preparing Visual Studio with the neccessary IoT extensions).


## HelloRPi
This is a simple 'Get Started' with Raspberry Pi 3 and Windows 10 IoT Core.
Basically, a LED will light up, connected to GPIO 27, when a tactile switch, connected to GPIO 22, is pressed.

![Image of HelloRPi Circuit Diagram](https://github.com/dvwl/Circuit-Diagram/HelloRPi.png)


## ServoRPi
This is a demo on how to use software pulse width modulation to drive a servomotor
This sample uses the Microsoft.IoT.Devices NuGet plugin.

![Image of ServoRPi Circuit Diagram](https://github.com/dvwl/Circuit-Diagram/ServoRPi.png)


## MoodRPi
This is a demo on how we leverage existing .NET API written for UWP such as HTTPClient on IoT devices.
This demo uses OpenWeatherMap API to get weather data in JSON format.
Weather data is then interpretted using Newtonsoft.JSON NuGET plugin to a WeatherDTO object.
The RGB LED then lights up as per the weather condition.


![Image of MoodRPi Circuit Diagram](https://github.com/dvwl/Circuit-Diagram/MoodRPi.png)
