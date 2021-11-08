using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Account;
using View = ProjectIvy.Model.View.Account;

namespace ProjectIvy.Business.Handlers.Account
{
    public interface IAccountHandler
    {
        Task<IEnumerable<View.Account>> Get(AccountGetBinding b);

        Task ProcessHacTransactions(string accountKey, string csv);

        Task ProcessOtpBankTransactions(string accountKey, string csv);

        Task ProcessRevolutTransactions(string accountKey, string csv);
    }
}