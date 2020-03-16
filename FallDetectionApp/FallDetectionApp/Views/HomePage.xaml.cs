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
            var todoItem = new GeoLocation();
            todoItem.Id = 123;
            todoItem.Latitude = "13,5";
            await App.Database.SaveGeoLocationItemAsync(todoItem);
            Console.WriteLine("HEEEEJ!");
            //await Navigation.PopAsync();
        }

        //private async void btnDbPageClicked(object sender, EventArgs e)
        //{
        //    await Navigation.PushModalAsync(new NavigationPage(new ConfigPage()));
        //}
    }
}