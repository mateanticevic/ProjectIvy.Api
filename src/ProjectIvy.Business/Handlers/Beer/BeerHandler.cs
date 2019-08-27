using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.Database.Main.Beer;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Linq;
using View = ProjectIvy.Model.View.Beer;

namespace ProjectIvy.Business.Handlers.Beer
{
    public class BeerHandler : Handler<BeerHandler>, IBeerHandler
    {
        public BeerHandler(IHandlerContext<BeerHandler> context) : base(context)
        {
        }

        public string CreateBeer(string brandValueId, BeerBinding binding)
        {
            using (var context = GetMainContext())
            {
                int? brandId = context.BeerBrands.GetId(brandValueId);

                if (!brandId.HasValue)
                    throw new ResourceNotFoundException();

                var entity = binding.ToEntity(brandId.Value);
                context.Beers.Add(entity);
                context.SaveChanges();

                return entity.ValueId;
            }
        }

        public string CreateBrand(string name)
        {
            using (var context = GetMainContext())
            {
                var beerBrand = new BeerBrand()
                {
                    ValueId = name.ToValueId(),
                    Name = name
                };

                context.BeerBrands.Add(beerBrand);
                context.SaveChanges();

                return beerBrand.ValueId;
            }
        }

        public View.Beer GetBeer(string id)
        {
            using (var context = GetMainContext())
            {
                return context.Beers.SingleOrDefault(x => x.ValueId == id).ConvertTo(x => new View.Beer(x));
            }
        }

        public PagedView<View.Beer> GetBeers(BeerGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int? brandId = context.BeerBrands.GetId(binding.BrandId);

                return context.Beers.WhereIf(brandId.HasValue, x => x.BeerBrandId == brandId).OrderBy(binding)
                                    .WhereIf(!string.IsNullOrWhiteSpace(binding.Search), x => x.Name.ToLower().Contains(binding.Search.ToLower()))
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
