using System;
using Android.OS;

namespace FallDetectionApp.Droid.Services
{
    public class ServiceConnectedEventArgs : EventArgs
    {
        public IBinder Binder { get; set; }
    }
}
