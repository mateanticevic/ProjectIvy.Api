using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace AnticevicApi.Model.Database.Main.Tracking
{
    [Table("Tracking", Schema = "Tracking")]
    public class Tracking : UserEntity, IHasTimestamp
    {
        [Key]
        public int Id { get; set; }

        public double? Accuracy { get; set; }

        public double? Altitude { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public double? Speed { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
