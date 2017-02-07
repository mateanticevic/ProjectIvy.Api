using AnticevicApi.DL.Extensions;
using System.Collections.Generic;
using System.Linq;
using View = AnticevicApi.Model.View.Poi;

namespace AnticevicApi.BL.Handlers.Poi
{
    public class PoiHandler : Handler<PoiHandler>, IPoiHandler
    {
        public PoiHandler(IHandlerContext<PoiHandler> context) : base(context)
        {
        }

        public IEnumerable<View.Poi> GetByList(string listValueId)
        {
            using (var db = GetMainContext())
            {
                return db.PoiLists.WhereUser(User.Id)
                                  .Join(db.Pois, x => x.Id, x => x.PoiListId, (List, Poi) => new { List, Poi })
                                  .Where(x => x.List.User.Id == User.Id && x.List.ValueId == listValueId)
                                  .Join(db.PoiCategories, x => x.Poi.PoiCategoryId, x => x.Id, (PoiList, Category) => new { PoiList.Poi, Category })
                                  .ToList()
                                  .Select(x => new View.Poi(x.Poi));           
            }
        }

        public IEnumerable<View.PoiCategory> GetCategories()
        {
            using (var db = GetMainContext())
            {
                return db.PoiCategories.OrderBy(x => x.Name)
                                       .ToList()
                                       .Select(x => new View.PoiCategory(x));
            }
        }

        public IEnumerable<View.PoiList> GetLists()
        {
            using (var db = GetMainContext())
            {
                return db.PoiLists.WhereUser(User.Id)
                                  .OrderBy(x => x.Name)
                                  .ToList()
                                  .Select(x => new View.PoiList(x));
            }
        }
    }
}
