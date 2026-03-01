using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        var query = context.ToDos.WhereUser(UserId);

        var requestedTagValueIds = binding.TagId?.Where(x => !string.IsNullOrWhiteSpace(x))
                                                .Distinct()
                                                .ToList();

        if (requestedTagValueIds?.Any() == true)
        {
            var resolvedTagIds = await context.Tags.WhereUser(UserId)
                                                   .Where(x => requestedTagValueIds.Contains(x.ValueId))
                                                   .Select(x => x.Id)
                                                   .ToListAsync();

            if (!resolvedTagIds.Any() || resolvedTagIds.Count != requestedTagValueIds.Count)
            {
                return new PagedView<View.ToDo>
                {
                    Count = 0,
                    Items = Enumerable.Empty<View.ToDo>()
                };
            }

            query = query.Where(x => context.ToDoTags
                                            .Where(y => y.ToDoId == x.Id && resolvedTagIds.Contains(y.TagId))
                                            .Select(y => y.TagId)
                                            .Distinct()
                                            .Count() == resolvedTagIds.Count);
        }

        if (!string.IsNullOrEmpty(binding.Search))
        {
            var searchLower = binding.Search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(searchLower) || x.ValueId.ToLower().Contains(searchLower));
        }

        var pagedToDos = await query.OrderByDescending(x => x.ValueId == binding.Search)
                                    .ThenByDescending(x => x.Created)
                                    .ThenBy(x => x.Name)
                                    .Page(binding)
                                    .ToListAsync();

        var todoIds = pagedToDos.Select(x => x.Id).ToList();

        var tagsByToDoId = await context.ToDoTags
                                        .Where(x => todoIds.Contains(x.ToDoId))
                                        .Join(context.Tags,
                                              toDoTag => toDoTag.TagId,
                                              tag => tag.Id,
                                              (toDoTag, tag) => new { toDoTag.ToDoId, Tag = new Model.View.Tag.Tag(tag) })
                                        .GroupBy(x => x.ToDoId)
                                        .ToDictionaryAsync(x => x.Key, x => x.Select(y => y.Tag).AsEnumerable());

        var items = pagedToDos.Select(x =>
        {
            var toDo = new View.ToDo(x);
            toDo.Tags = tagsByToDoId.GetValueOrDefault(x.Id) ?? Enumerable.Empty<Model.View.Tag.Tag>();
            return toDo;
        });

        return new PagedView<View.ToDo>
        {
            Count = query.LongCount(),
            Items = items
        };
    }
}
