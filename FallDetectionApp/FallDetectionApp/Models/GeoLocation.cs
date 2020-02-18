using System;

namespace FallDetectionApp.Models
{
    public class GeoLocation
    {
        public string Id { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Altitude { get; set; }
        public string Provider { get; set; }

    }
}