using FallDetectionApp.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FallDetectionApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class HomePage : ContentPage
    {


        public HomePage()
        {
            InitializeComponent();
            BindingContext = new HomeViewModel(); 
        }

        private async void btnDbPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new ConfigPage()));
        }
    }
}