using ProjectIvy.Common.Extensions;

namespace ProjectIvy.Model.View.Beer
{
    public class BeerBrand
    {
        public BeerBrand(Database.Main.Beer.BeerBrand b)
        {
            Id = b.ValueId;
            Name = b.Name;
            Country = b.Country.ConvertTo(x => new Country.Country(x));
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public Country.Country Country { get; set; }
    }
}
