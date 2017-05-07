using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Poi;
using AnticevicApi.Model.Database.Main.Travel;

namespace AnticevicApi.BL.MapExtensions
{
    public static class PoiExtensions
    {
        public static Poi ToEntity(this PoiBinding binding, MainContext context, Poi entity = null)
        {
            entity = entity == null ? new Poi() : entity;

            entity.ValueId = binding.Id;
            entity.Name = binding.Name;
            entity.Latitude = binding.Latitude;
            entity.Longitude = binding.Longitude;
            entity.PoiCategoryId = context.PoiCategories.GetId(binding.PoiCategoryId).Value;

            return entity;
        }
    }
}
