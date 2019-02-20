using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.Database.Main.Beer;

namespace ProjectIvy.BL.MapExtensions
{
    public static class BeerExtensions
    {
        public static Beer ToEntity(this BeerBinding binding, int brandId, Beer b = null)
        {
            b = b.DefaultIfNull();

            b.Abv = binding.Abv;
            b.Name = binding.Name;
            b.ValueId = binding.Name.ToValueId();
            b.BeerBrandId = brandId;

            return b;
        }
    }
}
