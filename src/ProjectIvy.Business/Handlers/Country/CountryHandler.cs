using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Country;
using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Country;

namespace ProjectIvy.Business.Handlers.Country
{
    public class CountryHandler : Handler<CountryHandler>, ICountryHandler
    {
        public CountryHandler(IHandlerContext<CountryHandler> context) : base(context)
        {
        }

        public long Count(CountryGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var countries = context.Countries;

                return countries.Count();
            }
        }

        public long CountVisited()
        {
            using (var context = GetMainContext())
            {
                return context.Trips.WhereUser(User)
                                    .Where(x => x.TimestampEnd < DateTime.Now)
                                    .Include(x => x.Cities)
                                    .SelectMany(x => x.Cities)
                                    .Select(x => x.City.Country)
                                    .Distinct()
                                    .Select(x => x)
                                    .LongCount();
            }
        }

        public View.Country Get(string id)
        {
            using (var context = GetMainContext())
            {
                var country = context.Countries.SingleOrDefault(x => x.ValueId == id);

                return new View.Country(country);
            }
        }

        public PagedView<View.Country> Get(CountryGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var countries = context.Countries;

                long count = countries.Count();

                var items = countries.OrderBy(x => x.Name)
                                     .Page(binding)
                                     .ToList()
                                     .Select(x => new View.Country(x))
                                     .ToList();

                return new PagedView<View.Country>()
                {
                    Count = count,
                    Items = items
                };
            }
        }

        public async Task<IEnumerable<View.CountryList>> GetLists()
        {
            using (var context = GetMainContext())
            {
                return await context.CountryLists.Include(x => x.Countries)
                                                 .ThenInclude(x => x.Country)
                                                 .Where(x => !x.UserId.HasValue || x.UserId == User.Id)
                                                 .Select(x => new View.CountryList(x))
                                                 .ToListAsync();
            }
        }

        public async Task<IEnumerable<View.CountryListVisited>> GetListsVisited()
        {
            var lists = await GetLists();
            var visitedCountries = GetVisited(new TripGetBinding()).Select(x => x.Id).ToList();

            return lists.Select(x => new View.CountryListVisited(x)
            {
                CountriesNotVisited = x.Countries.Where(y => !visitedCountries.Contains(y.Id)),
                CountriesVisited = x.Countries.Where(y => visitedCountries.Contains(y.Id))
            });
        }

        public IEnumerable<View.Country> GetVisited(TripGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var countries = context.Trips
                                       .WhereUser(User)
                                       .Where(x => x.TimestampEnd < DateTime.Now)
                                       .WhereIf(binding.From.HasValue, x => x.TimestampEnd > binding.From.Value)
                                       .WhereIf(binding.To.HasValue, x => x.TimestampStart < binding.To.Value)
                                       .Include(x => x.Cities)
                                       .SelectMany(x => x.Cities)
                                       .Select(x => new { x.EnteredOn, x.City.Country, x.Trip.TimestampStart })
                                       .OrderBy(x => x.TimestampStart)
                                       .ThenBy(x => x.EnteredOn)
                                       .Select(x => new View.Country(x.Country))
                                       .ToList();

                var birthCountry = User.BirthCityId.HasValue ? context.Cities.Include(x => x.Country)
                                                                             .SingleOrDefault(x => x.Id == User.BirthCityId.Value)
                                                                             .Country : null;

                if (birthCountry != null)
                {
                    var existingBirthCountry = countries.FirstOrDefault(x => x.Id == birthCountry.ValueId);
                    if (existingBirthCountry != null)
                        countries.Remove(existingBirthCountry);

                    countries.Insert(0, new View.Country(birthCountry));
                }

                return countries.Distinct(new View.CountryComparer());
            }
        }

        public IEnumerable<View.CountryBoundaries> GetBoundaries(IEnumerable<View.Country> countries)
        {
            using (var context = GetMainContext())
            {
                var polygons = context.CountryPolygons.Where(x => countries.Any(y => y.Id == x.Country.ValueId))
                                                        .Include(x => x.Country)
                                                        .ToList();

                foreach (var countryPolygons in polygons.GroupBy(x => new { x.Country.ValueId }))
                {
                    var paths = new List<IEnumerable<Location>>();

                    foreach (var countryPolygon in countryPolygons.GroupBy(x => x.GroupId))
                    {
                        var path = countryPolygon.OrderBy(x => x.Index)
                                                 .Select(x => new Location(x.Latitude, x.Longitude))
                                                 .ToList();
                        paths.Add(path);
                    }

                    var countryBoundaries = new View.CountryBoundaries()
                    {
                        Country = countries.SingleOrDefault(x => x.Id == countryPolygons.Key.ValueId),
                        Polygons = paths
                    };

                    yield return countryBoundaries;
                }
            }
        }
    }
}
