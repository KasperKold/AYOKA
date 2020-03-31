﻿using System;
using Xamarin.Forms;
using FallDetectionApp.Models;

namespace FallDetectionApp.Views
{
    public partial class NewContactPage : ContentPage
    {
        public NewContactPage()
        {
            InitializeComponent();
        }

        async void OnSaveClicked(object sender, EventArgs e)
        {
            var contactItem = (Contact)BindingContext;
            await App.Database.SaveItemAsync(contactItem);
            await Navigation.PopAsync();
        }

        async void OnDeleteClicked(object sender, EventArgs e)
        {
            var contactItem = (Contact)BindingContext;
            await App.Database.DeleteItemAsync(contactItem);
            await Navigation.PopAsync();
        }

        async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
