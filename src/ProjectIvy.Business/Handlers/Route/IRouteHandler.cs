using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProjectIvy.Business.Handlers.Ride;

public interface IRouteHandler
{
    Task Create(RouteBinding binding);

    Task<IEnumerable<Model.View.Route.Route>> GetRoutes();

    Task<IEnumerable<decimal[]>> GetRoutePoints(string routeValueId);

    Task SetPointsFromKml(string routeValueId, XDocument kml, string kmlName);
}