using AnticevicApi.Model.Binding.Airport;
using AnticevicApi.Model.View;
using View = AnticevicApi.Model.View.Airport;

namespace AnticevicApi.BL.Handlers.Airport
{
    public interface IAirportHandler : IHandler
    {
        long Count(AirportGetBinding binding);

        PaginatedView<View.Airport> Get(AirportGetBinding binding);
    }
}
