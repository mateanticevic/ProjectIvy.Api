namespace ProjectIvy.Model.Binding.Account;

public class AccountBinding
{
    public string Name { get; set; }

    public string Iban { get; set; }

    public string BankId { get; set; }

    public string CurrencyId { get; set; }

    public bool Active { get; set; } = true;
}
