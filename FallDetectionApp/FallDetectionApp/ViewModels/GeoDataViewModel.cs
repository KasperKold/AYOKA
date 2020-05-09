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

using Xamarin.Essentials;
using System.Threading;


namespace FallDetectionApp.ViewModels
{
    public class GeoDataViewModel : BaseViewModel, INotifyPropertyChanged
    {

        public ICommand CmdToggleBtnActivate { get; }


        public GeoDataViewModel()
        {
            Title = "Home";

            CmdToggleBtnActivate = new Command(async () => ToggleBtnActivate());
            initialize();
        }


        public void initialize()
        {
            visited = false;
            if (Application.Current.Properties.ContainsKey("isVisited_state"))
            {
                visited = Convert.ToBoolean(Application.Current.Properties["isVisited_state"]);
                Debug.WriteLine("Visited variable: " + visited);
                Debug.WriteLine("Visited direct from properties: " + (Application.Current.Properties["isVisited_state"].ToString()));
            }

            //if app NOT coming from sleep (not visited befire) - set up
            if (!visited)
            {
                setUp();
                double geoPeriod = 1;
                double secToAlarm = 30;
                Application.Current.Properties["geoPeriod_setting"] = geoPeriod.ToString();
                Application.Current.Properties["secToAlarm_setting"] = secToAlarm.ToString();

            }
            //if app coming from sleep/resume
            else
            {
                checkAndSetPropertiesMonitor();
                checkAndSetPropertiesBtnActivate();
            }

        }

        async public void setUp()

        {
            //disable btn
            MessagingCenter.Send<Object, string>(this, "ableBtnActivate", "disable");
            btnActivateTxt = "Finding your \n location ...";
            isActivated = false;
            monitorReady = false; //Waiting for LocationService to establish   -  message ready in HandleLocationChanged in MainActivityMessaginCenter



            // from MainActivity HandleLocationChanged
            MessagingCenter.Subscribe<Object>(this, "GeoMonitorReady", (sender) =>
            {
                Debug.WriteLine("Message received from Location Manager");

                monitorReady = true;
                // enable btn
                MessagingCenter.Send<Object, string>(this, "ableBtnActivate", "enable");
                btnActivateTxt = "Activate";

                //saving to properties
                Application.Current.Properties["btnActivate_state"] = btnActivateTxt;
                Application.Current.Properties["monitorReady_state"] = monitorReady.ToString();
                Application.Current.Properties["isVisited_state"] = "true";
                Application.Current.Properties["isActivated_state"] = isActivated;

                // work done - unsubscribe
                MessagingCenter.Unsubscribe<Object>(this, "GeoMonitorReady");
            });


            MessagingCenter.Subscribe<Object>(this, "InactivityDetected", (sender) =>
            {

                isActivated = false;
                btnActivateTxt = "Activate";
                Application.Current.Properties["btnActivate_state"] = btnActivateTxt;
                Application.Current.Properties["isActivated_state"] = isActivated;



            });


            MessagingCenter.Subscribe<Object, string>(this, "SecToCheck", async (sender, arg) =>
            {
                btnActivateTxt = "DeActivate\n\nNext Check:\n" + arg;
            });
        }



        public void checkAndSetPropertiesMonitor()
        {

            if (Application.Current.Properties.ContainsKey("monitorReady_state"))
            {
                monitorReady = Convert.ToBoolean(Application.Current.Properties["monitorReady_state"].ToString());
                Debug.WriteLine("monitorReady_state: " + Application.Current.Properties["monitorReady_state"].ToString());
            }
        }


        public void checkAndSetPropertiesBtnActivate()
        {

            if (Application.Current.Properties.ContainsKey("isActivated_state"))
            {
                isActivated = Convert.ToBoolean(Application.Current.Properties["isActivated_state"].ToString());
                Debug.WriteLine("isActivated_state: " + Application.Current.Properties["isActivated_state"].ToString());
            }

            if (isActivated)
            {
                btnActivateTxt = "DEACTIVATE";
                Application.Current.Properties["btnActivate_state"] = btnActivateTxt;
                Application.Current.Properties["isActivated_state"] = isActivated;

            }
            else if (!isActivated)
            {
                if (!monitorReady)
                {

                    btnActivateTxt = "Preparing Location\n   Service";
                    Application.Current.Properties["btnActivate_state"] = btnActivateTxt;
                }
                else
                {

                    btnActivateTxt = "Activate";
                    Application.Current.Properties["btnActivate_state"] = btnActivateTxt;
                    Application.Current.Properties["isActivated_state"] = isActivated;
                }
            }
        }


        //  demand to activate or deactivate from btnActivate
        //  Checkin if monitor is ready and active
        //  sets button accordingly
        //  set the bool isActivated accrdingly


        public void ToggleBtnActivate()
        {

            if (isActivated && monitorReady)
            {
                btnActivateTxt = "Activate";
                isActivated = false;
                Application.Current.Properties["btnActivate_state"] = btnActivateTxt;
                Application.Current.Properties["isActivated_state"] = isActivated;
                MessagingCenter.Send<GeoDataViewModel>(this, "Deactivate");
            }
            else if (!isActivated && monitorReady)
            {
                btnActivateTxt = "DEACTIVATE";
                isActivated = true;
                Application.Current.Properties["btnActivate_state"] = btnActivateTxt;
                Application.Current.Properties["isActivated_state"] = isActivated;
                MessagingCenter.Send<GeoDataViewModel>(this, "Activate");
            }
        }


        private string privateBtnActivateTxt;
        public string btnActivateTxt
        {
            get { return privateBtnActivateTxt; }
            set
            {
                privateBtnActivateTxt = value;
                OnPropertyChanged(nameof(btnActivateTxt)); // Notify that there was a change on this property
            }
        }

        //not used as binding atm 
        private bool privateMonitorReady;
        public bool monitorReady
        {
            get { return privateMonitorReady; }
            set
            {
                privateMonitorReady = value;
                OnPropertyChanged(nameof(monitorReady)); // Notify that there was a change on this property
            }
        }

        private bool privateVisited;
        public bool visited
        {
            get { return privateVisited; }
            set
            {
                privateVisited = value;
                OnPropertyChanged(nameof(visited)); // Notify that there was a change on this property

            }
        }

        private bool privateIsActivated;
        public bool isActivated
        {
            get { return privateIsActivated; }
            set
            {
                privateIsActivated = value;
                OnPropertyChanged(nameof(isActivated)); // Notify that there was a change on this property

            }
        }
    }
}