using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FallDetectionApp.Models;
using FallDetectionApp.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FallDetectionApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeoDataPage : ContentPage
    {
        public GeoDataPage()
        {
            InitializeComponent();
            //BindingContext = new GeoDataViewModel();

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            geoDataListView.ItemsSource = await App.Database.GetGeoLocationItemsAsync();
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
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new GeoDataPage
                {
                    BindingContext = e.SelectedItem as GeoLocation
                });
            }
        }

        async void SaveGeoLocationItem(object sender, EventArgs e)
        {
            var todoItem = new GeoLocation();
            todoItem.Id = 123;
            await App.Database.SaveGeoLocationItemAsync(todoItem);
            await Navigation.PopAsync();
        }

    }
}

    
