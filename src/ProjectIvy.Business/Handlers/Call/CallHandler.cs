using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using System.Collections.Generic;
using System.Linq;
using View = ProjectIvy.Model.View.Call;

namespace ProjectIvy.Business.Handlers.Call
{
    public class CallHandler : Handler<CallHandler>, ICallHandler
    {
        public CallHandler(IHandlerContext<CallHandler> context) : base(context)
        {
        }

        public IEnumerable<View.Call> Get()
        {
            using (var context = GetMainContext())
            {
                return context.Calls.WhereUser(User)
                                    .Include(x => x.File)
                                    .Select(x => new View.Call(x))
                                    .ToList();
            }
        }
    }
}
