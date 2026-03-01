using System.Linq;
using System.Threading.Tasks;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.ToDo;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.ToDo;

namespace ProjectIvy.Business.Handlers.ToDo;

public class ToDoHandler : Handler<ToDoHandler>, IToDoHandler
{
    public ToDoHandler(IHandlerContext<ToDoHandler> context) : base(context)
    {
    }

    public async Task<PagedView<View.ToDo>> Get(ToDoGetBinding binding)
    {
        using var context = GetMainContext();
        var query = context.ToDos
                           .WhereUser(UserId);

        if (!string.IsNullOrEmpty(binding.Search))
        {
            var searchLower = binding.Search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(searchLower) || x.ValueId.ToLower().Contains(searchLower));
        }

        var resultQuery = query.OrderByDescending(x => x.ValueId == binding.Search)
                               .ThenByDescending(x => x.Created)
                               .ThenBy(x => x.Name)
                               .Select(x => new View.ToDo(x));

        return await resultQuery.ToPagedViewAsync(binding);
    }
}
