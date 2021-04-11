using System;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Ride
{
    public class Ride
    {
        public Ride(DatabaseModel.Transport.Ride r)
        {
            Arrival = r.DateOfArrival;
            Departure = r.DateOfDeparture;
            DestinationCity = r.DestinationCity is null ? null : new City.City(r.DestinationCity);
            OriginCity = r.OriginCity is null ? null : new City.City(r.OriginCity);
        }

        public DateTime Arrival { get; set; }

        public DateTime Departure { get; set; }

        public City.City DestinationCity { get; set; }

        public City.City OriginCity { get; set; }
    }
}
