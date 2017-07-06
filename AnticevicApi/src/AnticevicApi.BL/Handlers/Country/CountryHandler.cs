using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Country;
using AnticevicApi.Model.View;
using System.Linq;
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
    }
}
