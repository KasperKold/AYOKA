using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using FallDetectionApp.Models;
using FallDetectionApp.Views;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using FallDetectionApp.Services;
using Xamarin.Essentials;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(FallDetectionApp.ViewModels.GeoDataViewModel))]
namespace FallDetectionApp.ViewModels
{
    public class GeoDataViewModel : BaseViewModel, INotifyPropertyChanged
    {
        //public ObservableCollection<GeoLocation> GeoItems { get; set; }
        //public Command LoadItemsCommand { get; set; }
        public ICommand ToggleDidYouFall { get; }


        public GeoDataViewModel()
        {
            //Title = "Browse";
            //GeoItems = new ObservableCollection<GeoLocation>();
            //LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ToggleDidYouFall = new Command(async () => toggleDidYouFall());
            initialize();
        }


        public void initialize()
        {
            if (Application.Current.Properties.ContainsKey("isVisited_state"))
            {
                visited = Convert.ToBoolean(Application.Current.Properties["isVisited_state"]);
                Debug.WriteLine("Visited variable: " + visited);
                Debug.WriteLine("Visited direct from properties: " + (Application.Current.Properties["isVisited_state"].ToString()));
            }

            if (!visited)
            {
                setUp();
            }
            else
            {
                checkAndSetPropertiesMonitor();
                checkAndSetPropetiesBtnActivate();
            }

        }

        public void setUp()

        {
            btnActivateTxt = "Preparing Location\n   Service";
            isActivated = false;
            monitorReady = false; //Waiting for LocationService to establish   -  message ready in HandleLocationChanged in MainActivityMessaginCenter

            // from MainActivity HanleLocationChanged

            MessagingCenter.Subscribe<Object>(this, "GeoMonitorReady", (sender) =>
            {
                Debug.WriteLine("Message received once");
                monitorReady = true;
                btnActivateTxt = "Activate";

                Application.Current.Properties["btnActivate_state"] = btnActivateTxt;
                Application.Current.Properties["monitorReady_state"] = monitorReady.ToString();
                Application.Current.Properties["isVisited_state"] = "true";
                Application.Current.Properties["isActivated_state"] = isActivated;
                MessagingCenter.Unsubscribe<Object>(this, "GeoMonitorReady");
            });

        }

        /*
        // not used atm
        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                GeoItems.Clear();
                var items = await App.Database.GetGeoLocationItemsAsync();
                foreach (var item in items)
                {
                    GeoItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        */

        public void checkAndSetPropertiesMonitor()
        {

            if (Application.Current.Properties.ContainsKey("monitorReady_state"))
            {
                monitorReady = Convert.ToBoolean(Application.Current.Properties["monitorReady_state"].ToString());
                Debug.WriteLine("monitorReady_state: " + Application.Current.Properties["monitorReady_state"].ToString());

            }
        }


        public void checkAndSetPropetiesBtnActivate()
        {

            if (Application.Current.Properties.ContainsKey("isActivated_state"))
            {
                isActivated = Convert.ToBoolean(Application.Current.Properties["isActivated_state"].ToString());
                Debug.WriteLine("isActivated_state: " + Application.Current.Properties["isActivated_state"].ToString());
                if (isActivated)
                {
                    btnActivateTxt = "DEACTIVATE";
                    Application.Current.Properties["btnActivate_state"] = btnActivateTxt;
                    Application.Current.Properties["isActivated_state"] = isActivated;

                }
                else if (!isActivated)
                {
                    if (!monitorReady)
                    {
                        btnActivateTxt = "Preparing Location\n   Service";
                        Application.Current.Properties["btnActivate_state"] = btnActivateTxt;
                    }
                    else
                    {
                        btnActivateTxt = "Activate";
                        Application.Current.Properties["btnActivate_state"] = btnActivateTxt;
                        Application.Current.Properties["isActivated_state"] = isActivated;
                    }
                }
            }
        }


        // from btnActivate - Actvates the monitoring of session in HandleLocation Changed
        public void toggleDidYouFall()
        {

            if (isActivated && monitorReady)
            {
                btnActivateTxt = "Activate";
                isActivated = false;
                Application.Current.Properties["btnActivate_state"] = btnActivateTxt;
                Application.Current.Properties["isActivated_state"] = isActivated;
                MessagingCenter.Send<GeoDataViewModel>(this, "Deactivate");
            }
            else if (!isActivated && monitorReady)
            {
                btnActivateTxt = "DEACTIVATE";
                isActivated = true;
                Application.Current.Properties["btnActivate_state"] = btnActivateTxt;
                Application.Current.Properties["isActivated_state"] = isActivated;
                MessagingCenter.Send<GeoDataViewModel>(this, "Activate");
            }
        }


        private string privateBtnActivateTxt;
        public string btnActivateTxt
        {
            get { return privateBtnActivateTxt; }
            set
            {
                privateBtnActivateTxt = value;
                OnPropertyChanged(nameof(btnActivateTxt)); // Notify that there was a change on this property
            }
        }

        //not used as binding atm 
        private bool privateMonitorReady;
        public bool monitorReady
        {
            get { return privateMonitorReady; }
            set
            {
                privateMonitorReady = value;
                OnPropertyChanged(nameof(monitorReady)); // Notify that there was a change on this property

            }
        }

        private bool privateVisited;
        public bool visited
        {
            get { return privateVisited; }
            set
            {
                privateVisited = value;
                OnPropertyChanged(nameof(visited)); // Notify that there was a change on this property

            }
        }

        private bool privateIsActivated;
        public bool isActivated
        {
            get { return privateIsActivated; }
            set
            {
                privateIsActivated = value;
                OnPropertyChanged(nameof(isActivated)); // Notify that there was a change on this property

            }
        }

        // inherited from BaseView?

        /*
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
        */

    }
}