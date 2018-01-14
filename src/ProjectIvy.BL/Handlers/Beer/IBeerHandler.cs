using ProjectIvy.Model.Binding.Beer;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Beer;

namespace ProjectIvy.BL.Handlers.Beer
{
    public interface IBeerHandler
    {
        PagedView<View.Beer> GetBeers(BeerGetBinding binding);
    }
}
