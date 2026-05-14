using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Loan;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Loan;

namespace ProjectIvy.Business.Handlers.Loan;

public interface ILoanHandler : IHandler
{
    Task<PagedView<View.Loan>> Get(LoanGetBinding binding);
}
