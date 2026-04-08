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
        var tagIds = NormalizeTagIds(binding.TagIds);

        int? currencyId = null;

        if (binding.EstimatedPrice.HasValue)
            currencyId = context.GetCurrencyId(binding.CurrencyId, UserId);

        List<int> resolvedTagIds = [];
        if (tagIds.Count > 0)
        {
            resolvedTagIds = await context.Tags.WhereUser(UserId)
                                               .Where(x => tagIds.Contains(x.ValueId))
                                               .Select(x => x.Id)
                                               .ToListAsync();
        }

        var entity = new Database.ToDo
        {
            Name = binding.Name,
            Description = binding.Description,
            DueDate = binding.DueDate,
            IsCompleted = false,
            Created = DateTime.UtcNow,
            ValueId = binding.Name.ToValueId(),
            UserId = UserId,
            EstimatedPrice = binding.EstimatedPrice,
            CurrencyId = currencyId
        };

        await context.ToDos.AddAsync(entity);
        await context.SaveChangesAsync();

        if (resolvedTagIds.Count > 0)
        {
            await context.ToDoTags.AddRangeAsync(resolvedTagIds.Select(tagId => new Database.ToDoTag
            {
                ToDoId = entity.Id,
                TagId = tagId
            }));

            await context.SaveChangesAsync();
        }

        return entity.ValueId;
    }

    public async Task Delete(string toDoValueId)
    {
        using var context = GetMainContext();

        var toDo = await context.ToDos.WhereUser(UserId)
                                     .SingleOrDefaultAsync(x => x.ValueId == toDoValueId) ?? throw new ResourceNotFoundException();

        var toDoTags = await context.ToDoTags.Where(x => x.ToDoId == toDo.Id)
                                             .ToListAsync();

        if (toDoTags.Count > 0)
            context.ToDoTags.RemoveRange(toDoTags);

        var tripToDos = await context.TripToDos.Where(x => x.ToDoId == toDo.Id)
                                               .ToListAsync();

        if (tripToDos.Count > 0)
            context.TripToDos.RemoveRange(tripToDos);

        context.ToDos.Remove(toDo);
        await context.SaveChangesAsync();
    }

    public async Task Update(string toDoValueId, ToDoBinding binding)
    {
        using var context = GetMainContext();

        var toDo = await context.ToDos.WhereUser(UserId)
                                     .SingleOrDefaultAsync(x => x.ValueId == toDoValueId) ?? throw new ResourceNotFoundException();

        bool wasCompleted = toDo.IsCompleted;

        toDo.Name = binding.Name;
        toDo.Description = binding.Description;
        toDo.DueDate = binding.DueDate;
        toDo.IsCompleted = binding.IsCompleted;
        toDo.EstimatedPrice = binding.EstimatedPrice;
        toDo.CurrencyId = binding.EstimatedPrice.HasValue
            ? context.GetCurrencyId(binding.CurrencyId, UserId)
            : null;

        if (!wasCompleted && toDo.IsCompleted)
            toDo.CompletedOn = DateTime.UtcNow;

        if (wasCompleted && !toDo.IsCompleted)
            toDo.CompletedOn = null;

        if (binding.TagIds != null)
        {
            var requestedTagIds = NormalizeTagIds(binding.TagIds);
            List<int> resolvedTagIds = [];

            if (requestedTagIds.Count > 0)
            {
                resolvedTagIds = await context.Tags.WhereUser(UserId)
                                                   .Where(x => requestedTagIds.Contains(x.ValueId))
                                                   .Select(x => x.Id)
                                                   .ToListAsync();
            }

            var currentTagIds = await context.ToDoTags
                                             .Where(x => x.ToDoId == toDo.Id)
                                             .Select(x => x.TagId)
                                             .ToListAsync();

            var resolvedTagIdSet = resolvedTagIds.ToHashSet();
            var currentTagIdSet = currentTagIds.ToHashSet();

            var removeTagIds = currentTagIds.Where(x => !resolvedTagIdSet.Contains(x))
                                            .ToList();

            if (removeTagIds.Count > 0)
            {
                var linksToRemove = await context.ToDoTags.Where(x => x.ToDoId == toDo.Id && removeTagIds.Contains(x.TagId))
                                                           .ToListAsync();

                context.ToDoTags.RemoveRange(linksToRemove);
            }

            var addTagIds = resolvedTagIds.Where(x => !currentTagIdSet.Contains(x))
                                          .ToList();

            if (addTagIds.Count > 0)
            {
                await context.ToDoTags.AddRangeAsync(addTagIds.Select(tagId => new Database.ToDoTag
                {
                    ToDoId = toDo.Id,
                    TagId = tagId
                }));
            }
        }

        context.ToDos.Update(toDo);
        await context.SaveChangesAsync();
    }

    private static HashSet<string> NormalizeTagIds(IEnumerable<string> tagIds)
    {
        if (tagIds == null)
        {
            return [];
        }

        return tagIds.Where(x => !string.IsNullOrWhiteSpace(x))
                     .Select(x => x.Trim())
                     .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public async Task<PagedView<View.ToDo>> Get(ToDoGetBinding binding)
    {
        using var context = GetMainContext();
        var query = context.ToDos.WhereUser(UserId)
                                 .Where(binding, context, UserId);

        var orderedQuery = binding.IsCompleted == true
            ? query.OrderByDescending(x => x.CompletedOn)
                .ThenBy(x => x.Name)
            : query.OrderByDescending(x => x.DueDate.HasValue)
                .ThenBy(x => x.DueDate)
                .ThenByDescending(x => x.Created);

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

    public async Task<IEnumerable<KeyValuePair<Model.View.Currency.Currency, decimal>>> SumByCurrency(ToDoGetBinding binding)
    {
        using var context = GetMainContext();

        return await context.ToDos.WhereUser(UserId)
                                  .Where(binding, context, UserId)
                                  .Where(x => x.EstimatedPrice.HasValue && x.CurrencyId.HasValue)
                                  .Join(context.Currencies,
                                        toDo => toDo.CurrencyId!.Value,
                                        currency => currency.Id,
                                        (toDo, currency) => new
                                        {
                                            currency.ValueId,
                                            currency.Name,
                                            EstimatedPrice = (decimal)toDo.EstimatedPrice!.Value
                                        })
                                  .GroupBy(x => new { x.ValueId, x.Name })
                                  .Select(x => new KeyValuePair<Model.View.Currency.Currency, decimal>(
                                      new Model.View.Currency.Currency
                                      {
                                          Id = x.Key.ValueId,
                                          Name = x.Key.Name
                                      },
                                      x.Sum(y => y.EstimatedPrice)))
                                  .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<Model.View.Tag.Tag, IEnumerable<KeyValuePair<Model.View.Currency.Currency, decimal>>>>> SumByTag(ToDoGetBinding binding)
    {
        using var context = GetMainContext();
        var query = context.ToDos.WhereUser(UserId)
                                 .Where(binding, context, UserId)
                                 .Where(x => x.EstimatedPrice.HasValue && x.CurrencyId.HasValue);

        var aggregated = await context.ToDoTags
                                      .Join(query,
                                            toDoTag => toDoTag.ToDoId,
                                            toDo => toDo.Id,
                                            (toDoTag, toDo) => new
                                            {
                                                toDoTag.TagId,
                                                CurrencyId = toDo.CurrencyId!.Value,
                                                EstimatedPrice = (decimal)toDo.EstimatedPrice!.Value
                                            })
                                      .Join(context.Tags.WhereUser(UserId),
                                            x => x.TagId,
                                            tag => tag.Id,
                                            (x, tag) => new
                                            {
                                                x.CurrencyId,
                                                x.EstimatedPrice,
                                                TagId = tag.ValueId,
                                                TagName = tag.Name
                                            })
                                      .Join(context.Currencies,
                                            x => x.CurrencyId,
                                            currency => currency.Id,
                                            (x, currency) => new
                                            {
                                                x.TagId,
                                                x.TagName,
                                                CurrencyId = currency.ValueId,
                                                CurrencyName = currency.Name,
                                                x.EstimatedPrice
                                            })
                                      .GroupBy(x => new
                                      {
                                          x.TagId,
                                          x.TagName,
                                          x.CurrencyId,
                                          x.CurrencyName
                                      })
                                      .Select(x => new
                                      {
                                          x.Key.TagId,
                                          x.Key.TagName,
                                          x.Key.CurrencyId,
                                          x.Key.CurrencyName,
                                          Sum = x.Sum(y => y.EstimatedPrice)
                                      })
                                      .ToListAsync();

        return aggregated.GroupBy(x => new { x.TagId, x.TagName })
                         .OrderByDescending(x => x.Sum(y => y.Sum))
                         .Select(x => new KeyValuePair<Model.View.Tag.Tag, IEnumerable<KeyValuePair<Model.View.Currency.Currency, decimal>>>(
                             new Model.View.Tag.Tag(new Model.Database.Main.Common.Tag
                             {
                                 ValueId = x.Key.TagId,
                                 Name = x.Key.TagName
                             }),
                             x.OrderByDescending(y => y.Sum)
                              .Select(y => new KeyValuePair<Model.View.Currency.Currency, decimal>(
                                  new Model.View.Currency.Currency
                                  {
                                      Id = y.CurrencyId,
                                      Name = y.CurrencyName
                                  },
                                  y.Sum))));
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
