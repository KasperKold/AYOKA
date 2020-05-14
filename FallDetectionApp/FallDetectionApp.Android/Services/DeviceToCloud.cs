﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    [Service]
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





        public async Task<string> SendMessageToIotHubAsync(GeoLocation currentGeo)
        {
            GeoLocation messageGeoLocation = currentGeo;
            //messageGeoLocation.DeviceId="";
            //messageGeoLocation.Id ="";
            //messageGeoLocation.Latitude = "55.11111";
            //messageGeoLocation.Longitude = "13.34353636";
            //messageGeoLocation.TimeDate="";
            //messageGeoLocation.Info="";

            var telemetryDataPoint = new
            {
                messageId = messageId++,
                deviceId,
                messageGeoLocation
            };

            var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Message(System.Text.Encoding.ASCII.GetBytes(messageString));
            Log.Verbose(TAG, "\nSENDING MESSAGE TO IOT-HUB");
            Log.Verbose(TAG, "Message: " + message.ContentType + "\nMessageString: " + messageString);

            await deviceClient.SendEventAsync(message);

            return messageString;
        }

        public async Task<string> SendTEXTMessageToIotHubAsync(string test)
        {


            var telemetryDataPoint = new
            {
                messageId = messageId++,
                deviceId,
                test
            };

            var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Message(System.Text.Encoding.ASCII.GetBytes(messageString));
            Log.Verbose(TAG, "\nSENDING MESSAGE TO IOT-HUB");
            Log.Verbose(TAG, "Message: " + message.ContentType + "\nMessageString: " + messageString);

            await deviceClient.SendEventAsync(message);

            return messageString;
        }




        public async Task<string> SendListToIotHubAsync()
        {
            List<GeoLocation> session = new List<GeoLocation>(await App.Database.GetGeoLocationItemsAsync());

            var telemetrysToSend = new List<TelemetryDataPoint>();
            // Enumerable.Empty<object>().Select(o => definition).ToList()
            for (int i = 0; i < session.Count; i++)
            {

                TelemetryDataPoint tmdp = new TelemetryDataPoint();

                tmdp.latitude = session[i].Latitude;
                tmdp.longitude = session[i].Longitude;
                tmdp.date_time = session[i].TimeDate;
                tmdp.sessionId = session[i].SessionId;
                tmdp.deviceId = session[i].DeviceId;
                tmdp.inactivityDetected = session[i].InactivityDetected;
                // tmdp.alarmed = session[i].Alarmed;


                telemetrysToSend.Add(tmdp);

            }
            var messageString = JsonConvert.SerializeObject(telemetrysToSend);
            var message = new Message(System.Text.Encoding.ASCII.GetBytes(messageString));
            Log.Verbose(TAG, "\nSENDING MESSAGE TO IOT-HUB");
            Log.Verbose(TAG, "Message: " + message.ContentType + "\nMessageString: " + messageString);

            await deviceClient.SendEventAsync(message);
            session.Clear();
            telemetrysToSend.Clear();
            return messageString;
        }


    }


    /*
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
        */



}