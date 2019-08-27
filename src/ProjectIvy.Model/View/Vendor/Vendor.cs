using ProjectIvy.Common.Extensions;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Vendor
{
    public class Vendor
    {
        public Vendor(DatabaseModel.Finance.Vendor x)
        {
            City = x.City.ConvertTo(y => new City.City(y));
            Name = x.Name;
            Id = x.ValueId;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public City.City City { get; set; }
    }
}
