using ProjectIvy.DL.DbContexts;
using ProjectIvy.DL.Extensions;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Common.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIvy.BL.Handlers.Application
{
    public class ApplicationHandler : Handler<ApplicationHandler>, IApplicationHandler
    {
        public ApplicationHandler(IHandlerContext<ApplicationHandler> context) : base(context)
        {
        }

        public Dictionary<string, object> GetSettings(string applicationValueId)
        {
            using (var db = GetMainContext())
            {
                return db.Applications.Include(x => x.Settings)
                                      .SingleOrDefault(applicationValueId)
                                      .Settings
                                      .ToDictionary(x => x.Key, x => x.Value.ToProperType());
            }
        }
    }
}
