using System.Threading.Tasks;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Consumation;
using ProjectIvy.Model.Database.Main.Beer;

namespace ProjectIvy.Business.MapExtensions;

public static class ConsumationExtensions
{
    public static async Task<Consumation> ToEntity(this ConsumationBinding binding, MainContext context, Consumation entity = null)
    {
        entity = entity ?? new Consumation();

        entity.BeerId = (await context.Beers.GetIdAsync(binding.BeerId)).Value;
        entity.BeerServingId = (await context.BeerServings.GetIdAsync(binding.ServingId)).Value;
        entity.Date = binding.Date;
        entity.Volume = binding.Volume;

        return entity;
    }
}
