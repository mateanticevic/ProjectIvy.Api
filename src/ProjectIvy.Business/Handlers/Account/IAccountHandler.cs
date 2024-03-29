﻿using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Account;
using ProjectIvy.Model.Binding.Transaction;
using View = ProjectIvy.Model.View.Account;

namespace ProjectIvy.Business.Handlers.Account
{
    public interface IAccountHandler
    {
        Task CreateTransaction(string accountValueId, TransactionBinding binding);

        Task<IEnumerable<View.Account>> Get(AccountGetBinding b);

        Task<View.AccountOverview> GetOverview(string accountValueId);

        Task ProcessHacTransactions(string accountKey, string csv);

        Task ProcessOtpBankTransactions(string accountKey, string csv);

        Task ProcessRevolutTransactions(string accountKey, string csv);
    }
}