using System.Threading.Tasks;
using ProjectIvy.Model.Binding.City;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.City;

namespace ProjectIvy.Business.Handlers.City
{
    public interface ICityHandler
    {
        Task<PagedView<Model.View.City.City>> Get(CityGetBinding binding);
    }
}