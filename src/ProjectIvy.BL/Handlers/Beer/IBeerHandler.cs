using System.Collections.Generic;
using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Beer;

namespace ProjectIvy.BL.Handlers.Beer
{
    public interface IBeerHandler
    {
        string CreateBeer(string brandValueId, BeerBinding binding);

        string CreateBrand(string name);

        View.Beer GetBeer(string id);

        PagedView<View.Beer> GetBeers(BeerGetBinding binding);

        IEnumerable<View.BeerBrand> GetBrands();

        IEnumerable<View.BeerServing> GetServings();
    }
}
