using System;

namespace AevApp.Dependency
{
    public class Location
    {
        public decimal? Accuracy { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Provider { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
