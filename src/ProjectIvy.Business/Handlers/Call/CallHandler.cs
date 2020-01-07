using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Call;
using ProjectIvy.Model.View;
using System.Linq;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Call;

namespace ProjectIvy.Business.Handlers.Call
{
    public class CallHandler : Handler<CallHandler>, ICallHandler
    {
        public CallHandler(IHandlerContext<CallHandler> context) : base(context)
        {
        }

        public async Task<PagedView<View.Call>> Get(CallGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var calls = await context.Calls
                                         .WhereUser(User)
                                         .WhereIf(binding.From.HasValue, x => x.Timestamp >= binding.From.Value)
                                         .WhereIf(binding.To.HasValue, x => x.Timestamp <= binding.To.Value)
                                         .WhereIf(!string.IsNullOrEmpty(binding.Number), x => x.Number == binding.Number)
                                         .Include(x => x.File)
                                         .OrderByDescending(x => x.Timestamp)
                                         .Select(x => new View.Call(x))
                                         .ToPagedViewAsync(binding);

                foreach (var call in calls.Items)
                {
                    var person = await context.People.SingleOrDefaultAsync(x => x.Contacts.Any(y => y.Identifier == call.Number));
                    call.Person = person.ConvertTo(p => new Model.View.Person.Person(p));
                }

                return calls;
            }
        }

        public async Task<string> Create(CallBinding binding)
        {
            using (var context = GetMainContext())
            {
                if (context.CallBlacklist.WhereUser(User).Any(x => x.Number == binding.Number))
                    throw new ResourceForbiddenException();

                var entity = binding.ToEntity(context);
                entity.UserId = User.Id;

                await context.Calls.AddAsync(entity);
                await context.SaveChangesAsync();

                return entity.ValueId;
            }
        }

        public async Task<bool> IsNumberBlacklisted(string number)
        {
            using (var context = GetMainContext())
            {
                return await context.CallBlacklist.WhereUser(User).AnyAsync(x => x.Number == number);
            }
        }
    }
}
