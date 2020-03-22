using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Transport
{
    [Table(nameof(Car), Schema = nameof(Transport))]
    public class Car : UserEntity, IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public int CarModelId { get; set; }

        public short ProductionYear { get; set; }

        public CarModel CarModel { get; set; }

        public ICollection<CarLog> CarLogs { get; set; }
    }
}
