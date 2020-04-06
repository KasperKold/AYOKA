using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using FallDetectionApp.Models;
using FallDetectionApp.Services;
using FallDetectionApp.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace FallDetectionApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeoDataPage : ContentPage, INotifyPropertyChanged
    {
        public GeoDataPage()
        {
            InitializeComponent();

            BindingContext = new GeoDataViewModel();

            listenToRefreshList();


        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();


            // geoDataListView.ItemsSource = await App.Database.GetGeoLocationItemsAsync();
            geoItems.ItemsSource = await App.Database.GetGeoLocationItemsAsync();
            scrollToEndOfList(geoItems);
        }

        async void OnItemAdded(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GeoDataPage
            {
                BindingContext = new GeoLocation()
            });
        }



        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            /*
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new GeoDataPage
                {
                    BindingContext = e.SelectedItem as GeoLocation
                });
            }
            */
        }
        /*
        async void SaveGeoLocationItem(object sender, EventArgs e)
        {
            var testGeo = new GeoLocation();
            testGeo.Id = 999;
            testGeo.Latitude = "888888";
            testGeo.Longitude = "7777777";
            testGeo.Info = "from Save Button GeoDataPage";
            await App.Database.SaveGeoLocationItemAsync(testGeo);
            //await Navigation.PopAsync();
        }
        */

        public void listenToRefreshList()
        {
            MessagingCenter.Subscribe<Object>(this, "latestGeo", (sender) =>
                {
                    refreshList();
                });

        }



        async public void refreshList()
        {
            geoItems.ItemsSource = await App.Database.GetGeoLocationItemsAsync();
            scrollToEndOfList(geoItems);

        }


        async void UpdateDataListView(object sender, EventArgs e)
        {
            geoItems.ItemsSource = await App.Database.GetGeoLocationItemsAsync();
            scrollToEndOfList(geoItems);

            //geoDataListView.ItemsSource = await App.Database.GetGeoLocationItemsAsync();
            //await Navigation.PopAsync();
        }

        void scrollToEndOfList(ListView geoItems)
        {
            var v = geoItems.ItemsSource.Cast<object>().LastOrDefault();
            geoItems.ScrollTo(v, ScrollToPosition.End, true);
        }








    }
}


