using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Iot
{
    [Table(nameof(DeviceData), Schema = nameof(Iot))]
    public class DeviceData
    {
        [Key]
        public long Id { get; set; }

        public int DeviceId { get; set; }

        public DateTime Created { get; set; }

        public string FieldIdentifier { get; set; }

        public string Value { get; set; }

        public Device Device { get; set; }
    }
}
