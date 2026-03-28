using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
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

        int? currencyId = null;

        if (binding.EstimatedPrice.HasValue)
            currencyId = context.GetCurrencyId(binding.CurrencyId, UserId);

        var entity = new Database.ToDo
        {
            Name = binding.Name,
            Description = binding.Description,
            IsCompleted = false,
            Created = DateTime.UtcNow,
            ValueId = binding.Name.ToValueId(),
            UserId = UserId,
            EstimatedPrice = binding.EstimatedPrice,
            CurrencyId = currencyId
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
        toDo.EstimatedPrice = binding.EstimatedPrice;
        toDo.CurrencyId = binding.EstimatedPrice.HasValue
            ? context.GetCurrencyId(binding.CurrencyId, UserId)
            : null;

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
        var query = context.ToDos.WhereUser(UserId)
                                 .Where(binding, context, UserId);

         var orderedQuery = binding.IsCompleted == true
             ? query.OrderByDescending(x => x.CompletedOn)
                 .ThenBy(x => x.Name)
             : query.OrderByDescending(x => x.Created)
                 .ThenBy(x => x.Name);

         var pagedToDos = await orderedQuery.Page(binding)
                             .ToListAsync();

        var todoIds = pagedToDos.Select(x => x.Id).ToList();
        var currencyIds = pagedToDos.Where(x => x.CurrencyId.HasValue)
                                    .Select(x => x.CurrencyId.Value)
                                    .Distinct()
                                    .ToList();

        var currenciesById = await context.Currencies
                                          .Where(x => currencyIds.Contains(x.Id))
                                          .Select(x => new
                                          {
                                              x.Id,
                                              Currency = new Model.View.Currency.Currency(x)
                                          })
                                          .ToDictionaryAsync(x => x.Id, x => x.Currency);

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
                Currency = x.CurrencyId.HasValue
                    ? currenciesById.GetValueOrDefault(x.CurrencyId.Value)
                    : null,
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
        var query = context.ToDos.WhereUser(UserId)
                                 .Where(binding, context, UserId);

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
        var query = context.ToDos.WhereUser(UserId)
                                 .Where(binding, context, UserId);

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
