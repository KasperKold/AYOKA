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


namespace FallDetectionApp.ViewModels
{
    public class ConfigViewModel : BaseViewModel, INotifyPropertyChanged
    {
        // public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public ICommand CommandToConfigPage { get; private set; }

        // public ICommand CommandSaveSettings { get; }



        public ConfigViewModel()
        {

            // CommandSaveSettings = new Command(async () => await UpdateGuiWithSavedSettings());

            CommandToConfigPage = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PopAsync();
            });


            Title = "Confguration";



            // Items = new ObservableCollection<Item>();
            //  LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());


            //Old mockdatabase
            //MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            //{
            //    var newItem = item as Item;
            //    Items.Add(newItem);
            //    await DataStore.AddItemAsync(newItem);
            //});
        }



        /*
        private string privateSavedMonitorSettings;
        public string SavedMonitorSettings
        {
            get { return privateSavedMonitorSettings; }
            set
            {
                privateSavedMonitorSettings = value;
                OnPropertyChanged(nameof(SavedMonitorSettings)); // Notify that there was a change on this property
            }
        }
        */
        /*
        private string privateSliderThreshold;
        public string SliderThreshold
        {
            get { return privateSliderThreshold; }
            set
            {
                privateSliderThreshold = value;
                OnPropertyChanged(nameof(SliderThreshold)); // Notify that there was a change on this property
            }
        }
        */


        //NOT USED ATM
        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                //  Items.Clear();
                // var items = await DataStore.GetItemsAsync(true);
                /*
                foreach (var item in items)
                {
                    //     Items.Add(item);
                }
                */
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


        /*
        public async Task<bool> UpdateGuiWithSavedSettings()
        {

            Debug.WriteLine("Saved settings in properties:" + "Saved:\nGeo Location Period: " + Application.Current.Properties["geoPeriod_setting"].ToString() + "\n" + "Sec to Alarm: " + Application.Current.Properties["secToAlarm_setting"].ToString());
            SavedMonitorSettings = "Saved:\nGeo Location Period: " + Application.Current.Properties["geoPeriod_setting"].ToString() + "\n" + "Sec to Alarm: " + Application.Current.Properties["secToAlarm_setting"];
            return await Task.FromResult(true);
        }
        */
    }
}