using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(PaymentProviderAccount), Schema = nameof(Finance))]
public class PaymentProviderAccount : UserEntity, IHasValueId, IHasName
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }

    public int PaymentProviderId { get; set; }

    public PaymentProvider PaymentProvider { get; set; }

    public string Token { get; set; }
}
