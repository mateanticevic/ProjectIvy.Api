﻿using ProjectIvy.Model.View;
using System.Collections.Generic;
using ProjectIvy.Model.Binding.Flight;
using Views = ProjectIvy.Model.View;

namespace ProjectIvy.BL.Handlers.Flight
{
    public interface IFlightHandler
    {
        int Count(FlightGetBinding binding);

        IEnumerable<CountBy<Views.Airport.Airport>> CountByAirport(FlightGetBinding binding);

        IEnumerable<CountBy<int>> CountByYear(FlightGetBinding binding);

        PagedView<Views.Flight.Flight> Get(FlightGetBinding binding);
    }
}
