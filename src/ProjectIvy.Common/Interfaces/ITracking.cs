using System;

namespace ProjectIvy.Common.Interfaces;

public interface ITracking
{
    double? Accuracy { get; set; }

    double? Altitude { get; set; }

    decimal Latitude { get; set; }

    decimal Longitude { get; set; }

    double? Speed { get; set; }

    DateTime Timestamp { get; set; }
}
