using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectIvy.Model.Database.Main.Beer
{
    [Table(nameof(BeerBrand), Schema = nameof(Beer))]
    public class BeerBrand : IHasName, IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public ICollection<Beer> Beers { get; set; }
    }
}
