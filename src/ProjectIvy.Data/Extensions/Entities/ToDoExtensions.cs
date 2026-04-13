using System.Linq;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Model.Binding.ToDo;
using ProjectIvy.Model.Database.Main.User;

namespace ProjectIvy.Data.Extensions.Entities;

public static class ToDoExtensions
{
    public static IQueryable<ToDo> Where(this IQueryable<ToDo> query, ToDoGetBinding binding, MainContext context, int userId)
    {
        var requestedTagValueIds = binding.TagId?.Where(x => !string.IsNullOrWhiteSpace(x))
                                                .Distinct()
                                                .ToList();

        if (requestedTagValueIds?.Any() == true)
        {
            query = query.Where(x => context.ToDoTags
                                            .Where(y => y.ToDoId == x.Id)
                                            .Join(context.Tags.WhereUser(userId)
                                                              .Where(y => requestedTagValueIds.Contains(y.ValueId)),
                                                  toDoTag => toDoTag.TagId,
                                                  tag => tag.Id,
                                                  (_, tag) => tag.Id)
                                            .Distinct()
                                            .Count() == requestedTagValueIds.Count);
        }

        var requestedTripValueIds = binding.TripId?.Where(x => !string.IsNullOrWhiteSpace(x))
                                                  .Distinct()
                                                  .ToList();

        if (requestedTripValueIds?.Any() == true)
        {
            query = query.Where(x => context.TripToDos
                                            .Where(y => y.ToDoId == x.Id)
                                            .Join(context.Trips.WhereUser(userId)
                                                               .Where(y => requestedTripValueIds.Contains(y.ValueId)),
                                                  tripToDo => tripToDo.TripId,
                                                  trip => trip.Id,
                                                  (_, trip) => trip.Id)
                                            .Distinct()
                                            .Count() == requestedTripValueIds.Count);
        }

        return query.WhereIf(!string.IsNullOrEmpty(binding.Search), x => x.Name.ToLower().Contains(binding.Search.ToLower()) || x.ValueId.ToLower().Contains(binding.Search.ToLower()))
                    .WhereIf(binding.From.HasValue, x => x.Created >= binding.From.Value)
                    .WhereIf(binding.To.HasValue, x => x.Created <= binding.To.Value)
                    .WhereIf(binding.FromDueDate.HasValue, x => x.DueDate.HasValue && x.DueDate.Value >= binding.FromDueDate.Value)
                    .WhereIf(binding.ToDueDate.HasValue, x => x.DueDate.HasValue && x.DueDate.Value <= binding.ToDueDate.Value)
                    .WhereIf(binding.IsCompleted.HasValue, x => x.IsCompleted == binding.IsCompleted.Value);
    }
}