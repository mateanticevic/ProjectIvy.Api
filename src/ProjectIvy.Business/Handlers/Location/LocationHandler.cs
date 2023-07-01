using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;

namespace ProjectIvy.Business.Handlers.Location
{
    public class LocationHandler : Handler<LocationHandler>, ILocationHandler
    {
        public LocationHandler(IHandlerContext<LocationHandler> context) : base(context)
        {
        }

        public async Task<IEnumerable<DateTime>> GetDays(string locationId)
        {
            using var context = GetMainContext();

            var location = await context.Locations.WhereUser(UserId)
                                                  .SingleOrDefaultAsync(x => x.ValueId == locationId);

            var geohashes = context.LocationGeohashes.Where(x => x.LocationId == location.Id)
                                                      .Select(x => x.Geohash);

            var days = await context.Trackings.WhereUser(UserId)
                                              .Where(x => geohashes.Any(y => x.Geohash.StartsWith(y)))
                                              .Select(x => x.Timestamp.Date)
                                              .Distinct()
                                              .OrderByDescending(x => x)
                                              .ToListAsync();

            return days;
        }
    }
}

