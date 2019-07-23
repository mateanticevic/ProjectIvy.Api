using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.View;
using System.Linq;
using View = ProjectIvy.Model.View.Call;

namespace ProjectIvy.Business.Handlers.Call
{
    public class CallHandler : Handler<CallHandler>, ICallHandler
    {
        public CallHandler(IHandlerContext<CallHandler> context) : base(context)
        {
        }

        public PagedView<View.Call> Get(FilteredPagedBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Calls.WhereUser(User)
                                    .Include(x => x.File)
                                    .OrderByDescending(x => x.Timestamp)
                                    .Select(x => new View.Call(x))
                                    .ToPagedView(binding);
            }
        }
    }
}
