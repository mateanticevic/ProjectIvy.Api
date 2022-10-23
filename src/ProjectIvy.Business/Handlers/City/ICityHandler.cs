using ProjectIvy.Model.Binding.City;
using ProjectIvy.Model.View;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.City;

namespace ProjectIvy.Business.Handlers.City
{
    public interface ICityHandler
    {
        Task AddVisitedCity(string cityValueId);

        Task<PagedView<View.City>> Get(CityGetBinding binding);

        IEnumerable<View.City> GetVisited();
    }
}