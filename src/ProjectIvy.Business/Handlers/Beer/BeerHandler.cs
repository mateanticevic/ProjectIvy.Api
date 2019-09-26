﻿using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.Database.Main.Beer;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Beer;

namespace ProjectIvy.Business.Handlers.Beer
{
    public class BeerHandler : Handler<BeerHandler>, IBeerHandler
    {
        public BeerHandler(IHandlerContext<BeerHandler> context) : base(context)
        {
        }

        public async Task<string> CreateBeer(string brandValueId, BeerBinding binding)
        {
            using (var context = GetMainContext())
            {
                int? brandId = context.BeerBrands.GetId(brandValueId);

                if (!brandId.HasValue)
                    throw new ResourceNotFoundException();

                var entity = binding.ToEntity(brandId.Value);
                await context.Beers.AddAsync(entity);
                await context.SaveChangesAsync();

                return entity.ValueId;
            }
        }

        public async Task<string> CreateBrand(string name)
        {
            using (var context = GetMainContext())
            {
                var beerBrand = new BeerBrand()
                {
                    ValueId = name.ToValueId(),
                    Name = name
                };

                await context.BeerBrands.AddAsync(beerBrand);
                await context.SaveChangesAsync();

                return beerBrand.ValueId;
            }
        }

        public async Task<View.Beer> GetBeer(string id)
        {
            using (var context = GetMainContext())
            {
                var beers = await context.Beers.SingleOrDefaultAsync(x => x.ValueId == id);

                return beers.ConvertTo(x => new View.Beer(x));
            }
        }

        public async Task<PagedView<View.Beer>> GetBeers(BeerGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int? brandId = context.BeerBrands.GetId(binding.BrandId);

                return await context.Beers.WhereIf(brandId.HasValue, x => x.BeerBrandId == brandId).OrderBy(binding)
                                          .WhereIf(!string.IsNullOrWhiteSpace(binding.Search), x => x.Name.ToLower().Contains(binding.Search.ToLower()))
                                          .Select(x => new View.Beer(x))
                                          .ToPagedViewAsync(binding);
            }
        }

        public async Task<IEnumerable<View.BeerBrand>> GetBrands()
        {
            using (var context = GetMainContext())
            {
                return await context.BeerBrands.OrderBy(x => x.Name)
                                               .Select(x => new View.BeerBrand(x))
                                               .ToListAsync();
            }
        }

        public async Task<IEnumerable<View.BeerServing>> GetServings()
        {
            using (var context = GetMainContext())
            {
                return await context.BeerServings.Select(x => new View.BeerServing(x)).ToListAsync();
            }
        }
    }
}