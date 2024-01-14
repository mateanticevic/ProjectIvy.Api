using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Route;
using ProjectIvy.Model.Database.Main.Tracking;

namespace ProjectIvy.Business.MapExtensions;

public static class RouteExtensions
{
    public static Route ToEntity(this RouteBinding b)
    {
        return new Route
        {
            Name = b.Name,
            ValueId = b.Name.ToValueId()
        };
    }
}
