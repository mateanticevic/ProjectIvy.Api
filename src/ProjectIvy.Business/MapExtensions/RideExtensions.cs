using System.Threading.Tasks;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Ride;
using ProjectIvy.Model.Database.Main.Transport;

namespace ProjectIvy.Business.MapExtensions;

public static class RideExtensions
{
    public static async Task<Ride> ToEntity(this RideBinding b, MainContext context, Ride entity = null)
    {
        entity = entity ?? new Ride();

        entity.DateOfArrival = b.Arrival;
        entity.DateOfDeparture = b.Departure;
        entity.DestinationCityId = b.DestinationCityId is null ? null : await context.Cities.GetIdAsync(b.DestinationCityId);
        entity.OriginCityId = b.OriginCityId is null ? null : await context.Cities.GetIdAsync(b.OriginCityId);
        entity.DestinationPoiId = b.DestinationPoiId is null ? null : await context.Pois.GetIdAsync(b.DestinationPoiId);
        entity.OriginPoiId = b.OriginPoiId is null ? null : await context.Pois.GetIdAsync(b.OriginPoiId);
        entity.RideTypeId = (await context.RideTypes.GetIdAsync(b.TypeId)).Value;

        return entity;
    }
}
