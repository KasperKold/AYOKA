using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Locations;
using Xamarin.Forms;
using Application = Xamarin.Forms.Application;
using FallDetectionApp.ViewModels;
using Plugin.Messaging;
using Xamarin.Essentials;
using System.Collections.Generic;
using Microsoft.Azure.Devices.Client;
using FallDetectionApp.Droid.Services;
using FallDetectionApp.Services;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific.AppCompat;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Android.Widget;

[assembly: Dependency(typeof(FallDetectionApp.Droid.MainActivity))]
namespace FallDetectionApp.Droid
{

    [Activity(Label = "FallDetectionApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly string TAG = "Log MainActivity";


        public bool readyForSession;
        private PermissionService permissionService;
        private CallAndSms callAndSms;
        private Monitor monitor;
        private ToastAndroid deliverToasts;
        private DeviceToCloud deviceToCloud;

        static string deviceId;
        static string deviceKey;
        static string hostName;

        // HostName=IotFallApp.azure-devices.net;DeviceId=PederTestDevice;SharedAccessKey=kYMV9WOF4PSifDtML6K8JMO07ORitGaazeoWsCZHFBA=


        protected override void OnCreate(Bundle savedInstanceState)

        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;


            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            Forms.Init(this, savedInstanceState);


            deliverToasts = new ToastAndroid();
            deliverToasts = DependencyService.Get<IToast>() as ToastAndroid;

            deviceId = "PederTestDevice";
            deviceKey = "kYMV9WOF4PSifDtML6K8JMO07ORitGaazeoWsCZHFBA="; //primarykey
            hostName = "IotFallApp.azure-devices.net";
            deviceToCloud = new DeviceToCloud(deviceId, deviceKey, hostName);

            permissionService = new PermissionService(this);
            callAndSms = new CallAndSms();
            monitor = new Monitor(this, callAndSms);

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


            MessagingCenter.Subscribe<GeoDataViewModel>(this, "Activate", (sender) =>
            {
                //SendMessages();
                Console.WriteLine("STARTING Monitor");
                monitor.StartMonitor();
            });

            // from btnActivate
            MessagingCenter.Subscribe<GeoDataViewModel>(this, "Deactivate", (sender) =>
            {
                SendMessages();
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
        // saves locations to monitor ready to be monitored when activated

        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {

            monitor.SetGeoInstance(e);

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


        // sends message to IOT hub  via DeviceToCloud.cs- IN PROCESS!!

        public async void SendMessages()
        {


            // while (true)
            // {
            string msg;
            msg = await deviceToCloud.SendTEXTMessageToIotHubAsync("Hoppsan vilken dag!");
            System.Diagnostics.Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, msg);
            //await Task.Delay(3000);
            //}
        }
    }
}