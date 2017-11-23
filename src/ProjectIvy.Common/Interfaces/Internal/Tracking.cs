using System;

namespace ProjectIvy.Common.Interfaces.Internal
{
    internal class Tracking : ITracking
    {
        public double? Accuracy { get; set; }

        public double? Altitude { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public double? Speed { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
