using System;

using Xamarin.Forms;
using FallDetectionApp.Services;
using FallDetectionApp.Droid.Services;
using Android.Widget;
using Android.Views;

[assembly: Xamarin.Forms.Dependency(typeof(FallDetectionApp.Droid.Services.ToastAndroid))]
namespace FallDetectionApp.Droid.Services
{
    public class ToastAndroid : IToast
    {




        public ToastAndroid()
        {

        }


        public void LongAlert(string message)
        {
            Toast toast = Toast.MakeText(Xamarin.Essentials.Platform.CurrentActivity, message, ToastLength.Long);
            toast.SetGravity(GravityFlags.Center, 0, 0);
            toast.Show();
        }

        public void ShortAlert(string message)
        {

            Toast toast = Toast.MakeText(Xamarin.Essentials.Platform.CurrentActivity, message, ToastLength.Short);
            toast.SetGravity(GravityFlags.Center, 0, 0);
            toast.Show();
        }
    }
}
