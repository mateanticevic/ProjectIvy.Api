using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ProjectIvy.Model.Database.Main.Travel
{
    [Table(nameof(Poi), Schema = nameof(Travel))]
    public class Poi : IHasValueId, IHasName, IHasLocation, IHasCreatedModified
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public int PoiCategoryId { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public PoiCategory PoiCategory { get; set; }

        public ICollection<Finance.VendorPoi> VendorPois { get; set; }
    }
}
