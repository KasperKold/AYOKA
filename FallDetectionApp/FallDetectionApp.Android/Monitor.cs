using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Util;
using FallDetectionApp.Droid.Services;
using FallDetectionApp.Models;
using Plugin.Messaging;
using Xamarin.Essentials;
using Xamarin.Forms;

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
        private int notMovedCounter;
        public bool readyForSession;

        private Timer monitorTimer;
        private Timer alertTimer;

        private int defaultInterval;
        private int timerInterval;
        private int countSeconds;

        private AlertDialog alert;
        private AlertDialog.Builder dialog;
        public bool alertBool;

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
            notMovedCounter = 0;

            // create alert Dialogue
            dialog = new AlertDialog.Builder(Xamarin.Essentials.Platform.CurrentActivity);
            alert = dialog.Create();

            // create monitorTimer
            defaultInterval = 5000;
            monitorTimer = new Timer();

            // Tell the timer what to do when it elapses
            monitorTimer.Elapsed += new ElapsedEventHandler(MonitorSession);
            monitorTimer.Interval = defaultInterval;
            monitorTimer.Enabled = false;

            SetDeviceId();
        }



        // triggered from precreated Timer started from messageCenter

        //The Did You Fall monitoring:

        public void MonitorSession(object sender, ElapsedEventArgs e)
        {
            string txtCounter1plus = "COUNTER: +1 --> : ";
            string txtCounter1minus = "COUNTER: -1 --> : ";
            string txtCounter = "COUNTER: ";
            string txtStarRow = "******************************************************\n";
            string txtNewRow = "\n";
            string txtPrevLat = "Previous Lat: ";
            string txtPrevLong = "Previous Long: ";
            string txtNotMoved = "DEVICE HAS NOT MOVED FOR 25 SECONDS ! ! !\n";
            string space16 = "                ";
            string space11 = "          ";



            DateTime dateTime = DateTime.Now.ToLocalTime();
            //string time_string = dateTime.ToString("yyyy-MM-dd");

            string date_string = dateTime.Date.ToString(); // just date


            // Log.Debug(TAG, "Foreground updating");

            // these events are on a background thread, need to update on the UI thread
            mainActivity.RunOnUiThread(async () =>
            {
                this.tempGeoPos = GetCurrentGeoPos();


                if (notMovedCounter == 0)
                {
                    savedLat = tempGeoPos.Latitude;
                    savedLong = tempGeoPos.Longitude;
                    notMovedCounter++;

                    Console.WriteLine(txtNewRow + txtCounter1plus + notMovedCounter);

                    //assigns info
                    tempGeoPos.Info =
                        txtPrevLat + space16 + txtPrevLong + txtNewRow +
                        savedLat + space16 + space11 + savedLong + txtNewRow +
                        txtCounter1plus + notMovedCounter;

                }
                else if (notMovedCounter >= 4)
                {
                    savedLat = tempGeoPos.Latitude;
                    savedLong = tempGeoPos.Longitude;

                    Console.WriteLine(txtNewRow + txtStarRow);
                    Console.WriteLine(txtCounter + notMovedCounter);
                    Console.WriteLine(txtNotMoved);
                    Console.WriteLine(txtStarRow + txtNewRow);

                    currentGeoPos.Info =
                    txtStarRow + txtNotMoved + txtStarRow +
                    txtPrevLat + space16 + txtPrevLong + txtNewRow +
                    savedLat + space16 + space11 + savedLong + txtNewRow +
                    txtCounter + notMovedCounter;

                    notMovedCounter = 0;
                    DidYouFallAlert();
                }
                else
                {

                    // if both lat and long are equal to the previous lat and long - device has not moved

                    if (tempGeoPos.Latitude.Equals(savedLat) && tempGeoPos.Longitude.Equals(savedLong))
                    {
                        savedLat = tempGeoPos.Latitude;
                        savedLong = tempGeoPos.Longitude;
                        notMovedCounter++;

                        Console.WriteLine(txtNewRow + txtCounter1plus + notMovedCounter + txtNewRow);

                        currentGeoPos.Info =
                        txtPrevLat + space16 + txtPrevLong + txtNewRow +
                        savedLat + space16 + space11 + savedLong + txtNewRow +
                        txtCounter1plus + notMovedCounter;

                    }
                    else
                    {
                        notMovedCounter--;

                        Console.WriteLine(txtNewRow + txtCounter1minus + notMovedCounter);

                        tempGeoPos.Info =
                        txtPrevLat + space16 + txtPrevLong + txtNewRow +
                        savedLat + space16 + space11 + savedLong + txtNewRow +
                        txtCounter1minus + notMovedCounter;

                    }
                }
                SaveToDb(tempGeoPos);
                //iUiImplementation.setCurrentGeoPos(tempGeoPos);
                MessagingCenter.Send<Object>(this, "latestGeo");

            });
        }


        async void AlertContacts()
        {
            Console.Write("A L A R M I N G !");
            AlertConfirmation("A L A R M I N G !", "Contacts will receive \nSMS & Phone call shortly");
            await callAndSms.SmsToContact();
            await callAndSms.CallContacts();
        }


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


        // called from monitorsession when geo locaction has not changed 
        async void DidYouFallAlert()
        {

            // create alertTimer
            alertTimer = new Timer();
            alertTimer.Elapsed += OnTimedEvent;
            countSeconds = 10;
            alertTimer.Interval = 1000;
            alertTimer.Enabled = false;
            alertTimer.Start();
            alertBool = false;

            // userdialog - dismissed from user or when alertContacts() is called -  alarming!
            alert.SetTitle("Are You ok?");
            alert.SetMessage("");
            alert.SetButton("I´M OK!", (c, ev) =>
            {
                AlertConfirmation("GOT IT!", "                      YOU ARE OK!");
                alertBool = true;

            });

            alert.Show();

            //waiting before calling automatic alarm
            await Task.Delay(10000); //wait for ten seconds
            if (!alertBool)
            {
                alert.Dismiss();     //removing dialogue
                AlertContacts();
            }
        }

        // called from alertTimer counting down alertDialog
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            countSeconds--;
            if (countSeconds < 0 || alertBool == true)
            {
                alertTimer.Stop();
            }
            else
            {
                mainActivity.RunOnUiThread(() =>
                {
                    //Console.WriteLine("Timer: " + countSeconds.ToString() + " SECONDS");
                    alert.SetMessage("\n            " + countSeconds.ToString() + " seconds to ALARM...");
                });
            }
        }




        public void SaveToDb(GeoLocation currentGeo)
        {
            Console.WriteLine("SAVING to Db " + "\n");
            App.Database.SaveGeoLocationItemAsync(currentGeo);
        }


        public void SetGeoInstance(string lati, string longi, string dateTime)
        {
            currentGeoPos.Latitude = lati;
            currentGeoPos.Longitude = longi;
            currentGeoPos.TimeDate = dateTime;
            currentGeoPos.DeviceId = GetDeviceId();
            //Console.WriteLine("DEVICE ID: " + currentGeoPos.DeviceId);

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


        public void StartMonitor()
        {
            monitorTimer.Start();
        }

        public void StopMonitor()
        {
            monitorTimer.Stop();
        }


        public void SetAutodial()
        {
            CrossMessaging.Current.Settings().Phone.AutoDial = true;

            if (CrossMessaging.Current.Settings().Phone.AutoDial == true)
            {
                Console.WriteLine("*AutoDial enabled*");
            }
        }
    }
}
