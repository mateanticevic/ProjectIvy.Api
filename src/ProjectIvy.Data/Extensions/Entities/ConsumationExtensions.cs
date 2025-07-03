using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.Database.Main.Beer;

namespace ProjectIvy.Data.Extensions.Entities;

public static class ConsumationExtensions
{
    public static IQueryable<Consumation> Where(this IQueryable<Consumation> query, FilteredPagedBinding binding)
    {
        return query.Include(x => x.Beer)
                    .WhereIf(binding.From.HasValue, x => x.Date >= binding.From.Value)
                    .WhereIf(binding.To.HasValue, x => x.Date <= binding.To.Value);
    }

    public static IQueryable<Consumation> Where(this IQueryable<Consumation> query, ConsumationGetBinding binding, MainContext context)
    {
        var beerId = context.Beers.GetId(binding.BeerId);
        var beerBrandId = context.BeerBrands.GetId(binding.BrandId);
        var countryId = context.Countries.GetId(binding.CountryId);
        var beerStyleId = context.BeerStyles.GetId(binding.StyleId);

        return query.Include(x => x.Beer)
                    .WhereIf(binding.From.HasValue, x => x.Date >= binding.From.Value)
                    .WhereIf(binding.To.HasValue, x => x.Date <= binding.To.Value)
                    .WhereIf(binding.Serving.HasValue, x => x.BeerServingId == (int)binding.Serving.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(binding.CountryId), x => x.Beer.BeerBrand.CountryId == countryId.Value)
                    .WhereIf(beerId.HasValue, x => x.BeerId == beerId.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(binding.StyleId), x => x.Beer.BeerStyleId == beerStyleId)
                    .WhereIf(beerBrandId.HasValue, x => x.Beer.BeerBrandId == beerBrandId.Value);
    }
}
