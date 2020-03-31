using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using FallDetectionApp.Models;
using FallDetectionApp.Views;
using System.Windows.Input;

namespace FallDetectionApp.ViewModels
{
    public class GeoDataViewModel : BaseViewModel
    {
        public ObservableCollection<GeoLocation> GeoItems { get; set; }
        public Command LoadItemsCommand { get; set; }

        public GeoDataViewModel()
        {

            //Title = "Browse";
            GeoItems = new ObservableCollection<GeoLocation>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

        }

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
    }
}