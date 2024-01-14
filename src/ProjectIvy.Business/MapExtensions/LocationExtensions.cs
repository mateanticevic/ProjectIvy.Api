using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Database.Main.Tracking;

namespace ProjectIvy.Business.MapExtensions;

public static class LocationExtensions
{
    public static Location ToEntity(this LocationBinding b, MainContext context)
    {
        return new Location
        {
            Name = b.Name,
            LocationTypeId = context.LocationTypes.GetId(b.TypeId).Value,
            Latitude = b.Latitude,
            Longitude = b.Longitude,
            ValueId = b.Name.ToValueId()
        };
    }
}
