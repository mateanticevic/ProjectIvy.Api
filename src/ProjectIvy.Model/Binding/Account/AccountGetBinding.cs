using ProjectIvy.Model.Binding.Route;

namespace ProjectIvy.Model.Binding.Account;

public class AccountGetBinding : FilteredPagedBinding, ISearchable
{
    public IEnumerable<string> BankIds { get; set; }

    public bool? IsActive { get; set; }

    public string Search { get; set; }
}
