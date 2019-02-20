using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.Database.Main.Beer;

namespace ProjectIvy.BL.MapExtensions
{
    public static class ConsumationExtensions
    {
        public static Consumation ToEntity(this ConsumationBinding binding, MainContext context, Consumation entity = null)
        {
            entity = entity ?? new Consumation();

            entity.BeerId = context.Beers.GetId(binding.BeerId).Value;
            entity.BeerServingId = context.BeerServings.GetId(binding.ServingId).Value;
            entity.Date = binding.Date;
            entity.Volume = binding.Volume;

            return entity;
        }
    }
}
