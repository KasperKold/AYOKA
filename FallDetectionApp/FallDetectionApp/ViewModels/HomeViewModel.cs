using System;
using System.Threading.Tasks;
using System.Windows.Input;
using FallDetectionApp.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using FallDetectionApp.Data;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.Generic;
using FallDetectionApp.Views;
using System.Diagnostics;
using Plugin.Messaging;

//[assembly: Xamarin.Forms.Dependency(typeof(FallDetectionApp.ViewModels.HomeViewModel))]
namespace FallDetectionApp.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        public HomeViewModel()
        {
            Title = "Home";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://xamarin.com"));
            OpenGeoDatabase = new Command(async () => { await Application.Current.MainPage.Navigation.PushModalAsync(new Views.GeoDataPage()); });
            OpenTestCall = new Command(async () => await MakeTestCall()); //Alexa: +46760996722, Peder: +46733241061 
            OpenTestSMS = new Command(async () => await SendSMSToContact("Hello, this is a test of sending an sms to all contacts in my FallApp."));
            //OpenTestSMS = new Command(async () => await SendTestSMS("+4530295867", "Hello, testing 1-2-3"));   //"Hi, this is an automated text message from DidYouFallApp that tracks my movement. I might have fallen and potentially hurt myself. Please get in contact with me as soon as possible and make sure I am OK."
            
            // RefreshListView = new Command(async () => { await App.Database.GetGeoLocationItemsAsync(); });
            // AddGeoLocationToDatabase = new Command(async () =>
            listenGeo();

        }

        public ICommand OpenWebCommand { get; }
        public ICommand OpenGeoDatabase { get; }
        public ICommand OpenTestCall { get; }
        public ICommand OpenTestSMS { get; }
        //public ICommand RefreshListView { get; }
        //public ICommand AddGeoLocationToDatabase { get; }

        // Test Call function
        public async Task<bool> MakeTestCall()
        {
            //Checking for permission.

            try
            {
                var permissions = await Permissions.CheckStatusAsync<Permissions.Phone>();
                if (permissions != PermissionStatus.Granted)
                {
                    permissions = await Permissions.RequestAsync<Permissions.Phone>();
                }

                if (permissions != PermissionStatus.Granted)
                {
                    Debug.WriteLine("Permission to use native Phone function on the phone was denied.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Something is wrong: {ex.Message}");
            }

            // If permission has been granted, phone call commences

            List<Models.Contact> contactsFromLocalDB = await App.Database.GetItemsAsync();

            for (int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                var phoneCall = CrossMessaging.Current.PhoneDialer;
                if (phoneCall.CanMakePhoneCall)
                {
                    phoneCall.MakePhoneCall(contactsFromLocalDB[i].PhoneNr);
                }
            }

            Debug.WriteLine("Testing to find contact numbers in database: ");
            for (int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                Debug.Write(contactsFromLocalDB[i].Name + " " + contactsFromLocalDB[i].PhoneNr);
            }

            return await Task.FromResult(true);
        }

        // SMS function fetching contact from database
        public async Task<bool> SendSMSToContact(string text)
        {
            //Checking for permission.
            try
            {
                var permissions = await Permissions.CheckStatusAsync<Permissions.Sms>();
                if (permissions != PermissionStatus.Granted)
                {
                    permissions = await Permissions.RequestAsync<Permissions.Sms>();
                }

                if (permissions != PermissionStatus.Granted)
                {
                    Debug.WriteLine("Permission to use native SMS function on the phone was denied.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Something is wrong: {ex.Message}");
            }

            // If permission has been granted, sms-messenging commences

            List<Models.Contact> contactsFromLocalDB = await App.Database.GetItemsAsync();

            for(int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                var smsMessenger = CrossMessaging.Current.SmsMessenger;
                if (smsMessenger.CanSendSmsInBackground)
                {
                    smsMessenger.SendSmsInBackground(contactsFromLocalDB[i].PhoneNr, text);
                }
            }

            Debug.WriteLine("Testing to find contact numbers in database: ");
            for(int i = 0; i < contactsFromLocalDB.Count; i++)
            {
                Debug.Write(contactsFromLocalDB[i].Name + " " + contactsFromLocalDB[i].PhoneNr);
            }

            return await Task.FromResult(true);
        }


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




        public void listenGeo()
        {

            MessagingCenter.Subscribe<Object>(this, "latestGeo", (sender) =>
                {

                    GetGeoLocationAsync();

                });

        }


        public async Task<bool> GetGeoLocationAsync()
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