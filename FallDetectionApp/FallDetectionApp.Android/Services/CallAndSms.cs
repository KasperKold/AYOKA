using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Util;
using Java.Nio.Channels;
using Plugin.Messaging;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FallDetectionApp.Droid.Services
{
    public class CallAndSms
    {

        //private string callMessageDefault1 = "THIS IS A TEST ONLY: Attention! This is an automatic emergency message from a mobile application. Your friend Tomas need your help. Please get help to this location: Latitide: 55.8888 and Longitude: 13.4545. Thank you! This message will be repeated ";
        private string configPageAlarmMessage;
        private string alarmMyLocationIs = " My location is: ";
        private string alarmWordLongitude = " Longitude: ";
        private string alarmWordLatitude = " Latitude: ";
        private string alarmMessageDateTime = " Date and time is: ";
        private string alarmMessageEnd = " This is an automatic message from a Mobile application. This user need your help. Please get help to this location. ";
        private string alarmRepeat = "This message will be repeated once more. ";
        private readonly string TAG = "Log CallAndSms";
        private Monitor monitor;


        public CallAndSms()
        {
            initializeComponents();

            MessagingCenter.Subscribe<Object, string>(this, "alarmMessage", async (sender, arg) =>
            {
                configPageAlarmMessage = arg;
            });
        }



        public void initializeComponents()
        {



        }


        public void setMonitor(Monitor monitor)
        {
            this.monitor = monitor;
        }

        /*
                public async Task SpeakNow()
                {
                    var locales = await TextToSpeech.GetLocalesAsync();

                    // Grab the first locale
                    var locale = locales.FirstOrDefault();

                    var settings = new SpeechOptions()
                    {
                        Volume = .75f,
                        Pitch = 1.0f,
                        Locale = locale
                    };

                    await TextToSpeech.SpeakAsync("Hello World", settings);
                }
                */

        // Test Call function
        public async Task<bool> CallContacts()

        {
            //gather message
            var alarmMessage = assembleMessage();



            // speech settings - no or go?
            var locales = await TextToSpeech.GetLocalesAsync();

            // Grab the first locale
            var locale = locales.FirstOrDefault();

            var settings = new SpeechOptions()
            {
                Volume = .75f,
                Pitch = 1.0f,
                Locale = locale
            };

            // test - this is better sound

            // await TextToSpeech.SpeakAsync("Hello World! Nice weather today! I love Peder", settings);
            //await TextToSpeech.SpeakAsync("This is an automatic message!", settings);
            //Checking for permission.

            List<Models.Contact> contactsFromLocalDB = await App.Database.GetItemsAsync();

            for (int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                var phoneCall = CrossMessaging.Current.PhoneDialer;

                Log.Verbose(TAG, "Phone Contacts: " + contactsFromLocalDB[i].Name + " " + contactsFromLocalDB[i].PhoneNr);

                //&& !(contactsFromLocalDB[i].PhoneNr == "")

                if (phoneCall.CanMakePhoneCall)
                {

                    phoneCall.MakePhoneCall(contactsFromLocalDB[i].PhoneNr);

                    await Task.Delay(5000);

                    await TextToSpeech.SpeakAsync(alarmMessage);

                    await Task.Delay(5000);
                }
            }

            return await Task.FromResult(true);
        }


        public string assembleMessage()
        {
            var location = monitor.GetCurrentGeoPos();
            string message = configPageAlarmMessage + alarmMyLocationIs + alarmWordLongitude + location.Longitude + alarmWordLatitude
                  + location.Latitude + alarmMessageDateTime + location.TimeDate + alarmMessageEnd;
            return message + alarmRepeat + message;
        }


        public async Task<bool> SmsToContact()
        {

            var alarmMessage = assembleMessage();

            // If permission has been granted, sms-messenging commences

            List<Models.Contact> contactsFromLocalDB = await App.Database.GetItemsAsync();
            //&& !(contactsFromLocalDB[i].PhoneNr == "")
            for (int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                var smsMessenger = Plugin.Messaging.CrossMessaging.Current.SmsMessenger;
                if (smsMessenger.CanSendSmsInBackground)
                {
                    smsMessenger.SendSmsInBackground(contactsFromLocalDB[i].PhoneNr, alarmMessage + alarmMessage);
                }
            }

            // Debug.WriteLine("Testing to find contact numbers in database: ");
            for (int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                Log.Verbose(TAG, "SMS Contacts:" + contactsFromLocalDB[i].Name + " " + contactsFromLocalDB[i].PhoneNr); ;
            }

            return await Task.FromResult(true);
        }

    }
}
