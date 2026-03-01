using System.Threading.Tasks;
using ProjectIvy.Model.Binding.ToDo;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.ToDo;

namespace ProjectIvy.Business.Handlers.ToDo;

public interface IToDoHandler
{
    Task<PagedView<View.ToDo>> Get(ToDoGetBinding binding);
}
