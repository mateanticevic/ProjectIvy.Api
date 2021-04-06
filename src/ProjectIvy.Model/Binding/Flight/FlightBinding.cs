using System;
namespace ProjectIvy.Model.Binding.Flight
{
    public class FlightBinding
    {
        public string AirlineId { get; set; }

        public string DestinationId { get; set; }

        public string FlightNumber { get; set; }

        public string OriginId { get; set; }

        public DateTime Arrival { get; set; }

        public DateTime Departure { get; set; }
    }
}
