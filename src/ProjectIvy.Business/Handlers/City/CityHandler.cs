using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.City;
using ProjectIvy.Model.View;
using System;
using System.Collections.Generic;
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
                var cities = context.Trips
                                    .WhereUser(User)
                                    .Where(x => x.TimestampEnd < DateTime.Now)
                                    .Include(x => x.Cities)
                                    .OrderBy(x => x.TimestampStart)
                                    .SelectMany(x => x.Cities)
                                    .Select(x => new View.City(x))
                                    .ToList();

                return cities.Distinct(new View.CityComparer());
            }
        }
    }
}
