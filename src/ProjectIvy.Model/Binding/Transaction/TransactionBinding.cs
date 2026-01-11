namespace ProjectIvy.Model.Binding.Transaction;

public record TransactionBinding
{
    public decimal Amount { get; init; }

    public DateTime Created { get; init; }
}