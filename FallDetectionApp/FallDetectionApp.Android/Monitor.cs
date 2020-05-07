using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.Telephony;
using Android.Util;
using FallDetectionApp.Droid.Services;
using FallDetectionApp.Models;
using FallDetectionApp.ViewModels;
using Plugin.Messaging;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Android.Provider.Settings;
using Application = Xamarin.Forms.Application;


namespace FallDetectionApp.Droid
{
    public class Monitor
    {
        private readonly string TAG = "Log Monitor";

        private MainActivity mainActivity;
        private CallAndSms callAndSms;

        private string savedLat;
        private string savedLong;
        public bool readyForSession;
        public bool sessionStart;

        private Timer guiTimer;
        private Timer monitorTimer;
        private Timer alertTimer;

        private DeviceToCloud deviceToCloud;
        static string iotHubDeviceId;
        static string iotHubDeviceKey;
        static string iotHubHostName;

        private string sessionId;
        private string deviceId;

        private int geoPeriod;
        private int savedGeoPeriod;
        private int secToAlarm;
        private int savedSecToAlarm;
        private int countDownActivateBtn;
        private int savedCountDownActivateBtn;


        public bool alertBool;

        private AlertDialog alert;
        private AlertDialog.Builder dialog;

        private GeoLocation currentGeoPos;
        private GeoLocation tempGeoPos;


        List<GeoLocation> sessionGeoLocation;






        public Monitor(MainActivity mainActivity, CallAndSms callAndSms)
        {
            this.mainActivity = mainActivity;
            this.callAndSms = callAndSms;
            initializeComponents();
            SetDeviceId();



        }


        public void initializeComponents()
        {

            currentGeoPos = new GeoLocation { Latitude = "no lat yet", Longitude = "no long yet" };
            sessionGeoLocation = new List<GeoLocation>();

            // create alert Dialogue
            dialog = new AlertDialog.Builder(Xamarin.Essentials.Platform.CurrentActivity);
            alert = dialog.Create();

            // create GuiTimer
            guiTimer = new Timer();

            // Tell the timer what to do when it elapses
            guiTimer.Elapsed += new ElapsedEventHandler(CountDown);
            guiTimer.Enabled = false;
            guiTimer.Interval = 1000;

            //  create alert Timer
            alertTimer = new Timer();
            alertTimer.Elapsed += OnTimedEvent;
            alertTimer.Interval = 1000; //seconds
            alertTimer.Enabled = false;

            // create monitorTimer
            monitorTimer = new Timer();

            // Tell the timer what to do when it elapses
            monitorTimer.Elapsed += new ElapsedEventHandler(Session);
            monitorTimer.Enabled = false;

            iotHubDeviceId = "PederTestDevice";
            iotHubDeviceKey = "kYMV9WOF4PSifDtML6K8JMO07ORitGaazeoWsCZHFBA="; //primarykey
            iotHubHostName = "IotFallApp.azure-devices.net";
            deviceToCloud = new DeviceToCloud(iotHubDeviceId, iotHubDeviceKey, iotHubHostName);
        }



        public void SaveToDb(GeoLocation currentGeo)
        {
            Console.WriteLine("SAVING to Db and SENDING message for GUI update" + "\n");
            App.Database.SaveGeoLocationItemAsync(currentGeo);
            MessagingCenter.Send<Object>(this, "latestGeo");
            countDownActivateBtn = savedCountDownActivateBtn;

        }


        public void SetGeoInstance(LocationChangedEventArgs e)
        {

            var location = e.Location;

            //parse for not to sensitive accuracy
            string lati = $"{location.Latitude}";
            string longi = $"{location.Longitude}";

            //get date
            DateTime dateTime = DateTime.Now.ToLocalTime();
            string date_string = dateTime.ToString();

            currentGeoPos.Latitude = lati;
            currentGeoPos.Longitude = longi;
            currentGeoPos.TimeDate = date_string;
            currentGeoPos.DeviceId = GetDeviceId();
            currentGeoPos.SessionId = GetSessionId();


        }

        public GeoLocation GetCurrentGeoPos()
        {
            return currentGeoPos;
        }


        public void SetDeviceId()
        {
            deviceId = Preferences.Get("deviceId", string.Empty);
            //my_
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                //deviceId = Guid.NewGuid().ToString();
                deviceId = Secure.GetString(mainActivity.ContentResolver, Secure.AndroidId);
                Preferences.Set("deviceId", deviceId);

            }
            Log.Debug(TAG, "DEVICE ID:" + deviceId);
        }


        public string GetDeviceId()
        {
            return deviceId;
        }


        public void SetSessionId()
        {

            sessionId = Guid.NewGuid().ToString();
            Log.Debug(TAG, "SESSION ID:" + sessionId);
        }

        public string GetSessionId()
        {
            return sessionId;
        }

        public void setComparingGeo()
        {
            savedLat = GetCurrentGeoPos().Latitude;
            savedLong = GetCurrentGeoPos().Longitude;
            //Log.Debug(TAG, "COMPARING LONGITUDE : " + GetCurrentGeoPos().Longitude.ToString());

        }


