using ProjectIvy.Model.Binding.City;
using ProjectIvy.Model.Binding.Geohash;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.Geohash;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.City;

namespace ProjectIvy.Business.Handlers.City
{
    public interface ICityHandler
    {
        Task AddVisitedCity(string cityValueId);

        Task<PagedView<View.City>> Get(CityGetBinding binding);

        Task<IEnumerable<RouteTime>> GetRoutes(string fromCityValueId, string toCityValueId, RouteTimeSort sort);

        IEnumerable<View.City> GetVisited();
    }
}