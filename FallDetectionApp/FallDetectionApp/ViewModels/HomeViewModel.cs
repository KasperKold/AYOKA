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






        //public async Task<bool> GetGeoLocationAsync()
        //{
        //    try
        //    {
        //        var location = await Geolocation.GetLocationAsync();

        //        if (location != null)
        //        {
        //            Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
        //        }
        //    }
        //    catch (FeatureNotSupportedException fnsEx)
        //    {
        //        // Handle not supported on device exception
        //    }
        //    catch (FeatureNotEnabledException fneEx)
        //    {
        //        // Handle not enabled on device exception
        //    }
        //    catch (PermissionException pEx)
        //    {
        //        // Handle permission exception
        //    }
        //    catch (Exception ex)
        //    {
        //        // Unable to get location
        //    }

        //    return await Task.FromResult(true);
        //}
    }
}