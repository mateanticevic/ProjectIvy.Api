using ProjectIvy.Common.Extensions;
using System.Collections.Generic;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.City
{
    public class City
    {
        public City(DatabaseModel.Common.City x)
        {
            Name = x.Name;
            Id = x.ValueId;
            Country = x.Country?.ConvertTo(y => new Country.Country(y));
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public Country.Country Country { get; set; }
    }

    public class CityComparer : IEqualityComparer<City>
    {
        public bool Equals(City x, City y) => x.Id == y.Id;

        public int GetHashCode(City obj) => obj.Id.GetHashCode();
    }
}
