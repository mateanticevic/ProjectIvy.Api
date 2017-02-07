using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Poi
{
    public class PoiList
    {
        public PoiList(DatabaseModel.Tracking.PoiList x)
        {
            ValueId = x.ValueId;
            Name = x.Name;
        }

        public string ValueId { get; set; }

        public string Name { get; set; }       
    }
}
