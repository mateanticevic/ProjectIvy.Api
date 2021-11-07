using System.Threading.Tasks;

namespace ProjectIvy.Business.Handlers.Account
{
    public interface IAccountHandler
    {
        Task ProcessHacTransactions(string accountKey, string csv);

        Task ProcessOtpBankTransactions(string accountKey, string csv);

        Task ProcessRevolutTransactions(string accountKey, string csv);
    }
}