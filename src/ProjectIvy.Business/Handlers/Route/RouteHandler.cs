using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Geohash;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Database.Main.Tracking;

namespace ProjectIvy.Business.Handlers.Ride;

public class RouteHandler : Handler<RouteHandler>, IRouteHandler
{
    public RouteHandler(IHandlerContext<RouteHandler> context) : base(context)
    {
    }

    public async Task Create(RouteBinding binding)
    {
        using var context = GetMainContext();
        var entity = binding.ToEntity();
        entity.UserId = UserId;

        await context.Routes.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Model.View.Route.Route>> GetRoutes()
    {
        using var context = GetMainContext();
        return await context.Routes.WhereUser(UserId)
                                    .Select(x => new Model.View.Route.Route()
                                    {
                                        Id = x.ValueId,
                                        Name = x.Name
                                    })
                                    .ToListAsync();
    }

    public async Task<IEnumerable<decimal[]>> GetRoutePoints(string routeValueId)
    {
        using var context = GetMainContext();
        var routeId = context.Routes.WhereUser(UserId)
                                    .Single(x => x.ValueId == routeValueId).Id;

        return await context.RoutePoints.Where(x => x.RouteId == routeId)
                                        .OrderBy(x => x.Index)
                                        .Select(x => new decimal[] { x.Lat, x.Lng })
                                        .ToListAsync();
    }

    public async Task SetPointsFromKml(string routeValueId, XDocument kml, string kmlName)
    {
        using var context = GetMainContext();
        int routeId = context.Routes.WhereUser(UserId)
                                    .Single(x => x.ValueId == routeValueId).Id;

        var rootNamespace = kml.Root.Name.Namespace;

        var folder = kml.Root.Element(rootNamespace + "Document")
                             .Element(rootNamespace + "Folder");

        var geohasher = new Geohasher();

        foreach (var placemark in folder.Elements(rootNamespace + "Placemark"))
        {
            var name = placemark.Element(rootNamespace + "name").Value;

            if (name != kmlName)
                continue;

            var coordinates = placemark.Element(rootNamespace + "LineString")
                                       .Element(rootNamespace + "coordinates")
                                       .Value;

            var points = coordinates.Split('\n');
            var routePoints = points.Select(x => x.Trim().Split(','))
                                    .Where(x => string.IsNullOrWhiteSpace(x[0]) == false)
                                    .Select((x, i) => new RoutePoint()
                                    {
                                        Lat = Convert.ToDecimal(x[1], new System.Globalization.CultureInfo("en-US")),
                                        Lng = Convert.ToDecimal(x[0], new System.Globalization.CultureInfo("en-US")),
                                        Index = i,
                                        RouteId = routeId,
                                        Geohash = geohasher.Encode((double)Convert.ToDecimal(x[1], new System.Globalization.CultureInfo("en-US")), (double)Convert.ToDecimal(x[0], new System.Globalization.CultureInfo("en-US")), 8)
                                    });

            await context.RoutePoints.AddRangeAsync(routePoints);
            await context.SaveChangesAsync();
        }
    }
}
