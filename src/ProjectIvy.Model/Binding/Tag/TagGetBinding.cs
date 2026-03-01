using ProjectIvy.Model.Binding.Route;

namespace ProjectIvy.Model.Binding.Tag;

public class TagGetBinding : PagedBinding, ISearchable
{
    public string Search { get; set; }
}
