using ProjectIvy.Model.Database.Main.Log;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Inv
{
    [Table(nameof(Device), Schema = nameof(Inv))]
    public class Device : UserEntity, IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public int DeviceTypeId { get; set; }

        public DeviceType DeviceType { get; set; }

        public ICollection<BrowserLog> BrowserLogs { get; set; }
    }
}
