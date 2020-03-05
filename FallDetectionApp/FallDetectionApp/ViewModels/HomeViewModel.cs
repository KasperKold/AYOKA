using System;
using System.Threading.Tasks;
using System.Windows.Input;
using FallDetectionApp.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
namespace FallDetectionApp.ViewModels
{
    public class HomeViewModel : GeoLocViewModel
    {
        public HomeViewModel()
        {
            Title = "Home";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://xamarin.com"));
            GetGeoLocation = new Command(async () => await GetGeoLocationAsync());


        }

        public ICommand OpenWebCommand { get; }
        public ICommand GetGeoLocation { get; }

    }
}