using ProjectIvy.Model.Binding.Route;

namespace ProjectIvy.Model.Binding.ToDo;

public class ToDoGetBinding : PagedBinding, ISearchable
{
    public bool? IsCompleted { get; set; }

    public IEnumerable<string> TagId { get; set; }

    public IEnumerable<string> TripId { get; set; }

    public string Search { get; set; }
}
