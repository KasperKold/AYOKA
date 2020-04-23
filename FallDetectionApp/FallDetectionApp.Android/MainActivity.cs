using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Content;
using Xamarin.Forms.PlatformConfiguration;
using Android;
using Android.Util;
using Android.Locations;
using System.Timers;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using FallDetectionApp.Models;
using FallDetectionApp.Services;
using Xamarin.Forms;
using System.Threading.Tasks;
using Android.Icu.Util;
using View = Android.Views.View;
using FallDetectionApp.Views;
using FallDetectionApp.ViewModels;
using System.Security.Policy;
using Plugin.Messaging;
using Android.Content;
using System.Linq.Expressions;
using Xamarin.Essentials;
using System.Collections.Generic;
using Microsoft.Azure.Devices.Client;
using FallDetectionApp.Droid.Services;

[assembly: Dependency(typeof(FallDetectionApp.Droid.MainActivity))]
namespace FallDetectionApp.Droid
{


    [Activity(Label = "FallDetectionApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly string TAG = "Log MainActivity";
        static readonly int RC_REQUEST_LOCATION_PERMISSION = 1000;

        //static readonly int RC_REQUEST_PHONECALL_PERMISSION = 1000;
        //static readonly int RC_REQUEST_PHONESMS_PERMISSION = 1000;
        //static readonly int RC_REQUEST_READPHONESTATE_PERMISSION = 1000;

        static readonly string[] REQUIRED_PERMISSIONS = { Manifest.Permission.AccessFineLocation };
        //static readonly string[] REQUIRED_PHONECALL_PERMISSIONS = { Manifest.Permission.CallPhone };
        //static readonly string[] REQUIRED_PHONESMS_PERMISSIONS = { Manifest.Permission.SendSms };
        //static readonly string[] REQUIRED_READPHONESTATE_PERMISSIONS = { Manifest.Permission.ReadPhoneState };



        private string savedLat;
        private string savedLong;
        private int notMovedCounter;
        public bool readyForSession;


        private Timer monitorTimer;
        private Timer alertTimer;
        private int defaultInterval;
        private int timerInterval;
        private int countSeconds;

        //private string sessionStartDateTime;

        private AlertDialog alert;
        private AlertDialog.Builder dialog;
        public bool alertBool;


        private GeoLocation currentGeoPos;
        private GeoLocation tempGeoPos;
        private UiLocationHandler iUiImplementation;// will be removed



        static string deviceId = "PederTestDevice";
        static string deviceKey = "kYMV9WOF4PSifDtML6K8JMO07ORitGaazeoWsCZHFBA="; //primarykey
        static string hostName = "IotFallApp.azure-devices.net";
        // HostName=IotFallApp.azure-devices.net;DeviceId=PederTestDevice;SharedAccessKey=kYMV9WOF4PSifDtML6K8JMO07ORitGaazeoWsCZHFBA=


        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);



            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);

            // will not be used later
            iUiImplementation = new UiLocationHandler();
            iUiImplementation.setMainActivity(this);
            iUiImplementation = DependencyService.Get<IUiHandler>() as UiLocationHandler;

            initializeComponents();
            LoadApplication(new App());




            // This event fires when the ServiceConnection lets the client (our App class) know that
            // the Service is connected. We use this event to start updating the UI with location
            // updates from the Service
            LocationHandler.Current.LocationServiceConnected += (sender, e) =>
            {
                Log.Debug(TAG, "ServiceConnected Event Raised");
                // notifies us of location changes from the system
                LocationHandler.Current.LocationService.LocationChanged += HandleLocationChanged;
                //notifies us of user changes to the location provider (ie the user disables or enables GPS)
                LocationHandler.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
                LocationHandler.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
                // notifies us of the changing status of a provider (ie GPS no longer available)
                LocationHandler.Current.LocationService.StatusChanged += HandleStatusChanged;
            };
            Log.Debug(TAG, "OnCreate: Location app is coming to life.");

            // Start the location service:
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
            {
                Log.Debug(TAG, "User already has granted permission.");
                LocationHandler.StartLocationService();
            }
            else
            {
                Log.Debug(TAG, "Have to request permission from the user. ");
                RequestLocationPermission();
            }





