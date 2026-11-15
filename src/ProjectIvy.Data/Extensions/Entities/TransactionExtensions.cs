using System.Linq;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Data.Extensions.Entities;

public static class TransactionExtensions
{
    public static IQueryable<Transaction> WhereUnlinked(this IQueryable<Transaction> query, MainContext context)
        => query.Where(x => !context.ExpenseTransactions.Any(link => link.TransactionId == x.Id)
            && !context.IncomeTransactions.Any(link => link.TransactionId == x.Id));
}
