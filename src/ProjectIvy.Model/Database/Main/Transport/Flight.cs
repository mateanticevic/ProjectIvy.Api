using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ProjectIvy.Model.Database.Main.Transport
{
    [Table("Flight", Schema = "Transport")]
    public class Flight : UserEntity
    {
        [Key]
        public int Id { get; set; }

        public int DestinationAirportId { get; set; }

        public Airport DestinationAirport { get; set; }

        public int OriginAirportId { get; set; }

        public Airport OriginAirport { get; set; }

        public DateTime DateOfDeparture { get; set; }

        public DateTime DateOfArrival { get; set; }

        public string FlightNumber { get; set; }
    }
}
