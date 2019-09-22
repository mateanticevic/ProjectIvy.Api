using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.City;
using ProjectIvy.Model.View;
using System.Linq;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.City
{
    public class CityHandler : Handler<CityHandler>, ICityHandler
    {
        public CityHandler(IHandlerContext<CityHandler> context) : base(context)
        {
        }

        public async Task<PagedView<View.City.City>> Get(CityGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var query = context.Cities.Include(x => x.Country)
                                          .WhereIf(!string.IsNullOrEmpty(binding.Search), city => city.Name.ToLower().Contains(binding.Search.ToLower()))
                                          .WhereIf(!string.IsNullOrEmpty(binding.CountryId), city => city.Country.ValueId == binding.CountryId)
                                          .Select(x => new View.City.City(x))
                                          .OrderBy(x => x.Name);

                return await query.ToPagedViewAsync(binding);
            }
        }
    }
}
