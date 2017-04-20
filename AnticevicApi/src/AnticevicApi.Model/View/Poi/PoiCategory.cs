using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Poi
{
    public class PoiCategory
    {
        public PoiCategory(DatabaseModel.Travel.PoiCategory x)
        {
            ValueId = x.ValueId;
            Name = x.Name;
        }

        public string Name { get; set; }

        public string ValueId { get; set; }
    }
}
