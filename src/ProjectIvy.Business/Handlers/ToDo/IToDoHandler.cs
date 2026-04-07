using System.Threading.Tasks;
using ProjectIvy.Model.Binding.ToDo;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.ToDo;

namespace ProjectIvy.Business.Handlers.ToDo;

public interface IToDoHandler
{
    Task<string> Create(ToDoBinding binding);

    Task Delete(string toDoValueId);

    Task<PagedView<View.ToDo>> Get(ToDoGetBinding binding);

    Task<IEnumerable<KeyValuePair<Model.View.Tag.Tag, int>>> GetCountByTag(ToDoGetBinding binding);

    Task<IEnumerable<KeyValuePair<Model.View.Trip.Trip, int>>> GetCountByTrip(ToDoGetBinding binding);

    Task<IEnumerable<KeyValuePair<Model.View.Currency.Currency, decimal>>> SumByCurrency(ToDoGetBinding binding);

    Task<IEnumerable<KeyValuePair<Model.View.Tag.Tag, IEnumerable<KeyValuePair<Model.View.Currency.Currency, decimal>>>>> SumByTag(ToDoGetBinding binding);

    Task LinkTag(string toDoValueId, string tagValueId);

    Task UnlinkTag(string toDoValueId, string tagValueId);

    Task Update(string toDoValueId, ToDoBinding binding);
}
