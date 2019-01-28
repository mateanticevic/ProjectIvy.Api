using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ProjectIvy.Model.Database.Main.Transport
{
    [Table("CarLog", Schema = "Transport")]
    public class CarLog
    {
        [Key]
        public int Id { get; set; }

        public int CarId { get; set; }

        public Car Car { get; set; }

        public int? Odometer { get; set; }

        public DateTime Timestamp { get; set; }

        public short? EngineRpm { get; set; }

        public short? SpeedKmh { get; set; }

        public short? CoolantTemperature { get; set; }

        public short? AmbientAirTemperature { get; set; }

        public short? BarometricPressure { get; set; }
    }
}
