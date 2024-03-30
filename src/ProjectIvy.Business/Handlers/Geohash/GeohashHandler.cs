using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Geohash;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Geohash;
using ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Business.Handlers.Geohash
{
    public class GeohashHandler : Handler<GeohashHandler>, IGeohashHandler
    {
        private const string GeohashChars = "0123456789bcdefghjkmnpqrstuvwxyz";

        public GeohashHandler(IHandlerContext<GeohashHandler> context) : base(context)
        {
        }

        public async Task AddGeohashToCity(string cityValueId, IEnumerable<string> geohashes)
        {
            using var context = GetMainContext();

            int cityId = context.Cities.GetId(cityValueId).Value;
            await AddGeohashesTo(context.CityGeohashes, geohashes, x => x.CityId == cityId, () => new Model.Database.Main.Common.CityGeohash() { CityId = cityId });
            _ = context.Trackings.WhereUser(UserId)
                                 .Where(x => geohashes.Any(y => x.Geohash.StartsWith(y)))
                                 .ExecuteUpdateAsync(x => x.SetProperty(x => x.CityId, cityId));
            await context.SaveChangesAsync();
        }

        public async Task AddGeohashToCountry(string countryValueId, IEnumerable<string> geohashes)
        {
            using var context = GetMainContext();

            int countryId = context.Countries.GetId(countryValueId).Value;
            await AddGeohashesTo(context.CountryGeohashes, geohashes, x => x.CountryId == countryId, () => new Model.Database.Main.Common.CountryGeohash() { CountryId = countryId });
            _ = context.Trackings.WhereUser(UserId)
                                 .Where(x => geohashes.Any(y => x.Geohash.StartsWith(y)))
                                 .ExecuteUpdateAsync(x => x.SetProperty(x => x.CountryId, countryId));
            await context.SaveChangesAsync();
        }

        public async Task AddGeohashToLocation(string locationValueId, IEnumerable<string> geohashes)
        {
            using var context = GetMainContext();

            int locationId = context.Locations.WhereUser(UserId).GetId(locationValueId).Value;
            await AddGeohashesTo(context.LocationGeohashes, geohashes, x => x.LocationId == locationId, () => new Model.Database.Main.Tracking.LocationGeohash() { LocationId = locationId });
            await context.SaveChangesAsync();
        }

        public async Task AddGeohashesTo<TGeohash>(DbSet<TGeohash> geohashItems, IEnumerable<string> geohashes, Expression<Func<TGeohash, bool>> matchItem, Func<TGeohash> itemFactory) where TGeohash : class, IHasGeohash
        {
            foreach (string geohash in geohashes)
            {
                var childGeohashes = geohashItems.Where(matchItem)
                                                 .Where(x => x.Geohash.StartsWith(geohash));

                geohashItems.RemoveRange(childGeohashes);
                var entity = itemFactory();
                entity.Geohash = geohash;
                await geohashItems.AddAsync(entity);
            }
        }

        public async Task<int> CountUnique(GeohashUniqueGetBinding binding)
        {
            using var context = GetMainContext();

            if (binding.OnlyNew)
            {
                return await context.Trackings.WhereUser(UserId)
                              .GroupBy(x => x.Geohash.Substring(0, binding.Precision))
                              .Select(x => new { x.Key, Timestamp = x.Min(y => y.Timestamp) })
                              .WhereIf(binding.From.HasValue, x => x.Timestamp > binding.From)
                              .WhereIf(binding.From.HasValue, x => x.Timestamp >= binding.From)
                              .WhereIf(binding.To.HasValue, x => x.Timestamp <= binding.To)
                              .CountAsync();
            }

            return await context.Trackings.WhereUser(UserId)
                                          .WhereTimestampInclusive(binding)
                                          .GroupBy(x => x.Geohash.Substring(0, binding.Precision))
                                          .Select(x => new { x.Key, Timestamp = x.Min(y => y.Timestamp) })
                                          .CountAsync();
        }

        public async Task DeleteTrackings(string geohash)
        {
            using var context = GetMainContext();

            if (geohash.Length < 7)
                throw new ArgumentException("Geohash must be at least 5 characters long");

            await context.Trackings.WhereUser(UserId)
                                   .Where(x => x.Geohash.StartsWith(geohash))
                                   .ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<Model.View.Geohash.RouteTime>> FromGeohashToGeohash(IEnumerable<string> fromGeohashes, IEnumerable<string> toGeohashes, RouteTimeSort sort)
        {
            using var context = GetMainContext();


            var fromTms = TimestampsByDay(context, fromGeohashes.FirstOrDefault(), true);

            var fromTimestamps = fromGeohashes.Select(x => TimestampsByDay(context, x, true))
                                              .SelectMany(x => x)
                                              .GroupBy(x => x.Date)
                                              .Select(x => x.Max())
                                              .ToList();

            var toTimestamps = toGeohashes.Select(x => TimestampsByDay(context, x, false))
                                        .SelectMany(x => x)
                                        .GroupBy(x => x.Date)
                                        .Select(x => x.Min())
                                        .ToList();

            var routes = fromTimestamps.Join(toTimestamps, x => x.Date, x => x.Date, (from, to) => (from, to))
                                       .Where(x => x.Item1 < x.Item2)
                                       .Select(x => new Model.View.Geohash.RouteTime() { From = x.Item1, To = x.Item2, Duration = x.Item2.Subtract(x.Item1) });

            return sort == RouteTimeSort.Date
                           ? routes.OrderByDescending(x => x.From)
                           : routes.OrderBy(x => x.Duration);
        }

        public async Task<IEnumerable<DateOnly>> GetDays(string geohash)
        {
            using var context = GetMainContext();

            var dateTimes = await context.Trackings.WhereUser(UserId)
                                          .Where(x => x.Geohash.StartsWith(geohash))
                                          .Select(x => x.Timestamp.Date)
                                          .Distinct()
                                          .OrderByDescending(x => x.Date)
                                          .ToListAsync();

            return dateTimes.Select(DateOnly.FromDateTime);
        }

        public async Task<Model.View.Geohash.Geohash> GetGeohash(string geohashId)
        {
            using (var context = GetMainContext())
            {
                var query = context.Trackings.WhereUser(UserId)
                                                      .Where(x => x.Geohash.StartsWith(geohashId));

                var firstIn = (await query.OrderBy(x => x.Timestamp)
                                          .FirstOrDefaultAsync())?.Timestamp;

                if (firstIn is null)
                    return null;

                int totalCount = await query.CountAsync();

                var lastIn = totalCount == 1 ? null : (await query.OrderBy(x => x.Timestamp)
                      .OrderByDescending(x => x.Timestamp)
                      .FirstOrDefaultAsync())?.Timestamp;

                int dayCount = await query.GroupBy(x => x.Timestamp.Date)
                                           .CountAsync();

                return new Model.View.Geohash.Geohash()
                {
                    DayCount = dayCount,
                    FirstIn = firstIn.Value,
                    LastIn = lastIn.HasValue ? lastIn.Value : firstIn.Value,
                    TotalCount = totalCount
                };
            }
        }

        public async Task<IEnumerable<string>> GetGeohashes(GeohashGetBinding binding)
        {
            var geohasher = new Geohasher();

            using (var context = GetMainContext())
            {
                var neighbours = binding.Geohash is null ? null : geohasher.GetNeighbors(binding.Geohash);

                return await context.Trackings.WhereUser(UserId)
                                              .WhereTimestampInclusive(binding)
                                              .WhereIf(neighbours is not null, x => x.Geohash.StartsWith(binding.Geohash)
                                                     || x.Geohash.StartsWith(neighbours[Direction.North])
                                                     || x.Geohash.StartsWith(neighbours[Direction.South])
                                                     || x.Geohash.StartsWith(neighbours[Direction.East])
                                                     || x.Geohash.StartsWith(neighbours[Direction.West])
                                                     || x.Geohash.StartsWith(neighbours[Direction.NorthEast])
                                                     || x.Geohash.StartsWith(neighbours[Direction.NorthWest])
                                                     || x.Geohash.StartsWith(neighbours[Direction.SouthEast])
                                                     || x.Geohash.StartsWith(neighbours[Direction.SouthWest]))
                                              .Select(x => x.Geohash.Substring(0, binding.Precision))
                                              .Distinct()
                                              .ToListAsync();
            }
        }

        public async Task<IEnumerable<string>> GetChildren(string geohash)
        {
            using (var context = GetMainContext())
            {
                int geohashLength = geohash?.Length ?? 0;
                return await context.Trackings.WhereUser(UserId)
                                              .WhereIf(!string.IsNullOrWhiteSpace(geohash), x => x.Geohash.StartsWith(geohash))
                                              .GroupBy(x => x.Geohash.Substring(0, geohashLength + 1))
                                              .Select(x => x.Key)
                                              .ToListAsync();
            }
        }

        public async Task<Model.View.City.City> GetCity(string geohash)
        {
            using (var context = GetMainContext())
            {
                var geohashes = Enumerable.Range(0, geohash.Length)
                                          .Select(x => geohash.Substring(0, geohash.Length - x))
                                          .ToList();

                return await context.CityGeohashes.Include(x => x.City)
                                                  .ThenInclude(x => x.Country)
                                                  .Where(x => geohashes.Contains(x.Geohash))
                                                  .OrderByDescending(x => x.Geohash.Length)
                                                  .Select(x => new Model.View.City.City(x.City))
                                                  .FirstOrDefaultAsync();
            }
        }

        public async Task<Model.View.Country.Country> GetCountry(string geohash)
        {
            using (var context = GetMainContext())
            {
                var geohashes = Enumerable.Range(0, geohash.Length)
                                          .Select(x => geohash.Substring(0, geohash.Length - x))
                                          .ToList();

                return await context.CountryGeohashes.Include(x => x.Country)
                                                     .Where(x => geohashes.Contains(x.Geohash))
                                                     .OrderByDescending(x => x.Geohash.Length)
                                                     .Select(x => new Model.View.Country.Country(x.Country))
                                                     .FirstOrDefaultAsync();
            }
        }

        public async Task<IEnumerable<string>> GetCityGeohashes(string cityValueId)
        {
            using (var context = GetMainContext())
            {
                int cityId = context.Cities.GetId(cityValueId).Value;
                return await context.CityGeohashes.Where(x => x.CityId == cityId)
                                                  .Select(x => x.Geohash)
                                                  .ToListAsync();
            }
        }

        public async Task<IEnumerable<string>> GetCountryGeohashes(string countryValueId)
        {
            using (var context = GetMainContext())
            {
                int countryId = context.Countries.GetId(countryValueId).Value;
                return await context.CountryGeohashes.Where(x => x.CountryId == countryId)
                                                     .Select(x => x.Geohash)
                                                     .ToListAsync();
            }
        }

        public async Task<IEnumerable<string>> GetUnique(GeohashUniqueGetBinding binding)
        {
            using var context = GetMainContext();

            if (binding.OnlyNew)
            {
                return await context.Trackings.WhereUser(UserId)
                              .GroupBy(x => x.Geohash.Substring(0, binding.Precision))
                              .Select(x => new { x.Key, Timestamp = x.Min(y => y.Timestamp) })
                              .WhereIf(binding.From.HasValue, x => x.Timestamp > binding.From)
                              .WhereIf(binding.From.HasValue, x => x.Timestamp >= binding.From)
                              .WhereIf(binding.To.HasValue, x => x.Timestamp <= binding.To)
                              .Select(x => x.Key)
                              .ToListAsync();
            }

            return await context.Trackings.WhereUser(UserId)
                                          .WhereTimestampInclusive(binding)
                                          .GroupBy(x => x.Geohash.Substring(0, binding.Precision))
                                          .Select(x => new { x.Key, Timestamp = x.Min(y => y.Timestamp) })
                                          .Select(x => x.Key)
                                          .ToListAsync();
        }

        public async Task RemoveGeohashFromCity(string cityValueId, IEnumerable<string> geohashes)
        {
            using var context = GetMainContext();

            int cityId = context.Cities.GetId(cityValueId).Value;
            await RemoveGeohashFrom(context.CityGeohashes, geohashes, x => x.CityId == cityId, x => new Model.Database.Main.Common.CityGeohash() { CityId = cityId });
            await context.Trackings.WhereUser(UserId)
                                   .Where(x => geohashes.Any(y => x.Geohash.StartsWith(y)))
                                   .ExecuteUpdateAsync(x => x.SetProperty(x => x.CityId, (int?)null));
            await context.SaveChangesAsync();
        }

        public async Task RemoveGeohashFromCountry(string countryValueId, IEnumerable<string> geohashes)
        {
            using var context = GetMainContext();

            int countryId = context.Countries.GetId(countryValueId).Value;
            await RemoveGeohashFrom(context.CountryGeohashes, geohashes, x => x.CountryId == countryId, x => new Model.Database.Main.Common.CountryGeohash() { CountryId = countryId });
            await context.Trackings.WhereUser(UserId)
                                   .Where(x => geohashes.Any(y => x.Geohash.StartsWith(y)))
                                   .ExecuteUpdateAsync(x => x.SetProperty(x => x.CountryId, (int?)null));
            await context.SaveChangesAsync();
        }

        public async Task RemoveGeohashFromLocation(string locationValueId, IEnumerable<string> geohashes)
        {
            using var context = GetMainContext();

            int locationId = context.Locations.WhereUser(UserId).GetId(locationValueId).Value;
            await RemoveGeohashFrom(context.LocationGeohashes, geohashes, x => x.LocationId == locationId, x => new Model.Database.Main.Tracking.LocationGeohash() { LocationId = locationId });
            await context.SaveChangesAsync();
        }

        private async Task RemoveGeohashFrom<TGeohash>(DbSet<TGeohash> geohashItems, IEnumerable<string> geohashes, Expression<Func<TGeohash, bool>> matchItem, Func<int, TGeohash> itemFactory) where TGeohash : class, IHasGeohash
        {
            TGeohash itemGeohash = null;
            var geohashesToAdd = new List<string>();
            var geohashesToDelete = new List<TGeohash>();
            foreach (string geohash in geohashes.OrderBy(x => x))
            {
                var parentGeohashes = Enumerable.Range(0, geohash.Length - 1).Select(x => geohash.Substring(0, geohash.Length - x));

                itemGeohash = itemGeohash != null && geohash.StartsWith(itemGeohash.Geohash) ? itemGeohash : await geohashItems.Where(matchItem)
                                                            .SingleOrDefaultAsync(x => parentGeohashes.Contains(x.Geohash));

                if (itemGeohash is null)
                    continue;
                else if (itemGeohash.Geohash == geohash)
                {
                    geohashesToDelete.Add(itemGeohash);
                    continue;
                }

                for (int i = geohash.Length; i > itemGeohash.Geohash.Length; i--)
                {
                    char geoChar = geohash[i - 1];
                    string parentGeohash = geohash.Substring(0, i - 1);

                    var neighbours = GeohashChars.Where(x => x != geoChar)
                                                 .Select(x => $"{parentGeohash}{x}")
                                                 .Where(x => !geohashesToAdd.Contains(x))
                                                 .Where(x => !geohashes.Contains(x))
                                                 .ToList();
                    geohashesToAdd.AddRange(neighbours);
                }

                geohashItems.Remove(itemGeohash);
            }
            await geohashItems.AddRangeAsync(geohashesToAdd.Distinct().Select(x =>
            {
                var item = itemFactory(0);
                item.Geohash = x;
                return item;
            }));
            geohashItems.RemoveRange(geohashesToDelete.Distinct());
        }

        private IQueryable<DateTime> TimestampsByDay(MainContext context, string geohash, bool last)
        {
            return context.Trackings.WhereUser(UserId)
                                    .Where(x => x.Geohash.StartsWith(geohash))
                                    .GroupBy(x => x.Timestamp.Date)
                                    .Select(x => last ? x.Max(y => y.Timestamp) : x.Min(y => y.Timestamp));
        }
    }
}
