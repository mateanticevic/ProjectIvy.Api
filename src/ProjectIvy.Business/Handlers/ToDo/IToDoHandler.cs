using System.Threading.Tasks;
using ProjectIvy.Model.Binding.ToDo;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.ToDo;

namespace ProjectIvy.Business.Handlers.ToDo;

public interface IToDoHandler
{
    Task<string> Create(ToDoBinding binding);

    Task Update(string toDoValueId, ToDoBinding binding);

    Task<PagedView<View.ToDo>> Get(ToDoGetBinding binding);

    Task<IEnumerable<KeyValuePair<Model.View.Tag.Tag, int>>> GetCountByTag();

    Task LinkTag(string toDoValueId, string tagValueId);

    Task UnlinkTag(string toDoValueId, string tagValueId);
}
