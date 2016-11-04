using System;

namespace AnticevicApi.Model.Binding.Tracking
{
    public class TrackingBinding
    {
        public double? Accuracy { get; set; }
        public double? Altitude { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public double? Speed { get; set; }
    }
}
