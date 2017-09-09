using ProjectIvy.Extensions.BuiltInTypes;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Model.Database.Main.Tracking;

namespace ProjectIvy.BL.MapExtensions
{
    public static class TrackingExtensions
    {
        public static Tracking ToEntity(this TrackingBinding tb, Tracking t = null)
        {
            t = t.DefaultIfNull();

            t.Accuracy = tb.Accuracy;
            t.Altitude = tb.Altitude;
            t.Latitude = tb.Latitude;
            t.Longitude = tb.Longitude;
            t.Speed = tb.Speed;
            t.Timestamp = tb.Timestamp;

            return t;
        }
    }
}
