using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Extensions.BuiltInTypes;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers
{
    public class ApplicationHandler : Handler
    {
        public ApplicationHandler(int userId) : base(userId)
        {

        }

        public Dictionary<string, object> GetSettings(string applicationValueId)
        {
            using (var db = new MainContext())
            {
                return db.Applications.Include(x => x.Settings)
                                      .SingleOrDefault(applicationValueId)
                                      .Settings
                                      .ToDictionary(x => x.Key, x => (object)x.Value.ToProperType());
            }
        }
    }
}
