using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProjectIvy.Business.Caching;
using ProjectIvy.Business.Handlers.Geohash;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.City;
using ProjectIvy.Model.Binding.Geohash;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.Geohash;
using View = ProjectIvy.Model.View.City;

namespace ProjectIvy.Business.Handlers.City;

public class CityHandler : Handler<CityHandler>, ICityHandler
{
    private readonly IGeohashHandler _geohashHandler;

    public CityHandler(IHandlerContext<CityHandler> context, IGeohashHandler geohashHandler, IMemoryCache memoryCache) : base(context, memoryCache, nameof(GeohashHandler))
    {
        _geohashHandler = geohashHandler;
    }

    public async Task AddVisitedCity(string cityValueId)
    {
        using (var context = GetMainContext())
        {
            int cityId = context.Cities.GetId(cityValueId).Value;
            var entity = new Model.Database.Main.Travel.CityVisited()
            {
                CityId = cityId,
                UserId = UserId
            };

            await context.CitiesVisited.AddAsync(entity);
            await context.SaveChangesAsync();
        }
    }

    public async Task<PagedView<View.City>> Get(CityGetBinding binding)
    {
        using (var context = GetMainContext())
        {
            var query = context.Cities.Include(x => x.Country)
                                      .WhereSearch(binding)
                                      .WhereIf(!string.IsNullOrEmpty(binding.CountryId), city => city.Country.ValueId == binding.CountryId)
                                      .OrderByDescending(x => x.ValueId == binding.Search)
                                      .ThenBy(x => x.Name)
                                      .Select(x => new View.City(x));

            return await query.ToPagedViewAsync(binding);
        }
    }

    public async Task<IEnumerable<DateTime>> GetDays(string cityValueId, FilteredBinding binding)
    {
        string cacheKey = BuildUserCacheKey(CacheKeyGenerator.CityDays(cityValueId, binding));
        return await MemoryCache.GetOrCreateAsync(cacheKey,
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                using var context = GetMainContext();

                var city = await context.Cities.SingleOrDefaultAsync(x => x.ValueId == cityValueId);

                var days = await context.Trackings.WhereUser(UserId)
                                                  .Where(x => x.CityId == city.Id)
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

    public async Task<IEnumerable<RouteTime>> GetRoutes(string fromCityValueId, string toCityValueId, RouteTimeSort sort)
    {
        using var context = GetMainContext();

        var fromGeohashes = await context.CityAccessGeohashes.Include(x => x.City)
                                                             .Where(x => x.City.ValueId == fromCityValueId)
                                                             .Select(x => x.Geohash)
                                                             .ToListAsync();

        var toGeohashes = await context.CityAccessGeohashes.Include(x => x.City)
                                                             .Where(x => x.City.ValueId == toCityValueId)
                                                             .Select(x => x.Geohash)
                                                             .ToListAsync();

        return await _geohashHandler.FromGeohashToGeohash(fromGeohashes, toGeohashes, sort);
    }

    public IEnumerable<View.City> GetVisited()
    {
        using (var context = GetMainContext())
        {
            var cities = context.Trackings
                                .Include(x => x.City)
                                .WhereUser(UserId)
                                .Where(x => x.CityId.HasValue)
                                .Select(x => x.City)
                                .Distinct()
                                .Select(x => new View.City(x))
                                .ToList();

            return cities.Distinct(new View.CityComparer());
        }
    }
}
