﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Locations;
using Android.Util;
using FallDetectionApp.Droid.Services;
using FallDetectionApp.Models;
using FallDetectionApp.ViewModels;
using Plugin.Messaging;
using Xamarin.Essentials;
using Xamarin.Forms;
using Application = Xamarin.Forms.Application;

[assembly: Dependency(typeof(FallDetectionApp.Droid.MainActivity))]
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

        private int geoPeriod;
        private int savedGeoPeriod;
        private int secToAlarm;
        private int savedSecToAlarm;
        private int countDownActivateBtn;
        private int SavedCountDownActivateBtn;
        public bool alertBool;

        private AlertDialog alert;
        private AlertDialog.Builder dialog;



        private GeoLocation currentGeoPos;
        private GeoLocation tempGeoPos;



        public Monitor(MainActivity mainActivity, CallAndSms callAndSms)
        {
            this.mainActivity = mainActivity;
            this.callAndSms = callAndSms;
            initializeComponents();


        }


        public void initializeComponents()
        {
            currentGeoPos = new GeoLocation { Latitude = "no lat yet", Longitude = "no long yet" };
            sessionStart = false;

            //geoPeriod = 1;
            //savedGeoPeriod = geoPeriod;
            //secToAlarm = 10;
            //savedSecToAlarm = secToAlarm;
            //countDownActivateBtn = secToAlarm;
            //SavedCountDownActivateBtn = countDownActivateBtn;








            // create alert Dialogue
            dialog = new AlertDialog.Builder(Xamarin.Essentials.Platform.CurrentActivity);
            alert = dialog.Create();

            // create GuiTimer
            guiTimer = new Timer();

            // Tell the timer what to do when it elapses
            guiTimer.Elapsed += new ElapsedEventHandler(CountDown);
            guiTimer.Enabled = false;
            guiTimer.Interval = 1000;

            // create monitorTimer
            monitorTimer = new Timer();

            // Tell the timer what to do when it elapses
            monitorTimer.Elapsed += new ElapsedEventHandler(Session);
            monitorTimer.Enabled = false;




            SetDeviceId();


        }



        public void SaveToDb(GeoLocation currentGeo)
        {
            Console.WriteLine("SAVING to Db and SENDING message" + "\n");
            App.Database.SaveGeoLocationItemAsync(currentGeo);
            MessagingCenter.Send<Object>(this, "latestGeo");
            countDownActivateBtn = SavedCountDownActivateBtn;

        }


        public void SetGeoInstance(LocationChangedEventArgs e)
        {

            var location = e.Location;

            //parse for not to sensitive accuracy
            string lati = $"{location.Latitude}".Substring(0, 7);
            string longi = $"{location.Longitude}".Substring(0, 7);

            //get date
            DateTime dateTime = DateTime.Now.ToLocalTime();
            string date_string = dateTime.Date.ToString(); // just date?
            currentGeoPos.Latitude = lati;
            currentGeoPos.Longitude = longi;
            currentGeoPos.TimeDate = date_string;
            currentGeoPos.DeviceId = GetDeviceId();

        }

        public GeoLocation GetCurrentGeoPos()
        {
            return currentGeoPos;
        }


        public void SetDeviceId()
        {
            var deviceId = Preferences.Get("my_deviceId", string.Empty);
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = System.Guid.NewGuid().ToString();
                Preferences.Set("deviceId", deviceId);
            }
        }


        public string GetDeviceId()
        {
            return Preferences.Get("deviceId", string.Empty);
        }



        public int GetGeoPeriod()
        {
            return geoPeriod;
        }

        public void StartMonitor()
        {

            Log.Debug(TAG, "SEC_TO_ALARM : " + Convert.ToInt32(Application.Current.Properties["secToAlarm_setting"]).ToString());
            Log.Debug(TAG, "GEO_PERIOD : " + Convert.ToInt32(Application.Current.Properties["geoPeriod_setting"]).ToString());

            secToAlarm = Convert.ToInt32(Application.Current.Properties["secToAlarm_setting"]);
            savedSecToAlarm = secToAlarm;
            countDownActivateBtn = (60 * Convert.ToInt32(Application.Current.Properties["geoPeriod_setting"])); // right now seconds
            SavedCountDownActivateBtn = countDownActivateBtn;
            geoPeriod = 1000 * (60 * Convert.ToInt32(Application.Current.Properties["geoPeriod_setting"]));
            savedGeoPeriod = geoPeriod;

            Log.Debug(TAG, "countDownActivateBtn: " + countDownActivateBtn);
            Log.Debug(TAG, "geoPeriod variabel: " + geoPeriod);
            Log.Debug(TAG, "SecToAlarm variable: " + secToAlarm);

            monitorTimer.Interval = geoPeriod;
            sessionStart = true;


            monitorTimer.Start();
            guiTimer.Start();
            this.tempGeoPos = GetCurrentGeoPos();
            SaveToDb(tempGeoPos);


        }



        void CountDown(object sender, ElapsedEventArgs e)
        {

            MessagingCenter.Send<Object, string>(this, "SecToCheck", countDownActivateBtn.ToString());
            countDownActivateBtn--;


        }



        async public void Session(object sender, ElapsedEventArgs e)
        {
            mainActivity.RunOnUiThread(async () =>
            {
                if (CheckInactivity())
                {
                    ;
                    if (DidYouFallAlert()) // Alarm
                    {
                        StopMonitor();
                        MessagingCenter.Send<Object>(this, "InactivityDetected"); //setting button to "Activate
                        AlertContacts();

                    }

                    // do domething....

                }


            });

        }


        public void Alarm()
        {


        }



        public void StopMonitor()
        {
            monitorTimer.Stop();
            guiTimer.Stop();
            sessionStart = false;
        }


        public void SetAutodial()
        {
            CrossMessaging.Current.Settings().Phone.AutoDial = true;

            if (CrossMessaging.Current.Settings().Phone.AutoDial == true)
            {
                Console.WriteLine("*AutoDial enabled*");
            }
        }



        // triggered from pre-created Timer started from messageCenter in HandleLocationChanged in Mainactivity

        //The Did You Fall monitoring:

        public bool CheckInactivity()
        {

            bool inactivityDetected = false;
            string txtNewRow = "\n";
            string txtPrevLat = "Previous Lat: ";
            string txtPrevLong = "Previous Long: ";
            string txtNotMoved = "DEVICE HAS NOT MOVED FOR" + geoPeriod + " SECONDS ! ! !\n";
            string space16 = "                ";
            string space11 = "          ";



            // these events are on a background thread, need to update on the UI thread

            this.tempGeoPos = GetCurrentGeoPos();

            if (sessionStart) //if first time - no saved Lat or Long exists
            {
                savedLat = tempGeoPos.Latitude;
                savedLong = tempGeoPos.Longitude;

                sessionStart = false;
                //assigns info
                tempGeoPos.Info = txtPrevLat + space16 + txtPrevLong + txtNewRow + savedLat + space16 + space11 + savedLong + txtNewRow;

                inactivityDetected = false;


            }
            else if (tempGeoPos.Latitude.Equals(savedLat) && tempGeoPos.Longitude.Equals(savedLong))
            {
                savedLat = tempGeoPos.Latitude;
                savedLong = tempGeoPos.Longitude;

                tempGeoPos.Info =
                txtPrevLat + space16 + txtPrevLong + txtNewRow +
                savedLat + space16 + space11 + savedLong + txtNewRow;
                //assigns info
                tempGeoPos.Info = txtPrevLat + space16 + txtPrevLong + txtNewRow + savedLat + space16 + space11 + savedLong + txtNewRow;

                inactivityDetected = true; //ALARM
            }

            SaveToDb(tempGeoPos);
            return inactivityDetected;

        }




        // creating an alert that disspears after 4 sec
        public async void AlertConfirmation(string title, string message)
        {
            AlertDialog.Builder alertConfirmBuilder = new AlertDialog.Builder(Xamarin.Essentials.Platform.CurrentActivity);
            AlertDialog alertConfirm = alertConfirmBuilder.Create();
            alertConfirm.SetTitle(title);
            alertConfirm.SetMessage(message);
            alertConfirm.SetCancelable(false);
            alertConfirm.Create();
            alertConfirm.Show();
            await Task.Delay(4000);
            // After some action
            alertConfirm.Dismiss();
        }



        public bool DidYouFallAlert()
        {

            // create alertTimer with OnTimedEvent with Alarm User Dialogue countdown

            alertTimer = new Timer();
            alertTimer.Elapsed += OnTimedEvent;

            alertTimer.Interval = 1000; //seconds
            alertTimer.Enabled = false;
            alertTimer.Start();
            alertBool = false;

            // user alarm dialogue 
            alert.SetTitle("Are You ok?");
            alert.SetMessage("Device did not move for " + 60 * savedGeoPeriod + " Seconds...");
            alert.SetButton("I´M OK!", (c, ev) =>
            {
                AlertConfirmation("GOT IT!", "                      YOU ARE OK!");
                alertBool = false;
            });

            alert.Show();

            //waiting before alarming
            Task.Delay(1000 * savedSecToAlarm); //wait for X seconds
            if (alertBool) //if  I´m ok button not pressed
            {
                alert.Dismiss();     //removing dialogue
            }

            return alertBool;

        }

        // called from alertTimer counting down alertDialog
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {

            secToAlarm--;
            if (secToAlarm < 0 || alertBool == true)
            {
                alertTimer.Stop();
                secToAlarm = savedSecToAlarm;
            }
            else
            {
                mainActivity.RunOnUiThread(() =>
                {
                    //Console.WriteLine("Timer: " + secToAlarm.ToString() + " SECONDS");
                    alert.SetMessage("\n            " + secToAlarm.ToString() + " Seconds to ALARM...");
                });
            }
        }

        async void AlertContacts()
        {
            Console.Write("A L A R M I N G !");
            AlertConfirmation("A L A R M I N G !", "Contacts will receive \nSMS & Phone call shortly");
            await callAndSms.SmsToContact();
            await callAndSms.CallContacts();
        }




    }
}
