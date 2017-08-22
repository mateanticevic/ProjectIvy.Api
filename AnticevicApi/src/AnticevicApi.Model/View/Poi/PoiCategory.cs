using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Poi
{
    public class PoiCategory
    {
        public PoiCategory(DatabaseModel.Travel.PoiCategory x)
        {
            Id = x.ValueId;
            Name = x.Name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
