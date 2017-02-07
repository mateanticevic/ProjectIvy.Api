using System.Collections.Generic;
using View = AnticevicApi.Model.View.Poi;

namespace AnticevicApi.BL.Handlers.Poi
{
    public interface IPoiHandler : IHandler
    {
        IEnumerable<View.Poi> GetByList(string listValueId);

        IEnumerable<View.PoiCategory> GetCategories();

        IEnumerable<View.PoiList> GetLists();
    }
}
