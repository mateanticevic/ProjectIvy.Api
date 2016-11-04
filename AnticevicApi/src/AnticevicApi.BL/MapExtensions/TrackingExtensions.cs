using AnticevicApi.Extensions.BuiltInTypes;
using AnticevicApi.Model.Binding.Tracking;
using AnticevicApi.Model.Database.Main.Tracking;

namespace AnticevicApi.BL.MapExtensions
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