            ////Check for phone call permission
            //if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.CallPhone) == (int)Permission.Granted)
            //{
            //    Log.Debug(TAG, "User already granted PhoneCall permission. ");
            //}
            //else
            //{
            //    Log.Debug(TAG, "Have to request PhoneCall permission from the user. ");
            //    RequestPhoneCallPermission();
            //}

            ////Check for sms permission
            //if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.SendSms) == (int)Permission.Granted)
            //{
            //    Log.Debug(TAG, "User already granted SendSMS permission.");
            //}
            //else
            //{
            //    Log.Debug(TAG, "Have to request SendSMS permission from the user. ");
            //    RequestPhoneSMSPermission();
            //}

            ////Check for read phone state permission

            //if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadPhoneState) == (int)Permission.Granted)
            //{
            //    Log.Debug(TAG, "User already granted READPHONESTATE permission.");
            //}
            //else
            //{
            //    Log.Debug(TAG, "Have to request READPHONESTATE permission from the user. ");
            //    RequestReadPhoneStatePermission();
            //}



            CrossMessaging.Current.Settings().Phone.AutoDial = true;

            if (CrossMessaging.Current.Settings().Phone.AutoDial == true)
            {
                Console.WriteLine("*AutoDial enabled*");
            }

            //  from btnActivate
            MessagingCenter.Subscribe<GeoDataViewModel>(this, "Activate", (sender) =>
            {
                SendMessages();
                Console.WriteLine("STARTING Monitor");
                monitorTimer.Start();
            });

