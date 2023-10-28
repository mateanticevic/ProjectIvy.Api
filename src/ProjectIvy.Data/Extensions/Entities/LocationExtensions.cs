using System.Linq;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Model.Database.Main.Tracking;

namespace ProjectIvy.Data.Extensions.Entities
{
    public static class LocationExtensions
    {
        public static GeoCoordinate ToGeoCoordinate(this Location l) => new GeoCoordinate((double)l.Latitude, (double)l.Longitude);

        public static async Task<IEnumerable<string>> ToGeohashes(this IQueryable<Location> locations, int userId, string locationValueId)
            => await locations.WhereUser(userId)
                        .Where(x => x.ValueId == locationValueId)
                        .Include(x => x.Geohashes)
                        .Select(x => x.Geohashes)
                        .SelectMany(x => x)
                        .Select(x => x.Geohash)
                        .ToListAsync();
    }
}
