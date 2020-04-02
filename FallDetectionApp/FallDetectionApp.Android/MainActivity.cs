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

[assembly: Dependency(typeof(FallDetectionApp.Droid.MainActivity))]
namespace FallDetectionApp.Droid
{


    [Activity(Label = "FallDetectionApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly string TAG = "Log MainActivity";
        static readonly int RC_REQUEST_LOCATION_PERMISSION = 1000;
        static readonly string[] REQUIRED_PERMISSIONS = { Manifest.Permission.AccessFineLocation };


        private Timer myTimer;
        private string savedLat;
        private string savedLong;
        private int notMovedCounter;
        public bool readyForSession;
        private int defaultInterval;
        private int timerInterval;

        private string sessionStartDateTime;


        private GeoLocation currentGeoPos;
        private GeoLocation tempGeoPos;
        private UiLocationHandler iUiImplementation;// will be removed


        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            //Store our interface class.


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

        }

        public void initializeComponents()
        {

            defaultInterval = 5000;
            createTimer(defaultInterval);
            currentGeoPos = new GeoLocation { Latitude = "no lat yet", Longitude = "no long yet" };
            notMovedCounter = 0;
        }


        // not used atm
        public void setTimerInterval(int inputInterval)
        {
            timerInterval = inputInterval;

            // Set it to go off every n seconds 1s =10000
            myTimer.Interval = timerInterval;
        }

        // not used atm
        public int getTimerInterval()
        {
            return timerInterval;

        }


        public void createTimer(int interval)
        {
            // Create a timer
            myTimer = new Timer();

            // Tell the timer what to do when it elapses
            myTimer.Elapsed += new ElapsedEventHandler(monitorSession);
            myTimer.Interval = interval;
            myTimer.Enabled = false;

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



        // triggered from precreated Timer started in HandleLocationChanged atm

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



        public void saveToDb(GeoLocation currentGeo)
        {
            Console.WriteLine("SAVE to DB " + "\n");
            App.Database.SaveGeoLocationItemAsync(currentGeo);
        }


        public void setGeoInstance(string lati, string longi, string dateTime)
        {
            currentGeoPos.Latitude = lati;
            currentGeoPos.Longitude = longi;
            currentGeoPos.TimeDate = dateTime;
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


            // btnActivate
            MessagingCenter.Subscribe<GeoDataViewModel>(this, "Activate", (sender) =>
            {

                Console.WriteLine("STARTING Monitor");
                myTimer.Start();
            });

            // btnActivate
            MessagingCenter.Subscribe<GeoDataViewModel>(this, "Deactivate", (sender) =>
            {
                Console.WriteLine("STOPPING Monitor");
                myTimer.Stop();
            });
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


    }

}