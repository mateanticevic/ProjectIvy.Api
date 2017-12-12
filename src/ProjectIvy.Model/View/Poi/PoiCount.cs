using DatabaseModel = ProjectIvy.Model.Database.Main;
using ProjectIvy.Common.Extensions;

namespace ProjectIvy.Model.View.Poi
{
    public class PoiCount
    {
        public PoiCount(DatabaseModel.Travel.Poi p, int count)
        {
            Count = count;
            Poi = p.ConvertTo(x => new Poi(x));
        }

        public int Count { get; set; }

        public Poi Poi { get; set; }
    }
}
