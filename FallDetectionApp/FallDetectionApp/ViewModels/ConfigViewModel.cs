using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using FallDetectionApp.Models;
using FallDetectionApp.Views;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using FallDetectionApp.Services;


namespace FallDetectionApp.ViewModels
{
    public class ConfigViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private static ConfigViewModel _instance;
        public static ConfigViewModel Instance { get { return _instance; } }
        public Command LoadItemsCommand { get; set; }
        public ICommand CommandToConfigPage { get; private set; }

        // public ICommand CommandSaveSettings { get; }



        public ConfigViewModel()
        {
            _instance = this;
            // CommandSaveSettings = new Command(async () => await UpdateGuiWithSavedSettings());
            if (Application.Current.Properties.ContainsKey("userAlarmMessage"))
            {
                textAlarmMessage = Application.Current.Properties["userAlarmMessage"].ToString();

            }
            else
            {
                textAlarmMessage = "Hi, this is an automatic message and I need your help.";
                Application.Current.Properties["userAlarmMessage"] = textAlarmMessage;
            }

            CommandToConfigPage = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PopAsync();
            });

            Title = "Confguration";


        }


        private string privateTextAlarmMessage;
        public string textAlarmMessage
        {
            get { return privateTextAlarmMessage; }
            set
            {
                privateTextAlarmMessage = value;
                OnPropertyChanged(nameof(textAlarmMessage)); // Notify that there was a change on this property
            }
        }
    }
}