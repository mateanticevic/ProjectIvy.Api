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
using ProjectIvy.Model.Binding;
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
                                          .OrderBy(x => x.Name)
                                          .Select(x => new Model.View.Location.Location(x))
                                          .ToPagedViewAsync(b);
        }

        public async Task<IEnumerable<string>> GetGeohashes(string valueId)
        {
            using var context = GetMainContext();

            return (await context.Locations.WhereUser(UserId)
                                                  .Include(x => x.Geohashes)
                                                  .FirstOrDefaultAsync(x => x.ValueId == valueId))
                                                  ?.Geohashes
                                                  .Select(x => x.Geohash);
        }

        public async Task<IEnumerable<KeyValuePair<DateTime, IEnumerable<Model.View.Location.Location>>>> GetByDay(FilteredBinding b)
        {
            if (b.From == null || b.To == null)
                throw new ArgumentException("From and To are required for GetByDay");

            using var context = GetMainContext();

            var locationsPerDay = await (from t in context.Trackings.WhereUser(UserId)
                                         from lg in context.Locations.WhereUser(UserId).Include(x => x.Geohashes).SelectMany(x => x.Geohashes)
                                         where t.Timestamp >= b.From.Value && t.Timestamp <= b.To.Value.AddDays(1)
                                         where t.Geohash.StartsWith(lg.Geohash)
                                         select new { lg.Location, t.Timestamp.Date }).Distinct().ToListAsync();

            return locationsPerDay.GroupBy(x => x.Date)
                                  .OrderByDescending(x => x.Key)
                                  .Select(x => new KeyValuePair<DateTime, IEnumerable<Model.View.Location.Location>>(x.Key, x.Select(y => new Model.View.Location.Location(y.Location))))
                                  .ToList();
        }

        public async Task<IEnumerable<DateTime>> GetDays(string locationId, FilteredBinding binding)
        {
            string cacheKey = BuildUserCacheKey(CacheKeyGenerator.LocationDays(locationId));
            return await MemoryCache.GetOrCreateAsync(cacheKey,
                async cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    using var context = GetMainContext();

                    var location = await context.Locations.WhereUser(UserId)
                                                          .SingleOrDefaultAsync(x => x.ValueId == locationId);

                    var days = await context.Trackings.WhereUser(UserId)
                                                      .Where(x => x.LocationId == location.Id)
                                                      .WhereIf(binding.From != null, x => x.Timestamp >= binding.From)
                                                      .WhereIf(binding.To != null, x => x.Timestamp <= binding.To)
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
    }
}
