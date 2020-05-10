using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using FallDetectionApp.Views;
using FallDetectionApp.Data;
using FallDetectionApp.Models;
using FallDetectionApp.ViewModels;
using System.Diagnostics;

namespace FallDetectionApp
{
    public partial class App : Application
    {

        public static SQLiteDatabase database;

        public App()
        {
            InitializeComponent();
            Application.Current.Properties["isVisited_state"] = "false";
            database = Database;
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

        protected override void OnStart()
        {
            Debug.WriteLine(" App.xaml OnStart");
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
