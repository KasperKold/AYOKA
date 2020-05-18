using System;
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
            sliderThreshold.Value = Convert.ToDouble(Application.Current.Properties["geoPeriod_setting"]);
            sliderDialogue.Value = Convert.ToDouble(Application.Current.Properties["secToAlarm_setting"]);
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

        void EditorCompleted(object sender, EventArgs e)
        {
            Application.Current.Properties["userAlarmMessage"] = ((Editor)sender).Text;
        }

        void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {

            if (sender == sliderThreshold)

            {
                String.Format("The Slider value is {0}", args.NewValue);
                // Debug.WriteLine("Slider Threshold:" + String.Format("{0:F0}", args.NewValue));
                Application.Current.Properties["geoPeriod_setting"] = String.Format("{0:F0}", args.NewValue);
            }
            else if (sender == sliderDialogue)
            {
                // Debug.WriteLine("Slider Dialogue:" + String.Format("{0:F0}", args.NewValue));
                Application.Current.Properties["secToAlarm_setting"] = String.Format("{0:F0}", args.NewValue);
            }
        }
    }
}
