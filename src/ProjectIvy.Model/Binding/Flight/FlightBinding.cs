namespace ProjectIvy.Model.Binding.Flight
{
    public class FlightBinding
    {
        public string AirlineId { get; set; }

        public string DestinationId { get; set; }

        public string Number { get; set; }

        public string OriginId { get; set; }

        public DateTime Arrival { get; set; }

        public DateTime ArrivalLocal { get; set; }

        public DateTime Departure { get; set; }

        public DateTime DepartureLocal { get; set; }
    }
}
