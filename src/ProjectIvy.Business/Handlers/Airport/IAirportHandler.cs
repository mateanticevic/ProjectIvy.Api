using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Airport;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Airport;

namespace ProjectIvy.Business.Handlers.Airport;

public interface IAirportHandler : IHandler
{
    Task<long> Count(AirportGetBinding binding);

    Task<PagedView<View.Airport>> Get(AirportGetBinding binding);
}
