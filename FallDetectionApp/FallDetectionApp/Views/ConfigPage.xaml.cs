using System;
using Xamarin.Forms;
using FallDetectionApp.Models;
using FallDetectionApp.ViewModels;
using System.Diagnostics;

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


        void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {

            if (sender == sliderThreshold)

            { //sliderThreshold.Value)
                String.Format("The Slider value is {0}", args.NewValue);
                Debug.WriteLine("Slider Threshold:" + String.Format("{0:F0}", args.NewValue));
                Application.Current.Properties["geoPeriod_setting"] = String.Format("{0:F0}", args.NewValue);

                // var vm = (ConfigViewModel)BindingContext;
                // vm.SliderThreshold=
                //lblSliderDialogue.Text = String.Format("XXX = {0:X2}", (int)args.NewValue);
                //string test = Convert.ToString(sliderThreshold.Value);

            }
            else if (sender == sliderDialogue)
            {
                // (Math.Round(sliderDialogue.Value)).ToString()
                Debug.WriteLine("Slider Dialogue:" + String.Format("{0:F0}", args.NewValue));
                Application.Current.Properties["secToAlarm_setting"] = String.Format("{0:F0}", args.NewValue);
                //+String.Format("{0:F0}", args.NewValue))
            }
        }
    }
}
