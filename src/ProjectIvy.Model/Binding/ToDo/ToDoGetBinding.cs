using ProjectIvy.Model.Binding.Route;

namespace ProjectIvy.Model.Binding.ToDo;

public class ToDoGetBinding : PagedBinding, ISearchable
{
    public IEnumerable<string> TagId { get; set; }

    public string Search { get; set; }
}
