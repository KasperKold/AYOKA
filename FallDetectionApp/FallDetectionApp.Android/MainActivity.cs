using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Locations;
using Android.Telephony;
using Android.Content;
using FallDetectionApp.ViewModels;
using FallDetectionApp.Droid.Services;
using Xamarin.Forms;
using Plugin.Messaging;


namespace FallDetectionApp.Droid
{

    [Activity(Label = "FallDetectionApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly string TAG = "Log MainActivity";
        public bool readyForSession;
        private PermissionService permissionService;
        private CallAndSms callAndSms;
        private Monitor monitor;



        protected override void OnCreate(Bundle savedInstanceState)

        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            Forms.Init(this, savedInstanceState);

            permissionService = new PermissionService(this);
            callAndSms = new CallAndSms(this);
            monitor = new Monitor(this, callAndSms);

            // deactivated and not used in Services/callAndSms atm
            // TelephonyManager telephonyManager = (TelephonyManager)GetSystemService(Context.TelephonyService);
            // telephonyManager.Listen(callAndSms, PhoneStateListenerFlags.CallState);

            LoadApplication(new App());
            permissionService.CheckBuildAndPermissions();

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


            // from btnActivate
            MessagingCenter.Subscribe<HomeViewModel>(this, "Activate", (sender) =>
            {
                Console.WriteLine("STARTING Monitor");
                monitor.StartMonitor();
            });


            MessagingCenter.Subscribe<HomeViewModel>(this, "Deactivate", (sender) =>
            {
                Console.WriteLine("STOPPING Monitor");
                monitor.StopMonitor();
            });
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

            //Application.Current.Properties["isVisited_state"] = "false";
            Log.Debug(TAG, "OnDestroy: Location app is becoming inactive");
            base.OnDestroy();

            // Stop the location service:
            LocationHandler.StopLocationService();

        }


        // checking results (not very throughly atm) and starts services accordingly (Location, autodial)

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {

            Log.Debug(TAG, "RC_CODE: " + requestCode);


            // if granted then 0 else -1

            if (grantResults[0] == Permission.Granted && grantResults[1] == Permission.Granted && grantResults[2] == Permission.Granted && grantResults[3] == Permission.Granted)
            {
                Log.Debug(TAG, "User has granted all permissions.");
                LocationHandler.StartLocationService();
                Log.Debug(TAG, "LocationService Started");
                //monitor.SetAutodial();
                CrossMessaging.Current.Settings().Phone.AutoDial = true;

                if (CrossMessaging.Current.Settings().Phone.AutoDial == true)
                {
                    Console.WriteLine("*AutoDial enabled*");
                }

            }
            else if (grantResults[0] == Permission.Granted)
            {
                Log.Debug(TAG, "Location permission granted");
                LocationHandler.StartLocationService();
                Log.Debug(TAG, "LocationService Started");

            }
            else
            {

                monitor.AlertConfirmation("Permissions", "Some permissions NOT granted", 1500);
            }
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        // always monitoring according to LocationService.cs ->   LocMgr.RequestLocationUpdates(locationProvider, 2000, 0, this); (every 2 seconds)
        // registres when locations are coming in and  enables Activate button in GUI.
        // saves locations to monitor ready - to be monitored when activated

        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {

            monitor.SetGeoInstance(e);

            // Send message - ready to monitor to HomeViewModel
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

        // not used atm
        public void UpdateCallState(CallState state, string incomingNumber)
        {
            // numberLabel.Text = ...
        }

    }
}
