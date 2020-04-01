using System;
using System.Threading.Tasks;
using FallDetectionApp.Droid;
using FallDetectionApp.Services;
using Xamarin.Forms;
using FallDetectionApp.Models;
using Javax.Security.Auth;
using Android.Util;
using FallDetectionApp.ViewModels;
using System.Timers;


[assembly: Dependency(typeof(FallDetectionApp.Droid.UiLocationHandler))]


namespace FallDetectionApp.Droid
{
    public class UiLocationHandler : HomeViewModel, IUiHandler
    {
        private readonly string TAG = "Log UiLocationHandler";
        private GeoLocation currentGeoPos;
        private MainActivity mainActivity;

        public UiLocationHandler()
        {
            DependencyService.Register<IUiHandler>();
            currentGeoPos = new GeoLocation { Id = 555, Latitude = "no lat yet x", Longitude = "no long yet x", Info = "no info" };

        }


        public void setCurrentGeoPos(GeoLocation location)
        {
            this.currentGeoPos = location;
        }


        public void setMainActivity(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }


        public GeoLocation getCurrentGeoPos()
        {
            return this.currentGeoPos;
        }


        async Task<GeoLocation> IUiHandler.GeoUpdateAsync()

        {

            // Only sending the geolocation further
            // Log.Debug(TAG, "Sending geolocation further via IUiHandler");

            return await Task.FromResult(currentGeoPos);
        }

    }
}

