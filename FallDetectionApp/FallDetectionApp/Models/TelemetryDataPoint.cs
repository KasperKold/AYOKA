using System;
namespace FallDetectionApp.Models
{
    public class TelemetryDataPoint
    {

        public string latitude { get; set; }
        public string longitude { get; set; }
        public DateTime date_time { get; set; }
        public string deviceId { get; set; }
        public string sessionId { get; set; }
        public int inactivityDetected { get; set; }
        // public int alarmed { get; set; }

        public TelemetryDataPoint()
        {
        }
    }
}


