using AnticevicApi.Model.Database.Main.Tracking;
using GeoCoordinatePortable;
using System.Linq;
using System;

namespace AnticevicApi.DL.Extensions.Entities
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
                                    .Select(x => new GeoCoordinate((double)x.Latitude, (double)x.Longitude, (double)x.Altitude))
                                    .ToList();
            double sum = 0;
            for (int i = 0; i < filtered.Count() - 1; i++)
            {
                sum += filtered[i].GetDistanceTo(filtered[i + 1]);
            }

            return (int)sum;
        }
    }
}
