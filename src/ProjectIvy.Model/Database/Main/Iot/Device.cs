
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Iot
{
    [Table(nameof(Device), Schema = nameof(Iot))]
    public class Device : UserEntity, IHasValueId, IHasName
    {
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public string MacAddress { get; set; }

        public DateTime LastPing { get; set; }
    }
}
