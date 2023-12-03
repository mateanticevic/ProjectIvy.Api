using ProjectIvy.Common.Extensions;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Flight
{
    public class Flight
    {
        public Flight(DatabaseModel.Transport.Flight f)
        {
            Airline = f.ConvertTo(x => new Airline.Airline(x.Airline));
            Arrival = f.DateOfArrival;
            ArrivalLocal = f.DateOfArrivalLocal;
            Departure = f.DateOfDeparture;
            DepartureLocal = f.DateOfDepartureLocal;
            Destination = f.ConvertTo(x => new Airport.Airport(x.DestinationAirport));
            Id = f.ValueId;
            Number = f.FlightNumber;
            Origin = f.ConvertTo(x => new Airport.Airport(x.OriginAirport));

            DistanceInKm = (int)(Origin.Poi.Location.ToGeoCoordinate()
                                                    .GetDistanceTo(Destination.Poi.Location.ToGeoCoordinate()) / 1000);
        }

        public string Id { get; set; }

        public Airline.Airline Airline { get; set; }

        public DateTime? Arrival { get; set; }

        public DateTime ArrivalLocal { get; set; }

        public DateTime? Departure { get; set; }

        public DateTime DepartureLocal { get; set; }

        public Airport.Airport Destination { get; set; }   

        public int? DistanceInKm { get; set; }

        public string Number { get; set; }

        public Airport.Airport Origin { get; set; }
    }
}
