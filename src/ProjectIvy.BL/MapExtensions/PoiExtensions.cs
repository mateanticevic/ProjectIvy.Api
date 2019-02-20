using System;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Poi;
using ProjectIvy.Model.Database.Main.Travel;

namespace ProjectIvy.BL.MapExtensions
{
    public static class PoiExtensions
    {
        public static Poi ToEntity(this PoiBinding binding, MainContext context, Poi entity = null)
        {
            entity = entity ?? new Poi();

            entity.ValueId = binding.Name.Replace(" ", "-").ToLowerInvariant();
            entity.Name = binding.Name;
            entity.Address = binding.Address;
            entity.Latitude = binding.Latitude;
            entity.Longitude = binding.Longitude;
            entity.PoiCategoryId = context.PoiCategories.GetId(binding.PoiCategoryId).Value;
            entity.Modified = DateTime.Now;
            entity.Created = entity.Created != default(DateTime) ? entity.Created : DateTime.Now;

            return entity;
        }
    }
}
