using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Transport
{
    [Table(nameof(Flight), Schema = nameof(Transport))]
    public class Flight : UserEntity
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public Airline Airline { get; set; }

        public int AirlineId { get; set; }

        public int DestinationAirportId { get; set; }

        public Airport DestinationAirport { get; set; }

        public int OriginAirportId { get; set; }

        public Airport OriginAirport { get; set; }

        public DateTime DateOfDeparture { get; set; }

        public DateTime DateOfDepartureLocal { get; set; }

        public DateTime DateOfArrival { get; set; }

        public DateTime DateOfArrivalLocal { get; set; }

        public string FlightNumber { get; set; }
    }
}
