using Finance = ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Model.View.Account;

public class TransactionWithAccount(Finance.Transaction transaction) : Transaction(transaction)
{
    public TransactionAccount Account { get; } = new(transaction.Account);
}
