using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Loan;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Loan;

namespace ProjectIvy.Business.Handlers.Loan;

public class LoanHandler : Handler<LoanHandler>, ILoanHandler
{
    public LoanHandler(IHandlerContext<LoanHandler> context) : base(context)
    {
    }

    public async Task<PagedView<View.Loan>> Get(LoanGetBinding binding)
    {
        using var context = GetMainContext();
        var query = context.Loans
                           .WhereUser(UserId)
                           .Include(x => x.Bank)
                           .Include(x => x.Currency)
                           .OrderByDescending(x => x.StartDate)
                           .Select(x => new View.Loan(x));

        return await query.ToPagedViewAsync(binding);
    }
}
