using FallDetectionApp.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FallDetectionApp.Models;
using Xamarin.Essentials;
using System.Diagnostics;

namespace FallDetectionApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class HomePage : ContentPage
    {


        public HomePage()
        {
            InitializeComponent();
            BindingContext = new HomeViewModel();
        }

        private async void OnActivateBtnClicked(object sender, EventArgs e)
        {
            var testGeo = new GeoLocation();
            testGeo.Id = 123;
            testGeo.Latitude = "1111111";
            testGeo.Longitude = "2222222";
            testGeo.Info = "from Activate Button Homepage";

            await App.Database.SaveGeoLocationItemAsync(testGeo);           
        }

       private async void OnbtnTestSMS_Clicked(object sender, EventArgs e)
        {
            try
            {
                var permissions = await Permissions.CheckStatusAsync<Permissions.Sms>();
                if(permissions != PermissionStatus.Granted)
                {
                    permissions = await Permissions.RequestAsync<Permissions.Sms>();
                }

                if(permissions != PermissionStatus.Granted)
                {
                    return;
                }
            } catch(Exception ex)
            {
                Debug.WriteLine($"Something is wrong: {ex.Message}");
            }

            
        }
    }
}