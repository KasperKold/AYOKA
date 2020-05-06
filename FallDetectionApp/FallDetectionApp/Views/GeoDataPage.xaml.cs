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

            MessagingCenter.Subscribe<Object, string>(this, "ableBtnActivate", async (sender, arg) =>
            {
                if (arg == "enable")
                {
                    btnActivate.IsEnabled = true;
                }
                else if (arg == "disable")
                {
                    btnActivate.IsEnabled = false;
                }


            });


        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var vm = (GeoDataViewModel)BindingContext;
            vm.initialize();


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

        /*
        async void UpdateDataListView(object sender, EventArgs e)
        {
            geoItems.ItemsSource = await App.Database.GetGeoLocationItemsAsync();
            scrollToEndOfList(geoItems);


            //await Navigation.PopAsync();
        }
        */

        void scrollToEndOfList(ListView geoItems)
        {
            var v = geoItems.ItemsSource.Cast<object>().LastOrDefault();
            geoItems.ScrollTo(v, ScrollToPosition.End, true);
        }

    }
}


