using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.City;
using ProjectIvy.Model.View;
using System.Linq;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.City;

namespace ProjectIvy.Business.Handlers.City
{
    public class CityHandler : Handler<CityHandler>, ICityHandler
    {
        public CityHandler(IHandlerContext<CityHandler> context) : base(context)
        {
        }

        public async Task<PagedView<View.City>> Get(CityGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var query = context.Cities.Include(x => x.Country)
                                          .WhereIf(!string.IsNullOrEmpty(binding.Search), city => city.Name.ToLower().Contains(binding.Search.ToLower()))
                                          .WhereIf(!string.IsNullOrEmpty(binding.CountryId), city => city.Country.ValueId == binding.CountryId)
                                          .OrderBy(x => x.Name)
                                          .Select(x => new View.City(x));

                return await query.ToPagedViewAsync(binding);
            }
        }

        public IEnumerable<View.City> GetVisited()
        {
            using (var context = GetMainContext())
            {
                var cities = context.CitiesVisited
                                    .Include(x => x.City)
                                    .WhereUser(UserId)
                                    .Where(x => !x.TripId.HasValue || x.Trip.TimestampEnd < DateTime.Now)
                                    .Select(x => new View.City(x.City))
                                    .ToList();

                return cities.Distinct(new View.CityComparer());
            }
        }
    }
}
