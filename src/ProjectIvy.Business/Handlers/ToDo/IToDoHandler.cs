using ProjectIvy.Model.Binding.ToDo;
using ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.ToDo
{
    public interface IToDoHandler
    {
        string Create(string name);

        PagedView<Model.View.ToDo.ToDo> GetPaged(ToDoGetBinding binding);

        void SetDone(string valueId);
    }
}