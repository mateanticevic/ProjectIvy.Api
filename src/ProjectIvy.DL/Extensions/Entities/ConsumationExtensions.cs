﻿using Microsoft.EntityFrameworkCore;
using ProjectIvy.DL.DbContexts;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.Database.Main.Beer;
using System.Linq;

namespace ProjectIvy.DL.Extensions.Entities
{
    public static class ConsumationExtensions
    {
        public static IQueryable<Consumation> Where(this IQueryable<Consumation> query, ConsumationGetBinding binding, MainContext context)
        {
            var beerId = context.Beers.GetId(binding.BeerId);
            var beerBrandId = context.BeerBrands.GetId(binding.BeerBrandId);

            return query.Include(x => x.Beer)
                        .WhereIf(binding.From.HasValue, x => x.Date >= binding.From.Value)
                        .WhereIf(binding.To.HasValue, x => x.Date <= binding.To.Value)
                        .WhereIf(binding.Serving.HasValue, x => x.BeerServingId == (int)binding.Serving.Value)
                        .WhereIf(beerId.HasValue, x => x.BeerId == beerId.Value)
                        .WhereIf(beerBrandId.HasValue, x => x.Beer.BeerBrandId == beerBrandId.Value);
        }
    }
}