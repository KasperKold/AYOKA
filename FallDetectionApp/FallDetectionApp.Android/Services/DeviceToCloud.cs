﻿using System;
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
        }


        public Task<string> SendFakeDeviceToCloudDataAsync()
        {
            return SendDeviceToCloudDataAsync(GetALittleMessage());
        }

        async Task<string> SendDeviceToCloudDataAsync(string text)
        {
            var telemetryDataPoint = new
            {
                messageId = messageId++,
                deviceId = deviceId
            };
            var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            await deviceClient.SendEventAsync(message);
            return messageString;
        }

        string GetALittleMessage()
        {
            string testMessage = "Dolores";

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
            Log.Verbose(TAG, "\nSENDING STRING DOLORES");
            return testMessage;
        }

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
