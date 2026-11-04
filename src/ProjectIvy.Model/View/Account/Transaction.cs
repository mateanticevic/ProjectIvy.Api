namespace ProjectIvy.Model.View.Account;

public class Transaction(Database.Main.Finance.Transaction t)
{
    public decimal Amount { get; set; } = t.Amount;

    public DateTime Created { get; set; } = t.Created;

    public string Description { get; set; } = t.Description;

    public string Id { get; set; } = t.ValueId;
}
