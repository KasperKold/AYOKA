using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FallDetectionApp.Services;
using FallDetectionApp.Views;
using FallDetectionApp.Data;
using FallDetectionApp.Models;
using FallDetectionApp.ViewModels;
using System.Diagnostics;

namespace FallDetectionApp
{
    public partial class App : Application
    {
        //TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
        //To debug on Android emulators run the web backend against .NET Core not IIS
        //If using other emulators besides stock Google images you may need to adjust the IP address
        public static string AzureBackendUrl =
            DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";
        public static bool UseMockDataStore = true;

        public static SQLiteDatabase database;

        public App()
        {
            InitializeComponent();

            if (UseMockDataStore)
                DependencyService.Register<MockDataStore>();
            else
                DependencyService.Register<AzureDataStore>();
            MainPage = new MainPage();
        }

        public static SQLiteDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new SQLiteDatabase();
                }
                return database;
            }
        }

        async protected override void OnStart()
        {
            Debug.WriteLine(" App.xaml OnStart");
            Application.Current.Properties["isVisited_state"] = "false";

        }

        protected override void OnSleep()
        {
            Debug.WriteLine(" App.xaml OnSleep");
            Application.Current.SavePropertiesAsync();
        }

        protected override void OnResume()
        {
            Debug.WriteLine(" App.xaml OnResume");

        }
    }
}
