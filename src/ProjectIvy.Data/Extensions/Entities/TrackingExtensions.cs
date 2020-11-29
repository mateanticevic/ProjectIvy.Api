using GeoCoordinatePortable;
using ProjectIvy.Model.Database.Main.Tracking;
using System;
using System.Linq;

namespace ProjectIvy.Data.Extensions.Entities
{
    public static class TrackingExtensions
    {
        /// <summary>
        /// Returns distance in meters.
        /// </summary>
        /// <returns></returns>
        public static int Distance(this IQueryable<Tracking> trackings, DateTime from, DateTime to)
        {
            var filtered = trackings.Where(x => x.Timestamp >= from && x.Timestamp <= to)
                                    .OrderBy(x => x.Timestamp)
                                    .ToList()
                                    .Select(x => new GeoCoordinate((double)x.Latitude, (double)x.Longitude, x.Altitude ?? 0))
                                    .ToList();
            double sum = 0;
            for (var i = 0; i < filtered.Count() - 1; i++)
            {
                sum += filtered[i].GetDistanceTo(filtered[i + 1]);
            }

            return (int)sum;
        }

        public static GeoCoordinate ToGeoCoordinate(this Tracking t) => new GeoCoordinate((double)t.Latitude, (double)t.Longitude, t.Altitude ?? 0);
    }
}
