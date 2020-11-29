using GeoCoordinatePortable;
using ProjectIvy.Model.Database.Main.Tracking;

namespace ProjectIvy.Data.Extensions.Entities
{
    public static class LocationExtensions
    {
        public static GeoCoordinate ToGeoCoordinate(this Location l) => new GeoCoordinate((double)l.Latitude, (double)l.Longitude);
    }
}
