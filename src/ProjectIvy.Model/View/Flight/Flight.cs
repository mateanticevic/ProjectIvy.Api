using DatabaseModel = ProjectIvy.Model.Database.Main;
using ProjectIvy.Common.Extensions;
using System;

namespace ProjectIvy.Model.View.Flight
{
    public class Flight
    {
        public Flight(DatabaseModel.Transport.Flight f)
        {
            Arrival = f.DateOfArrival;
            Departure = f.DateOfDeparture;
            Destination = f.ConvertTo(x => new Airport.Airport(x.DestinationAirport));
            Origin = f.ConvertTo(x => new Airport.Airport(x.OriginAirport));

            DistanceInKm = (int)(Origin.Poi.Location.ToGeoCoordinate()
                                                    .GetDistanceTo(Destination.Poi.Location.ToGeoCoordinate()) / 1000);
        }

        public Airport.Airport Destination { get; set; }

        public Airport.Airport Origin { get; set; }

        public DateTime Arrival { get; set; }

        public DateTime Departure { get; set; }

        public int DistanceInKm { get; set; }
    }
}
