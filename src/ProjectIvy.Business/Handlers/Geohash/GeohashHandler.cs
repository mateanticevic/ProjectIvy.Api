﻿using System.Linq;
using System.Threading.Tasks;
using Geohash;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Geohash;

namespace ProjectIvy.Business.Handlers.Geohash
{
    public class GeohashHandler : Handler<GeohashHandler>, IGeohashHandler
    {
        public GeohashHandler(IHandlerContext<GeohashHandler> context) : base(context)
        {
        }

        public async Task<IEnumerable<Model.View.Geohash.RouteTime>> FromGeohashToGeohash(string fromGeohash, string toGeohash, RouteTimeSort sort)
        {
            using var context = GetMainContext();

            var fromTimestamps = await context.Trackings.WhereUser(UserId)
                                                        .Where(x => x.Geohash.StartsWith(fromGeohash))
                                                        .GroupBy(x => x.Timestamp.Date)
                                                        .Select(x => new { Day = x.Key, Time = x.Max(y => y.Timestamp) })
                                                        .ToListAsync();

            var toTimestamps = await context.Trackings.WhereUser(UserId)
                                                      .Where(x => x.Geohash.StartsWith(toGeohash))
                                                      .GroupBy(x => x.Timestamp.Date)
                                                      .Select(x => new { Day = x.Key, Time = x.Min(y => y.Timestamp) })
                                                      .ToListAsync();

            var routes = fromTimestamps.Join(toTimestamps, x => x.Day, x => x.Day, (from, to) => (from.Time, to.Time))
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
                                          .OrderByDescending(x => x.Date)
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

                return await context.GeohashCities.Include(x => x.City)
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

                return await context.GeohashCountries.Include(x => x.Country)
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
                return await context.GeohashCities.Where(x => x.CityId == cityId)
                                                  .Select(x => x.Geohash)
                                                  .ToListAsync();
            }
        }

        public async Task<IEnumerable<string>> GetCountryGeohashes(string countryValueId)
        {
            using (var context = GetMainContext())
            {
                int countryId = context.Countries.GetId(countryValueId).Value;
                return await context.GeohashCountries.Where(x => x.CountryId == countryId)
                                                     .Select(x => x.Geohash)
                                                     .ToListAsync();
            }
        }
    }
}
