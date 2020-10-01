using System.Collections.Generic;

namespace ProjectIvy.Model.View.Country
{
    public class CountryListVisited
    {
        public CountryListVisited(CountryList x)
        {
            Id = x.Id;
            Name = x.Name;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Country> CountriesNotVisited { get; set; }

        public IEnumerable<Country> CountriesVisited { get; set; }
    }
}
