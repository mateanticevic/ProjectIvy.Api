using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.Database.Main.Beer;

namespace ProjectIvy.Business.MapExtensions;

public static class BeerExtensions
{
    public static Beer ToEntity(this BeerBinding binding, MainContext context, Beer beer = null)
    {
        var b = beer.DefaultIfNull();

        b.Abv = binding.Abv;
        b.Name = binding.Name;
        b.ValueId = beer?.ValueId ?? binding.Name.ToValueId();

        b.BeerBrandId = string.IsNullOrWhiteSpace(binding.BrandId) ? b.BeerBrandId : context.BeerBrands.GetId(binding.BrandId).Value;
        b.BeerStyleId = context.BeerStyles.GetId(binding.StyleId);

        return b;
    }

    public static BeerBrand ToEntity(this BrandBinding binding, MainContext context, BeerBrand brand = null)
    {
        var b = brand.DefaultIfNull();

        b.Name = binding.Name;
        b.CountryId = context.Countries.GetId(binding.CountryId);
        b.ValueId = brand?.ValueId ?? binding.Name.ToValueId();

        return b;
    }
}
