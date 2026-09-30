using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Flight;
using ProjectIvy.Model.View;
using Views = ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Flight;

public interface IFlightHandler
{
    Task<int> Count(FlightGetBinding binding);

    Task<IEnumerable<KeyValuePair<Views.Airline.Airline, int>>> CountByAirline(FlightGetBinding binding);

    Task<IEnumerable<KeyValuePair<Views.Airport.Airport, int>>> CountByAirport(FlightGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> CountByYear(FlightGetBinding binding);

    Task Create(FlightBinding binding);

    Task<PagedView<Views.Flight.Flight>> Get(FlightGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> GetDistanceByYear();

    Task Update(string valueId, FlightBinding flight);
}
