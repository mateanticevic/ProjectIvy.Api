using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AnticevicApi.Model.Database.Main.Transport
{
    [Table("Car", Schema = "Transport")]
    public class Car : UserEntity, IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string Model { get; set; }

        public string ValueId { get; set; }

        public short ProductionYear { get; set; }

        public ICollection<CarLog> CarLogs { get; set; }
    }
}
