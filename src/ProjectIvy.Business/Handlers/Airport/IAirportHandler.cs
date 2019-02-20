using ProjectIvy.Model.Binding.Airport;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Airport;

namespace ProjectIvy.Business.Handlers.Airport
{
    public interface IAirportHandler : IHandler
    {
        long Count(AirportGetBinding binding);

        PagedView<View.Airport> Get(AirportGetBinding binding);
    }
}