        public void StartMonitor()
        {
            //setting up
            SetSessionId();
            setComparingGeo(); // to compare with
            this.tempGeoPos = GetCurrentGeoPos();
            SaveToDb(tempGeoPos); // initial save and sent to GUI/db

            Log.Debug(TAG, "SEC_TO_ALARM : " + Convert.ToInt32(Application.Current.Properties["secToAlarm_setting"]).ToString());
            Log.Debug(TAG, "GEO_PERIOD : " + Convert.ToInt32(Application.Current.Properties["geoPeriod_setting"]).ToString());

            // for alarm dialogue countdown
            secToAlarm = Convert.ToInt32(Application.Current.Properties["secToAlarm_setting"]);

            // to reset the above with
            savedSecToAlarm = secToAlarm;

            // for btnActivate  countdown
            countDownActivateBtn = (60 * Convert.ToInt32(Application.Current.Properties["geoPeriod_setting"])); // right now seconds

            // to reset the above with
            savedCountDownActivateBtn = countDownActivateBtn;

            // for geoPeriod interval in milliseconds
            geoPeriod = 1000 * (60 * Convert.ToInt32(Application.Current.Properties["geoPeriod_setting"]));

            // to reset the above with
            savedGeoPeriod = geoPeriod;

            Log.Debug(TAG, "countDownActivateBtn: " + countDownActivateBtn);
            Log.Debug(TAG, "geoPeriod variabel: " + geoPeriod);
            Log.Debug(TAG, "SecToAlarm variable: " + secToAlarm);

            monitorTimer.Interval = geoPeriod;

            // start
            monitorTimer.Start();
            guiTimer.Start();

        }



        void CountDown(object sender, ElapsedEventArgs e)
        {


            TimeSpan time = TimeSpan.FromSeconds(countDownActivateBtn);
            string str = time.ToString(@"mm\:ss");

            MessagingCenter.Send<Object, string>(this, "SecToCheck", str);
            countDownActivateBtn--;
        }


        async public void Session(object sender, ElapsedEventArgs e)
        {
            mainActivity.RunOnUiThread(async () =>
          {
              if (CheckInactivity())
              {
                  alertTimer.Start();
                  alertBool = true;

                  // user alarm dialogue 
                  alert.SetTitle("Are You OK?");
                  alert.SetMessage("\n            " + secToAlarm + " Seconds to ALARM ...");
                  alert.SetButton("I´M OK!", (c, ev) =>
                  {
                      alertBool = false;
                      AlertConfirmation("GOT IT!", "                      YOU ARE OK!", 1500);
                  });

                  alert.Show();
                  //waiting before alarming
                  await Task.Delay(1000 * savedSecToAlarm); //wait for X seconds
                  if (!alertBool) //if  I´m ok button not pressed
                  {
                      alert.Dismiss();     //removing dialogue
                  }
                  else
                  {
                      StopMonitor();
                      alert.Dismiss();
                      AlertContacts();
                  }
              }
          });

        }

        async public void StopMonitor()
        {
            monitorTimer.Stop();
            guiTimer.Stop();
            MessagingCenter.Send<Object>(this, "InactivityDetected"); //setting button to "Activate
            SendMessages();
            await App.Database.DeleteAllGeoLocationItemAsync();
        }

        /*
                public void SetAutodial()
                {
                    CrossMessaging.Current.Settings().Phone.AutoDial = true;

                    if (CrossMessaging.Current.Settings().Phone.AutoDial == true)
                    {
                        Console.WriteLine("*AutoDial enabled*");
                    }
                }

            */
        public bool CheckInactivity()
        {
            bool inactivityDetected = false;
            this.tempGeoPos = GetCurrentGeoPos();

            // these events are on a background thread, need to update on the UI thread

            if (tempGeoPos.Latitude.Substring(0, 6).Equals(savedLat.Substring(0, 6)) && tempGeoPos.Longitude.Substring(0, 6).Equals(savedLong.Substring(0, 6)))
            {
                tempGeoPos.Info = "INACTIVITY DETECTED";
                inactivityDetected = true; //ALARM
            }

            savedLat = tempGeoPos.Latitude;
            savedLong = tempGeoPos.Longitude;

            SaveToDb(tempGeoPos);
            return inactivityDetected;

        }


        // called from alertTimer counting down alertDialog
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            secToAlarm--;

            if (secToAlarm < 0)
            {
                alertTimer.Stop();
                secToAlarm = savedSecToAlarm;
            }
            else
            {
                alert.SetMessage("\n            " + secToAlarm + " Seconds to ALARM ...");
            }
        }


        // creating an alert that disspears after 4 sec
        public async void AlertConfirmation(string title, string message, int messageDuration)
        {
            int duration = messageDuration;
            AlertDialog.Builder alertConfirmBuilder = new AlertDialog.Builder(Xamarin.Essentials.Platform.CurrentActivity);
            AlertDialog alertConfirm = alertConfirmBuilder.Create();
            alertConfirm.SetTitle(title);
            alertConfirm.SetMessage(message);
            alertConfirm.SetCancelable(false);
            alertConfirm.Create();
            alertConfirm.Show();
            await Task.Delay(messageDuration);
            alertConfirm.Dismiss();
        }





        async void AlertContacts()
        {
            Console.Write("A L A R M I N G !");
            AlertConfirmation("A L A R M I N G !", "Contacts will receive \nSMS & Phone call shortly", 2500);
            await callAndSms.SmsToContact();
            await callAndSms.CallContacts();
        }





        public async void SendMessages()
        {

            string msg;
            msg = await deviceToCloud.SendListToIotHubAsync();
            System.Diagnostics.Debug.WriteLine("{0} > Sending message[FROM MONITOR]: {1}", DateTime.Now, msg);


        }
    }
}
