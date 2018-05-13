using DatabaseModel = ProjectIvy.Model.Database.Main;
using ProjectIvy.Common.Extensions;
using System;

namespace ProjectIvy.Model.View.Flight
{
    public class Flight
    {
        public Flight(DatabaseModel.Transport.Flight f)
        {
            DestinationAirport = f.ConvertTo(x => new Airport.Airport(x.DestinationAirport));
            OriginAirport = f.ConvertTo(x => new Airport.Airport(x.OriginAirport));
            DateOfArrival = f.DateOfArrival;
            DateOfDeparture = f.DateOfDeparture;
        }

        public Airport.Airport DestinationAirport { get; set; }

        public Airport.Airport OriginAirport { get; set; }

        public DateTime DateOfArrival { get; set; }

        public DateTime DateOfDeparture { get; set; }
    }
}
