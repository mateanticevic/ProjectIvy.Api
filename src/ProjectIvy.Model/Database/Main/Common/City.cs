using ProjectIvy.Model.Database.Main.Finance;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectIvy.Model.Database.Main.Common
{
    [Table(nameof(City), Schema = nameof(Common))]
    public class City : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }

        public Country Country { get; set; }

        public ICollection<Transport.Airport> Airports { get; set; }

        public ICollection<Vendor> Vendors { get; set; }
    }
}
