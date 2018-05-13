using GeoCoordinatePortable;

namespace ProjectIvy.Model.View
{
    public class Location
    {
        public Location(decimal latitude, decimal longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }

    public static class LocationExtensions
    {
        public static GeoCoordinate ToGeoCoordinate(this Location location) => new GeoCoordinate((double)location.Latitude, (double)location.Longitude);
    }
}
