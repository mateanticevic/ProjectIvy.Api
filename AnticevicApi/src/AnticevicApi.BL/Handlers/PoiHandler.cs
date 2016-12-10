using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.View.Poi;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers
{
    public class PoiHandler : Handler
    {
        public PoiHandler(string connectionString, int userId) : base(connectionString, userId)
        {

        }

        public IEnumerable<Poi> GetByList(string listValueId)
        {
            using (var db = new MainContext(ConnectionString))
            {
                return db.PoiLists.WhereUser(UserId)
                                  .Join(db.Pois, x => x.Id, x => x.PoiListId, (List, Poi) => new { List, Poi })
                                  .Where(x => x.List.UserId == UserId && x.List.ValueId == listValueId)
                                  .Join(db.PoiCategories, x => x.Poi.PoiCategoryId, x => x.Id, (PoiList, Category) => new { PoiList.Poi, Category })
                                  .ToList()
                                  .Select(x => new Poi(x.Poi));
                                                    
            }
        }

        public IEnumerable<PoiCategory> GetCategories()
        {
            using (var db = new MainContext(ConnectionString))
            {
                return db.PoiCategories.OrderBy(x => x.Name)
                                       .ToList()
                                       .Select(x => new PoiCategory(x));
            }
        }

        public IEnumerable<PoiList> GetLists()
        {
            using (var db = new MainContext(ConnectionString))
            {
                return db.PoiLists.WhereUser(UserId)
                                  .OrderBy(x => x.Name)
                                  .ToList()
                                  .Select(x => new PoiList(x));
            }
        }
    }
}
