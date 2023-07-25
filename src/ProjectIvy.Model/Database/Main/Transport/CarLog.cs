using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Transport
{
    [Table(nameof(CarLog), Schema = nameof(Transport))]
    public class CarLog : IHasTimestamp
    {
        [Key]
        public int Id { get; set; }

        public int CarId { get; set; }

        public Car Car { get; set; }

        public int? Odometer { get; set; }

        public DateTime Timestamp { get; set; }

        public decimal? AccelerationAxisX { get; set; }

        public decimal? AccelerationAxisY { get; set; }

        public decimal? AccelerationAxisZ { get; set; }

        public decimal? AccelerationTotal { get; set; }

        public decimal? FuelUsed { get; set; }

        public decimal? MassAirFlowRate { get; set; }

        public decimal? ConsumptionPer100Km { get; set; }

        public short? EngineRpm { get; set; }

        public short? SpeedKmh { get; set; }

        public short? CoolantTemperature { get; set; }

        public short? AmbientAirTemperature { get; set; }

        public short? IntakeAirTemperature { get; set; }

        public short? IntakeManifoldPressure { get; set; }

        public short? BarometricPressure { get; set; }

        public short? ExhaustGasTemperature1 { get; set; }

        public int? FuelRailPressure { get; set; }

        public short? TransmissionTemperature1 { get; set; }

        public int? TripDistance { get; set; }

        public string Session { get; set; }
    }
}
