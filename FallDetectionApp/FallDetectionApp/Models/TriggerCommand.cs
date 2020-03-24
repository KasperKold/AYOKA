using System;
using System.Threading.Tasks;
using FallDetectionApp.Services;
using FallDetectionApp.ViewModels;
using Xamarin.Forms;

[assembly: Dependency(typeof(FallDetectionApp.Models.TriggerCommand))]

namespace FallDetectionApp.Models
{
    public class TriggerCommand : HomeViewModel, IUiTrigger
    {
        public TriggerCommand()
        {
        }

        public async Task ShootAsync()
        {
            await GetGeoLocationAsync();
        }

        async Task<bool> IUiTrigger.Trigger()
        {



            GetGeoLocation.Execute(GetGeoLocation.CanExecute(true));
            return await Task.FromResult(true);

        }
    }
}
