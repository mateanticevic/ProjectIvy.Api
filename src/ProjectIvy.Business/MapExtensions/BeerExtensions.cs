using System.Threading.Tasks;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.Database.Main.Beer;

namespace ProjectIvy.Business.MapExtensions;

public static class BeerExtensions
{
    public static async Task<Beer> ToEntity(this BeerBinding binding, MainContext context, Beer beer = null)
    {
        var b = beer.DefaultIfNull();

        b.Abv = binding.Abv;
        b.Name = binding.Name;
        b.ValueId = beer?.ValueId ?? binding.Name.ToValueId();

        b.BeerBrandId = string.IsNullOrWhiteSpace(binding.BrandId) ? b.BeerBrandId : (await context.BeerBrands.GetIdAsync(binding.BrandId)).Value;
        b.BeerStyleId = await context.BeerStyles.GetIdAsync(binding.StyleId);

        return b;
    }

    public static async Task<BeerBrand> ToEntity(this BrandBinding binding, MainContext context, BeerBrand brand = null)
    {
        var b = brand.DefaultIfNull();

        b.Name = binding.Name;
        b.CountryId = await context.Countries.GetIdAsync(binding.CountryId);
        b.ValueId = brand?.ValueId ?? binding.Name.ToValueId();

        return b;
    }
}
