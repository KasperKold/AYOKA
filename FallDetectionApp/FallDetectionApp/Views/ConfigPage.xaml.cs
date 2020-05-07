using System;
using Xamarin.Forms;
using FallDetectionApp.Models;
using FallDetectionApp.ViewModels;
using System.Diagnostics;
using FallDetectionApp.Services;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(FallDetectionApp.Views.ConfigPage))]
namespace FallDetectionApp.Views

{
    public partial class ConfigPage : ContentPage

    {

        public ConfigPage()
        {
            InitializeComponent();
            BindingContext = new ConfigViewModel();
            // listView.ItemsSource = await App.Database.GetItemsAsync();

            // CheckIfActivated();

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            listView.ItemsSource = await App.Database.GetItemsAsync();
            //CheckIfActivated();
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




        /*
        async void CheckIfActivated()
        {

            if (Application.Current.Properties.ContainsKey("isActivated_state") && Convert.ToBoolean(Application.Current.Properties["isActivated_state"].ToString()))
            {

                listView.ItemsSource = await App.Database.GetItemsAsync();
                string message = "Please deactivate to adjust settings!";
                DependencyService.Get<IToast>().ShortAlert(message);
                sliderThreshold.IsEnabled = false;
                sliderDialogue.IsEnabled = false;
                lblTxtMessage.IsEnabled = false;
                listView.IsEnabled = false;

            }
            else
            {
                listView.ItemsSource = await App.Database.GetItemsAsync();
                sliderThreshold.IsEnabled = true;
                sliderDialogue.IsEnabled = true;
                lblTxtMessage.IsEnabled = true;
                listView.IsEnabled = true;

            }
        }


    */




        void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {

            if (sender == sliderThreshold)

            { //sliderThreshold.Value)
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
