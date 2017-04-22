using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace AnticevicApi.Model.Database.Main.Travel
{
    [Table(nameof(Trip), Schema = nameof(Travel))]
    public class Trip : UserEntity, IHasName, IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public DateTime TimestampEnd { get; set; }

        public DateTime TimestampStart { get; set; }

        public ICollection<TripCity> Cities { get; set; }
    }
}
