using System;
using System.Threading.Tasks;
using FallDetectionApp.Droid;
using FallDetectionApp.Services;
using Xamarin.Forms;
using FallDetectionApp.Models;
using Javax.Security.Auth;
using Android.Util;
using FallDetectionApp.ViewModels;

[assembly: Dependency(typeof(FallDetectionApp.Droid.UiLocationHandler))]


namespace FallDetectionApp.Droid
{
    public class UiLocationHandler : GeoLocViewModel, IUiHandler
    {
        private readonly string TAG = "Log UiLocationHandler";
        private GeoLocation currentGeoPos;



        public UiLocationHandler()
        {
            DependencyService.Register<IUiHandler>();

            currentGeoPos = new GeoLocation { Id = 555, Latitude = "no lat yet x", Longitude = "no long yet x", Info = "no info" };
        }


        public void setCurrentGeoPos(GeoLocation location)
        {
            this.currentGeoPos = location;

        }

        public GeoLocation getCurrentGeoPos()
        {
            return this.currentGeoPos;

        }
        public async Task UiTriggerAsync()
        {
            // Log.Debug(TAG, "TRIGGER GetGeoLocationAsync() in GeoLocViewModel");
            await GetGeoLocationAsync();


        }

        async Task<GeoLocation> IUiHandler.GeoUpdateAsync()

        {

            // Only sending the geolocation further
            //GeoLocation tempPos = getCurrentGeoPos();
            //GeoLocation test = new GeoLocation { Id = Guid.NewGuid().ToString(), Latitude = "no lat yet", Longitude = "no long yet" };

            // Log.Debug(TAG, "Sending geolocation further via IUiHandler");


            return await Task.FromResult(currentGeoPos);
        }


    }
}

