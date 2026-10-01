using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(Account), Schema = nameof(Finance))]
public class Account : UserEntity, IHasValueId, IHasName
{
    public bool Active { get; set; }

    public Bank Bank { get; set; }

    public int? BankId { get; set; }

    public Common.Currency Currency { get; set; }

    public int CurrencyId { get; set; }

    public string Iban { get; set; }

    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public ICollection<Transaction> Transactions { get; set; }

    public TransactionSource? TransactionSource { get; set; }

    public string ValueId { get; set; }
}
