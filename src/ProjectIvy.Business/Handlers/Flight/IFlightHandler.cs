using ProjectIvy.Model.Binding.Flight;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using Views = ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Flight
{
    public interface IFlightHandler
    {
        int Count(FlightGetBinding binding);

        IEnumerable<KeyValuePair<Views.Airport.Airport, int>> CountByAirport(FlightGetBinding binding);

        IEnumerable<KeyValuePair<int, int>> CountByYear(FlightGetBinding binding);

        PagedView<Views.Flight.Flight> Get(FlightGetBinding binding);
    }
}
