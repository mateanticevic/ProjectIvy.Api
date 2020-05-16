using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Beer;

namespace ProjectIvy.Business.Handlers.Beer
{
    public interface IBeerHandler
    {
        Task<string> CreateBeer(string brandValueId, BeerBinding binding);

        Task<string> CreateBrand(string name);

        Task<View.Beer> GetBeer(string id);

        Task<PagedView<View.Beer>> GetBeers(BeerGetBinding binding);

        Task<IEnumerable<View.BeerBrand>> GetBrands();

        Task<IEnumerable<View.BeerServing>> GetServings();

        Task<IEnumerable<View.BeerStyle>> GetStyles();
    }
}
