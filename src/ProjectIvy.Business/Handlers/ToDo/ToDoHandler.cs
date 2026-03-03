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
            IsCompleted = false,
            Created = DateTime.UtcNow,
            ValueId = binding.Name.ToValueId(),
            UserId = UserId
        };

        await context.ToDos.AddAsync(entity);
        await context.SaveChangesAsync();

        return entity.ValueId;
    }

    public async Task Update(string toDoValueId, ToDoBinding binding)
    {
        using var context = GetMainContext();

        var toDo = await context.ToDos.WhereUser(UserId)
                                     .SingleOrDefaultAsync(x => x.ValueId == toDoValueId) ?? throw new ResourceNotFoundException();

        bool wasCompleted = toDo.IsCompleted;

        toDo.Name = binding.Name;
        toDo.Description = binding.Description;
        toDo.IsCompleted = binding.IsCompleted;

        if (!wasCompleted && toDo.IsCompleted)
            toDo.CompletedOn = DateTime.UtcNow;

        if (wasCompleted && !toDo.IsCompleted)
            toDo.CompletedOn = null;

        context.ToDos.Update(toDo);
        await context.SaveChangesAsync();
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

        var requestedTripValueIds = binding.TripId?.Where(x => !string.IsNullOrWhiteSpace(x))
                                                  .Distinct()
                                                  .ToList();

        if (requestedTripValueIds?.Any() == true)
        {
            var resolvedTripIds = await context.Trips.WhereUser(UserId)
                                                     .Where(x => requestedTripValueIds.Contains(x.ValueId))
                                                     .Select(x => x.Id)
                                                     .ToListAsync();

            if (!resolvedTripIds.Any() || resolvedTripIds.Count != requestedTripValueIds.Count)
            {
                return new PagedView<View.ToDo>
                {
                    Count = 0,
                    Items = Enumerable.Empty<View.ToDo>()
                };
            }

            query = query.Where(x => context.TripToDos
                                            .Where(y => y.ToDoId == x.Id && resolvedTripIds.Contains(y.TripId))
                                            .Select(y => y.TripId)
                                            .Distinct()
                                            .Count() == resolvedTripIds.Count);
        }

        if (!string.IsNullOrEmpty(binding.Search))
        {
            var searchLower = binding.Search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(searchLower) || x.ValueId.ToLower().Contains(searchLower));
        }

        query = query.WhereIf(binding.IsCompleted.HasValue, x => x.IsCompleted == binding.IsCompleted.Value);

         var orderedQuery = binding.IsCompleted == true
             ? query.OrderByDescending(x => x.CompletedOn)
                 .ThenBy(x => x.Name)
             : query.OrderByDescending(x => x.Created)
                 .ThenBy(x => x.Name);

         var pagedToDos = await orderedQuery.Page(binding)
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

        var tripsByToDoId = await context.TripToDos
                                         .Where(x => todoIds.Contains(x.ToDoId))
                                         .Join(context.Trips.WhereUser(UserId),
                                               tripToDo => tripToDo.TripId,
                                               trip => trip.Id,
                                               (tripToDo, trip) => new
                                               {
                                                   tripToDo.ToDoId,
                                                   Trip = new Model.View.Trip.Trip
                                                   {
                                                       Id = trip.ValueId,
                                                       Name = trip.Name,
                                                       TimestampStart = trip.TimestampStart,
                                                       TimestampEnd = trip.TimestampEnd
                                                   }
                                               })
                                         .GroupBy(x => x.ToDoId)
                                         .ToDictionaryAsync(x => x.Key, x => x.Select(y => y.Trip).AsEnumerable());

        var items = pagedToDos.Select(x =>
        {
            return new View.ToDo(x)
            {
                Tags = tagsByToDoId.GetValueOrDefault(x.Id) ?? [],
                Trips = tripsByToDoId.GetValueOrDefault(x.Id) ?? []
            };
        });

        return new PagedView<View.ToDo>
        {
            Count = query.LongCount(),
            Items = items
        };
    }

    public async Task<IEnumerable<KeyValuePair<Model.View.Tag.Tag, int>>> GetCountByTag(ToDoGetBinding binding)
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
                return Enumerable.Empty<KeyValuePair<Model.View.Tag.Tag, int>>();
            }

            query = query.Where(x => context.ToDoTags
                                            .Where(y => y.ToDoId == x.Id && resolvedTagIds.Contains(y.TagId))
                                            .Select(y => y.TagId)
                                            .Distinct()
                                            .Count() == resolvedTagIds.Count);
        }

        var requestedTripValueIds = binding.TripId?.Where(x => !string.IsNullOrWhiteSpace(x))
                                                  .Distinct()
                                                  .ToList();

        if (requestedTripValueIds?.Any() == true)
        {
            var resolvedTripIds = await context.Trips.WhereUser(UserId)
                                                     .Where(x => requestedTripValueIds.Contains(x.ValueId))
                                                     .Select(x => x.Id)
                                                     .ToListAsync();

            if (!resolvedTripIds.Any() || resolvedTripIds.Count != requestedTripValueIds.Count)
            {
                return Enumerable.Empty<KeyValuePair<Model.View.Tag.Tag, int>>();
            }

            query = query.Where(x => context.TripToDos
                                            .Where(y => y.ToDoId == x.Id && resolvedTripIds.Contains(y.TripId))
                                            .Select(y => y.TripId)
                                            .Distinct()
                                            .Count() == resolvedTripIds.Count);
        }

        if (!string.IsNullOrEmpty(binding.Search))
        {
            var searchLower = binding.Search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(searchLower) || x.ValueId.ToLower().Contains(searchLower));
        }

        query = query.WhereIf(binding.IsCompleted.HasValue, x => x.IsCompleted == binding.IsCompleted.Value);

        return await context.ToDoTags
                        .Join(query,
                            toDoTag => toDoTag.ToDoId,
                            toDo => toDo.Id,
                            (toDoTag, _) => toDoTag.TagId)
                        .Join(context.Tags.WhereUser(UserId),
                            tagId => tagId,
                            tag => tag.Id,
                            (_, tag) => tag)
                        .GroupBy(x => x)
                            .OrderByDescending(x => x.Count())
                            .Select(x => new KeyValuePair<Model.View.Tag.Tag, int>(new Model.View.Tag.Tag(x.Key), x.Count()))
                            .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<Model.View.Trip.Trip, int>>> GetCountByTrip(ToDoGetBinding binding)
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
                return Enumerable.Empty<KeyValuePair<Model.View.Trip.Trip, int>>();
            }

            query = query.Where(x => context.ToDoTags
                                            .Where(y => y.ToDoId == x.Id && resolvedTagIds.Contains(y.TagId))
                                            .Select(y => y.TagId)
                                            .Distinct()
                                            .Count() == resolvedTagIds.Count);
        }

        var requestedTripValueIds = binding.TripId?.Where(x => !string.IsNullOrWhiteSpace(x))
                                                  .Distinct()
                                                  .ToList();

        if (requestedTripValueIds?.Any() == true)
        {
            var resolvedTripIds = await context.Trips.WhereUser(UserId)
                                                     .Where(x => requestedTripValueIds.Contains(x.ValueId))
                                                     .Select(x => x.Id)
                                                     .ToListAsync();

            if (!resolvedTripIds.Any() || resolvedTripIds.Count != requestedTripValueIds.Count)
            {
                return Enumerable.Empty<KeyValuePair<Model.View.Trip.Trip, int>>();
            }

            query = query.Where(x => context.TripToDos
                                            .Where(y => y.ToDoId == x.Id && resolvedTripIds.Contains(y.TripId))
                                            .Select(y => y.TripId)
                                            .Distinct()
                                            .Count() == resolvedTripIds.Count);
        }

        if (!string.IsNullOrEmpty(binding.Search))
        {
            var searchLower = binding.Search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(searchLower) || x.ValueId.ToLower().Contains(searchLower));
        }

        query = query.WhereIf(binding.IsCompleted.HasValue, x => x.IsCompleted == binding.IsCompleted.Value);

        return await context.TripToDos
                        .Join(query,
                            tripToDo => tripToDo.ToDoId,
                            toDo => toDo.Id,
                            (tripToDo, _) => tripToDo.TripId)
                        .Join(context.Trips.WhereUser(UserId),
                            tripId => tripId,
                            trip => trip.Id,
                            (_, trip) => trip)
                        .GroupBy(x => new
                        {
                            x.ValueId,
                            x.Name,
                            x.TimestampStart,
                            x.TimestampEnd
                        })
                            .OrderByDescending(x => x.Count())
                            .Select(x => new KeyValuePair<Model.View.Trip.Trip, int>(
                                new Model.View.Trip.Trip
                                {
                                    Id = x.Key.ValueId,
                                    Name = x.Key.Name,
                                    TimestampStart = x.Key.TimestampStart,
                                    TimestampEnd = x.Key.TimestampEnd
                                },
                                x.Count()))
                            .ToListAsync();
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
