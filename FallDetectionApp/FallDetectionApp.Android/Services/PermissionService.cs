﻿using System;
using System.Resources;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using DotNetty.Common;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FallDetectionApp.Droid.Services
{
    public class PermissionService : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static readonly int RC_REQUEST_ALL_PERMISSION = 1004;
        private readonly string TAG = "Log PermissionService";
        private MainActivity mainActivity;





        public PermissionService(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }



        public void CheckBuildAndPermissions()
        {
            /*
            string buildNumber = AppInfo.BuildString;
            Log.Debug(TAG, "BUILDNUMBER:" + buildNumber); // prints 1 - not sure why
            if (int.Parse(buildNumber) >= 23)
            {
            */
            if (!CheckLocationPermission() || !CheckCallPermission()
                || !CheckSMSPermission() || !CheckReadPhoneStatePermission())
            {
                Log.Debug(TAG, "REQUESTING ALL PERMISSIONS");
                RequestAllPermissions();
            }
            else
            {
                LocationHandler.StartLocationService();
                Log.Debug(TAG, "LocationService Started");
                // Move to...
            }
            /*
            }
            else
            {

           
            Log.Debug(TAG, "Something wrong with build.");
            }
            */
        }




        private bool CheckLocationPermission()
        {
            if (ContextCompat.CheckSelfPermission(mainActivity, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckCallPermission()
        {
            if (ContextCompat.CheckSelfPermission(mainActivity, Manifest.Permission.CallPhone) == (int)Permission.Granted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckSMSPermission()
        {
            if (ContextCompat.CheckSelfPermission(mainActivity, Manifest.Permission.SendSms) == (int)Permission.Granted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckReadPhoneStatePermission()
        {
            if (ContextCompat.CheckSelfPermission(mainActivity, Manifest.Permission.ReadPhoneState) == (int)Permission.Granted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void RequestAllPermissions()
        {

            AlertDialog.Builder dialogPermissions = new AlertDialog.Builder(Xamarin.Essentials.Platform.CurrentActivity);
            AlertDialog alertPermissions = dialogPermissions.Create();

            alertPermissions.SetTitle("This app need your permission to:");
            alertPermissions.SetMessage("Monitor your Location\nCall your Contacts\nSend Sms your Contacts\nPlease press OK!");

            alertPermissions.SetButton("OK!", (c, ev) =>
            {
                ActivityCompat.RequestPermissions(mainActivity,
                new String[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.CallPhone, Manifest.Permission.SendSms, Manifest.Permission.ReadPhoneState },
                RC_REQUEST_ALL_PERMISSION);
            });
            alertPermissions.Show();
            // above Method  ActivityCompat.RequestPermissions() ends up in Mainactivity onRequestPermsisonresult where Geomonitoring is started.
        }
    }
}
