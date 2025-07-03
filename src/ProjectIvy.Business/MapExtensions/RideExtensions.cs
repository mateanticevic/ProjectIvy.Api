using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Ride;
using ProjectIvy.Model.Database.Main.Transport;

namespace ProjectIvy.Business.MapExtensions;

public static class RideExtensions
{
    public static Ride ToEntity(this RideBinding b, MainContext context, Ride entity = null)
    {
        entity = entity ?? new Ride();

        entity.DateOfArrival = b.Arrival;
        entity.DateOfDeparture = b.Departure;
        entity.DestinationCityId = b.DestinationCityId is null ? null : context.Cities.GetId(b.DestinationCityId);
        entity.OriginCityId = b.OriginCityId is null ? null : context.Cities.GetId(b.OriginCityId);
        entity.DestinationPoiId = b.DestinationPoiId is null ? null : context.Pois.GetId(b.DestinationPoiId);
        entity.OriginPoiId = b.OriginPoiId is null ? null : context.Pois.GetId(b.OriginPoiId);
        entity.RideTypeId = context.RideTypes.GetId(b.TypeId).Value;

        return entity;
    }
}
