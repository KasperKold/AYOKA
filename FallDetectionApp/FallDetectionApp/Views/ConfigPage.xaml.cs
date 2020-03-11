﻿using System;
using Xamarin.Forms;
using FallDetectionApp.Models;
using FallDetectionApp.ViewModels;

namespace FallDetectionApp.Views

{
    public partial class ConfigPage : ContentPage

    {

        public ConfigPage()
        {
            InitializeComponent();
            BindingContext = new ConfigViewModel(); 
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            listView.ItemsSource = await App.Database.GetItemsAsync();
        }

        async void OnItemAdded(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewContactPage
            {
                BindingContext = new Contact()
            });
        }

        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new NewContactPage
                {
                    BindingContext = e.SelectedItem as Contact
                });
            }
        }
    }
}
