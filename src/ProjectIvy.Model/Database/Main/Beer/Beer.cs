using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Beer
{
    [Table(nameof(Beer), Schema = nameof(Beer))]
    public class Beer : IHasName, IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public int BeerBrandId { get; set; }

        public int? BeerStyleId { get; set; }

        public decimal Abv { get; set; }

        public BeerBrand BeerBrand { get; set; }

        public BeerStyle BeerStyle { get; set; }
    }
}
