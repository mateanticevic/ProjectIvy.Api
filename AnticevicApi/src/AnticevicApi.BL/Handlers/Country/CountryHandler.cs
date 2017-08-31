using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Country;
using AnticevicApi.Model.View;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using View = AnticevicApi.Model.View.Country;

namespace AnticevicApi.BL.Handlers.Country
{
    public class CountryHandler : Handler<CountryHandler>, ICountryHandler
    {
        public CountryHandler(IHandlerContext<CountryHandler> context) : base(context)
        {
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
        
        public IEnumerable<View.Country> GetVisited()
        {
            using (var context = GetMainContext())
            {
                return context.Trips.WhereUser(User)
                                    .Where(x => x.TimestampEnd < DateTime.Now)
                                    .Include(x => x.Cities)
                                    .SelectMany(x => x.Cities)
                                    .Select(x => x.City.Country)
                                    .Distinct()
                                    .Select(x => new View.Country(x))
                                    .ToList();

            }
        }
    }
}
