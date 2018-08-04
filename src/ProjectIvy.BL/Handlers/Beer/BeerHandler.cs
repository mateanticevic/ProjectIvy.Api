using ProjectIvy.DL.Extensions;
using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.View;
using ProjectIvy.DL.Extensions.Entities;
using System.Collections.Generic;
using System.Linq;
using View = ProjectIvy.Model.View.Beer;

namespace ProjectIvy.BL.Handlers.Beer
{
    public class BeerHandler : Handler<BeerHandler>, IBeerHandler
    {
        public BeerHandler(IHandlerContext<BeerHandler> context) : base(context)
        {
        }

        public PagedView<View.Beer> GetBeers(BeerGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int? brandId = context.BeerBrands.GetId(binding.BrandId);

                return context.Beers.WhereIf(brandId.HasValue, x => x.BeerBrandId == brandId).OrderBy(binding)
                                    .Select(x => new View.Beer(x))
                                    .ToPagedView(binding);
            }
        }

        public IEnumerable<View.BeerBrand> GetBrands()
        {
            using (var context = GetMainContext())
            {
                return context.BeerBrands.OrderBy(x => x.Name)
                                         .Select(x => new View.BeerBrand(x))
                                         .ToList();
            }
        }

        public IEnumerable<View.BeerServing> GetServings()
        {
            using (var context = GetMainContext())
            {
                return context.BeerServings.Select(x => new View.BeerServing(x))
                                           .ToList();
            }
        }
    }
}
