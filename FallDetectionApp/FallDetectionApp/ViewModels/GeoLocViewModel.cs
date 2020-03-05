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
        public IGeoLocation GeoLoc => DependencyService.Get<IGeoLocation>();

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

        //Updating HomePage with lbl count - try out

        private string privateLblTxtLoopCount;
        public string lblTxtLoopCount
        {
            get { return privateLblTxtLoopCount; }
            set
            {
                privateLblTxtLoopCount = value;
                OnPropertyChanged(nameof(lblTxtLoopCount)); // Notify that there was a change on this property

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
            try
            {
                var location = await Geolocation.GetLocationAsync();

                if (location != null)

                {
                    GeoLocation geoLoc = new GeoLocation { Id = Guid.NewGuid().ToString(), Latitude = location.Latitude.ToString(), Longitude = location.Longitude.ToString(), Altitude = location.Altitude.ToString() };


                    CurrentLatitude = geoLoc.Latitude;
                    CurrentLongitude = geoLoc.Longitude;



                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    Console.WriteLine($"Latitude from variable:" + CurrentLatitude + "Longitude from variable: " + CurrentLongitude);


                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
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
