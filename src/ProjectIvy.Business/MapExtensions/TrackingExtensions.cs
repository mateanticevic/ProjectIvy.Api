using System;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Model.Database.Main.Tracking;

namespace ProjectIvy.Business.MapExtensions;

public static class TrackingExtensions
{
    public static Tracking ToEntity(this TrackingBinding tb, Tracking t = null)
    {
        t = t.DefaultIfNull();

        t.Accuracy = tb.Accuracy.HasValue ? (double)Math.Round(tb.Accuracy.Value, 2) : (double?)null;
        t.Altitude = tb.Altitude.HasValue ? (double)Math.Round(tb.Altitude.Value, 2) : (double?)null;
        t.Latitude = tb.Latitude;
        t.Longitude = tb.Longitude;
        t.Speed = tb.Speed.HasValue ? (double)Math.Round(tb.Speed.Value, 2) : (double?)null;
        t.Timestamp = tb.Timestamp;

        return t;
    }
}
