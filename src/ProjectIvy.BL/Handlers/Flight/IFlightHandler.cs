using ProjectIvy.Model.View;
using System.Collections.Generic;
using ProjectIvy.Model.Binding;
using Views = ProjectIvy.Model.View;

namespace ProjectIvy.BL.Handlers.Flight
{
    public interface IFlightHandler
    {
        int Count();

        IEnumerable<CountBy<Views.Airport.Airport>> CountByAirport();

        PagedView<Views.Flight.Flight> Get(FilteredPagedBinding binding);
    }
}
