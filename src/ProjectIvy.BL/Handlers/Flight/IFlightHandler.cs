using ProjectIvy.Model.View;
using System.Collections.Generic;

namespace ProjectIvy.BL.Handlers.Flight
{
    public interface IFlightHandler
    {
        int Count();

        IEnumerable<CountBy<Model.View.Airport.Airport>> CountByAirport();
    }
}
