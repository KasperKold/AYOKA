using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Util;
using Plugin.Messaging;
using Xamarin.Essentials;

namespace FallDetectionApp.Droid.Services
{
    public class CallAndSms
    {
        private string smsMessageDefault = "THIS IS A TEST ONLY: Hi, this is an automatic message from a Mobile application. This user need your help. Please get help to this location: ";
        private string callMessageDefault1 = "THIS IS A TEST ONLY: Attention! This is an automatic emergency message from a mobile application. Your friend Tomas need your help. Please get help to this location: Latitide: 55.8888 and Longitude: 13.4545. Thank you! This message will be repeated ";
        //private string user;
        private readonly string TAG = "Log CallAndSms";


        public CallAndSms()
        {
            initializeComponents();
        }



        public void initializeComponents()
        {



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

            await TextToSpeech.SpeakAsync("Hello World! Nice weather today! I love Peder", settings);
            await TextToSpeech.SpeakAsync("This is an automatic message!", settings);
            //Checking for permission.

            List<Models.Contact> contactsFromLocalDB = await App.Database.GetItemsAsync();

            for (int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                var phoneCall = CrossMessaging.Current.PhoneDialer;
                // Debug.WriteLine("Testing to find contact numbers in database: ");
                Log.Verbose(TAG, "Phone Contacts: " + contactsFromLocalDB[i].Name + " " + contactsFromLocalDB[i].PhoneNr);

                //&& !(contactsFromLocalDB[i].PhoneNr == "")

                if (phoneCall.CanMakePhoneCall)
                {

                    phoneCall.MakePhoneCall(contactsFromLocalDB[i].PhoneNr);
                    for (int j = 6; j < 0; j--)
                    {
                        // not so good sound
                        await TextToSpeech.SpeakAsync(callMessageDefault1 + j + " more times", settings);
                    }
                    await Task.Delay(20000);
                }
            }

            return await Task.FromResult(true);
        }




        public async Task<bool> SmsToContact()
        {


            // If permission has been granted, sms-messenging commences

            List<Models.Contact> contactsFromLocalDB = await App.Database.GetItemsAsync();
            //&& !(contactsFromLocalDB[i].PhoneNr == "")
            for (int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                var smsMessenger = Plugin.Messaging.CrossMessaging.Current.SmsMessenger;
                if (smsMessenger.CanSendSmsInBackground)
                {
                    smsMessenger.SendSmsInBackground(contactsFromLocalDB[i].PhoneNr, smsMessageDefault);
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
