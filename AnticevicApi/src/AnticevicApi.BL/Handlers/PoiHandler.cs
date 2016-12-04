﻿using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.View.Poi;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers
{
    public class PoiHandler : Handler
    {
        public PoiHandler(int userId) : base(userId)
        {

        }

        public IEnumerable<PoiCategory> GetCategories()
        {
            using (var db = new MainContext())
            {
                return db.PoiCategories.OrderBy(x => x.Name)
                                       .ToList()
                                       .Select(x => new PoiCategory(x));
            }
        }

        public IEnumerable<PoiList> GetLists()
        {
            using (var db = new MainContext())
            {
                return db.PoiLists.WhereUser(UserId)
                                  .OrderBy(x => x.Name)
                                  .ToList()
                                  .Select(x => new PoiList(x));
            }
        }
    }
}
