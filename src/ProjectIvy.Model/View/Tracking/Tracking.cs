using ProjectIvy.Common.Interfaces;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Tracking;

public class Tracking : ITracking
{
    public Tracking(DatabaseModel.Tracking.Tracking x)
    {
        Accuracy = x.Accuracy;
        Altitude = x.Altitude;
        Lat = x.Latitude;
        Lng = x.Longitude;
        Speed = x.Speed;
        Timestamp = x.Timestamp;
    }

    public double? Accuracy { get; set; }

    public double? Altitude { get; set; }

    public decimal Latitude
    {
        get => Lat;
        set => Lat = value;
    }

    public decimal Longitude
    {
        get => Lng;
        set => Lng = value;
    }

    public decimal Lat { get; set; }

    public decimal Lng { get; set; }

    public double? Speed { get; set; }

    public DateTime Timestamp { get; set; }
}
