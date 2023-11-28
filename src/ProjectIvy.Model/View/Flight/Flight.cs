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
            Departure = f.DateOfDeparture;
            Destination = f.ConvertTo(x => new Airport.Airport(x.DestinationAirport));
            Origin = f.ConvertTo(x => new Airport.Airport(x.OriginAirport));

            DistanceInKm = (int)(Origin.Poi.Location.ToGeoCoordinate()
                                                    .GetDistanceTo(Destination.Poi.Location.ToGeoCoordinate()) / 1000);
        }

        public Airline.Airline Airline { get; set; }

        public DateTime Arrival { get; set; }

        public DateTime Departure { get; set; }

        public Airport.Airport Destination { get; set; }   

        public int? DistanceInKm { get; set; }

        public Airport.Airport Origin { get; set; }
    }
}
