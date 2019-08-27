using ProjectIvy.Common.Extensions;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Poi
{
    public class Poi
    {
        public Poi(DatabaseModel.Travel.Poi x)
        {
            Category = x.PoiCategory?.ConvertTo(y => new PoiCategory(y));
            Name = x.Name;
            Address = x.Address;
            Id = x.ValueId;
            Location = new Location(x.Latitude, x.Longitude);
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public PoiCategory Category { get; set; }

        public Location Location { get; set; }
    }
}
