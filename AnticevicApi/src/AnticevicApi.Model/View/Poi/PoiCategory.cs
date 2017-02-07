using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Poi
{
    public class PoiCategory
    {
        public PoiCategory(DatabaseModel.Tracking.PoiCategory x)
        {
            ValueId = x.ValueId;
            Name = x.Name;
            Icon = x.Icon;
        }

        public string Name { get; set; }

        public string ValueId { get; set; }

        public string Icon { get; set; }
    }
}
