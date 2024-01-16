// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/main/iothub/device/samples
using Microsoft.Azure.Devices.Client;
using System.Text;
using System.Text.Json;

namespace SimulatedTelemetry
{
    /// <summary>
    /// This sample illustrates the very basics of a device app sending telemetry. For a more comprehensive device app sample, please see
    /// <see href="https://github.com/Azure-Samples/azure-iot-samples-csharp/tree/main/iot-hub/Samples/device/DeviceReconnectionSample"/>.
    /// </summary>
    internal class Program
    {
        private static string DeviceConnectionString = @"<DeviceConnectionString>";

        private static double IntervalInSeconds = 5.0;

        private static TimeSpan s_telemetryInterval = TimeSpan.FromSeconds(IntervalInSeconds);

        private static async Task Main(string[] args)
        {

            Console.WriteLine("IoT Hub Quickstarts #1 - Simulated device.");

            // Connect to the IoT hub using the MQTT protocol by default
            using var deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString);

            // Set up a condition to quit the sample
            Console.WriteLine("Press control-C to exit.");
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            // Run the telemetry loop
            await SendDeviceToCloudMessagesAsync(deviceClient, cts.Token);

            // SendDeviceToCloudMessagesAsync is designed to run until cancellation has been explicitly requested by Console.CancelKeyPress.
            // As a result, by the time the control reaches the call to close the device client, the cancellation token source would
            // have already had cancellation requested.
            // Hence, if you want to pass a cancellation token to any subsequent calls, a new token needs to be generated.
            // For device client APIs, you can also call them without a cancellation token, which will set a default
            // cancellation timeout of 4 minutes: https://github.com/Azure/azure-iot-sdk-csharp/blob/64f6e9f24371bc40ab3ec7a8b8accbfb537f0fe1/iothub/device/src/InternalClient.cs#L1922
            await deviceClient.CloseAsync();

            Console.WriteLine("Device simulator finished.");
        }

        

        // Async method to send simulated telemetry
        private static async Task SendDeviceToCloudMessagesAsync(DeviceClient deviceClient, CancellationToken ct)
        {
            // Initial telemetry values
            //double minTemperature = 20;
            //double minHumidity = 60;
            //var rand = new Random();

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    //double currentTemperature = minTemperature + rand.NextDouble() * 15;
                    //double currentHumidity = minHumidity + rand.NextDouble() * 20;

                    // Create JSON message
                    string messageBody = JsonSerializer.Serialize(
                        MyCustomTelemetry.GetInstance.GetMyCustomTelemetry()
                        );
                    using var message = new Message(Encoding.ASCII.GetBytes(messageBody))
                    {
                        ContentType = "application/json",
                        ContentEncoding = "utf-8",
                    };

                    // Add a custom application property to the message.
                    // An IoT hub can filter on these properties without access to the message body.
                    //message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                    // Send the telemetry message
                    await deviceClient.SendEventAsync(message, ct);
                    Console.WriteLine($"{DateTime.Now} > Sending message: {messageBody}");

                    await Task.Delay(s_telemetryInterval, ct);
                }
            }
            catch (TaskCanceledException) { } // ct was signaled
        }


        public sealed class MyCustomTelemetry
        {
            private static double minTemperature = 20.0;
            private static double minHumidity = 60.0;
            private static Random rand = new Random();

            private static readonly Lazy<MyCustomTelemetry> telemtry = 
                new Lazy<MyCustomTelemetry>(() => new MyCustomTelemetry()); 
            public static MyCustomTelemetry GetInstance
            {
                get
                {
                    return telemtry.Value;
                }
            }
            // Properties
            public double temperature { get; set; }
            public double humidity { get; set; }

            // Set function
            public MyCustomTelemetry GetMyCustomTelemetry()
            {
                temperature = Math.Round(minTemperature + rand.NextDouble() * 15, 2);
                humidity = Math.Round(minHumidity + rand.NextDouble() * 20, 2);
                return this;
            }
            
        }
    }
}