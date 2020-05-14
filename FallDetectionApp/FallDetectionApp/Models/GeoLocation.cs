using System;
using SQLite;

namespace FallDetectionApp.Models
{
    public class GeoLocation
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Info { get; set; }
        public DateTime TimeDate { get; set; }
        public string DeviceId { get; set; }
        public string SessionId { get; set; }
        public string GuiTime { get; set; }
        public int InactivityDetected { get; set; }
        //public int Alarmed { get; set; }

    }
}