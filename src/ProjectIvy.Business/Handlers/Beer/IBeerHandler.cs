using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Beer;

namespace ProjectIvy.Business.Handlers.Beer;

public interface IBeerHandler
{
    Task<string> CreateBeer(string brandValueId, BeerBinding binding);

    Task<string> CreateBrand(BrandBinding binding);

    Task<View.Beer> GetBeer(string id);

    Task<PagedView<View.Beer>> GetBeers(BeerGetBinding binding);

    Task<IEnumerable<View.BeerBrand>> GetBrands(BrandGetBinding binding);

    Task<IEnumerable<View.BeerServing>> GetServings();

    Task<IEnumerable<View.BeerStyle>> GetStyles();

    Task UpdateBeer(string id, BeerBinding binding);

    Task UpdateBrand(string id, BrandBinding binding);
}
