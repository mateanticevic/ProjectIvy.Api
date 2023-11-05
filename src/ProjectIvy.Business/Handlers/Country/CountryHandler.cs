using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Country;
using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.View;
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
                return context.Trips.WhereUser(UserId)
                                    .Where(x => x.TimestampEnd < DateTime.Now)
                                    .Include(x => x.Cities)
                                    .SelectMany(x => x.Cities)
                                    .Select(x => x.Country)
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

        public async Task<PagedView<Model.View.City.City>> GetCities(string countryValueId, FilteredPagedBinding binding)
        {
            using (var context = GetMainContext())
            {
                int countryId = context.Countries.GetId(countryValueId).Value;
                return context.Cities.Where(x => x.CountryId == countryId)
                                           .OrderByDescending(x => x.Population)
                                           .Select(x => new Model.View.City.City(x))
                                           .ToPagedView(binding);
            }
        }

        public async Task<IEnumerable<View.CountryList>> GetLists()
        {
            using (var context = GetMainContext())
            {
                return await context.CountryLists.Include(x => x.Countries)
                                                 .ThenInclude(x => x.Country)
                                                 .Where(x => !x.UserId.HasValue || x.UserId == UserId)
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
                return context.Trackings
                              .WhereUser(UserId)
                              .Where(x => x.CountryId.HasValue)
                              .Include(x => x.Country)
                              .GroupBy(x => x.Country)
                              .OrderBy(x => x.Min(y => y.Timestamp))
                              .Select(x => new View.Country(x.Key))
                              .ToList();


                var countries = context.CitiesVisited
                                       .Include(x => x.Trip)
                                       .Include(x => x.City)
                                       .Where(x => x.Trip.UserId == UserId)
                                       .Where(x => x.Trip.TimestampEnd < DateTime.Now)
                                       .WhereIf(binding.From.HasValue, x => x.Trip.TimestampEnd > binding.From.Value)
                                       .WhereIf(binding.To.HasValue, x => x.Trip.TimestampStart < binding.To.Value)
                                       .Select(x => new { EnteredOn = x.Timestamp, x.City.Country, TimestampStart = x.Trip.TimestampStart })
                                       .OrderBy(x => x.TimestampStart)
                                       .ThenBy(x => x.EnteredOn)
                                       .Select(x => new View.Country(x.Country))
                                       .ToList();

                var user = context.Users.GetById(UserId);

                var birthCountry = user.BirthCityId.HasValue ? context.Cities.Include(x => x.Country)
                                                                             .SingleOrDefault(x => x.Id == user.BirthCityId.Value)
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

        public async Task<IEnumerable<KeyValuePair<int, int>>> GetVisitedByYear()
        {
            using var context = GetMainContext();

            var list = await context.Trackings.WhereUser(UserId)
                                          .Where(x => x.CountryId.HasValue)
                                          .GroupBy(x => x.Timestamp.Date.Year)
                                          .Select(x => new KeyValuePair<int, int>(x.Key, x.Select(y => y.CountryId.Value).Distinct().Count()))
                                          .ToListAsync();

            return list.OrderBy(x => x.Key);
        }

        public IEnumerable<View.CountryBoundaries> GetBoundaries(IEnumerable<View.Country> countries)
        {
            using (var context = GetMainContext())
            {
                var countryValueIds = countries.Select(x => x.Id);
                var polygons = context.CountryPolygons.Where(x => countryValueIds.Any(y => y == x.Country.ValueId))
                                                      .Include(x => x.Country)
                                                      .ToList();

                foreach (var countryPolygons in polygons.GroupBy(x => new { x.Country.ValueId }))
                {
                    var paths = new List<IEnumerable<Model.View.LatLng>>();

                    foreach (var countryPolygon in countryPolygons.GroupBy(x => x.GroupId))
                    {
                        var path = countryPolygon.OrderBy(x => x.Index)
                                                 .Select(x => new Model.View.LatLng(x.Latitude, x.Longitude))
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

        public async Task<IEnumerable<KeyValuePair<View.Country, int>>> GetDaysInCountry()
        {
            using var context = GetMainContext();

            var countries = await context.Trackings.WhereUser(UserId)
                                          .Include(x => x.Country)
                                          .Where(x => x.CountryId.HasValue)
                                          .GroupBy(x => x.Country)
                                          .Select(x => new KeyValuePair<View.Country, int>(
                                              new View.Country(x.Key),
                                              x.Select(x => x.Timestamp.Date).Distinct().Count()
                                           ))
                                          .ToListAsync();

            return countries.OrderByDescending(x => x.Value);
        }
    }
}
