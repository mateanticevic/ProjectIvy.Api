using ProjectIvy.DL.DbContexts;
using ProjectIvy.DL.Extensions;
using ProjectIvy.Model.Binding.Poi;
using ProjectIvy.Model.Database.Main.Travel;

namespace ProjectIvy.BL.MapExtensions
{
    public static class PoiExtensions
    {
        public static Poi ToEntity(this PoiBinding binding, MainContext context, Poi entity = null)
        {
            entity = entity == null ? new Poi() : entity;

            entity.ValueId = binding.Id;
            entity.Name = binding.Name;
            entity.Address = binding.Address;
            entity.Latitude = binding.Latitude;
            entity.Longitude = binding.Longitude;
            entity.PoiCategoryId = context.PoiCategories.GetId(binding.PoiCategoryId).Value;

            return entity;
        }
    }
}
