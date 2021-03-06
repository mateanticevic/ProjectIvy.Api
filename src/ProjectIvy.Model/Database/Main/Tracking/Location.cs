﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Tracking
{
    [Table(nameof(Location), Schema = nameof(Tracking))]
    public class Location : UserEntity, IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Description { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public int Radius { get; set; }
    }
}
