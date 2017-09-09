using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectIvy.Model.Database.Main.Inv
{
    [Table(nameof(DeviceType), Schema = "Inv")]
    public class DeviceType : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public ICollection<Device> Devices { get; set; }
    }
}
