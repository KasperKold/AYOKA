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
[assembly: Xamarin.Forms.Dependency(typeof(FallDetectionApp.ViewModels.GeoDataViewModel))]
namespace FallDetectionApp.ViewModels
{
    public class GeoDataViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public ObservableCollection<GeoLocation> GeoItems { get; set; }
        public Command LoadItemsCommand { get; set; }
        private bool isActivated;



        public GeoDataViewModel()
        {

            //Title = "Browse";
            GeoItems = new ObservableCollection<GeoLocation>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ToggleDidYouFall = new Command(async () => toggleDidYouFall());
            btnActivateTxt = "Activate";
            isActivated = false;
        }

        public ICommand ToggleDidYouFall { get; }

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

        public async void toggleDidYouFall()
        {
            if (isActivated)
            {
                isActivated = false;
            }
            else
            {
                isActivated = true;
            }

            bool toggleBtn = DependencyService.Get<IToggleDidYouFall>().ToggleDidYouFallMainActivity(isActivated);

            if (toggleBtn) { btnActivateTxt = "Activate"; }
            else { btnActivateTxt = "DEACTIVATE"; }




            //await Navigation.PopAsync();

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
    }
}