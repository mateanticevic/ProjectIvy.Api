using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Bank;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Bank;

namespace ProjectIvy.Business.Handlers.Bank;

public interface IBankHandler
{
    Task<PagedView<View.Bank>> Get(BankGetBinding binding);
}
