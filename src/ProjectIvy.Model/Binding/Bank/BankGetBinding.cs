using ProjectIvy.Model.Binding.Route;

namespace ProjectIvy.Model.Binding.Bank;

public class BankGetBinding : PagedBinding, ISearchable
{
    public string Search { get; set; }
}
