using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnticevicApi.Model.Database.Main.Transport
{
    [Table("Car", Schema = "Transport")]
    public class Car : UserEntity
    {
        [Key]
        public int Id { get; set; }
        public string Model { get; set; }
        public string ValueId { get; set; }
        public short ProductionYear { get; set; }
        public ICollection<CarLog> CarLogs { get; set; }
    }
}
