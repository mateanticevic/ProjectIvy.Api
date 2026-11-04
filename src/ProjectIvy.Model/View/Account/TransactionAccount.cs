namespace ProjectIvy.Model.View.Account;

public class TransactionAccount(Database.Main.Finance.Account account)
{
    public string Id { get; } = account.ValueId;

    public string Name { get; } = account.Name;

    public Currency.Currency Currency { get; } = new(account.Currency);
}
