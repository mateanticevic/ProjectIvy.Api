namespace ProjectIvy.Model.View.Account;

public class Transaction
{
    public Transaction(Database.Main.Finance.Transaction t)
    {
        Amount = t.Amount;
        Created = t.Created;
        Description = t.Description;
    }

    public decimal Amount { get; set; }

    public DateTime Created { get; set; }

    public string Description { get; set; }
}