            // from btnActivate
            MessagingCenter.Subscribe<GeoDataViewModel>(this, "Deactivate", (sender) =>
            {
                SendMessages();
                Console.WriteLine("STOPPING Monitor");
                monitorTimer.Stop();
            });


        }

        public void initializeComponents()
        {

            currentGeoPos = new GeoLocation { Latitude = "no lat yet", Longitude = "no long yet" };
            notMovedCounter = 0;

            // create alert Dialogue
            dialog = new AlertDialog.Builder(this);
            alert = dialog.Create();

            // create monitorTimer
            defaultInterval = 5000;
            monitorTimer = new Timer();

            // Tell the timer what to do when it elapses
            monitorTimer.Elapsed += new ElapsedEventHandler(monitorSession);
            monitorTimer.Interval = defaultInterval;
            monitorTimer.Enabled = false;

            setDeviceId();
        }


        // not used atm
        public void setTimerInterval(int inputInterval)
        {
            timerInterval = inputInterval;

            // Set it to go off every n seconds 1s =10000
            monitorTimer.Interval = timerInterval;
        }

        // not used atm
        public int getTimerInterval()
        {
            return timerInterval;

        }

        public void setDeviceId()
        {
            var deviceId = Preferences.Get("my_deviceId", string.Empty);
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = System.Guid.NewGuid().ToString();
                Preferences.Set("deviceId", deviceId);
            }
        }

        public string getDeviceId()
        {
            return Preferences.Get("deviceId", string.Empty);
        }







        protected override void OnResume()
        {
            Log.Debug(TAG, "OnResume: Location app is moving into foreground");
            base.OnResume();
        }

        protected override void OnPause()
        {
            Log.Debug(TAG, "OnPause: Location app is moving to background");
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            Log.Debug(TAG, "OnDestroy: Location app is becoming inactive");
            base.OnDestroy();

            // Stop the location service:
            LocationHandler.StopLocationService();

        }



        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == RC_REQUEST_LOCATION_PERMISSION)
            {
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    Log.Debug(TAG, "User granted permission for location.");
                    LocationHandler.StartLocationService();

                }
                else
                {
                    Log.Warn(TAG, "User did not grant permission for the location.");

                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            }


        }

        void RequestLocationPermission()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            {
                var layout = FindViewById(Android.Resource.Id.Content);
                Snackbar.Make(layout,
                              Resource.String.permission_location_rationale,
                              Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.ok,
                                   new Action<View>(delegate
                                   {
                                       ActivityCompat.RequestPermissions(this, REQUIRED_PERMISSIONS,
                                                                         RC_REQUEST_LOCATION_PERMISSION);
                                   })
                                  ).Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, REQUIRED_PERMISSIONS, RC_REQUEST_LOCATION_PERMISSION);
            }
        }

        //void RequestPhoneCallPermission()
        //{
        //    if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.CallPhone))
        //    {
        //        var layout = FindViewById(Android.Resource.Id.Content);
        //        Snackbar.Make(layout,
        //                      Resource.String.permission_location_rationale,
        //                      Snackbar.LengthIndefinite)
        //                .SetAction(Resource.String.ok,
        //                           new Action<View>(delegate
        //                           {
        //                               ActivityCompat.RequestPermissions(this, REQUIRED_PHONECALL_PERMISSIONS,
        //                                                                 RC_REQUEST_PHONECALL_PERMISSION);
        //                           })
        //                          ).Show();
        //    }
        //    else
        //    {
        //        ActivityCompat.RequestPermissions(this, REQUIRED_PHONECALL_PERMISSIONS, RC_REQUEST_PHONECALL_PERMISSION);
        //    }
        //}

        //void RequestPhoneSMSPermission()
        //{
        //    if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.SendSms))
        //    {
        //        var layout = FindViewById(Android.Resource.Id.Content);
        //        Snackbar.Make(layout,
        //                      Resource.String.permission_location_rationale,
        //                      Snackbar.LengthIndefinite)
        //                .SetAction(Resource.String.ok,
        //                           new Action<View>(delegate
        //                           {
        //                               ActivityCompat.RequestPermissions(this, REQUIRED_PHONESMS_PERMISSIONS,
        //                                                                 RC_REQUEST_PHONESMS_PERMISSION);
        //                           })
        //                          ).Show();
        //    }
        //    else
        //    {
        //        ActivityCompat.RequestPermissions(this, REQUIRED_PHONESMS_PERMISSIONS, RC_REQUEST_PHONESMS_PERMISSION);
        //    }
        //}

        //void RequestReadPhoneStatePermission()
        //{
        //    if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadPhoneState))
        //    {
        //        var layout = FindViewById(Android.Resource.Id.Content);
        //        Snackbar.Make(layout,
        //                      Resource.String.permission_location_rationale,
        //                      Snackbar.LengthIndefinite)
        //                .SetAction(Resource.String.ok,
        //                           new Action<View>(delegate
        //                           {
        //                               ActivityCompat.RequestPermissions(this, REQUIRED_READPHONESTATE_PERMISSIONS,
        //                                                                 RC_REQUEST_READPHONESTATE_PERMISSION);
        //                           })
        //                          ).Show();
        //    }
        //    else
        //    {
        //        ActivityCompat.RequestPermissions(this, REQUIRED_READPHONESTATE_PERMISSIONS, RC_REQUEST_READPHONESTATE_PERMISSION);
        //    }
        //}



        // triggered from precreated Timer started from messageCenter

        //The Did You Fall monitoring:

        public void monitorSession(object sender, ElapsedEventArgs e)
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
            RunOnUiThread(async () =>
            {
                this.tempGeoPos = getCurrentGeoPos();


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
                    didYouFallAlert();
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
                saveToDb(tempGeoPos);
                iUiImplementation.setCurrentGeoPos(tempGeoPos);
                MessagingCenter.Send<Object>(this, "latestGeo");

            });
        }


        async void alertContacts()
        {
            Console.Write("A L A R M I N G !");
            alertConfirmation("A L A R M I N G !", "Contacts will receive \nSMS & Phone call shortly");
            //await SmsToContact("Hello, this is a test of sending an sms to all contacts in my FallApp."); //Alexa: +46760996722, Peder: +46733241061

            await CallContacts();
        }

        async void alertConfirmation(string title, string message)
        {
            AlertDialog.Builder alertConfirmBuilder = new AlertDialog.Builder(this);
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
        async void didYouFallAlert()
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

                alertConfirmation("GOT IT!", "                      YOU ARE OK!");
                alertBool = true;

            });

            alert.Show();

            //waiting before calling automatic alarm
            await Task.Delay(10000); //wait for ten seconds
            if (!alertBool)
            {

                alert.Dismiss();     //removing dialogue
                alertContacts();

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
                this.RunOnUiThread(() =>
                {
                    //Console.WriteLine("Timer: " + countSeconds.ToString() + " SECONDS");
                    alert.SetMessage("\n            " + countSeconds.ToString() + " seconds to ALARM...");
                });
            }
        }




        public void saveToDb(GeoLocation currentGeo)
        {
            Console.WriteLine("SAVING to Db " + "\n");
            App.Database.SaveGeoLocationItemAsync(currentGeo);
        }


        public void setGeoInstance(string lati, string longi, string dateTime)
        {
            currentGeoPos.Latitude = lati;
            currentGeoPos.Longitude = longi;
            currentGeoPos.TimeDate = dateTime;
            currentGeoPos.DeviceId = getDeviceId();
            //Console.WriteLine("DEVICE ID: " + currentGeoPos.DeviceId);

        }

        public GeoLocation getCurrentGeoPos()
        {
            return currentGeoPos;
        }


        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            var location = e.Location;
            string lati = $"{location.Latitude}".Substring(0, 7);
            string longi = $"{location.Longitude}".Substring(0, 7);

            DateTime dateTime = DateTime.Now.ToLocalTime();
            setGeoInstance(lati, longi, dateTime.ToString());

            // Send message - ready to monitor
            MessagingCenter.Send<Object>(this, "GeoMonitorReady");

        }


        public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {
            Log.Debug(TAG, "Location provider disabled event raised");
        }

        public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {
            Log.Debug(TAG, "Location provider enabled event raised");
        }

        public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Log.Debug(TAG, "Location status changed, event raised");
        }




        // Test Call function
        public async Task<bool> CallContacts()
        {
            await TextToSpeech.SpeakAsync("Hello World");
            await TextToSpeech.SpeakAsync("Detta är ett automatiskt meddelande");
            //Checking for permission.

            /* try
             {
                 var permissions = await Permissions.CheckStatusAsync<Permissions.Phone>();
                 if (permissions != PermissionStatus.Granted)
                 {
                     permissions = await Permissions.RequestAsync<Permissions.Phone>();
                 }

                 if (permissions != PermissionStatus.Granted)
                 {
                     Log.Verbose(TAG,  + "Permission to use native Phone function on the phone was denied.");
                 }
             }
             catch (Exception ex)
             {
                 Log.Verbose(TAG,  + $"Something is wrong: {ex.Message}");
             }
             */

            // If permission has been granted, phone call commences

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
                    for (int j = 0; i < 6; j++)
                    {
                        await TextToSpeech.SpeakAsync("Hello World         ");
                        await TextToSpeech.SpeakAsync("This is an automatic emergency message from a mobile application. Your friend Peder needs help please get help to this location: Latitide: 55.8888 and Longitude: 13.4545 ");
                    }
                    await Task.Delay(20000);



                }

            }




            return await Task.FromResult(true);
        }

        // SMS function fetching contact from database
        public async Task<bool> SmsToContact(string text)
        {
            /*
            //Checking for permission.
            try
            {
                var permissions = await Permissions.CheckStatusAsync<Permissions.Sms>();
                if (permissions != PermissionStatus.Granted)
                {
                    permissions = await Permissions.RequestAsync<Permissions.Sms>();
                }

                if (permissions != PermissionStatus.Granted)
                {
                     Log.Verbose(TAG, + "Permission to use native SMS function on the phone was denied.");
                }
            }
            catch (Exception ex)
            {
                Log.Verbose(TAG,  + $"Something is wrong: {ex.Message}");
            }
            */


            // If permission has been granted, sms-messenging commences

            List<Models.Contact> contactsFromLocalDB = await App.Database.GetItemsAsync();
            //&& !(contactsFromLocalDB[i].PhoneNr == "")
            for (int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                var smsMessenger = CrossMessaging.Current.SmsMessenger;
                if (smsMessenger.CanSendSmsInBackground)
                {
                    smsMessenger.SendSmsInBackground(contactsFromLocalDB[i].PhoneNr, text);
                }
            }

            // Debug.WriteLine("Testing to find contact numbers in database: ");
            for (int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                Log.Verbose(TAG, "SMS Contacts:" + contactsFromLocalDB[i].Name + " " + contactsFromLocalDB[i].PhoneNr); ;
            }

            return await Task.FromResult(true);
        }



        async Task<bool> getSessionGeo()
        {
            List<Models.GeoLocation> sessionGeoFromLocalDB = await App.Database.GetGeoLocationItemsAsync();
            return await Task.FromResult(true);
        }




        async void SendMessages()
        {
            var deviceToCloud = new DeviceToCloud(deviceId, deviceKey, hostName);

            // while (true)
            // {
            string msg;
            msg = await deviceToCloud.SendFakeDeviceToCloudDataAsync();
            System.Diagnostics.Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, msg);
            //await Task.Delay(3000);
            //}

        }






    }

}