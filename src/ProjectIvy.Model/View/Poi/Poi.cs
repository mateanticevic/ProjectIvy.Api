using DatabaseModel = ProjectIvy.Model.Database.Main;
using ProjectIvy.Extensions.BuiltInTypes;

namespace ProjectIvy.Model.View.Poi
{
    public class Poi
    {
        public Poi(DatabaseModel.Travel.Poi x)
        {
            Category = x.PoiCategory.ConvertTo(y => new PoiCategory(y));
            Latitude = x.Latitude;
            Longitude = x.Longitude;
            Name = x.Name;
            Address = x.Address;
            Id = x.ValueId;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public PoiCategory Category { get; set; }
    }
}
