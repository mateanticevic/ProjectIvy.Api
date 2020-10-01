using System.Collections.Generic;
using System.Linq;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Country
{
    public class CountryList
    {
        public CountryList(DatabaseModel.Travel.CountryList x)
        {
            Id = x.ValueId;
            Name = x.Name;
            Countries = x.Countries.Select(y => new Country(y.Country));
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Country> Countries { get; set; }
    }
}
