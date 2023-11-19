using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProjectIvy.Business.Caching;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Business.Handlers.Geohash;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Geohash;
using ProjectIvy.Model.View.Geohash;

namespace ProjectIvy.Business.Handlers.Location
{
    public class LocationHandler : Handler<LocationHandler>, ILocationHandler
    {
        private IGeohashHandler _geohashHandler;

        public LocationHandler(IHandlerContext<LocationHandler> context, IGeohashHandler geohashHandler, IMemoryCache memoryCache)
            : base(context, memoryCache, nameof(ExpenseHandler))
        {
            _geohashHandler = geohashHandler;
        }

        public async Task<IEnumerable<Model.View.Location.Location>> Get()
        {
            using var context = GetMainContext();

            var locations = await context.Locations.WhereUser(UserId)
                                                   .Include(x => x.Geohashes)
                                                   .ToListAsync();

            return locations.Select(x => new Model.View.Location.Location(x))
                            .OrderBy(x => x.Name);
        }

        public async Task<IEnumerable<DateTime>> GetDays(string locationId)
        {
            string cacheKey = BuildUserCacheKey(CacheKeyGenerator.LocationDays(locationId));
            return await MemoryCache.GetOrCreateAsync(cacheKey,
                async cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
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
            );
        }

        public async Task<IEnumerable<RouteTime>> FromLocationToLocation(string fromLocationValueId, string toLocationValueId, RouteTimeSort sort)
        {
            using var context = GetMainContext();
            var fromGeohashes = await context.Locations.ToGeohashes(UserId, fromLocationValueId);
            var toGeohashes = await context.Locations.ToGeohashes(UserId, toLocationValueId);

            return await _geohashHandler.FromGeohashToGeohash(fromGeohashes, toGeohashes, sort);
        }

        public async Task SetGeohashes(string locationValueId, IEnumerable<string> geohashes)
        {
            using var context = GetMainContext();

            var location = await context.Locations.WhereUser(UserId)
                                                  .SingleOrDefaultAsync(x => x.ValueId == locationValueId);

            await context.LocationGeohashes.Where(x => x.LocationId == location.Id)
                                           .ExecuteDeleteAsync();


            foreach (string geohash in geohashes)
                await context.LocationGeohashes.AddAsync(new Model.Database.Main.Tracking.LocationGeohash() { Location = location, Geohash = geohash });

            await context.SaveChangesAsync();
        }
    }
}

