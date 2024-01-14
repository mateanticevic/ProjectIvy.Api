using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProjectIvy.Business.Caching;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Business.Handlers.Geohash;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Binding.Geohash;
using ProjectIvy.Model.Binding.Location;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.Geohash;
using ProjectIvy.Model.View.Location;

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

        public async Task Create(LocationBinding b)
        {
            using var context = GetMainContext();

            var location = b.ToEntity(context);
            location.UserId = UserId;

            await context.Locations.AddAsync(location);
            await context.SaveChangesAsync();
        }

        public async Task<PagedView<Model.View.Location.Location>> Get(LocationGetBinding b)
        {
            using var context = GetMainContext();

            return await context.Locations.WhereUser(UserId)
                                          .Include(x => x.Geohashes)
                                          .WhereIf(!string.IsNullOrWhiteSpace(b.Search), x => x.Name.ToLower().Contains(b.Search.ToLower()))
                                          .Select(x => new Model.View.Location.Location(x))
                                          .ToPagedViewAsync(b);
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

        public async Task<IEnumerable<LocationType>> GetLocationTypes()
        {
            using var context = GetMainContext();

            return await context.LocationTypes.OrderBy(x => x.Name)
                                              .Select(x => new LocationType(x))
                                              .ToListAsync();
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
