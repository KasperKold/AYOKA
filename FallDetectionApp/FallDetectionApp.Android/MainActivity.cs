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

namespace FallDetectionApp.Droid
{


    [Activity(Label = "FallDetectionApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly string TAG = "Log MainActivity";

        static readonly int RC_REQUEST_LOCATION_PERMISSION = 1000;

        static readonly string[] REQUIRED_PERMISSIONS = { Manifest.Permission.AccessFineLocation };


        private int countSeconds;
        private Timer timer;
        private string savedLat;
        private string savedLong;
        private int notMovedCounter;


        private string latText;
        private string longText;
        private string speedText;
        private string altText;
        private string accText;
        private string bearText;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);


            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());



            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += OnTimedEvent;
            countSeconds = 20;
            timer.Enabled = true;
            timer.Start();

            Log.Debug(TAG, "OnCreate: Timer started. ");




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
        /*
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        */

        /*
                public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
                {

                    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                }

            */

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            countSeconds--;
            if (countSeconds < 0)
            {
                timer.Stop();
            }
            else
            {
                this.RunOnUiThread(() =>
                {
                    Console.WriteLine("Timer: " + countSeconds.ToString() + " SECONDS");

                    //FindViewById<TextView>(Resource.Id.textView).Text = countSeconds.ToString();
                });
            }
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

        /// <summary>
        ///     Updates UI with location data
        /// </summary>


        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            var location = e.Location;
            Log.Debug(TAG, "Foreground updating");

            // these events are on a background thread, need to update on the UI thread
            RunOnUiThread(() =>
            {
                if (notMovedCounter == 0)
                {
                    notMovedCounter++;
                    Console.WriteLine("COUNTER: INCREASED with 1 - Now: " + notMovedCounter);
                    latText = $"Latitude: {location.Latitude}";
                    savedLat = latText;
                    longText = $"Longitude: {location.Longitude}";
                    savedLong = longText;
                    altText = $"Altitude: {location.Altitude}";
                    speedText = $"Speed: {location.Speed}";
                    accText = $"Accuracy: {location.Accuracy}";
                    bearText = $"Bearing: {location.Bearing}";

                }
                else if (notMovedCounter >= 4)
                {
                    Console.WriteLine("COUNTER: " + notMovedCounter);
                    Console.WriteLine("*********************************************");
                    Console.WriteLine("THE DEVICE HAS NOT MOVED FOR 5 SECONDS ! ! !");
                    Console.WriteLine("*********************************************");

                    notMovedCounter = 0;


                }
                else
                {

                    latText = $"Latitude: {location.Latitude}";
                    longText = $"Longitude: {location.Longitude}";
                    altText = $"Altitude: {location.Altitude}";
                    speedText = $"Speed: {location.Speed}";
                    accText = $"Accuracy: {location.Accuracy}";
                    bearText = $"Bearing: {location.Bearing}";

                    if (latText.Equals(savedLat) && longText.Equals(longText))
                    {
                        notMovedCounter++;
                        Console.WriteLine("COUNTER: INCREASED with 1 - Now: " + notMovedCounter);
                    }
                    else
                    {
                        savedLat = latText;
                        savedLong = longText;
                        notMovedCounter--;
                        Console.WriteLine("COUNTER: decreased with 1 - Now: " + notMovedCounter);
                    }
                }


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