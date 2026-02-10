using ProjectIvy.Model.Binding.Route;

namespace ProjectIvy.Model.Binding.Person;

public class PersonGetBinding : PagedBinding, ISearchable
{
    public string Search { get; set; }
}
