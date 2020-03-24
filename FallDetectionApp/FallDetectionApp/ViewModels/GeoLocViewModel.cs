using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using Xamarin.Essentials;

using FallDetectionApp.Models;
using FallDetectionApp.Services;
using System.Threading.Tasks;


namespace FallDetectionApp.ViewModels
{
    public class GeoLocViewModel : INotifyPropertyChanged, IGeoLocation
    {
        // public IGeoLocation GeoLoc => DependencyService.Get<IGeoLocation>(); // NOT NEEDED? PDR Question
        //public IGeoLocationAndroid GeoLocAndroid => DependencyService.Get<IGeoLocationAndroid>();

        //public  UiLocationHandler handler => DependencyService.Get<UiLocationhandler>();
        //DependencyService.Get<UiLocationhandler>()



        private string privateCurrentLatitude;
        public string CurrentLatitude
        {
            get { return privateCurrentLatitude; }
            set
            {
                privateCurrentLatitude = value;
                OnPropertyChanged(nameof(CurrentLatitude)); // Notify that there was a change on this property

            }
        }

        private string privateCurrentLongitude;
        public string CurrentLongitude
        {
            get { return privateCurrentLongitude; }
            set
            {
                privateCurrentLongitude = value;
                OnPropertyChanged(nameof(CurrentLongitude)); // Notify that there was a change on this property
            }
        }


        private string privateSavedLatitude;
        public string SavedLatitude
        {
            get { return privateSavedLatitude; }
            set
            {
                privateSavedLatitude = value;
                OnPropertyChanged(nameof(SavedLatitude)); // Notify that there was a change on this property
            }
        }

        private string privateSavedLongitude;
        public string SavedLongitude
        {
            get { return privateSavedLongitude; }
            set
            {
                privateSavedLongitude = value;
                OnPropertyChanged(nameof(SavedLongitude)); // Notify that there was a change on this property
            }
        }

        private string privateGeoInfo;
        public string GeoInfo
        {
            get { return privateGeoInfo; }
            set
            {
                privateGeoInfo = value;
                OnPropertyChanged(nameof(GeoInfo)); // Notify that there was a change on this property

            }
        }


        private string privateCurrentTimeDate;
        public string CurrentTimeDate
        {
            get { return privateCurrentTimeDate; }
            set
            {
                privateCurrentTimeDate = value;
                OnPropertyChanged(nameof(CurrentTimeDate)); // Notify that there was a change on this property

            }
        }







        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }



        public async Task<bool> GetGeoLocationAsync()
        {
            //Console.WriteLine("Inside GetGeoLocationAsync" + "\n");
            try
            {

                var location = await DependencyService.Get<IUiHandler>().GeoUpdateAsync();
                //  Console.WriteLine("TESTING if instance of GeoLocation coming through: " + location.Latitude + "\n");
                if (location != null)

                {
                    //GeoLocation geoLoca = new GeoLocation { Id = Guid.NewGuid().ToString(), Latitude = location.Latitude.ToString(), Longitude = location.Longitude.ToString() };


                    CurrentLatitude = location.Latitude;
                    CurrentLongitude = location.Longitude;
                    GeoInfo = location.Info;
                    CurrentTimeDate = location.TimeDate;


                    //Altitude: {location.Altitude}"
                    //Console.WriteLine("\nFrom Dependencyservice GetGeoLocationAsync");
                    //Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                    //Console.WriteLine($"Latitude from var: " + CurrentLatitude + " Longitude from var: " + CurrentLongitude);
                    //Console.WriteLine("\n");


                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine("GeoLocViewModel: Not supported on device exception");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Console.WriteLine("GeoLocViewMode:  Not enabled on device Excepetion");
            }
            catch (PermissionException pEx)
            {
                Console.WriteLine("GeoLocViewMode:  Permissionexcepetion");
            }
            catch (Exception ex)
            {
                Console.WriteLine("GeoLocViewModel: Cannot get a location");
            }

            return await Task.FromResult(true);
        }



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }






        #endregion
    }
}