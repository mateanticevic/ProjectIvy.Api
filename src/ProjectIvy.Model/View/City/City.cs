using ProjectIvy.Common.Extensions;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.City
{
    public class City
    {
        public City(DatabaseModel.Common.City x)
        {
            Name = x.Name;
            Id = x.ValueId;
            Country = x.Country?.ConvertTo(y => new View.Country.Country(y));
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public Country.Country Country { get; set; }
    }
}
