using System.Threading.Tasks;
using System.Xml.Linq;
using ProjectIvy.Model.Binding.Route;
using ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Ride;

public interface IRouteHandler
{
    Task Create(RouteBinding binding);

    Task<PagedView<Model.View.Route.Route>> GetRoutes(RouteGetBinding b);

    Task<IEnumerable<decimal[]>> GetRoutePoints(string routeValueId);

    Task SetPointsFromKml(string routeValueId, XDocument kml, string kmlName);
}