using Xamarin.Forms;
using System.Windows.Input;
using System.ComponentModel;


namespace FallDetectionApp.ViewModels
{
    public class ConfigViewModel : BaseViewModel, INotifyPropertyChanged
    {

        public Command LoadItemsCommand { get; set; }
        public ICommand CommandToConfigPage { get; private set; }

        public ConfigViewModel()
        {

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