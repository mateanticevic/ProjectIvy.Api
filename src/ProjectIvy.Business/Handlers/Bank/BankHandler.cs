using System.Linq;
using System.Threading.Tasks;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Bank;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Bank;

namespace ProjectIvy.Business.Handlers.Bank;

public class BankHandler : Handler<BankHandler>, IBankHandler
{
    public BankHandler(IHandlerContext<BankHandler> context) : base(context)
    {
    }

    public async Task<PagedView<View.Bank>> Get(BankGetBinding binding)
    {
        using var context = GetMainContext();
        var query = context.Banks
                           .WhereSearch(binding)
                           .OrderByDescending(x => x.ValueId == binding.Search)
                           .ThenBy(x => x.Name)
                           .Select(x => new View.Bank(x));

        return await query.ToPagedViewAsync(binding);
    }
}
