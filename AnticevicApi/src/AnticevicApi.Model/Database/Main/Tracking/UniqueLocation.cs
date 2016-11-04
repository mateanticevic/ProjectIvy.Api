using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace AnticevicApi.Model.Database.Main.Tracking
{
    [Table("UniqueLocation", Schema = "Tracking")]
    public class UniqueLocation : UserEntity, IHasTimestamp
    {
        [Key]
        public int Id { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
