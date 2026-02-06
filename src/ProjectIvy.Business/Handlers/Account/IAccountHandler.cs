using System.Threading.Tasks;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Account;
using ProjectIvy.Model.Binding.Transaction;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Account;

namespace ProjectIvy.Business.Handlers.Account;

public interface IAccountHandler
{
    Task<string> Create(AccountBinding binding);

    Task CreateTransaction(string accountValueId, TransactionBinding binding);

    Task<IEnumerable<View.Account>> Get(AccountGetBinding b);

    Task<decimal> GetNetWorth();

    Task<View.AccountOverview> GetOverview(string accountValueId);

    Task<PagedView<View.Transaction>> GetTransactions(string accountValueId, FilteredPagedBinding b);

    Task ProcessHacTransactions(string accountKey, string csv);

    Task ProcessOtpBankTransactions(string accountKey, string csv);

    Task ProcessRevolutTransactions(string accountKey, string csv);

    Task Update(string accountValueId, AccountBinding binding);
}