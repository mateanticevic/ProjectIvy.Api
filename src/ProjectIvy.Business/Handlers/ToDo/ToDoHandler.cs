using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.ToDo;
using ProjectIvy.Model.View;
using Database = ProjectIvy.Model.Database.Main.User;
using View = ProjectIvy.Model.View.ToDo;

namespace ProjectIvy.Business.Handlers.ToDo;

public class ToDoHandler : Handler<ToDoHandler>, IToDoHandler
{
    public ToDoHandler(IHandlerContext<ToDoHandler> context) : base(context)
    {
    }

    public async Task<string> Create(ToDoBinding binding)
    {
        using var context = GetMainContext();

        var entity = new Database.ToDo
        {
            Name = binding.Name,
            Description = binding.Description,
            Created = DateTime.UtcNow,
            ValueId = binding.Name.ToValueId(),
            UserId = UserId
        };

        await context.ToDos.AddAsync(entity);
        await context.SaveChangesAsync();

        return entity.ValueId;
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

    public async Task LinkTag(string toDoValueId, string tagValueId)
    {
        using var context = GetMainContext();

        var toDoId = await context.ToDos.WhereUser(UserId)
                                   .Where(x => x.ValueId == toDoValueId)
                                   .Select(x => (long?)x.Id)
                                   .SingleOrDefaultAsync() ?? throw new ResourceNotFoundException();

        var tagId = await context.Tags.WhereUser(UserId)
                                      .Where(x => x.ValueId == tagValueId)
                                      .Select(x => (int?)x.Id)
                                      .SingleOrDefaultAsync() ?? throw new ResourceNotFoundException();

        bool exists = await context.ToDoTags.AnyAsync(x => x.ToDoId == toDoId && x.TagId == tagId);
        if (exists)
        {
            return;
        }

        await context.ToDoTags.AddAsync(new Database.ToDoTag
        {
            ToDoId = toDoId,
            TagId = tagId
        });

        await context.SaveChangesAsync();
    }

    public async Task UnlinkTag(string toDoValueId, string tagValueId)
    {
        using var context = GetMainContext();

        var toDoId = await context.ToDos.WhereUser(UserId)
                                        .Where(x => x.ValueId == toDoValueId)
                                        .Select(x => (long?)x.Id)
                                        .SingleOrDefaultAsync() ?? throw new ResourceNotFoundException();

        var tagId = await context.Tags.WhereUser(UserId)
                                      .Where(x => x.ValueId == tagValueId)
                                      .Select(x => (int?)x.Id)
                                      .SingleOrDefaultAsync() ?? throw new ResourceNotFoundException();

        var link = await context.ToDoTags
                                .SingleOrDefaultAsync(x => x.ToDoId == toDoId && x.TagId == tagId) ?? throw new ResourceNotFoundException();

        context.ToDoTags.Remove(link);
        await context.SaveChangesAsync();
    }
}
