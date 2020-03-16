using System;
using System.Threading.Tasks;
using System.Windows.Input;
using FallDetectionApp.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using FallDetectionApp.Data;

namespace FallDetectionApp.ViewModels
{
    public class HomeViewModel : GeoLocViewModel
    {
        public HomeViewModel()
        {
            Title = "Home";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://xamarin.com"));
            GetGeoLocation = new Command(async () => await GetGeoLocationAsync());
            OpenGeoDatabase = new Command(async () => { await Application.Current.MainPage.Navigation.PushModalAsync(new Views.GeoDataPage()); });
            // AddGeoLocationToDatabase = new Command(async () =>   

        }

        public ICommand OpenWebCommand { get; }
        public ICommand GetGeoLocation { get; }
        public ICommand OpenGeoDatabase { get; }
        public ICommand AddGeoLocationToDatabase { get; }


    }
}