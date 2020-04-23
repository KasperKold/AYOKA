using System;

using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using DotNetty.Common.Utilities;
using FallDetectionApp.Models;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Encoding = System.Text.Encoding;
using Message = Microsoft.Azure.Devices.Client.Message;



namespace FallDetectionApp.Droid.Services
{
    public class DeviceToCloud
    {
        private readonly string TAG = "Log DeviceToCloud";
        int messageId = 1;
        DeviceClient deviceClient;

        string deviceId;
        string deviceKey;
        string iotHostName;


        public DeviceToCloud(string deviceId, string deviceKey, string iotHostName)
        {
            this.deviceId = deviceId;
            this.deviceKey = deviceKey;
            this.iotHostName = iotHostName;

            deviceClient = DeviceClient.Create(iotHostName, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey));
            //deviceClient = DeviceClient.CreateFromConnectionString();
        }


        string GetLittleMessage()
        {
            string testMessage = "Dolores";

            return testMessage;
        }

        public Task<string> SendFakeDeviceToCloudDataAsync()
        {
            return SendDeviceToCloudDataAsync(GetLittleMessage());
        }

        async Task<string> SendDeviceToCloudDataAsync(string text)
        {
            var telemetryDataPoint = new
            {
                messageId = messageId++,
                deviceId = deviceId,
                test = GetLittleMessage()
            };

            var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Message(System.Text.Encoding.ASCII.GetBytes(messageString));
            Log.Verbose(TAG, "\nSENDING STRING DOLORES");
            Log.Verbose(TAG, "Message: " + message.ContentType + "\nMessageString: " + messageString);

            await deviceClient.SendEventAsync(message);

            return messageString;
        }




        //var rand = new Random();

        //var testGeo = new GeoLocation();
        /* {
             testGeo.Id = rand.Next(),
             testGeo.Latitude = rand.NextDouble(),
             testGeo.Longitude = rand.NextDouble(),
             testGeo.TimeDate=rand.NextDouble(),
             GyroY = rand.NextDouble(),
             GyroZ = rand.NextDouble(),
         };
         */

        async void ReceiveCloudToDeviceMessagesAsync()
        {
            Log.Verbose(TAG, "\nReceiving cloud to device messages from service");

            while (true)
            {
                var receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage == null)
                    continue;

                Log.Verbose(TAG, "Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));

                await deviceClient.CompleteAsync(receivedMessage);
            }
        }



    }
}
