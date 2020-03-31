using FallDetectionApp.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FallDetectionApp.Models;

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
            //Console.WriteLine("HEEEEJ!");
            //await Navigation.PopAsync();
        }

        //private async void btnDbPageClicked(object sender, EventArgs e)
        //{
        //    await Navigation.PushModalAsync(new NavigationPage(new ConfigPage()));
        //}
    }
}