using System.Collections.Generic;
//!!!Keeping these out commented below here for future tries with calls!
//using System.Linq;
using System.Threading.Tasks;
//using Android.Content;
using Android.Telephony;
using Android.Util;
//using Android.Views;
//using Android.Widget;
//using Plugin.Messaging;
//using Xamarin.Essentials;
using Xamarin.Forms;

namespace FallDetectionApp.Droid.Services
{
    public class CallAndSms : PhoneStateListener
    {
        private string configPageAlarmMessage;
        private string alarmMyLocationIs = "\nMy location: ";
        private string alarmWordLongitude = "\nLongitude: ";
        private string alarmWordLatitude = " \nLatitude: ";
        private string alarmGooglelink = "http://www.google.com/maps/place/";
        private string alarmMessageDateTime = "\nDate and time is:\n";
        private string alarmMessageEnd = "\nThis is an automatic message from a Mobile application. This person needs your help. Please get help to this location. ";
        private readonly string TAG = "Log CallAndSms";
        private Monitor monitor;
        private MainActivity mainActivity;
        //private bool isPhoneCalling = false;

        public CallAndSms(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
            initializeComponents();

        }



        public void initializeComponents()
        {
        }


        public void setMonitor(Monitor monitor)
        {
            this.monitor = monitor;
        }


        // Test Call function - works but volume of text to speech is low during call.

        /*public async Task<bool> CallContacts()

        {
            var alarmMessage = assembleMessage();

            // speech settings - no or go?
            var locales = await TextToSpeech.GetLocalesAsync();

            // Grab the first locale
            var locale = locales.FirstOrDefault();

            var settings = new SpeechOptions()
            {
                Volume = 0.9f,
                Pitch = 1.0f,
                Locale = locale
            };

            //test - this is better sound
            //await TextToSpeech.SpeakAsync("Hello World! Nice weather today!", settings);
            //await TextToSpeech.SpeakAsync("This is an automatic message!", settings);

            //Checking for permission.
            List<Models.Contact> contactsFromLocalDB = await App.Database.GetItemsAsync();

            for (int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                var phoneCall = CrossMessaging.Current.PhoneDialer;

                Log.Verbose(TAG, "Phone Contacts: " + contactsFromLocalDB[i].Name + " " + contactsFromLocalDB[i].PhoneNr);

                if (phoneCall.CanMakePhoneCall && contactsFromLocalDB[i].PhoneNr != "")
                {
                    phoneCall.MakePhoneCall(contactsFromLocalDB[i].PhoneNr);
                    await Task.Delay(3000);

                    await TextToSpeech.SpeakAsync(alarmMessage);

                    await Task.Delay(5000);


                }
                else
                {
                    Intent intent = mainActivity.PackageManager.GetLaunchIntentForPackage(mainActivity.PackageName);
                    intent.AddFlags(ActivityFlags.ClearTop);
                    mainActivity.StartActivity(intent);

                    Toast errorToast = Toast.MakeText(Xamarin.Essentials.Platform.CurrentActivity, "Not Able to make a phone call", ToastLength.Short);
                    errorToast.SetGravity(GravityFlags.Center, 0, 0);
                    errorToast.Show();
                    Log.Debug(TAG, "Not able to make a call");

                }
            }
            return await Task.FromResult(true);
        }
        */


        public string assembleMessage()
        {
            configPageAlarmMessage = Application.Current.Properties["userAlarmMessage"].ToString();
            var location = monitor.GetCurrentGeoPos();
            string message = configPageAlarmMessage + alarmMyLocationIs + alarmWordLongitude + location.Latitude + alarmWordLatitude
                  + location.Longitude + alarmMessageDateTime + location.TimeDate + "\n" + alarmGooglelink + location.Latitude + "," + location.Longitude + "\n" + alarmMessageEnd;
            return message;
        }


        public async Task<bool> SmsToContact()
        {

            var alarmMessage = assembleMessage();

            // If permission has been granted, sms-messenging commences

            List<Models.Contact> contactsFromLocalDB = await App.Database.GetItemsAsync();

            for (int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                var smsMessenger = Plugin.Messaging.CrossMessaging.Current.SmsMessenger;
                if (smsMessenger.CanSendSmsInBackground)
                {
                    smsMessenger.SendSmsInBackground(contactsFromLocalDB[i].PhoneNr, alarmMessage);
                }
            }

            // Debug.WriteLine("Testing to find contact numbers in database: ");
            for (int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                Log.Verbose(TAG, "SMS Contacts:" + contactsFromLocalDB[i].Name + " " + contactsFromLocalDB[i].PhoneNr); ;
            }

            return await Task.FromResult(true);
        }




        //This works recognizes states but great together with callContacts() - needs to be sorted out

        /*
                async public override void OnCallStateChanged(CallState state, string incomingNumber)
                {
                    base.OnCallStateChanged(state, incomingNumber);

                    //mainActivity.UpdateCallState(state, incomingNumber);

                    //gather message
                    // var alarmMessage = assembleMessage();

                    switch (state)
                    {
                        case CallState.Ringing:
                            // phone ringing
                            Log.Debug(TAG, "RINGING, number: " + incomingNumber);
                            break;

                        case CallState.Offhook:
                            // active
                            Log.Debug(TAG, "OFFHOOK");



                            isPhoneCalling = true;
                            break;
                        case CallState.Idle:
                            Log.Debug(TAG, "IDLE");

                            if (isPhoneCalling)
                            {

                                Log.Debug(TAG, "restart app");

                                Intent i = mainActivity.PackageManager.GetLaunchIntentForPackage(mainActivity.PackageName);
                                i.AddFlags(ActivityFlags.ClearTop);
                                mainActivity.StartActivity(i);

                            }
                            break;
                    }   
            }
            */
    }
}
