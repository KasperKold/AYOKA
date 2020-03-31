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

[assembly: Dependency(typeof(FallDetectionApp.Droid.MainActivity))]
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

        //private string geoInfo;
        private string sessionStartDateTime;


        private GeoLocation currentGeoPos;
        private UiLocationHandler iUiImplementation;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);


            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);

            //Store our interface class.
            iUiImplementation = DependencyService.Get<IUiHandler>() as UiLocationHandler;

            //iUiImplementation = new UiLocationHandler();
            //iUiImplementation = DependencyService.Get<IUpdateGeo>() as UiLocationHandler;
            //Init our interface.
            //iUiImplementation.Init();

            LoadApplication(new App());
            currentGeoPos = new GeoLocation { Latitude = "no lat yet", Longitude = "no long yet" };

            notMovedCounter = 0;


            /*

            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += OnTimedEvent;
            countSeconds = 20;
            timer.Enabled = true;
            timer.Start();

            Log.Debug(TAG, "OnCreate: Timer started. ");
            */

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

        public GeoLocation getCurrentGeoPos()
        {
            return currentGeoPos;
        }


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
        /// </summary
        ///

        public string setInfoString(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, string k, string l, string m, int n)
        {
            return a + b + c + d + e + f + g + h + i + j + k + l + m + n;

        }




        public void saveToDb(GeoLocation currentGeo)
        {
            Console.WriteLine("SAVEtoDB" + currentGeo.Info + "\n");

            App.Database.SaveGeoLocationItemAsync(currentGeo);
        }


        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            string txtCounter1plus = "COUNTER: +1 --> : ";
            string txtCounter1minus = "COUNTER: -1 --> : ";
            string txtCounter = "COUNTER: ";
            string txtComparedLat = "Compared Lat: ";
            string txtComparedLong = "Compared Long: ";
            string txtStarRow = "******************************************************\n";
            string txtNewRow = "\n";
            string txtPrevLat = "Previous Lat: ";
            string txtPrevLong = "Previous Long: ";
            string txtNotMoved = "THE DEVICE HAS NOT MOVED FOR 25 SECONDS ! ! !\n";


            DateTime dateTime = DateTime.Now.ToLocalTime();
            //string time_string = dateTime.ToString("yyyy-MM-dd");
            string date_string = dateTime.Date.ToString(); // just date

            var location = e.Location;
            // Log.Debug(TAG, "Foreground updating");

            // these events are on a background thread, need to update on the UI thread
            RunOnUiThread(async () =>
            {
                if (sessionStartDateTime == "")
                {
                    sessionStartDateTime = "Session: " + date_string;
                }


                if (notMovedCounter == 0)
                {
                    notMovedCounter++;

                    latText = $"{location.Latitude}";
                    savedLat = latText;

                    longText = $"{location.Longitude}";
                    savedLong = longText;

                    Console.WriteLine(txtNewRow + txtCounter1plus + notMovedCounter);
                    Console.WriteLine(txtComparedLat + latText.Substring(0, 7));
                    Console.WriteLine(txtComparedLong + longText.Substring(0, 7) + "\n");


                    //assigns location to instance of GeoLocation
                    currentGeoPos.Latitude = latText;
                    currentGeoPos.Longitude = longText;
                    //currentGeoPos.sessionGeoCounter = notMovedCounter;
                    //currentGeoPos.sessionId = sessionStartDateTime;

                    //assigns info
                    currentGeoPos.Info = setInfoString(txtPrevLat, savedLat.Substring(0, 7), txtNewRow,
                    txtPrevLong, savedLong.Substring(0, 7), txtNewRow, txtComparedLat, latText.Substring(0, 7), txtNewRow,
                    txtComparedLong, longText.Substring(0, 7), txtNewRow, txtCounter1plus, notMovedCounter);

                    //sends Geolcation to separate class
                    iUiImplementation.setCurrentGeoPos(currentGeoPos);

                    //DependencyService.Get<IGeoLocation>().GetGeoLocationAsync(currentGeoPos);
                    // trigger the dependencyservice to get the location for UI but cannot update UI from here ATM
                    saveToDb(currentGeoPos);

                    // await iUiImplementation.UiTriggerAsync();




                }
                else if (notMovedCounter >= 4)
                {
                    Console.WriteLine(txtNewRow + txtStarRow);
                    Console.WriteLine(txtCounter + notMovedCounter);
                    Console.WriteLine(txtNotMoved);
                    Console.WriteLine(txtComparedLat + latText.Substring(0, 7));
                    Console.WriteLine(txtComparedLong + longText.Substring(0, 7));
                    Console.WriteLine(txtStarRow + txtNewRow);

                    currentGeoPos.Info =
                    txtStarRow + txtNotMoved + txtStarRow + txtPrevLat + savedLat.Substring(0, 7) +
                    txtNewRow + txtPrevLong + savedLong.Substring(0, 7) + txtNewRow + txtComparedLat +
                    latText.Substring(0, 7) + txtNewRow +
                    txtComparedLong + longText.Substring(0, 7) + txtNewRow +
                    txtCounter + notMovedCounter;

                    currentGeoPos.TimeDate = dateTime.ToString();
                    //currentGeoPos.sessionGeoCounter = notMovedCounter;
                    //currentGeoPos.sessionId = sessionStartDateTime;


                    iUiImplementation.setCurrentGeoPos(currentGeoPos);
                    //DependencyService.Get<IUpdateGeo>().updateGeo();
                    //DependencyService.Get<IGeoLocation>().GetGeoLocationAsync(currentGeoPos);
                    saveToDb(currentGeoPos);
                    //await iUiImplementation.UiTriggerAsync();
                    notMovedCounter = 0;
                }
                else
                {
                    savedLat = latText;
                    savedLong = longText;
                    latText = $"{location.Latitude}";
                    longText = $"{location.Longitude}";

                    currentGeoPos.Latitude = latText;
                    currentGeoPos.Longitude = longText;

                    if (latText.Substring(0, 7).Equals(savedLat.Substring(0, 7)) && longText.Substring(0, 7).Equals(savedLong.Substring(0, 7)))
                    {
                        notMovedCounter++;

                        Console.WriteLine(txtNewRow + txtCounter1plus + notMovedCounter + txtNewRow);
                        Console.WriteLine(txtComparedLat + latText.Substring(0, 7));
                        Console.WriteLine(txtComparedLong + longText.Substring(0, 7));

                        currentGeoPos.Info = setInfoString(txtPrevLat, savedLat.Substring(0, 7), txtNewRow,
                        txtPrevLong, savedLong.Substring(0, 7), txtNewRow, txtComparedLat, latText.Substring(0, 7), txtNewRow,
                        txtComparedLong, longText.Substring(0, 7), txtNewRow,
                        txtCounter1plus, notMovedCounter);

                        currentGeoPos.TimeDate = dateTime.ToString();
                        //currentGeoPos.sessionGeoCounter = notMovedCounter;
                        //currentGeoPos.sessionId = sessionStartDateTime;

                        iUiImplementation.setCurrentGeoPos(currentGeoPos);
                        //DependencyService.Get<IUpdateGeo>().updateGeo();
                        //DependencyService.Get<IGeoLocation>().GetGeoLocationAsync(currentGeoPos);
                        saveToDb(currentGeoPos);
                        //await iUiImplementation.UiTriggerAsync();

                    }
                    else
                    {

                        notMovedCounter--;

                        Console.WriteLine(txtNewRow + txtCounter1minus + notMovedCounter);
                        Console.WriteLine(txtComparedLat + latText.Substring(0, 7));
                        Console.WriteLine(txtComparedLong + longText.Substring(0, 7));

                        currentGeoPos.Latitude = latText;
                        currentGeoPos.Longitude = longText;
                        currentGeoPos.Info = txtPrevLat + savedLat.Substring(0, 7) + txtNewRow +
                        txtPrevLong + savedLong.Substring(0, 7) + txtNewRow + txtComparedLat + latText.Substring(0, 7) +
                        txtNewRow + txtComparedLong + longText.Substring(0, 7) + txtNewRow +
                        txtCounter1minus + notMovedCounter;
                        currentGeoPos.TimeDate = dateTime.ToString();
                        //currentGeoPos.sessionGeoCounter = notMovedCounter;
                        //currentGeoPos.sessionId = sessionStartDateTime;

                        iUiImplementation.setCurrentGeoPos(currentGeoPos);
                        // await App.Database.SaveGeoLocationItemAsync(currentGeoPos);
                        // DependencyService.Get<IUpdateGeo>().updateGeo();
                        //DependencyService.Get<IGeoLocation>().GetGeoLocationAsync(currentGeoPos);
                        saveToDb(currentGeoPos);
                        //await iUiImplementation.UiTriggerAsync();

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