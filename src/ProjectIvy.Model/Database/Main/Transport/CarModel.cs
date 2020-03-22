using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Transport
{
    [Table(nameof(CarModel), Schema = nameof(Transport))]
    public class CarModel : IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public int ManufacturerId { get; set; }

        public short ModelYear { get; set; }

        public short Power { get; set; }

        public short EngineDisplacement { get; set; }

        public Manufacturer Manufacturer { get; set; }
    }
}
