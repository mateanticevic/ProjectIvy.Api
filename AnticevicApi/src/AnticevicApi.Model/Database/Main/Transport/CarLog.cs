using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace AnticevicApi.Model.Database.Main.Transport
{
    [Table("CarLog", Schema = "Transport")]
    public class CarLog
    {
        [Key]
        public int Id { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public int Odometer { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
